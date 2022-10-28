using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ExpenseTracker.Services;
using ExpenseTracker.Models;
using Newtonsoft.Json;
using ExpenseTracker.Repository;

namespace ExpenseTracker.Pages
{
    public partial class PayablePage : ComponentBase
    {
        [Inject] IHelperService HelperService { get; set; } = null!;
        public List<Payable>? PayablesRecurring { get; set; } = new();
        public bool IsAddNewPayable { get; set; } = true;
        public Payable CurrentPayable { get; set; } = new();
        public bool IsChecked { get; set; } = false;
        PayableRepository repoPayable = new PayableRepository();

        protected override async Task OnInitializedAsync()
        {
            CurrentPayable.IsRecuring = false;
            await LoadData();
        }

        public async Task LoadData()
        {
            PayablesRecurring = repoPayable.GetAll().Where(s=>s.IsRecuring == true).ToList();
            await Task.CompletedTask;
        }

        public async Task SavePayable()
        {
            var isrecuring = (CurrentPayable.IsRecuring.Value == true ? 1 : 0);

            if (CurrentPayable.Payableid > 0 )
            {
                repoPayable.Update(CurrentPayable);
                await LoadData();
            }
            else
            {
                repoPayable.Create(CurrentPayable);
                PayablesRecurring?.Add(CurrentPayable);
            }

            CurrentPayable = new();
            StateHasChanged();
        }

        public async Task EditPayable(Payable model)
        {
            CurrentPayable = model;
            await Task.CompletedTask;
        }
    }
}