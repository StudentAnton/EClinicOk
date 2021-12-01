using System;
using EClinic.Models.ViewModels;
using EClinic.Models.Domain;
using EClinic.Data;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using System.Threading.Tasks;

namespace EClinic.Managers
{
    public class MedicalCardsManager
    {
        private readonly ClinicContext _ClinicContext;
        private readonly IMapper _Mapper;

        public MedicalCardsManager(IMapper mapper, ClinicContext clinicContext)
        {
            _ClinicContext = clinicContext;
            _Mapper = mapper;
        }

        public async Task SaveMedicalCard(CreateMedicalCardViewModel model)
        {
            var domainMedicalCard = _Mapper.Map<MedicalCard>(model);
            domainMedicalCard.UpdatedAt = DateTime.Now;
            if(domainMedicalCard.Id == 0)
            {
                domainMedicalCard.CreatedAt = DateTime.Now;
            }
            await _ClinicContext.SaveMedicalCardAsync(domainMedicalCard);
        }


    }
}
