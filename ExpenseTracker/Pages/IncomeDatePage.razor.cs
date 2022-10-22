using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Microsoft.JSInterop;

namespace ExpenseTracker.Pages
{
    public partial class IncomeDatePage : ComponentBase
    {
        [Inject] IHelperService HelperService {get;set;} = null!;
        [Inject] IJSRuntime JS {get;set;} = null!;
        public List<IncomeDateAndAmount>? IncomeDateAndAmounts { get; set; } = new();
        public List<ExpenseDetail>? ExpenseDetailsList { get; set; } = new();
        public List<Payable>? PayablesList { get; set; } = new();
        public IncomeDateAndAmount CurrentIncomeDateAndAmount { get; set; } = new();
        public bool IsAddNewExpenseDetail { get; set; }= false;
        public ExpenseDetail CurrentExpenseDetail { get; set; } = new();
        public IncomeDateAndAmount CurrentIncomeDateAndAmountToAddExpenseDetail { get; set; } = new();
        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        public async Task LoadData()
        {
            var IncomeDateAndAmountQuery = HelperService.DbQuery($@"select * from IncomeDateAndAmount");
            IncomeDateAndAmounts = JsonConvert.DeserializeObject<List<IncomeDateAndAmount>>(IncomeDateAndAmountQuery);

            var ExpenseDetailQuery = HelperService.DbQuery($@"select * from ExpenseDetails");
            ExpenseDetailsList = JsonConvert.DeserializeObject<List<ExpenseDetail>>(ExpenseDetailQuery);

            var PayableQuery = HelperService.DbQuery($@"select * from Payable");
            PayablesList = JsonConvert.DeserializeObject<List<Payable>>(PayableQuery);

            await Task.CompletedTask;
        }

        public async Task Save()
        {
            if (CurrentIncomeDateAndAmount.Id > 0)
            {
                HelperService.DbQuery($@"UPDATE IncomeDateAndAmount set IncomeDate = '{CurrentIncomeDateAndAmount.IncomeDate}',
                                        IncomeAmount = {CurrentIncomeDateAndAmount.IncomeAmount}
                                        where Id = {CurrentIncomeDateAndAmount.Id}");
            }else
            {
                HelperService.DbQuery($@"IF NOT EXISTS(SELECT 1 FROM IncomeDateAndAmount WHERE IncomeDate = '{CurrentIncomeDateAndAmount.IncomeDate}'
                                    and IncomeAmount = {CurrentIncomeDateAndAmount.IncomeAmount})
                                        INSERT INTO IncomeDateAndAmount VALUES('{CurrentIncomeDateAndAmount.IncomeDate}',
                                                    {CurrentIncomeDateAndAmount.IncomeAmount})");
            }

            await LoadData();
        }

        public async Task DeleteIncomeDate(IncomeDateAndAmount model)
        {
            bool confirmed = await JS.InvokeAsync<bool>("confirm", "Delete this income date?");
            if (confirmed)
            {
                HelperService.DbQuery($"DELETE FROM IncomeDateAndAmount where Id = {model.Id}");
                HelperService.DbQuery($"DELETE FROM ExpenseDetails where IncomeDateAndAmountId = {model.Id}");
                await LoadData();
            }
        }

        public async Task ViewExpenseDetail(IncomeDateAndAmount model)
        {
            CurrentIncomeDateAndAmountToAddExpenseDetail = model;
            await Task.CompletedTask;
        }

        async Task RefreshExpenseDetails()
        {
            IsAddNewExpenseDetail = true;
            await ViewExpenseDetail(CurrentIncomeDateAndAmountToAddExpenseDetail);
            var ExpenseDetailQuery = HelperService.DbQuery($@"select * from ExpenseDetails");
            ExpenseDetailsList = JsonConvert.DeserializeObject<List<ExpenseDetail>>(ExpenseDetailQuery);
        }

        public async Task SaveExpenseDetail()
        {
            if (CurrentExpenseDetail.Id > 0)
            {
                HelperService.DbQuery($@"UPDATE ExpenseDetails SET PayableId = {CurrentExpenseDetail.PayableId},
                                        Amount = {CurrentExpenseDetail.Amount},
                                        Comment = '{CurrentExpenseDetail.Comment}'
                                        where Id = {CurrentExpenseDetail.Id}");
            }else
            {
                HelperService.DbQuery($@"INSERT INTO ExpenseDetails VALUES({CurrentIncomeDateAndAmountToAddExpenseDetail.Id},
                                                    {CurrentExpenseDetail.PayableId},
                                                    {CurrentExpenseDetail.Amount},
                                                    '{CurrentExpenseDetail.Comment}')");
            }

            await RefreshExpenseDetails();

            CurrentExpenseDetail = new();

            StateHasChanged();
        }

        public async Task DeleteExpenseDetail(ExpenseDetail detail)
        {
            bool confirmed = await JS.InvokeAsync<bool>("confirm", "Delete this expense?");
            if (confirmed)
            {
                HelperService.DbQuery($"DELETE FROM ExpenseDetails where Id = {detail.Id}");

                await RefreshExpenseDetails();
            }
        }

        public async Task AddAllRecuring(IncomeDateAndAmount incomedate)
        {
            CurrentIncomeDateAndAmountToAddExpenseDetail = incomedate;
            var getRcurringPayables = HelperService.DbQuery($@"SELECT * from Payable where IsRecuring = 1");
            var recurringPayables = JsonConvert.DeserializeObject<List<Payable>>(getRcurringPayables);

            //delete all first
            //HelperService.DbQuery($@"DELETE FROM ExpenseDetails where IncomeDateAndAmountId = {CurrentIncomeDateAndAmountToAddExpenseDetail.Id}");
            foreach (var item in recurringPayables)
            {
                HelperService.DbQuery($@"IF NOT EXISTS(SELECT 1 FROM ExpenseDetails WHERE payableid = {item.Payableid} and IncomeDateAndAmountId = {CurrentIncomeDateAndAmountToAddExpenseDetail.Id})
                                        INSERT INTO ExpenseDetails VALUES({CurrentIncomeDateAndAmountToAddExpenseDetail.Id},
                                                    {item.Payableid},
                                                    {(item.Amount.HasValue ? item.Amount : 0)},
                                                    'added from recurring')");
            }

            await RefreshExpenseDetails();
        }

        public async Task SetExpenseStatus(ExpenseDetail detail)
        {
            var status = (detail.IsPaid.HasValue ? (detail.IsPaid.Value == true ? 0 : 1) : 1 );
            HelperService.DbQuery($@"UPDATE ExpenseDetails SET IsPaid = {status}
                                        where Id = {detail.Id}");

            await RefreshExpenseDetails();

            StateHasChanged();
        }
    }
}