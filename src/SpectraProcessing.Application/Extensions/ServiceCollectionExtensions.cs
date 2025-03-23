using Microsoft.Extensions.DependencyInjection;
using ScottPlot;
using ScottPlot.WinForms;
using SpectraProcessing.Application.Controllers;
using SpectraProcessing.Bll.Controllers.Interfaces;

namespace SpectraProcessing.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        FormsPlot plot)
    {
        services
            .AddWinformSpecificVariables(plot)
            .AddWinformSpecificControllers();

        return services;
    }

    private static IServiceCollection AddWinformSpecificVariables(
        this IServiceCollection services,
        FormsPlot plot)
    {
        services
            .AddSingleton<FormsPlot>(_ => plot)
            .AddSingleton<Plot>(_ => plot.Plot)
            .AddTransient<IPalette>(_ => new ScottPlot.Palettes.Category20());

        return services;
    }

    private static IServiceCollection AddWinformSpecificControllers(this IServiceCollection services)
    {
        services.AddSingleton<IPeakController, WinformPeakController>();
        services.AddTransient<IDialogController, WinformDialogController>();

        return services;
    }
}
