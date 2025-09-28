using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos
{
    public class CreateVacationRequestDto
    {
        public string UserId { get; set; }

        [Required]
        public DateTime FromTime { get; set; }

        [Required]
        public DateTime ToTime { get; set; }

        [Required]
        [StringLength(500)]
        public string? Reason { get; set; }


    }
}
