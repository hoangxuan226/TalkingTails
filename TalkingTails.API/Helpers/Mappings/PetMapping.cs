using TalkingTails.API.Models.Pets;
using TalkingTails.Business.Models.Pets;

namespace TalkingTails.API.Helpers.Mappings
{
    public static class PetMapping
    {
        public static PetListRequestDto ToListRequestDto(this PetListRequest request)
        {
            return new PetListRequestDto
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                SearchByName = request.SearchByName,
                Sort = null,
                FilterBySpecies = request.FilterBySpecies,
                FilterByStatus = null,
                FilterByAge = null
            };
        }

        public static PetListRequestDto ToListRequestDto(this OrganPetListRequest request)
        {
            return new PetListRequestDto
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                SearchByName = request.SearchByName,
                Sort = request.Sort,
                FilterBySpecies = request.FilterBySpecies,
                FilterByStatus = request.FilterByStatus,
                FilterByAge = request.FilterByAge
            };
        }

        public static InterviewedPetListRequestDto ToInterviewedPetListRequestDto(
            this InterviewedPetListRequest request)
        {
            return new InterviewedPetListRequestDto
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                FilterBySpecies = request.FilterBySpecies,
                FilterByAge = request.FilterByAge,
                SearchByName = request.SearchByName
            };
        }

        //public static CreatePetRequestDto ToCreatePetRequestDto(this CreatePetRequest request,
        //    List<PetInfoItem> information, string organizationId)
        //{
        //    return new CreatePetRequestDto
        //    {
        //        PetName = request.PetName,
        //        Species = request.Species,
        //        Breed = request.Breed,
        //        Age = request.Age,
        //        Weight = request.Weight,
        //        Gender = request.Gender,
        //        Description = request.Description,
        //        LivingEnvironmentNeeds = request.LivingEnvironmentNeeds,
        //        Information = information,
        //        PetImages = request.PetImages,
        //        OrganizationId = organizationId
        //    };
        //}
    }
}