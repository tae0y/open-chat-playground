using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Components;
using OpenChat.PlaygroundApp.Endpoints;
using OpenChat.PlaygroundApp.OpenApi;
using OpenChat.PlaygroundApp.Services;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var settings = ArgumentOptions.Parse(config, args);
ArgumentOptions.DisplayBanner();
if (settings.Help == true)
{
    ArgumentOptions.DisplayHelp();
    return;
}

builder.Services.AddSingleton(settings!);

builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

// Register ChatClientFactory as Singleton + IHostedService for lazy-loaded IChatClient
builder.Services.AddSingleton<ChatClientFactory>();
builder.Services.AddSingleton<IChatClientFactory>(sp => sp.GetRequiredService<ChatClientFactory>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<ChatClientFactory>());
builder.Services.AddScoped<IConnectorStateManager, ConnectorStateManager>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddOpenApi("openapi", options =>
{
    options.AddDocumentTransformer<OpenApiDocumentTransformer>();
});

builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    app.UseHttpsRedirection();
}

app.UseAntiforgery();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/{documentName}.json");
}

var group = app.MapGroup("/api");
app.MapEndpoints(group);

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

await app.RunAsync();
