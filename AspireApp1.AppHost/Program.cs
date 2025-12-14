var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.AspireApp1_ApiService>("apiservice");
var dbApiService = builder.AddProject<Projects.AspireApp1_DbApi>("dbapi");

var webFrontend = builder.AddProject<Projects.AspireApp1_Web>("webfrontend");

webFrontend.WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WithReference(dbApiService);

webFrontend.WaitFor(apiService);
webFrontend.WaitFor(dbApiService);

builder.Build().Run();
