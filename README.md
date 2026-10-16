# Resound — Blazor WebAssembly (C#)

Second-hand instrument marketplace + live auctions, ported from the Figma export to Blazor WASM (.NET 8).

## Run

```bash
dotnet run
```
Then open http://localhost:5107 (see `Properties/launchSettings.json`). Requires the .NET 8 SDK and internet access
(Google Fonts and Unsplash images are loaded from CDNs).

## Structure

| Folder | Purpose |
| --- | --- |
| `Models/` | Plain C# types: `Auction`, `BidEntry`, `FixedItem`, `Order`, `UserProfile`, enums |
| `Data/` | `SeedData` — demo data from the Figma file |
| `Services/` | `AuctionService` (live state, 1s ticker, bidding), `CartService`, `CatalogService`, `OrderService` |
| `Helpers/` | `Format` — money and countdown formatting |
| `Components/` | Reusable UI: `AuctionCard`, `FixedCard`, `BidRow`, `BidBadge`, `SectionHead`, `Icon` |
| `Layout/` | `MainLayout`, `NavBar`, `Footer` |
| `Pages/` | Routable pages: `Home` (/), `Auctions` (/auctions), `AuctionDetail` (/auctions/{id}), `Profile` (/profile), `Sell` (/sell), `ItemDetail` (/items/{id}), `Cart` (/cart), `Checkout` (/checkout), `OrderConfirmation` (/order/{number}) |
| `wwwroot/css/app.css` | Design tokens + shared primitives; component styles are scoped in `*.razor.css` |

Components that show countdowns inherit `LiveComponentBase`, which re-renders on every `AuctionService.Changed` tick.
State is in-memory only and resets on reload.

## Second-hand transparency

Every listing can carry a `ConditionReport` (`Models/ConditionReport.cs`): year made, owners, usage, reason for selling,
original retail price (for a "% below new" check), disclosed wear and flaws with severity, a tested/working checklist,
what's included, non-original parts, and serial/receipt verification. A 0–100 transparency score summarises how much
the seller disclosed. Sellers fill this in on `/sell` (with a live score and tips); buyers see it on the product and auction
pages (`Components/ConditionReportView`). The reports on the demo listings are invented sample data.
