using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Asisya.ProviderOptimizer.Application.Interfaces;
using Asisya.ProviderOptimizer.Domain.Entities;

namespace Asisya.ProviderOptimizer.Application.Queries;

public class GetAvailableProvidersQuery : IRequest<List<Provider>>
{
    public double Lat { get; set; }
    public double Lng { get; set; }
    public double RadiusKm { get; set; } = 20.0;
}

public class GetAvailableProvidersQueryHandler : IRequestHandler<GetAvailableProvidersQuery, List<Provider>>
{
    private readonly IProviderRepository _repository;

    public GetAvailableProvidersQueryHandler(IProviderRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Provider>> Handle(GetAvailableProvidersQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAvailableProvidersAsync();
    }
}
