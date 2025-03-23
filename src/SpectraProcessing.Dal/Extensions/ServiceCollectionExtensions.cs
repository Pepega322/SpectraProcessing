using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpectraProcessing.Dal.IO;
using SpectraProcessing.Dal.IO.Interfaces;
using SpectraProcessing.Domain.Spectra.Abstractions;

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
        services.AddSingleton<IDataRepository<SpectraData>, SpectraDataFileRepository>();

        return services;
    }
}
