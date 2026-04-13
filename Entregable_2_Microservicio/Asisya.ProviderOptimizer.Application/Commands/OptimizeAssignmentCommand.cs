using System;
using System.Collections.Generic;
using MediatR;

namespace Asisya.ProviderOptimizer.Application.Commands;

public class OptimizeAssignmentCommand : IRequest<OptimizationResultDto>
{
    public double IncidentLat { get; set; }
    public double IncidentLng { get; set; }
    public string VehicleCategory { get; set; } = string.Empty;
    public string IncidentType { get; set; } = string.Empty;
    public bool IsPremiumUser { get; set; }
}

public class OptimizationResultDto
{
    public Guid WinnerProviderId { get; set; }
    public string WinnerName { get; set; } = string.Empty;
    public double MatchScore { get; set; }
    public double EstimatedETA { get; set; }
    public double DistanceKm { get; set; }
    public List<SimulatedBoardItem> BoardFallback { get; set; } = new();
}

public class SimulatedBoardItem
{
    public Guid ProviderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Score { get; set; }
}
