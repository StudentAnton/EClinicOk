using System;
namespace EClinic.Models.ViewModels
{
    public class CreateDoctorViewModel : SignInViewModel
    {
        public int DoctorTypeId { get; set; }
        public string Cabinet { get; set; }
    }
}
