var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Ligot_ApiService>("apiservice");
var dbApiService = builder.AddProject<Projects.Ligot_BackEnd>("dbapi");

var webFrontend = builder.AddProject<Projects.Ligot_FrontEnd>("webfrontend");

webFrontend.WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WithReference(dbApiService);

webFrontend.WaitFor(apiService);
webFrontend.WaitFor(dbApiService);

builder.Build().Run();

