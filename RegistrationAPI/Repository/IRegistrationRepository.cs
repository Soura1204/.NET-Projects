using System.Collections.Generic;
using RegistrationAPI.Models;

namespace RegistrationAPI.Repository
{
    public interface IRegistrationRepository
    {
        IEnumerable<RegistrationModel> GetAll();
        RegistrationModel GetById(int id);
        bool Insert(RegistrationModel model);
        bool Update(RegistrationModel model);
        bool Delete(int id);
        IEnumerable<RegistrationModel> SearchByPrefix(string term); // ADD THIS
        CandidateFullDetailsModel GetFullDetails(int id);
        bool UpdateFullDetails(CandidateFullDetailsModel model);
        RegistrationModel GetUserByEmail(string email);
    }
}
