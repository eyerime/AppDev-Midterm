using Microsoft.AspNetCore.Components;
using MidtermProject.Services;

namespace MidtermProject.Components;

/// <summary>
/// Base class for components that show live auction data. Re-renders on every
/// <see cref="AuctionService.Changed"/> tick (countdowns, new bids).
/// </summary>
public abstract class LiveComponentBase : ComponentBase, IDisposable
{
    [Inject] protected AuctionService Service { get; set; } = default!;

    protected override void OnInitialized() => Service.Changed += HandleChanged;

    private void HandleChanged() => _ = InvokeAsync(StateHasChanged);

    public virtual void Dispose() => Service.Changed -= HandleChanged;
}
