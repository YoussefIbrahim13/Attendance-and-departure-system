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
using EmployeesModels.Shared;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;
using Domain.Entities;
using Domain.Enums;


namespace AttendanceSystem.ImportFile.ui.Pages
{
    public partial class AttendanceImport
    {

        IBrowserFile? selectedFile;
        List<AttendanceRecord>? pendingAttendance;
        AttendanceRecord? editRow;
        TimeSpan editCheckInTime = TimeSpan.Zero;
        TimeSpan editCheckOutTime = TimeSpan.Zero;
        AttendanceStatus? editActualStatus;
        AttendanceStatus editStatusValue
        {
            get => editActualStatus ?? AttendanceStatus.Present;
            set => editActualStatus = value;
        }
        string? editNote;
        ApprovalStatus editApprovalStatus;
        string? message;
        bool isSavingAll = false;

        // Sorting and searching
        string? searchId;
        SortDirection sortDirection = SortDirection.Ascending;

        public enum SortColumn
        {
            EmployeeId,
            Date
        }

        SortColumn selectedSortColumn = SortColumn.EmployeeId;
        SortDirection selectedSortDirection = SortDirection.Ascending;
        SortDirection appliedSortDirection = SortDirection.Ascending;
        SortColumn appliedSortColumn = SortColumn.EmployeeId;
        bool isSorting = false;

        IEnumerable<AttendanceRecord> filteredAttendance
        {
            get
            {
                var query = (pendingAttendance ?? Enumerable.Empty<AttendanceRecord>())
                    .Where(x => string.IsNullOrEmpty(searchId) || x.Code.ToString() == searchId);
                if (appliedSortColumn == SortColumn.EmployeeId)
                    return appliedSortDirection == SortDirection.Ascending ? query.OrderBy(x => x.Code) : query.OrderByDescending(x => x.Code);
                else if (appliedSortColumn == SortColumn.Date)
                    return appliedSortDirection == SortDirection.Ascending ? query.OrderBy(x => x.Date) : query.OrderByDescending(x => x.Date);
                else
                    return query;
            }
        }
        string? GetUserRoleFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

             var handler = new JwtSecurityTokenHandler();
             var jwtToken = handler.ReadJwtToken(token);

            // ابحث عن claim الدور (role أو roles)
             var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == "roles");
            return roleClaim?.Value;
        }
        [Inject] NavigationManager Navigation { get; set; }
        [Inject] Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
       
        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateTask;
            var user = authState.User;

            string[] roles = { "Admin" };

            if (!user.Identity.IsAuthenticated || !roles.Any(role => user.IsInRole(role)))
            {
                Navigation.NavigateTo("/access-denied");
                return;
            }

            var token = await LocalStorage.GetItemAsync<string>("authToken");
            var role = GetUserRoleFromToken(token);
            Console.WriteLine($"Role in token: {role}");
        }

         
        

        async Task ApplySortAsync()
        {

            isSorting = true;
            StateHasChanged();
            await Task.Delay(400); // لمحاكاة وقت السورت
            appliedSortColumn = selectedSortColumn;
            appliedSortDirection = selectedSortDirection;
            isSorting = false;
            StateHasChanged();
        }

        void OnFileChange(InputFileChangeEventArgs e)
        {
            selectedFile = e.File;
        }

        async Task UploadFile()
        {
            if (selectedFile == null) return;
            var content = new MultipartFormDataContent();
            var stream = selectedFile.OpenReadStream(10 * 1024 * 1024); // 10MB max
            content.Add(new StreamContent(stream), "file", selectedFile.Name);
            pendingAttendance = await AttendanceService.UploadCsvAsync(content);
            // Set ApprovalStatus to Pending for all imported records
            if (pendingAttendance != null)
            {
                foreach (var rec in pendingAttendance)
                {
                    rec.ApprovalStatus = ApprovalStatus.Pending;
                }
                Snackbar.Add("File uploaded successfully", Severity.Success);
            }
            else
            {
                Snackbar.Add("File upload failed", Severity.Error);
            }
            StateHasChanged();
        }

        void StartEdit(AttendanceRecord rec)
        {
            editRow = rec;
            editCheckInTime = rec.CheckIn;
            editCheckOutTime = rec.CheckOut;
            editActualStatus = rec.ActualStatus;
            editApprovalStatus = rec.ApprovalStatus;
            editNote = rec.Note;
        }

        void CancelEdit()
        {
            editRow = null;
        }

        async Task SaveEdit(AttendanceRecord rec)
        {
            if (editRow == null) return;
            var dto = new EditAttendanceDto
            {
                Code = rec.Code,
                Date = rec.Date,
                CheckIn = editCheckInTime,
                CheckOut = editCheckOutTime,
                ActualStatus = editActualStatus ?? rec.ActualStatus,
                ApprovalStatus = editApprovalStatus,
                Note = editNote
            };
            var ok = await AttendanceService.EditPendingAttendanceAsync(dto);
            if (ok)
            {
                rec.CheckIn = editCheckInTime;
                rec.CheckOut = editCheckOutTime;
                rec.Note = editNote;
                rec.ActualStatus = editActualStatus ?? rec.ActualStatus;
                rec.ApprovalStatus = editApprovalStatus;
                editRow = null;
                Snackbar.Add("Edit saved successfully", Severity.Success);
            }
            else
            {
                Snackbar.Add("Edit failed", Severity.Error);
            }
        }

        async Task SaveAll()
        {
            isSavingAll = true;
            StateHasChanged();
            var ok = false;
            try
            {
                ok = await AttendanceService.SaveAttendanceAsync(pendingAttendance);
            }
            catch
            {
                ok = false;
            }
            if (ok)
            {
                Snackbar.Add("Saved successfully", Severity.Success);
                pendingAttendance = null;
            }
            else
            {
                Snackbar.Add("Save failed", Severity.Error);
            }
            isSavingAll = false;
            StateHasChanged();
        }
    }
}