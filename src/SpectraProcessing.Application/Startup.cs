using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScottPlot;
using ScottPlot.WinForms;
using SpectraProcessing.Application.Controllers;
using SpectraProcessing.Bll.Controllers;
using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Bll.Providers;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Bll.Settings;
using SpectraProcessing.DataSource.InputOutput;
using SpectraProcessing.Domain.DataProcessors;
using SpectraProcessing.Domain.InputOutput;
using SpectraProcessing.Graphics.DataProcessors;
using SpectraProcessing.Models.Collections.Keys;
using SpectraProcessing.Models.Peak;
using SpectraProcessing.Models.Spectra.Abstractions;

namespace SpectraProcessing.Application;

public static class Startup
{
    public static IServiceProvider GetServiceProvider(FormsPlot plot)
    {
        return CreateConfigurationBuilder()
            .ConfigureServices(plot)
            .BuildServiceProvider();
    }

    private static IConfigurationRoot CreateConfigurationBuilder()
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Environment.CurrentDirectory);
        builder.AddJsonFile("appsettings.json", false, true);
        return builder.Build();
    }

    private static IServiceCollection ConfigureServices(this IConfigurationRoot configuration, FormsPlot plot)
    {
        return new ServiceCollection()
            .AddSettings(configuration)
            .AddFormVariables(plot)
            .AddDomainComponents()
            .AddControllers()
            .AddLogging();
    }

    private static IServiceCollection AddSettings(this IServiceCollection services, IConfigurationRoot configuration)
    {
        return services
            .Configure<DataStorageSettings>(configuration.GetSection(nameof(DataStorageSettings)));
    }

    private static IServiceCollection AddFormVariables(this IServiceCollection services, FormsPlot plot)
    {
        services.AddSingleton<FormsPlot>(_ => plot);
        services.AddSingleton<Plot>(_ => plot.Plot);
        services.AddTransient<IPalette>(_ => new ScottPlot.Palettes.Category20());

        return services;
    }

    private static IServiceCollection AddDomainComponents(this IServiceCollection services)
    {
        services.AddTransient<IDataWriter, FileWriter>(_ => new FileWriter(FileMode.Create));
        services.AddTransient<IDataReader<SpectraData>, SpectraFileReader>();
        // services.AddSingleton<IDataReader<PeakBordersSet>, PeakBordersSetReader>();
        services.AddTransient<IDataPlotBuilder<SpectraData, SpectraDataPlot>, SpectraDataPlotBuilder>();
        services.AddTransient<IDataPlotBuilder<PeakData, PeakDataPlot>, PeakDataPlotBuilder>();
        services.AddTransient<IDataPlotDrawer<SpectraDataPlot>, SpectraDataPlotDrawer>();
        services.AddTransient<IDataPlotDrawer<PeakDataPlot>, PeakDataPlotDrawer>();

        return services;
    }

    private static IServiceCollection AddControllers(this IServiceCollection services)
    {
        services
            .AddSingleton<ICoordinateProvider, CoordinateProvider>()
            .AddSingleton<IDataStorageProvider<StringKey, SpectraData>,
                DataStorageProvider<StringKey, SpectraData>>()
            .AddSingleton<IDataStorageProvider<StringKey, SpectraDataPlot>,
                DataStorageProvider<StringKey, SpectraDataPlot>>()
            .AddSingleton<IDataStorageProvider<SpectraKey, PeakDataPlot>,
                DataStorageProvider<SpectraKey, PeakDataPlot>>()
            .AddSingleton<ISpectraController, SpectraController>()
            .AddTransient<IPeakController, PeakController>()
            .AddTransient<IDialogController, DialogController>()
            .AddTransient<IDataProvider<SpectraData>, DirectoryDataProvider<SpectraData>>()
            .AddTransient<IProcessingController, ProcessingController>();

        return services;
    }
}
