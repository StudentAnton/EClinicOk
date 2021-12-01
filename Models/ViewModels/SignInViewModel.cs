using System;
using System.ComponentModel.DataAnnotations;

namespace EClinic.Models.ViewModels
{
    public class SignInViewModel
    {
        [Required(ErrorMessage = "UserName cannot be blank."),
            MinLength(5, ErrorMessage = "Too short (Minimum 5 characters)."),
            MaxLength(25, ErrorMessage = "Too long (Maximum 25 characters).")]
        public string UserName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email cannot be blank.")]
        public string Email { get; set; }

        [MinLength(10, ErrorMessage = "Not correct"),
            MaxLength(250, ErrorMessage = "Too long fullname.")]
        [Required(ErrorMessage = "Fill fullname.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Phone number cannot be empty.")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password cannot be blank.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords should be the same.")]
        [Required(ErrorMessage = "Password Confirmation cannot be blank.")]
        [DataType(DataType.Password)]
        public string PasswordConfirmation { get; set; }

    }
}
