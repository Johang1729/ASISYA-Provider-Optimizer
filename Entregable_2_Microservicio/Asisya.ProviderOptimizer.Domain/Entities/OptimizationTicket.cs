namespace Asisya.ProviderOptimizer.Domain.Entities;

public class OptimizationTicket
{
    public Guid TicketId { get; set; }
    public double IncidentLat { get; set; }
    public double IncidentLng { get; set; }
    public string VehicleCategory { get; set; } = string.Empty;
    public string IncidentType { get; set; } = string.Empty;
    public bool IsPremiumUser { get; set; }
}
