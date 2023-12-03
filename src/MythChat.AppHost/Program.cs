var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedisContainer("redis");

var api = builder.AddProject<Projects.MythChat_ApiService>("api")
    .WithReference(cache);

builder.AddProject<Projects.MythChat_Web>("web")
    .WithReference(api)
    .WithReference(cache);

builder.Build().Run();
