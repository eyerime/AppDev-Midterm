-- Resound: database/schema.sql
-- Supabase Dashboard -> SQL Editor -> New query -> paste -> Run.
-- Safe to re-run (tables use IF NOT EXISTS, policies are dropped first).

-- ---------------------------------------------------------------- tables --
-- Ids are TEXT because the app uses ids like 'f1', 'a1', 'u3fa9c2'.
-- Money is INTEGER because the app uses whole-number prices.

create table if not exists public.fixed_items (
  id          text primary key,
  name        text not null,
  brand       text not null,
  category    text not null,                       -- ItemCategory name, e.g. 'Guitars'
  condition   text not null,
  price       integer not null check (price >= 0),
  img         text not null,
  is_yours    boolean not null default false,
  report      jsonb,                               -- ConditionReport (read as a whole, never filtered on)
  deleted_at  timestamptz,                         -- SOFT DELETE: set when sold/removed
  created_at  timestamptz not null default now()
);

create table if not exists public.auctions (
  id            text primary key,
  name          text not null,
  brand         text not null,
  condition     text not null,
  img           text not null,
  starting_bid  integer not null check (starting_bid >= 0),
  ends_at_utc   timestamptz not null,
  is_yours      boolean not null default false,
  bid_count     integer not null default 0 check (bid_count >= 0),
  leader        text not null default 'Other' check (leader in ('Other', 'You')),
  your_bid      integer,
  report        jsonb,
  created_at    timestamptz not null default now()
);

-- APPEND-ONLY LOG: bids are only ever added, never edited or deleted.
create table if not exists public.bids (
  id          bigint generated always as identity primary key,
  auction_id  text not null references public.auctions(id) on delete cascade,
  who         text not null,
  amount      integer not null check (amount > 0),
  ago_label   text,                                -- only for seeded demo bids ("2h ago")
  created_at  timestamptz not null default now()
);

create table if not exists public.orders (
  number       text primary key,                   -- 'RS-48213'
  item_name    text not null,
  status_text  text not null,
  total        integer not null check (total >= 0),
  shipping     integer not null default 0 check (shipping >= 0),
  details      jsonb,                              -- CheckoutDetails (shipping address)
  created_at   timestamptz not null default now()
);

-- Each fact once: an order points at its items instead of copying them.
create table if not exists public.order_items (
  id            bigint generated always as identity primary key,
  order_number  text not null references public.orders(number) on delete cascade,
  item_id       text not null references public.fixed_items(id),
  price         integer not null check (price >= 0)   -- price at the time of purchase
);

-- --------------------------------------------------------------- indexes --
-- Habit 1: index the columns the app filters / sorts / joins on.
create index if not exists fixed_items_name_idx     on public.fixed_items (name);
create index if not exists fixed_items_category_idx on public.fixed_items (category) where deleted_at is null;
create index if not exists auctions_ends_idx        on public.auctions (ends_at_utc);
create index if not exists bids_auction_idx         on public.bids (auction_id, id desc);
create index if not exists orders_created_idx       on public.orders (created_at desc);
create index if not exists order_items_order_idx    on public.order_items (order_number);

-- ----------------------------------------------------------------- locks --
-- Lock 1 (GRANT) + Lock 2 (RLS). These open policies are for class only.
do $$
declare t text;
begin
  foreach t in array array['fixed_items', 'auctions', 'orders', 'order_items'] loop
    execute format('grant select, insert, update, delete on public.%I to anon', t);
    execute format('alter table public.%I enable row level security', t);
    execute format('drop policy if exists "read_all"   on public.%I', t);
    execute format('drop policy if exists "insert_all" on public.%I', t);
    execute format('drop policy if exists "update_all" on public.%I', t);
    execute format('drop policy if exists "delete_all" on public.%I', t);
    execute format('create policy "read_all"   on public.%I for select to anon using (true)', t);
    execute format('create policy "insert_all" on public.%I for insert to anon with check (true)', t);
    execute format('create policy "update_all" on public.%I for update to anon using (true) with check (true)', t);
    execute format('create policy "delete_all" on public.%I for delete to anon using (true)', t);
  end loop;
end $$;

-- bids is append-only, so the app may only read and add.
grant select, insert on public.bids to anon;
alter table public.bids enable row level security;
drop policy if exists "read_all"   on public.bids;
drop policy if exists "insert_all" on public.bids;
create policy "read_all"   on public.bids for select to anon using (true);
create policy "insert_all" on public.bids for insert to anon with check (true);
