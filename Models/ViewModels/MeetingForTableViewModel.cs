using System;
namespace EClinic.Models.ViewModels
{
    public class MeetingForTableViewModel
    {
        public DateTime MeettingDate { get; set; }
        public string PatientName { get; set; }
        public int PatientId { get; set; }
        public int StatusId { get; set; }
    }
}
