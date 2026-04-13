using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Asisya.ProviderOptimizer.Domain.Entities;
using Asisya.ProviderOptimizer.Application.Interfaces;
using Asisya.ProviderOptimizer.Infrastructure.Persistence;

namespace Asisya.ProviderOptimizer.Infrastructure.Repositories;

public class ProviderRepository : IProviderRepository
{
    private readonly AppDbContext _context;

    public ProviderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Provider>> GetAvailableProvidersAsync()
    {
        return await _context.Providers.Where(p => p.Status == "Libre").ToListAsync();
    }
}
