using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Nexora.TaxInspection.Application.Interfaces;
using Nexora.TaxInspection.Application.Services;
using Nexora.TaxInspection.Infrastructure.Messaging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddScoped<ITaxService, TaxService>();
builder.Services.AddHostedService<RabbitMqConsumerBackgroundService>();

var host = builder.Build();
host.Run();
