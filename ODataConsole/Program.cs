using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ODataConsole;
using ODataConsole.Infrastructure;
using ODataConsole.Screens;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging(logging => logging.AddFile("logs/{Date}.txt"));

builder.Services.AddScreens();
builder.Services.AddInfrastructure();

builder.Services.AddHostedService<App>();

var host = builder.Build();

await host.RunAsync();