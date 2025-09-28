using Applications.Employees.DTO.EmployeeDtos;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Employees.Commands.UploadProfileImagecommand;
    public class UploadProfileImageCommand : IRequest<UploadProfileImageResultDto>
    {


        public string EmployeeCode { get; set; }
        public IFormFile File { get; set; }
        public HttpContext HttpContext { get;set; }

      
}

