using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EClinic.Models.ViewModels
{
    public class MeetingCreateViewModel
    {
        [Required]
        public int DoctorTypeId { get; set; }
        [Required]
        public DateTime MeettingDate { get; set; }
        public List<DateTime> AllowedTimes { get; set; }
        public string ErrorMessage { get; set; }
    }
}
