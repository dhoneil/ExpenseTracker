using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ExpenseTracker.Services;
using ExpenseTracker.Models;
using Newtonsoft.Json;

namespace ExpenseTracker.Pages
{
    public partial class PayablePage : ComponentBase
    {
        [Inject] IHelperService HelperService { get; set; } = null!;
        public List<Payable>? Payables { get; set; } = new();
        public bool IsAddNewPayable { get; set; } = true;
        public Payable CurrentPayable { get; set; } = new();
        public bool IsChecked { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            CurrentPayable.IsRecuring = false;
            await LoadData();
        }

        public async Task LoadData()
        {
            var payablesquery = HelperService.DbQuery($@"select * from Payable");
            Payables = JsonConvert.DeserializeObject<List<Payable>>(payablesquery);
        }

        public async Task SavePayable()
        {
            var isrecuring = (CurrentPayable.IsRecuring.Value == true ? 1 : 0);

            if (CurrentPayable.Payableid > 0 )
            {
                HelperService.DbQuery($@"UPDATE Payable SET Payablename = '{CurrentPayable.Payablename}',
                                        IsRecuring = {isrecuring},
                                        Amount = {(CurrentPayable.Amount.HasValue ? CurrentPayable.Amount : 0)}
                                        where Payableid  = {CurrentPayable.Payableid}");
                await LoadData();
            }
            else
            {
                HelperService.DbQuery($@"INSERT INTO Payable
                                    values('{CurrentPayable.Payablename}',
                                    {(CurrentPayable.IsRecuring.HasValue ? isrecuring : 0)},
                                    {(CurrentPayable.Amount.HasValue ? CurrentPayable.Amount : 0)})");
                Payables?.Add(CurrentPayable);
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