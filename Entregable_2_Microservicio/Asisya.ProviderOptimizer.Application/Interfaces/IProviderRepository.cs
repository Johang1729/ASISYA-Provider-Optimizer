namespace Asisya.ProviderOptimizer.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asisya.ProviderOptimizer.Domain.Entities;

public interface IProviderRepository
{
    // Recupera todos los proveedores que actualmente están en modo Libre para asignación
    Task<List<Provider>> GetAvailableProvidersAsync();
}
