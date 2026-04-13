using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asisya.ProviderOptimizer.Application.Interfaces;
using Asisya.ProviderOptimizer.Domain.Entities;
using Asisya.ProviderOptimizer.Domain.Services;
using MediatR;

namespace Asisya.ProviderOptimizer.Application.Commands;

public class OptimizeAssignmentCommandHandler : IRequestHandler<OptimizeAssignmentCommand, OptimizationResultDto>
{
    private readonly IProviderRepository _providerRepository;
    private readonly ScoringPolicy _scoringPolicy;

    public OptimizeAssignmentCommandHandler(IProviderRepository providerRepository)
    {
        _providerRepository = providerRepository;
        _scoringPolicy = new ScoringPolicy();
    }

    public async Task<OptimizationResultDto> Handle(OptimizeAssignmentCommand request, CancellationToken cancellationToken)
    {
        var ticket = new OptimizationTicket
        {
            TicketId = Guid.NewGuid(),
            IncidentLat = request.IncidentLat,
            IncidentLng = request.IncidentLng,
            VehicleCategory = request.VehicleCategory,
            IncidentType = request.IncidentType,
            IsPremiumUser = request.IsPremiumUser
        };

        var activeProviders = await _providerRepository.GetAvailableProvidersAsync();
        var scoredProviders = new List<(Provider provider, double score, double distance, double eta)>();

        foreach (var provider in activeProviders)
        {
            double distKm = CalculateDistanceHaversine(ticket.IncidentLat, ticket.IncidentLng, provider.CurrentLat, provider.CurrentLng);

            if (distKm > 20.0) continue;

            double etaMins = distKm * 3.0;

            double score = _scoringPolicy.CalculateScore(provider, ticket, distKm, etaMins);

            if (score > 0)
            {
                scoredProviders.Add((provider, score, distKm, etaMins));
            }
        }

        if (!scoredProviders.Any())
            throw new InvalidOperationException("No providers matched the required criteria within the operational geofence.");

        var leaderboard = scoredProviders.OrderByDescending(x => x.score).ToList();
        var winner = leaderboard.First();

        var result = new OptimizationResultDto
        {
            WinnerProviderId = winner.provider.Id,
            WinnerName = winner.provider.Name,
            MatchScore = Math.Round(winner.score, 2),
            DistanceKm = Math.Round(winner.distance, 2),
            EstimatedETA = Math.Round(winner.eta, 2)
        };

        var backupTop5 = leaderboard.Skip(1).Take(5);
        foreach (var fb in backupTop5)
        {
            result.BoardFallback.Add(new SimulatedBoardItem
            {
                ProviderId = fb.provider.Id,
                Name = fb.provider.Name,
                Score = Math.Round(fb.score, 2)
            });
        }

        return result;
    }

    /// <summary>
    /// Computes the great-circle distance between two geographical points using the Haversine formula.
    /// </summary>
    private double CalculateDistanceHaversine(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371;
        var dLat = (lat2 - lat1) * Math.PI / 180.0;
        var dLon = (lon2 - lon1) * Math.PI / 180.0;
        var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
}
