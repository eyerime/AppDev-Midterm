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

## Database (Supabase)

Schema: `database/schema.sql`. Tables: `fixed_items`, `auctions`, `bids`, `orders`, `order_items`.

### Storage patterns
- **`fixed_items`: soft delete.** When a listing is bought, the row is not removed; `deleted_at` is filled in and the shop query skips it (`WHERE deleted_at IS NULL`). Past orders still point to the item through `order_items.item_id`, so order history keeps working, and a mistaken removal can be undone by clearing `deleted_at`.
- **`bids`: append-only log.** A bid is only ever added. Bids are never edited or deleted, which gives each auction a trustworthy history. The app is not even granted update or delete on this table.

### Fast habits
- Indexes on the columns we filter, sort and join on (name, category, ends_at_utc, bids.auction_id, order_items.order_number).
- The database filters (`deleted_at IS NULL`) and pages (`Range(0, 199)`) instead of the app.
- No request inside a loop: auctions + bids load in 2 requests, orders + lines + items in 3, and soft-deleting several sold items is a single PATCH.

### Setup
1. Run `database/schema.sql` in the Supabase SQL Editor.
2. Put the Project URL and **publishable** key in `wwwroot/appsettings.json`.
3. `dotnet run`. The first launch fills the tables from `SeedData`.
