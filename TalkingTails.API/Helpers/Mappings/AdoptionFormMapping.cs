using TalkingTails.API.Models.AdoptionForms;
using TalkingTails.Business.Models.AdoptionForms;
using TalkingTails.Repository.Entities;

namespace TalkingTails.API.Helpers.Mappings
{
    public static class AdoptionFormMapping
    {
        public static AdoptionForm ToAdoptionForm(this CreateAdoptFormRequest request)
        {
            return new AdoptionForm
            {
                PetId = request.PetId,
                FullName = request.FullName,
                ContactPhone = request.ContactPhone,
                ContactEmail = request.ContactEmail,
                ContactAddress = request.ContactAddress,
                LivingConditions = request.LivingConditions,
                HasOtherPets = request.HasOtherPets,
                AvailableContactTime = request.AvailableContactTime
            };
        }

        public static AdoptionForm ToAdoptionForm(this UpdateAdoptFormRequest request)
        {
            return new AdoptionForm
            {
                Id = request.Id,
                FullName = request.FullName,
                ContactPhone = request.ContactPhone,
                ContactEmail = request.ContactEmail,
                ContactAddress = request.ContactAddress,
                LivingConditions = request.LivingConditions,
                HasOtherPets = request.HasOtherPets,
                AvailableContactTime = request.AvailableContactTime
            };
        }

        public static FormListRequestDto ToFormListRequestDto(this OrganAdoptFormListRequest request)
        {
            return new FormListRequestDto
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                FilterByStatus = request.FilterByStatus,
                Sort = request.Sort
            };
        }
    }
}