using System;
namespace EClinic.Models.ViewModels
{
    public class MeetingForListViewModel
    {
        public string DoctorName { get; set; }
        public string DoctorType { get; set; }
        public DateTime MeettingDate { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string Cabinet { get; set; }
    }
}
