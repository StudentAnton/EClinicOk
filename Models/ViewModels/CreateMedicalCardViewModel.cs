using System;
using System.ComponentModel.DataAnnotations;

namespace EClinic.Models.ViewModels
{
    public class CreateMedicalCardViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Content { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
