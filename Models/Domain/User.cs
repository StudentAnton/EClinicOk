using System;
using Microsoft.AspNetCore.Identity;

namespace EClinic.Models.Domain
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
    }
}
