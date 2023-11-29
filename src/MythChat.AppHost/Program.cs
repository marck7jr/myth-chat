var builder = DistributedApplication.CreateBuilder(args);

var apiservice = builder.AddProject<Projects.MythChat_ApiService>("apiservice");

builder.AddProject<Projects.MythChat_Web>("webfrontend")
    .WithReference(apiservice);

builder.Build().Run();
