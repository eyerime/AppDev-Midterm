using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MidtermProject;
using MidtermProject.Data;
using MidtermProject.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Supabase client (publishable key only; never put a secret key in the browser)
var url = builder.Configuration["Supabase:Url"]!;
var key = builder.Configuration["Supabase:Key"]!;
var supabase = new Supabase.Client(url, key, new Supabase.SupabaseOptions { AutoConnectRealtime = false });
await supabase.InitializeAsync();
builder.Services.AddSingleton(supabase);
builder.Services.AddSingleton<SupabaseDb>();

// App state (singletons live for the lifetime of the browser tab)
builder.Services.AddSingleton<AuctionService>();
builder.Services.AddSingleton<CartService>();
builder.Services.AddSingleton<CatalogService>();
builder.Services.AddSingleton<OrderService>();

var host = builder.Build();

// Fill empty tables from SeedData (first run only), then load everything from Supabase.
await host.Services.GetRequiredService<SupabaseDb>().SeedIfEmptyAsync();
await host.Services.GetRequiredService<AuctionService>().LoadAsync();
await host.Services.GetRequiredService<CatalogService>().LoadAsync();
await host.Services.GetRequiredService<OrderService>().LoadAsync();

await host.RunAsync();
