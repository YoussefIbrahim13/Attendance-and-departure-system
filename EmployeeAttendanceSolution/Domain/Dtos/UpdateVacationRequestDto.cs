using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos
{
    public class UpdateVacationRequestDto
    {
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public string? Reason { get; set; }
    }
}
