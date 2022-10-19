using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace ExpenseTracker.Pages
{
    public partial class IncomeDatePage : ComponentBase
    {
        [Inject] IHelperService HelperService {get;set;} = null!;
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
            var res = HelperService.DbQuery($@"INSERT INTO IncomeDateAndAmount VALUES('{CurrentIncomeDateAndAmount.IncomeDate}',
                                                    {CurrentIncomeDateAndAmount.IncomeAmount})");
            await LoadData();
        }

        public async Task ViewExpenseDetail(IncomeDateAndAmount model)
        {
            CurrentIncomeDateAndAmountToAddExpenseDetail = model;
            IsAddNewExpenseDetail = true;
        }

        public async Task SaveExpenseDetail()
        {
            var res = HelperService.DbQuery($@"INSERT INTO ExpenseDetails VALUES({CurrentIncomeDateAndAmountToAddExpenseDetail.Id},
                                                    {CurrentExpenseDetail.PayableId},
                                                    {CurrentExpenseDetail.Amount},
                                                    '{CurrentExpenseDetail.Comment}')");
            await ViewExpenseDetail(CurrentIncomeDateAndAmountToAddExpenseDetail);

            var ExpenseDetailQuery = HelperService.DbQuery($@"select * from ExpenseDetails");
            ExpenseDetailsList = JsonConvert.DeserializeObject<List<ExpenseDetail>>(ExpenseDetailQuery);

            CurrentExpenseDetail = new();

            StateHasChanged();
        }
    }
}