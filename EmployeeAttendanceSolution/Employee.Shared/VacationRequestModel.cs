using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesModels.Shared
{
    public class VacationRequestModel
    {

        [Required(ErrorMessage = "Start date is required")]
        public DateTime? FromTime { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime? ToTime { get; set; }

        [Required(ErrorMessage = "Reason is required")]
        [StringLength(200, ErrorMessage = "Reason must be shorter than 200 characters")]
        public string? Reason { get; set; }
    }
}
