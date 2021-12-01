using System;
using System.Threading.Tasks;
using System.Linq;
using EClinic.Data;
using EClinic.Models.Domain;
using System.Collections.Generic;

namespace EClinic.Managers
{
    public class DoctorsManager
    {
        private readonly ClinicContext _ClinicContext;

        public DoctorsManager(ClinicContext clinicContext)
        {
            _ClinicContext = clinicContext;
        }

        public async Task<Doctor> GetFreeDoctorOnDateTime(DateTime dateTime, int doctorTypeId)
        {
            List<Meetting> dayMeettings = await _ClinicContext.GetMeettingsOnDayForDoctorTypeAsync(dateTime, doctorTypeId);

            List<Doctor> allDoctors = dayMeettings.Select(m => m.Doctor).ToList();

            return allDoctors.FirstOrDefault(d => dayMeettings.Any(m => d.DoctorTypeId != m.DoctorId));
        }

        public async Task<List<DateTime>> GetFreeTimeMeetingOnDay(DateTime dateTime, int doctorTypeId)
        {
            List<DateTime> result = new List<DateTime>();
            List<Meetting> dayMeettings = await _ClinicContext.GetMeettingsOnDayForDoctorTypeAsync(dateTime, doctorTypeId);

            int numberOfDoctorsOfTypes = (await _ClinicContext.GetDoctorsByTypesAsync(doctorTypeId)).Count();
            if(numberOfDoctorsOfTypes == 0)
            {
                return result;
            }

            DateTime endDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 18, 0, 0);

            for (DateTime iterator = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 9, 0, 0); iterator < endDate; iterator = iterator.AddHours(1))
            {
                var timeMeeting = dayMeettings.Where(m => m.MeettingDate == iterator);
                if(timeMeeting.Count() < numberOfDoctorsOfTypes)
                {
                    result.Add(iterator);
                }    
            }

            return result;
        }
    }
}
