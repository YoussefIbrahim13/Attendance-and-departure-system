using Applications.CSVFile.Commandss.EditPendingAttendance;
using Applications.CSVFile.Commandss.SavePendingAttendanceCommand;
using Applications.CSVFile.DTOS.AttendanceRecord;
using Applications.CSVFile.Querys.UploadCSVFilequery;
using Applications.DailyAttendance.Querys;
using Applications.Employees.Commands.AddEmployees;
using Applications.Employees.Commands.DeleteEmployee;
using Applications.Employees.Commands.UpdataEmployeecommand;
using Applications.Employees.Commands.UploadProfileImagecommand;
using Applications.Employees.Querys.GetEmployeeByCode;
using Applications.Employees.Querys.GetEmployeesquery;
using Applications.MonthView.Querys.GetMonthViewquery;
using Applications.PlanAttendance.Command;
using Applications.UpdateAttendanceRecord.Commands;
using Applications.YearView.Querys;
using AutoMapper;
using Domain.Entities;
using EmployeesModels.Shared;
using Infrastructure_;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static MudBlazor.CategoryTypes;
using AttendanceSystem.ImportFile.API.Controllers.Dto;
using Infrastructure_.DBContext;

namespace AttendanceSystem.ImportFile.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;


        private static List<AttendanceRecordDto> _pendingAttendance = new();

        public AttendanceController(IMediator mediator , ApplicationDbContext context, IMapper mapper)
        {
            _mediator = mediator;
            _context = context;
            _mapper = mapper;
        }

        // ===================== CSV Upload =====================
        [HttpPost("upload-csv")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCsv([FromForm] UploadCsvDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("No file uploaded.");

            var query = new UploadCSVFilequery(dto.File);
            var result = await _mediator.Send(query);

            _pendingAttendance = result;

            return Ok(result);
        }

        [HttpPut("edit-pending")]
        public async Task<IActionResult> EditPendingAttendance([FromBody] EditPendingAttendanceCommand command)
        {
            if (command == null)
                return BadRequest("Invalid request data.");

            var result = await _mediator.Send(command);

            return Ok(new { Message = result });
        }
        [HttpPost("save")]
        public async Task<IActionResult> SavePendingAttendance([FromBody] List<AttendanceRecord> pendingAttendance)
        {
            if (pendingAttendance == null || !pendingAttendance.Any())
                return BadRequest("Pending attendance data is required.");

            // 1. تحويل من DTO → Entity
            var entities = _mapper.Map<List<AttendanceRecord>>(pendingAttendance);

            // 2. الحفظ في الداتا بيز
            _context.AttendanceRecords.AddRange(entities);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Saved Successfully" });
        }


        // ===================== Employee Queries =====================
        [HttpGet("employee-by-code/{code}")]
        public async Task<IActionResult> GetEmployeeByCode(string code)
        {
            var result = await _mediator.Send(new GetEmployeeByCodeQuery { Code = code });
            if (result == null)
                return NotFound("Employee not found.");
            return Ok(result);
        }

        // ===================== Day / Month / Year Views =====================

        [HttpGet("day-view")]
        public async Task<IActionResult> GetDayView([FromQuery] DateTime date)
        {
            var query = new GetDayViewquery(date);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        [HttpGet("month-view")]
        public async Task<IActionResult> GetMonthView(int year, int month)
        {
            var query = new GetMonthViewquery(year, month);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("year-view/{year}")]
        public async Task<IActionResult> GetYearView(int year)
        {
            var query = new GetYearViewquery(year);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // ===================== Employee Commands =====================
        [HttpPost("add-employee")]
        public async Task<IActionResult> AddEmployee([FromBody] AddEmployeesCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }
        [HttpPut("update-employee")]
        public async Task<IActionResult> UpdateEmployee([FromBody] UpdataEmployeecommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Code))
                return BadRequest("Employee Code is required.");

            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }


        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees()
        {
            var query = new GetEmployeesquerys();
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        [HttpDelete("delete-employee/{Code}")]
        public async Task<IActionResult> DeleteEmployee(string code)
        {
            var result = await _mediator.Send(new DeleteEmployeeCommand { Code = code });
            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result.Message);
        }

        [HttpPost("plan-attendance")]
        public async Task<IActionResult> PlanAttendance([FromBody] PlanAttendancecommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        [HttpPut("update-attendance-record")]
        public async Task<IActionResult> UpdateAttendanceRecord([FromBody] UpdateAttendanceRecordcommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(true);
        }
        [HttpPost("upload-profile-image/{employeeCode}")]
        public async Task<IActionResult> UploadProfileImage(string employeeCode, [FromForm] UploadProfileImageDto dto)
        {
            var command = new UploadProfileImageCommand
            {
                EmployeeCode = employeeCode,
                File = dto.File,
                HttpContext = HttpContext
            };
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(new { message = result.Message });
            return Ok(new { imageUrl = result.ImageUrl, message = result.Message });
        }



    }
}
