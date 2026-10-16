using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MidtermProject;
using MidtermProject.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// App state (singletons live for the lifetime of the browser tab)
builder.Services.AddSingleton<AuctionService>();
builder.Services.AddSingleton<CartService>();
builder.Services.AddSingleton<CatalogService>();
builder.Services.AddSingleton<OrderService>();

await builder.Build().RunAsync();
