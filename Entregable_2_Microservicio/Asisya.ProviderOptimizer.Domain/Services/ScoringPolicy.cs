using System;
using Asisya.ProviderOptimizer.Domain.Entities;

namespace Asisya.ProviderOptimizer.Domain.Services;

public class ScoringPolicy
{
    private const double WeightDistance = 0.35;
    private const double WeightETA = 0.25;
    private const double WeightRating = 0.20;
    private const double WeightAcceptance = 0.20;

    /// <summary>
    /// Calculates the suitability score for a given provider based on multiple weighted variables.
    /// Evaluates base hard filters (Capacity/Status) before assigning the normalized score.
    /// </summary>
    public double CalculateScore(Provider provider, OptimizationTicket ticket, double estimatedDistanceKm, double estimatedEtaMinutes)
    {
        if (provider.Status != "Libre") return 0;
        if (!provider.VehicleCapacity.Contains(ticket.VehicleCategory, StringComparison.OrdinalIgnoreCase)) return 0;
        if (!provider.IncidentCapacity.Contains(ticket.IncidentType, StringComparison.OrdinalIgnoreCase)) return 0;

        double distanceScore = Math.Max(0, 20.0 - estimatedDistanceKm) / 20.0 * 100;
        double etaScore = Math.Max(0, 60.0 - estimatedEtaMinutes) / 60.0 * 100;
        double ratingScore = (provider.Rating / 5.0) * 100;
        double acceptanceScore = provider.AcceptanceRate * 100;

        double finalScore = 
            (distanceScore * WeightDistance) + 
            (etaScore * WeightETA) + 
            (ratingScore * WeightRating) + 
            (acceptanceScore * WeightAcceptance);

        if (ticket.IsPremiumUser)
        {
            finalScore *= 1.20; 
        }

        return Math.Min(100.0, Math.Round(finalScore, 2));
    }
}
