using RegistrationAPI.Models;
using RegistrationAPI.Repository;
using System.Collections.Generic;

namespace RegistrationAPI.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRegistrationRepository _repo;

        public RegistrationService(IRegistrationRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<RegistrationModel> GetAll() => _repo.GetAll();
        public RegistrationModel GetById(int id) => _repo.GetById(id);
        public bool Insert(RegistrationModel model) => _repo.Insert(model);
        public bool Update(RegistrationModel model) => _repo.Update(model);
        public bool Delete(int id) => _repo.Delete(id);

        // New methods
        public IEnumerable<RegistrationModel> SearchByPrefix(string term) => _repo.SearchByPrefix(term);
        public List<RegistrationModel> Search(string term)
        {
            return _repo.SearchByPrefix(term)?.ToList() ?? new List<RegistrationModel>();
        }

        public CandidateFullDetailsModel GetFullDetails(int id) => _repo.GetFullDetails(id);

        public bool UpdateFullDetails(CandidateFullDetailsModel model) => _repo.UpdateFullDetails(model);
        public RegistrationModel GetUserByEmail(string email)
        {
            return _repo.GetUserByEmail(email); // ✅ Clean and correct
        }

    }
}
