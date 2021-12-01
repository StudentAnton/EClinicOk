using System;
using System.Threading.Tasks;
using EClinic.Models.Domain;
using EClinic.Models.ViewModels;
using EClinic.Managers;
using EClinic.Data;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace EClinic.Managers
{
    public class MeettingsManager
    {
        private readonly ClinicContext _ClinicContext;
        private readonly DoctorsManager _DoctorsManager;
        private readonly IMapper _Mapper;

        public MeettingsManager(ClinicContext clinicContext, DoctorsManager doctorsManager, IMapper mapper)
        {
            _DoctorsManager = doctorsManager;
            _ClinicContext = clinicContext;
            _Mapper = mapper;
        }

        public async Task<string> CreateMeettingAsync(MeetingCreateViewModel model, int patientId)
        {
            if((await _ClinicContext.
                GetMeettingsOnDayTimeForDoctorTypeAsync(model.MeettingDate, model.DoctorTypeId)).Count == 0)
            {
                var doctor = await _DoctorsManager.GetFreeDoctorOnDateTime(model.MeettingDate, model.DoctorTypeId);
                if(doctor == null)
                {
                    return "No such doctor";
                }
                Meetting meetting = new Meetting();
                meetting.MeettingDate = model.MeettingDate;
                meetting.MeettingStatusId = 1;
                meetting.Doctor = doctor;
                meetting.PatientId = patientId;
                await _ClinicContext.CreateMeetingAsync(meetting);
                return "";
            }
            return "All doctors are busy";
        }

        public async Task CancelMeettingAsync(int meetingId)
        {
            var meeting = await _ClinicContext.GetMeettingAsync(meetingId);
            if(meeting != null)
            {
                meeting.MeettingStatusId = 3;
                await _ClinicContext.UpdateMeetingAsync(meeting);
            }
        }

        public async Task<List<List<MeetingForTableViewModel>>> GetWeekListForDoctor(string userId)
        {
            Doctor doctor = await _ClinicContext.GetDoctorByUserIDAsync(userId);

            var weekMeetings = await _ClinicContext.GetMeettingsOnWeekForAsync(doctor.Id, true);

            List<List<MeetingForTableViewModel>> result = new List<List<MeetingForTableViewModel>>();

            DateTime dateTimeIndex = DateTime.Today.Date;
            for(int day = 0; day < 7; day++)
            {
                result.Add(new List<MeetingForTableViewModel>());
                for(int hour = 9; hour < 19; hour++)
                {
                    var meeting = weekMeetings.FirstOrDefault(m => m.MeettingDate == dateTimeIndex.AddDays(day).AddHours(hour));
                    if(meeting != null)
                    {
                        result[day].Add(_Mapper.Map<MeetingForTableViewModel>(meeting));
                    }
                    else
                    {
                        result[day].Add(null);
                    }
                }
            }

            return result;
        }

        public async Task<List<MeetingForListViewModel>> GetWeekListForPatient(string userId)
        {
            Patient patient = await _ClinicContext.GetPatientByUserIdAsync(userId);

            List<Meetting> weekMeetings = await _ClinicContext.GetMeettingsOnWeekForAsync(patient.Id);
            var result = weekMeetings.Select(e => _Mapper.Map<MeetingForListViewModel>(e)).ToList();
            foreach (var item in result)
            {
                item.PatientId = patient.Id;
            }
            return result;
        }
    }
}
