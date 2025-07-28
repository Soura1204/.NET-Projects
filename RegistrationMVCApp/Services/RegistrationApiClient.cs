using RegistrationApp.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace RegistrationApp.Services
{
    public class RegistrationApiClient
    {
        private readonly HttpClient _client;
        public string BaseAddress => _client.BaseAddress?.ToString()?.TrimEnd('/') ?? "";

        public RegistrationApiClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<RegistrationModel>> GetAllRegistrationsAsync()
        {
            return await _client.GetFromJsonAsync<List<RegistrationModel>>("api/registration/GetAll");
        }

        public async Task<bool> SaveRegistrationAsync(RegistrationModel model)
        {
            using var formData = new MultipartFormDataContent();
            AddFormData(model, formData);

            var response = await _client.PostAsync("api/registration/Insert", formData);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateRegistrationAsync(RegistrationModel model)
        {
            using var formData = new MultipartFormDataContent();
            AddFormData(model, formData);

            // Always send ID
            formData.Add(new StringContent(model.Id.ToString()), "Id");

            // If no new image uploaded, send existing path
            if (model.Image == null && !string.IsNullOrEmpty(model.ImagePath))
            {
                formData.Add(new StringContent(model.ImagePath), "ImagePath");
            }

            formData.Add(new StringContent(model.ContentType ?? ""), "ContentType");

            formData.Add(new StringContent(model.Password ?? ""), "Password");

            var response = await _client.PutAsync("api/registration/Update", formData);
            return response.IsSuccessStatusCode;
        }

        private void AddFormData(RegistrationModel model, MultipartFormDataContent formData)
        {
            formData.Add(new StringContent(model.FName ?? ""), "FName");
            formData.Add(new StringContent(model.LName ?? ""), "LName");
            formData.Add(new StringContent(model.Address ?? ""), "Address");
            formData.Add(new StringContent(model.DOB?.ToString("yyyy-MM-dd") ?? ""), "DOB");
            formData.Add(new StringContent(model.Email ?? ""), "Email");
            formData.Add(new StringContent(model.MobileNo ?? ""), "MobileNo");
            formData.Add(new StringContent(model.Password ?? ""), "Password");

            if (model.Image != null)
            {
                var fileContent = new StreamContent(model.Image.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.Image.ContentType);
                formData.Add(fileContent, "Image", model.Image.FileName);
            }
        }

        // Search and Candidate Details
        public async Task<List<RegistrationModel>> SearchCandidatesAsync(string term)
        {
            return await _client.GetFromJsonAsync<List<RegistrationModel>>($"api/registration/Search?term={term}");
        }

        public async Task<CandidateFullDetailsModel?> GetCandidateFullDetailsAsync(int id)
        {
            return await _client.GetFromJsonAsync<CandidateFullDetailsModel>($"api/registration/GetCandidateFullDetails/{id}");
        }

        public async Task<bool> UpdateCandidateFullDetailsAsync(CandidateFullDetailsModel model)
        {
            using var formData = new MultipartFormDataContent();

            // Registration fields
            formData.Add(new StringContent(model.Registration.Id.ToString()), "Registration.Id");
            formData.Add(new StringContent(model.Registration.FName ?? ""), "Registration.FName");
            formData.Add(new StringContent(model.Registration.LName ?? ""), "Registration.LName");
            formData.Add(new StringContent(model.Registration.Address ?? ""), "Registration.Address");
            formData.Add(new StringContent(model.Registration.DOB?.ToString("yyyy-MM-dd") ?? ""), "Registration.DOB");
            formData.Add(new StringContent(model.Registration.Email ?? ""), "Registration.Email");
            formData.Add(new StringContent(model.Registration.MobileNo ?? ""), "Registration.MobileNo");

            if (model.Registration.Image != null)
            {
                var fileContent = new StreamContent(model.Registration.Image.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.Registration.Image.ContentType);
                formData.Add(fileContent, "Registration.Image", model.Registration.Image.FileName);
            }
            else if (!string.IsNullOrEmpty(model.Registration.ImagePath))
            {
                formData.Add(new StringContent(model.Registration.ImagePath), "Registration.ImagePath");
            }

            // CandidateDetails fields
            if (model.CandidateDetails != null)
            {
                formData.Add(new StringContent(model.CandidateDetails.Details_ID?.ToString() ?? ""), "CandidateDetails.Details_ID");
                formData.Add(new StringContent(model.CandidateDetails.Candt_ID.ToString()), "CandidateDetails.Candt_ID");
                formData.Add(new StringContent(model.CandidateDetails.Aadhar_No ?? ""), "CandidateDetails.Aadhar_No");
                formData.Add(new StringContent(model.CandidateDetails.Pan_No ?? ""), "CandidateDetails.Pan_No");
                formData.Add(new StringContent(model.CandidateDetails.Gender ?? ""), "CandidateDetails.Gender");
                formData.Add(new StringContent(model.CandidateDetails.Highest_Qualification ?? ""), "CandidateDetails.Highest_Qualification");
                formData.Add(new StringContent(model.CandidateDetails.Company_Name ?? ""), "CandidateDetails.Company_Name");
                formData.Add(new StringContent(model.CandidateDetails.Dept ?? ""), "CandidateDetails.Dept");
                formData.Add(new StringContent(model.CandidateDetails.Post ?? ""), "CandidateDetails.Post");
                formData.Add(new StringContent(model.CandidateDetails.Mode ?? ""), "CandidateDetails.Mode");
            }

            var response = await _client.PutAsync("api/registration/UpdateFullDetails", formData);
            return response.IsSuccessStatusCode;
        }

        public async Task<RegistrationModel?> LoginAsync(LoginViewModel model)
        {
            var response = await _client.PostAsJsonAsync("api/Login/Login", model);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<RegistrationModel>();
            }
            return null;
        }
    }
}
