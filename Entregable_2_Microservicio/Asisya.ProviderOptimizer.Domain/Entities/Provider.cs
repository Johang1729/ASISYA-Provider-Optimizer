namespace Asisya.ProviderOptimizer.Domain.Entities;

using System.ComponentModel.DataAnnotations;

public class Provider
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string VehicleCapacity { get; set; } = string.Empty;
    public string IncidentCapacity { get; set; } = string.Empty;
    public string Status { get; set; } = "Libre";

    public double Rating { get; set; }
    public double AcceptanceRate { get; set; }

    public double CurrentLat { get; set; }
    public double CurrentLng { get; set; }

    [Timestamp]
    public byte[]? RowVersion { get; set; }
}
