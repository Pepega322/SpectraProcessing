using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScottPlot.WinForms;
using SpectraProcessing.Application.Extensions;
using SpectraProcessing.Bll.Extensions;
using SpectraProcessing.Dal.Extensions;

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
            .AddApplication(plot)
            .AddBll(configuration)
            .AddDal(configuration);
    }
}
