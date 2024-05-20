using Application.Controllers;
using Controllers;
using Controllers.Interfaces;
using Controllers.Settings;
using DataSource.InputOutput;
using Domain.Graphics;
using Domain.InputOutput;
using Domain.SpectraData;
using Domain.SpectraData.Parser;
using MathStatistics.InputOutput;
using MathStatistics.SpectraProcessing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scott.Formats;
using Scott.Graphics;
using ScottPlot;
using ScottPlot.WinForms;
using Plot = ScottPlot.Plot;

namespace Application;

public static class Startup
{
	public static IServiceProvider GetServiceProvider(FormsPlot formsPlot)
	{
		return CreateConfigurationBuilder()
			.ConfigureServices(formsPlot)
			.BuildServiceProvider();
	}

	private static IConfigurationRoot CreateConfigurationBuilder()
	{
		var builder = new ConfigurationBuilder();
		builder.SetBasePath(Environment.CurrentDirectory);
		builder.AddJsonFile("appsettings.json", false, true);
		return builder.Build();
	}

	private static IServiceCollection ConfigureServices(this IConfigurationRoot configuration, FormsPlot formsPlot)
	{
		return new ServiceCollection()
			.Configure<DataReaderControllerSettings>(configuration.GetSection(nameof(DataReaderControllerSettings)))
			.Configure<DataStorageSettings>(configuration.GetSection(nameof(DataStorageSettings)))
			.AddFormVariables(formsPlot)
			.AddDomainComponents()
			.AddControllers();
	}

	private static IServiceCollection AddFormVariables(this IServiceCollection services, FormsPlot formsPlot)
	{
		services.AddSingleton<FormsPlot>(_ => formsPlot);
		services.AddSingleton<Plot>(_ => formsPlot.Plot);
		services.AddTransient<IPalette>(_ => new ScottPlot.Palettes.Category20());
		services.AddSingleton<CoordinateController>();

		return services;
	}

	private static IServiceCollection AddDomainComponents(this IServiceCollection services)
	{
		services.AddSingleton<IDataReader<Spectra>, SpectraFileReader>();
		services.AddSingleton<ISpectraParser, SpectraParser>();
		services.AddSingleton<IDataReader<PeakBordersSet>, PeakBordersSetReader>();
		services.AddSingleton<IDataWriter, FileWriter>(_ => new FileWriter(FileMode.Create));
		services.AddSingleton<IPlotBuilder<Spectra, SpectraPlot>, ScottSpectraPlotBuilder>();
		services.AddSingleton<IPlotBuilder<PeakBorders, PeakBorderPlot>, ScottPeakBorderPlotBuilder>();
		services.AddSingleton<IPlotDrawer<SctPlot>, ScottPlotDrawer>();
		return services;
	}

	private static IServiceCollection AddControllers(this IServiceCollection services)
	{
		services.AddSingleton<IDialogController, DialogController>();
		services.AddSingleton<IDataSourceController<Spectra>, DirectoryDataSourceController<Spectra>>();
		services.AddSingleton<IDataWriterController, DirectoryDataWriterController>();
		services.AddSingleton<IDataStorageController<Spectra>, DataStorageController<Spectra>>();
		services.AddSingleton<IDataStorageController<SpectraPlot>, DataStorageController<SpectraPlot>>();
		services.AddSingleton<IGraphicsController<SpectraPlot>, ScottSpectraGraphicsController>();
		services.AddSingleton<ISpectraProcessingController, SpectraProcessingController>();
		services.AddSingleton<ICoordinateController, CoordinateController>();
		services.AddSingleton<IPlotController, PlotController>();
		return services;
	}
}