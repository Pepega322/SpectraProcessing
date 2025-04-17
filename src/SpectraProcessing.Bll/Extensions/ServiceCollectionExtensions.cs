using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpectraProcessing.Bll.Controllers;
using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Bll.Models.ScottPlot.Peak;
using SpectraProcessing.Bll.Models.ScottPlot.Spectra.Abstractions;
using SpectraProcessing.Bll.Models.Settings;
using SpectraProcessing.Bll.Providers;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Domain.Collections.Keys;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Extensions;

using SpectraPeakDataStorageProvider = DataStorageProvider<SpectraKey, PeakDataPlot>;
using SpectraPlotStorageProvider = DataStorageProvider<StringKey, SpectraDataPlot>;
using SpectraDataStorageProvider = DataStorageProvider<StringKey, SpectraData>;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBll(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddLogging()
            .ConfigureSettings(configuration)
            .AddProviders()
            .AddControllers();

        return services;
    }

    private static IServiceCollection ConfigureSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .Configure<DataStorageSettings>(configuration.GetSection(nameof(DataStorageSettings)));

        return services;
    }

    private static IServiceCollection AddProviders(this IServiceCollection services)
    {
        services
            .AddSingleton<ICoordinateProvider, CoordinateProvider>()
            .AddSingleton<IDataStorageProvider<StringKey, SpectraData>, SpectraDataStorageProvider>()
            .AddSingleton<IDataStorageProvider<StringKey, SpectraDataPlot>, SpectraPlotStorageProvider>()
            .AddSingleton<IDataStorageProvider<SpectraKey, PeakDataPlot>, SpectraPeakDataStorageProvider>()
            .AddSingleton<ISpectraDataPlotProvider, SpectraDataPlotProvider>()
            .AddSingleton<IPeakDataPlotProvider, PeakDataPlotProvider>();

        services
            .AddTransient<IDataProvider<SpectraData>, DirectoryDataProvider<SpectraData>>();

        return services;
    }

    private static IServiceCollection AddControllers(this IServiceCollection services)
    {
        services
            .AddSingleton<ISpectraController, SpectraController>()
            .AddSingleton<IProcessingController, ProcessingController>();

        return services;
    }
}
