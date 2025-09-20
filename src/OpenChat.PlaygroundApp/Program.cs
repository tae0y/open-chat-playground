using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Components;

using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

var config = builder.Configuration;
var settings = ArgumentOptions.Parse(config, args);
if (settings.Help == true)
{
    ArgumentOptions.DisplayHelp();
    return;
}

builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

// Configure Language Model Client
var chatClient = await LanguageModelConnector.CreateChatClientAsync(settings);
IDistributedCache cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

builder.Services.AddChatClient(chatClient)
                .UseDistributedCache(cache)
                .UseFunctionInvocation()
                .UseLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseStaticFiles();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

await app.RunAsync();
