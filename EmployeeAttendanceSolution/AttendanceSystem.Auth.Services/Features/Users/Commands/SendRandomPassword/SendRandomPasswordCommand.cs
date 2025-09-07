using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystem.Auth.Services.Features.Users.Commands.SendRandomPassword
{
    public record SendRandomPasswordCommand(string To) : IRequest<SendRandomPasswordResponse>;

    public class SendRandomPasswordResponse
    {
        public string Password { get; set; } = string.Empty;
        public bool EmailSent { get; set; }
        public string? Error { get; set; }
    }
}
