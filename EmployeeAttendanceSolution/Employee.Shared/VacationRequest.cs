using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesModels.Shared
{
    public class VacationRequest
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string UserId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime FromTime { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ToTime { get; set; }

        [StringLength(500)]
        public string Reason { get; set; }

        public VacationRequestStatus Status { get; set; } = VacationRequestStatus.Pending;

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual ApplicationUser User { get; set; }
    }
}
