using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Microsoft.JSInterop;
using ExpenseTracker.Repository;
using ExpenseTracker.ViewModel;

namespace ExpenseTracker.Pages
{
    public partial class IncomeDatePage : ComponentBase
    {
        [Inject] IHelperService HelperService {get;set;} = null!;
        [Inject] IJSRuntime JS {get;set;} = null!;
        public List<IncomeDateAndAmount>? IncomeDateAndAmounts { get; set; } = new();
        public List<ExpenseDetail>? ExpenseDetailsList { get; set; } = new();
        public List<Payable>? RecurringPayablesList { get; set; } = new();
        public List<Payable>? PayablesList { get; set; } = new();
        public IncomeDateAndAmount CurrentIncomeDateAndAmount { get; set; } = new();
        public bool IsAddNewExpenseDetail { get; set; }= false;
        public ExpenseDetail CurrentExpenseDetail { get; set; } = new();
        public IncomeDateAndAmount CurrentIncomeDateAndAmountToAddExpenseDetail { get; set; } = new();
        public bool IsAddNewPayable { get; set; } =  false;
        public string NewPayable { get; set; } = string.Empty;
        public List<ExpenseDetailVM> ExpenseDetailVMList { get; set; } = new();
        IncomeDateAndAmountRepository repoIncomeDate = new IncomeDateAndAmountRepository();
        ExpenseDetailRepository repoExpenseDetail = new ExpenseDetailRepository();
        PayableRepository repoPayable = new PayableRepository();
        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        public async Task LoadData()
        {
            IncomeDateAndAmounts = repoIncomeDate.GetAll().ToList();
            ExpenseDetailsList = repoExpenseDetail.GetAll().ToList();
            PayablesList = repoPayable.GetAll().ToList();
            RecurringPayablesList = repoPayable.GetAll().Where(s=>s.IsRecuring.Value == true && s.IsComplete.Value == false).ToList();
            await Task.CompletedTask;
        }

        public async Task Save()
        {
            if (CurrentIncomeDateAndAmount.Id > 0)
            {
                repoIncomeDate.Update(CurrentIncomeDateAndAmount);
            }else
            {
                repoIncomeDate.Create(CurrentIncomeDateAndAmount);
            }

            CurrentExpenseDetail = new();
            await LoadData();
        }

        public async Task DeleteIncomeDate(IncomeDateAndAmount model)
        {
            bool confirmed = await JS.InvokeAsync<bool>("confirm", "Delete this income date?");
            if (confirmed)
            {
                repoExpenseDetail.Delete(s=>s.IncomeDateAndAmountId == model.Id);
                repoIncomeDate.Delete(s=>s.Id == model.Id);
                await LoadData();
            }
            CurrentExpenseDetail = new();
        }

        public async Task ViewExpenseDetail(IncomeDateAndAmount model)
        {
            CurrentIncomeDateAndAmountToAddExpenseDetail = model;
            List<ExpenseDetailVM> expenseDetailVMs = new List<ExpenseDetailVM>();
            var expendetails = repoExpenseDetail.GetAll().ToList();
            await Task.CompletedTask;
        }

        async Task RefreshExpenseDetails()
        {
            IsAddNewExpenseDetail = true;
            await ViewExpenseDetail(CurrentIncomeDateAndAmountToAddExpenseDetail);
            ExpenseDetailsList = repoExpenseDetail.GetAll().ToList();
        }

        public async Task OnChangePayablesDropdown(int payabledSelected)
        {
            var selectedPayable = payabledSelected;
            CurrentExpenseDetail.PayableId = selectedPayable;
            CurrentExpenseDetail.Amount = repoPayable.Get(selectedPayable)?.Amount;
            await Task.CompletedTask;
        }

        public async Task SaveExpenseDetail()
        {
            if (CurrentExpenseDetail.Id > 0)
            {
                repoExpenseDetail.Update(CurrentExpenseDetail);
            }
            else
            {
                var flag = true;
                if (IsAddNewPayable)
                {
                    Payable payable = new Payable{
                        Payablename = NewPayable,
                        IsRecuring = false,
                        Amount = (String.IsNullOrEmpty(Convert.ToString(CurrentExpenseDetail.Amount)) ? 0 : CurrentExpenseDetail.Amount),
                        IsComplete  = false
                    };
                    repoPayable.Create(payable);
                    var lastcreated = repoPayable.GetAll().ToList().Last();
                    PayablesList.Add(lastcreated);
                    CurrentExpenseDetail.PayableId = lastcreated.Payableid;
                    IsAddNewPayable = false;
                    NewPayable = string.Empty;
                }

                if (repoExpenseDetail.GetAll()
                                     .ToList()
                                     .Where(s=>s.PayableId == CurrentExpenseDetail.PayableId &&
                                               s.Amount == CurrentExpenseDetail.Amount)
                                               .Count() > 0)
                {
                    flag = false;
                    await JS.InvokeVoidAsync("alert", "Expense with the same amount is already added");
                    return;
                }


                if (flag)
                {
                    CurrentExpenseDetail.IncomeDateAndAmountId = CurrentIncomeDateAndAmountToAddExpenseDetail.Id;
                    CurrentExpenseDetail.IsPaid = false;
                    repoExpenseDetail.Create(CurrentExpenseDetail);
                }
            }

            await RefreshExpenseDetails();
            CurrentExpenseDetail = new();
            StateHasChanged();
        }

        public async Task EditExpenseDetail(ExpenseDetail detail)
        {
            //await OnChangePayablesDropdown((int)detail.PayableId);
            await JS.InvokeVoidAsync("SelectOption","PayablesDropdown",detail.PayableId);
            CurrentExpenseDetail = detail;
        }

        public async Task DeleteExpenseDetail(ExpenseDetail detail)
        {
            bool confirmed = await JS.InvokeAsync<bool>("confirm", "Delete this expense?");
            if (confirmed)
            {
                repoExpenseDetail.Delete(s=>s.Id == detail.Id);

                await RefreshExpenseDetails();
            }
        }

        public async Task AddAllRecuring(IncomeDateAndAmount incomedate)
        {
            CurrentIncomeDateAndAmountToAddExpenseDetail = incomedate;
            var recurringPayables = repoPayable.GetAll().ToList().Where(s=>s.IsRecuring == true);

            List<ExpenseDetail> expenseDetails = new List<ExpenseDetail>();
            foreach (var item in recurringPayables)
            {
                if (!repoExpenseDetail.GetAll()
                                    .Any(s=>s.PayableId == item.Payableid &&
                                           s.IncomeDateAndAmountId == CurrentIncomeDateAndAmountToAddExpenseDetail.Id))
                {
                    if (item.EndDate > DateTime.Now || item.EndDate is null)
                    {
                        ExpenseDetail expenseDetail = new ExpenseDetail{
                            IncomeDateAndAmountId = CurrentIncomeDateAndAmountToAddExpenseDetail.Id,
                            PayableId = item.Payableid,
                            Amount = (item.Amount.HasValue ? item.Amount : 0),
                            Comment = "added from recurring",
                            IsPaid = false,
                        };
                        expenseDetails.Add(expenseDetail);
                    }
                }
            }
            repoExpenseDetail.Create(expenseDetails);

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