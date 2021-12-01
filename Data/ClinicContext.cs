using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EClinic.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EClinic.Data
{
    public class ClinicContext : IdentityDbContext<User>
    {
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorType> DoctorTypes { get; set; }
        public DbSet<MedicalCard> MedicalCards { get; set; }
        public DbSet<Meetting> Meettings { get; set; }
        public DbSet<MeettingStatus> MeettingStatuses { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<User> Users { get; set; }

        public ClinicContext(DbContextOptions<ClinicContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DoctorType>(e =>
            {
                e.ToTable("DoctorTypes", "meta");
                e.HasData(new DoctorType() { Id = 1, Name = "Лікар педіатр" },
                new DoctorType() { Id = 2, Name = "Сімейний лікар" },
                new DoctorType() { Id = 3, Name = "ЛОР" },
                new DoctorType() { Id = 4, Name = "Хірург" },
                new DoctorType() { Id = 5, Name = "Дерматолог" },
                new DoctorType() { Id = 6, Name = "Стоматолог" });
            });

            builder.Entity<MeettingStatus>(e =>
            {
                e.ToTable("MeettingStatus", "meta");
                e.HasData(new MeettingStatus() { Id = 1, Name = "Заплановано" },
                new MeettingStatus() { Id = 2, Name = "В процесі" },
                new MeettingStatus() { Id = 3, Name = "Відмінено" },
                new MeettingStatus() { Id = 4, Name = "Проведено" });
            });


            base.OnModelCreating(builder);
        }

        public async Task<Doctor> GetDoctorAsync(int Id)
        {
            return await Doctors.FindAsync(Id);
        }

        public async Task<Doctor> GetDoctorByUserIDAsync(string userId)
        {
            return await Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
        }

        public async Task<List<Doctor>> GetDoctorsByTypesAsync(int typeId)
        {
            return await Doctors.Where(d => d.DoctorTypeId == typeId).ToListAsync();
        }

        public async Task SaveMedicalCardAsync(MedicalCard medicalCard)
        {
            MedicalCards.Add(medicalCard);
            if (medicalCard.Id != 0)
            {
                var existed = await MedicalCards.FindAsync(medicalCard.Id);
                Entry(medicalCard).State = EntityState.Modified;
            }
            await SaveChangesAsync();
        }

        public async Task<MedicalCard> GetMedicalCardAsync(int doctorId, int patientId)
        {
            return await MedicalCards.FirstOrDefaultAsync(mc => mc.DoctorId == doctorId && mc.PatientId == patientId);
        }

        public async Task CreateMeetingAsync(Meetting meetting)
        {
            await Meettings.AddAsync(meetting);
            await SaveChangesAsync();
        }

        public async Task UpdateMeetingAsync(Meetting meetting)
        {
            Meettings.Update(meetting);
            await SaveChangesAsync();
        }

        public async Task<Meetting> GetMeettingAsync(int Id)
        {
            return await Meettings.FindAsync(Id);
        }

        public Task<List<Meetting>> GetMeettingsAsync(int? doctorTypeId, string doctorName, DateTime? date)
        {
            if (!doctorTypeId.HasValue && string.IsNullOrEmpty(doctorName) && !date.HasValue)
            {
                return Task.FromResult(new List<Meetting>());
            }
            IQueryable<Meetting> result = Meettings
                .Include(e => e.Doctor.User).Include(e => e.Patient.User)
                .Include(e => e.MeettingStatus).Include(e => e.Doctor.DoctorType);

            if (doctorTypeId.HasValue)
            {
                result = result.Where(e => e.Doctor.DoctorTypeId == doctorTypeId);
            }
            if (!string.IsNullOrEmpty(doctorName))
            {
                result = result.Where(e => e.Doctor.User.FullName.Contains(doctorName));
            }
            if (date.HasValue)
            {
                result = result.Where(e => e.MeettingDate.Date == date.Value.Date);

                if (date.Value.Hour != 0)
                {
                    result = result.Where(e => e.MeettingDate == date.Value);
                }
            }
            return result.ToListAsync();
        }

        public async Task<List<Meetting>> GetMeettingsOnDayAsync(DateTime dateTime)
        {
            return await Meettings.Where(m => m.MeettingDate.Date == dateTime.Date && m.MeettingStatusId != 3).ToListAsync();
        }

        public async Task<List<Meetting>> GetMeettingsOnDayForDoctorTypeAsync(DateTime dateTime, int doctorTypeId)
        {
            return await Meettings.Where(m => m.MeettingDate.Date == dateTime.Date && m.Doctor.DoctorTypeId == doctorTypeId && m.MeettingStatusId != 3).Include(p => p.Doctor).ToListAsync();
        }

        public async Task<List<Meetting>> GetMeettingsOnDayTimeForDoctorTypeAsync(DateTime dateTime, int doctorTypeId)
        {
            return await Meettings.Where(m => m.MeettingDate == dateTime && m.Doctor.DoctorTypeId == doctorTypeId && m.MeettingStatusId != 3).Include(p => p.Doctor).ToListAsync();
        }

        public async Task<List<Meetting>> GetMeettingsOnWeekForAsync(int personId, bool isDoctor = false)
        {
            List<Meetting> result = new List<Meetting>();
            DateTime indexDay = DateTime.Today.Date;
            for (int intIndexDay = 0; intIndexDay < 7; intIndexDay++)
            {
                var tempRes = Meettings
                    .Where(m => m.MeettingDate.Date == indexDay.AddDays(intIndexDay).Date
                        && m.MeettingStatusId != 3)
                    .OrderBy(q => q.MeettingDate).Include(a => a.Patient.User);
                if (isDoctor)
                {
                    result.AddRange(await tempRes.Where(e => e.DoctorId == personId).ToListAsync());
                }
                else
                {
                    result.AddRange(await tempRes.Where(e => e.PatientId == personId)
                        .Include(p => p.Doctor.User).Include(p => p.Doctor.DoctorType).ToListAsync());
                }
            }
            return result;
        }

        public async Task<Patient> GetPatientByUserIdAsync(string userId)
        {
            return await Patients.FirstOrDefaultAsync(p => p.UserId == userId);
        }

    }
}
