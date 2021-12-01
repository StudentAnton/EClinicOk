using System;
using System.Collections.Generic;

namespace EClinic.Models.Domain
{
    public class Doctor
    {
        public int Id { get; set; }
        public int DoctorTypeId { get; set; }
        public string Cabinet { get; set; }
        public DoctorType DoctorType { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        public List<MedicalCard> MedicalCards { get; set; }
        public List<Meetting> Meettings { get; set; }
    }
}
