using Azure.Identity;
using Serilog;
using WebHybridClient;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.AzureApp()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web host");

    var builder = WebApplication.CreateBuilder(args);

    builder.WebHost
        .ConfigureKestrel(serverOptions => { serverOptions.AddServerHeader = false; })
        .ConfigureAppConfiguration((context, configurationBuilder) =>
        {
            var config = configurationBuilder.Build();
            var azureKeyVaultEndpoint = config["AzureKeyVaultEndpoint"];
            if (!string.IsNullOrEmpty(azureKeyVaultEndpoint))
            {
                // Add Secrets from KeyVault
                Log.Information("Use secrets from {AzureKeyVaultEndpoint}", azureKeyVaultEndpoint);
                configurationBuilder.AddAzureKeyVault(new Uri(azureKeyVaultEndpoint), new DefaultAzureCredential());
            }
            else
            {
                // Add Secrets from UserSecrets for local development
                configurationBuilder.AddUserSecrets("81795b76-afe1-496b-bb97-8835d573e9c4");
            }
        });

    builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(context.Configuration));

    var app = builder
        .ConfigureServices()
        .ConfigurePipeline();

    app.Run();
}
catch (Exception ex) when (ex.GetType().Name is not "StopTheHostException"
    && ex.GetType().Name is not "HostAbortedException")
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
