using System;
namespace EClinic.Models.Domain
{
    public class MedicalCard
    {
        public int Id { get; set; }
        public Patient Patient { get; set; }
        public int PatientId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
    }
}
