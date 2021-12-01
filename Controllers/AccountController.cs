using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EClinic.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EClinic.Models.ViewModels;
using EClinic.Data;
using Microsoft.AspNetCore.Authorization;

namespace EClinic.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _Mapper;
        private readonly ClinicContext _ClinicContext;
        public static List<DoctorType> _DoctorTypesCache;

        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager,
            IMapper mapper, ClinicContext clinicContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _Mapper = mapper;
            _roleManager = roleManager;
            _ClinicContext = clinicContext;
            _DoctorTypesCache = _ClinicContext.DoctorTypes.ToList();
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Main()
        {
            var user = _ClinicContext.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            var result = _Mapper.Map<ShowUserViewModel>(user);
            result.Roles = string.Join(',', await _userManager.GetRolesAsync(user));
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = _Mapper.Map<User>(model);
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Patient");
                    _ClinicContext.Patients.Add(new Patient() { UserId = user.Id });
                    _ClinicContext.SaveChanges();
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Main");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByEmailAsync(model.Email);
                if(user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Main");
                    }
                }
                
                ModelState.AddModelError("", "Wrong email or password.");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("LogIn", "Account");
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.DoctorTypes = _DoctorTypesCache;
            return View();
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            List<UserIndexViewModel> users = new List<UserIndexViewModel>();
            
            foreach(var user in _userManager.Users)
            {
                var userIndex = _Mapper.Map<UserIndexViewModel>(user);
                IList<string> roles = await _userManager.GetRolesAsync(user);
                userIndex.Role = string.Join(',', roles);
                users.Add(userIndex);
            }
            return View(users);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(CreateDoctorViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = _Mapper.Map<User>(model);
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Doctor");
                    var doctor = _Mapper.Map<Doctor>(model);
                    doctor.UserId = user.Id;
                    _ClinicContext.Doctors.Add(doctor);
                    _ClinicContext.SaveChanges();
                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }
    }
}
