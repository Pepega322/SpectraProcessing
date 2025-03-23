using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpectraProcessing.Dal.Repositories;
using SpectraProcessing.Dal.Repositories.Interfaces;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Dal.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDal(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRepositories();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IDataRepository<SpectraData>, SpectraDataIORepository>();

        return services;
    }
}
