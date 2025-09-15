//namespace AttendanceSystem.ImportFile.ui.Shared
//{
//    public enum AttendanceStatus
//    {
//        Present,
//        Absent,
//        Vacation,
//        WorkFromHome,
//        //Sick,
//        //Late,
//        //EarlyLeave
//    }


using AttendanceSystem.ImportFile.ui.Services;
using Domain.Entities;
using Domain.Enums;
using EmployeesModels.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AttendanceSystem.ImportFile.ui.Pages
{
    public partial class AddEmployeeDialog
    {

        // Add these parameters at the top of your code block
        [Parameter]
        public DateTime InitialDate { get; set; }

        [Parameter]
        public DateTime? DefaultDateFrom { get; set; }

        [Parameter]
        public DateTime? DefaultDateTo { get; set; }

        [CascadingParameter]
        IMudDialogInstance MudDialog { get; set; } = null!;

        private EmployeeDto? selectedEmployee;

        private List<AttendanceSystem.ImportFile.ui.Services.EmployeeDto> employees = new();
        private bool isProcessing;

        [CascadingParameter]
        public IDialogReference DialogReference { get; set; } = default!;

        private MudForm form = null!;
        private bool isFormValid;
        private DateTime? dateFrom;
        private DateTime? dateTo;
        private AttendanceStatus actualStatus = AttendanceStatus.No_status;
        private AttendanceStatus plannedStatus = AttendanceStatus.No_status;

        protected override async Task OnInitializedAsync()
        {
            // Set dates from parameters or fall back to InitialDate
            dateFrom = DefaultDateFrom ?? InitialDate;
            dateTo = DefaultDateTo ?? InitialDate;
            await LoadEmployees();
        }

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task LoadEmployees()
        {
            employees = await AttendanceService.GetAllEmployeesAsync();
        }
        private Task<IEnumerable<EmployeeDto>> SearchEmployees(string value, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Task.FromResult(employees.AsEnumerable());

            return Task.FromResult(employees.Where(x =>
                x.Name.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                x.Code.Contains(value, StringComparison.OrdinalIgnoreCase)));
        }

        private async Task Submit()
        {
            if (!form.IsValid || !dateFrom.HasValue || !dateTo.HasValue || selectedEmployee == null)
            {
                return;
            }

            if (dateTo.Value < dateFrom.Value)
            {
                Snackbar.Add("Date_To cannot be before Date_From", Severity.Error);
                return;
            }

            isProcessing = true;

            try
            {
                bool allSuccess = true;
                for (var d = dateFrom.Value.Date; d <= dateTo.Value.Date; d = d.AddDays(1))
                {
                    var record = new AttendanceRecord
                    {
                        Code = selectedEmployee.Code,
                        Date = d,
                        ActualStatus = actualStatus,
                        PlannedStatus = plannedStatus,
                        // يمكن إضافة خصائص أخرى مثل ApprovalStatus, CheckIn, CheckOut, Note إذا أردت
                    };
                    var success = await AttendanceService.UpdateEmployeeAttendanceRecordAsync(record);
                    if (!success)
                    {
                        allSuccess = false;
                    }
                }

                if (allSuccess)
                {
                    Snackbar.Add("Attendance status updated successfully", Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    Snackbar.Add("Some days failed to update attendance status", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error: {ex.Message}", Severity.Error);
            }
            finally
            {
                isProcessing = false;
            }
        }
    }
}