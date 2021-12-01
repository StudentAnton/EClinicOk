using System;
namespace EClinic.Models.ViewModels
{
    public class MettingForManagingListViewModel
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorTypeName { get; set; }
        public int DoctorTypeId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime MeettingDate { get; set; }
        public int MeettingStatusId { get; set; }
        public string MeettingStatusName { get; set; }
    }
}
