using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EClinic.Models.ViewModels;
using EClinic.Managers;
using EClinic.Data;
using EClinic.Models.Domain;
using AutoMapper;

namespace EClinic.Controllers
{
    [Authorize]
    public class MeetingsController : Controller
    {
        private readonly UserManager<User> _UserManager;
        private readonly MeettingsManager _MeettingsManager;
        private readonly DoctorsManager _DoctorsManager;
        private readonly IMapper _Mapper;
        private readonly ClinicContext _ClinicContext;

        public MeetingsController(MeettingsManager meettingsManager,
            UserManager<User> userManager, DoctorsManager doctorsManager,
            ClinicContext clinicContext,
            IMapper mapper)
        {
            _MeettingsManager = meettingsManager;
            _UserManager = userManager;
            _DoctorsManager = doctorsManager;
            _ClinicContext = clinicContext;
            _Mapper = mapper;
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.DoctorTypes = AccountController._DoctorTypesCache;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(MeetingCreateViewModel model)
        {
            ViewBag.DoctorTypes = AccountController._DoctorTypesCache;
            if (!ModelState.IsValid || model.MeettingDate <= DateTime.Now || model.MeettingDate.Minute != 0 || model.MeettingDate.Hour < 9 || model.MeettingDate.Hour > 17)
            {
                ModelState.AddModelError(nameof(model.MeettingDate), "Uncorrect input date");
                return View(model);
            }

            var currentUser = await _UserManager.FindByNameAsync(User.Identity.Name);
            var patient = await _ClinicContext.GetPatientByUserIdAsync(currentUser.Id);
            var result = await _MeettingsManager.CreateMeettingAsync(model, patient.Id);

            if(!string.IsNullOrEmpty(result))
            {
                model.AllowedTimes = await _DoctorsManager.GetFreeTimeMeetingOnDay(model.MeettingDate, model.DoctorTypeId);
                ModelState.AddModelError(nameof(model.DoctorTypeId), result);
                return View(model);
            }
            else
            {
                return RedirectToAction("ShowMeetingsForPatient");
            }    
        }
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult> ShowMeetingsForDoctor()
        {
            var meetings = await _MeettingsManager.GetWeekListForDoctor((await _UserManager.FindByNameAsync(User.Identity.Name)).Id);
            return View(meetings);
        }

        [Authorize(Roles = "Patient")]
        public async Task<ActionResult> ShowMeetingsForPatient()
        {
            var meetings = await _MeettingsManager.GetWeekListForPatient((await _UserManager.FindByNameAsync(User.Identity.Name)).Id);
            return View(meetings);
        }

        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> ManageMeetings(MeetingManageViewModel model = null)
        {
            if(model == null)
            {
                model = new MeetingManageViewModel();
            }
            var meetings = await _ClinicContext.GetMeettingsAsync(model.doctorTypeId, model.doctorName, model.date);
            ViewBag.DoctorTypes = AccountController._DoctorTypesCache;
            model.ModelsList = meetings.Select(m => _Mapper.Map<MettingForManagingListViewModel>(m)).ToList();
            return View(model);
        }

        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> CancelMeetingAsync(int meetingId)
        {
            await _MeettingsManager.CancelMeettingAsync(meetingId);
            return RedirectToAction("ManageMeetings");
        }
    }
}
