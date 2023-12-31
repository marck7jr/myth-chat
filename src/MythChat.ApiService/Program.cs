using Carter;

using FluentValidation;

using MythChat.ApiService.Configuration;
using MythChat.ApiService.Extensions;
using MythChat.ApiService.Features.Chat;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddRedisDistributedCache("redis");

builder.Services.ConfigureOptions<ChatOptionsBuilder>();
builder.Services.ConfigureOptions<SemanticKernelOptionsBuilder>();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddCarter();
builder.Services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));
builder.Services.AddSemanticKernel();

builder.Services.AddChatFeature();

var app = builder.Build();

app.MapCarter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseStaticFiles();

app.MapDefaultEndpoints();

app.Run();