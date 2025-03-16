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
using SpectraProcessing.Models.PeakEstimate;
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
            .AddSettings(configuration)
            .AddFormVariables(plot)
            .AddDomainComponents()
            .AddControllers()
            .AddLogging();
    }

    private static IServiceCollection AddSettings(this IServiceCollection services, IConfigurationRoot configuration)
    {
        return services
            .Configure<DataReaderControllerSettings>(configuration.GetSection(nameof(DataReaderControllerSettings)))
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
        services.AddTransient<IDataPlotBuilder<PeakEstimateData, PeakEstimateDataPlot>, PeakEstimateDataPlotBuilder>();
        services.AddTransient<IDataPlotDrawer<SpectraDataPlot>, SpectraDataPlotDrawer>();
        services.AddTransient<IDataPlotDrawer<PeakEstimateDataPlot>, PeakEstimateDataPlotDrawer>();

        return services;
    }

    private static IServiceCollection AddControllers(this IServiceCollection services)
    {
        services.AddTransient<IDialogController, DialogController>();
        services.AddTransient<IDataSourceController<SpectraData>, DirectoryDataSourceController<SpectraData>>();
        services.AddTransient<IDataWriterController, DirectoryDataWriterController>();
        services.AddTransient<IDataStorageController<SpectraData>, DataStorageController<SpectraData>>();
        services.AddTransient<IDataStorageController<SpectraDataPlot>, DataStorageController<SpectraDataPlot>>();
        services.AddTransient<IGraphicsController<SpectraDataPlot>, SpectraDataGraphicsController>();
        services.AddTransient<ISpectraProcessingController, SpectraProcessingController>();
        services.AddTransient<ICoordinateController, CoordinateController>();
        services.AddTransient<IPlotController, PlotController>();

        return services;
    }
}
