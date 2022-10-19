using Microsoft.AspNetCore.Components;
using ExpenseTracker.Services;
using ExpenseTracker.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.JSInterop;

namespace ExpenseTracker.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject] NavigationManager NavigationManager {get;set;} = null!;
        protected override async Task OnInitializedAsync()
        {
            NavigationManager.NavigateTo("payables");
            await Task.CompletedTask;
        }
    }
}
