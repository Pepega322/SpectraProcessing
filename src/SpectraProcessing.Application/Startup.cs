using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScottPlot;
using ScottPlot.WinForms;
using SpectraProcessing.Application.Controllers;
using SpectraProcessing.Controllers;
using SpectraProcessing.Controllers.Interfaces;
using SpectraProcessing.Controllers.Settings;
using SpectraProcessing.DataSource.InputOutput;
using SpectraProcessing.Domain.DataProcessors;
using SpectraProcessing.Domain.InputOutput;
using SpectraProcessing.Graphics.DataProcessors;
using SpectraProcessing.Models;
using SpectraProcessing.Models.Spectra.Abstractions;
using Plot = ScottPlot.Plot;

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
            .AddLogging()
            .Configure<DataReaderControllerSettings>(configuration.GetSection(nameof(DataReaderControllerSettings)))
            .Configure<DataStorageSettings>(configuration.GetSection(nameof(DataStorageSettings)))
            .AddFormVariables(plot)
            .AddDomainComponents()
            .AddControllers();
    }

    private static IServiceCollection AddFormVariables(this IServiceCollection services, FormsPlot plot)
    {
        services.AddSingleton<FormsPlot>(_ => plot);
        services.AddSingleton<Plot>(_ => plot.Plot);
        services.AddSingleton<CoordinateController>();
        services.AddTransient<IPalette>(_ => new ScottPlot.Palettes.Category20());

        return services;
    }

    private static IServiceCollection AddDomainComponents(this IServiceCollection services)
    {
        services.AddSingleton<IDataReader<SpectraData>, SpectraFileReader>();
        // services.AddSingleton<IDataReader<PeakBordersSet>, PeakBordersSetReader>();
        services.AddSingleton<IDataWriter, FileWriter>(_ => new FileWriter(FileMode.Create));
        services.AddSingleton<IDataPlotBuilder<SpectraData, SpectraDataPlot>, SpectraDataPlotBuilder>();
        // services.AddSingleton<IPlotBuilder<PeakInfo, PeakBorderPlot>, ScottPeakBorderPlotBuilder>();
        services.AddSingleton<IDataPlotDrawer<SpectraDataPlot>, SpectraDataPlotDrawer>();

        return services;
    }

    private static IServiceCollection AddControllers(this IServiceCollection services)
    {
        services.AddSingleton<IDialogController, DialogController>();
        services.AddSingleton<IDataSourceController<SpectraData>, DirectoryDataSourceController<SpectraData>>();
        services.AddSingleton<IDataWriterController, DirectoryDataWriterController>();
        services.AddSingleton<IDataStorageController<SpectraData>, DataStorageController<SpectraData>>();
        services.AddSingleton<IDataStorageController<SpectraDataPlot>, DataStorageController<SpectraDataPlot>>();
        services.AddSingleton<IGraphicsController<SpectraDataPlot>, ScottSpectraGraphicsController>();
        services.AddSingleton<ISpectraProcessingController, SpectraProcessingController>();
        services.AddSingleton<ICoordinateController, CoordinateController>();
        services.AddSingleton<IPlotController, PlotController>();

        return services;
    }
}
