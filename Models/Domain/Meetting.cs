using System;
namespace EClinic.Models.Domain
{
    public class Meetting
    {
        public int Id { get; set; }
        public Doctor Doctor { get; set; }
        public int DoctorId { get; set; }
        public Patient Patient { get; set; }
        public int PatientId { get; set; }
        public DateTime MeettingDate { get; set; }
        public int MeettingStatusId { get; set; }
        public MeettingStatus MeettingStatus { get; set; }
    }
}
