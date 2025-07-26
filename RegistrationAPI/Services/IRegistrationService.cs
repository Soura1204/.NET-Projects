using RegistrationAPI.Models;
using System.Collections.Generic;

namespace RegistrationAPI.Services
{
    public interface IRegistrationService
    {
        IEnumerable<RegistrationModel> GetAll();
        List<RegistrationModel> Search(string term);
        RegistrationModel GetById(int id);
        bool Insert(RegistrationModel model);
        bool Update(RegistrationModel model);
        bool Delete(int id);

        // New methods for candidate details
        IEnumerable<RegistrationModel> SearchByPrefix(string term);
        CandidateFullDetailsModel GetFullDetails(int id);
        bool UpdateFullDetails(CandidateFullDetailsModel model);
        RegistrationModel GetUserByEmail(string email);
    }
}
