var builder = DistributedApplication.CreateBuilder(args);
builder.AddProject<Projects.OpenChat_PlaygroundApp>("webchat");

builder.Build().Run();
