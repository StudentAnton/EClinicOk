using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EClinic.Models.ViewModels;
using EClinic.Models.Domain;
using EClinic.Data;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using EClinic.Managers;

namespace EClinic.Controllers
{
    [Authorize]
    public class MedicalCardsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ClinicContext _ClinicContext;
        private readonly MedicalCardsManager _MedicalCardsManager;
        private readonly IMapper _Mapper;


        public MedicalCardsController(UserManager<User> userManager, MedicalCardsManager medicalCardsManager, IMapper mapper, ClinicContext clinicContext)
        {
            _userManager = userManager;
            _MedicalCardsManager = medicalCardsManager;
            _Mapper = mapper;
            _ClinicContext = clinicContext;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult> CreateAsync(CreateMedicalCardViewModel model)
        {
            if(ModelState.IsValid)
            {
                var doctor = await _ClinicContext.GetDoctorByUserIDAsync((await _userManager.FindByNameAsync(User.Identity.Name)).Id);
                model.DoctorId = doctor.Id;
                await _MedicalCardsManager.SaveMedicalCard(model);
                return RedirectToAction("ShowMeetingsForDoctor", "Meetings");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> ShowMedicalCard(int? doctorId, int patientId)
        {
            if(!doctorId.HasValue)
            {
                doctorId = (await _ClinicContext.GetDoctorByUserIDAsync((await _userManager.FindByNameAsync(User.Identity.Name)).Id)).Id;
            }
            var medicalCard = await _ClinicContext.GetMedicalCardAsync(doctorId.Value, patientId);
            return View(medicalCard != null ? _Mapper.Map<CreateMedicalCardViewModel>(medicalCard) : new CreateMedicalCardViewModel());
        }
    }
}
