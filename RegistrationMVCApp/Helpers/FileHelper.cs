using Microsoft.AspNetCore.Http;

namespace RegistrationApp.Helpers
{
    public static class FileHelper
    {
        public static string SaveFile(IFormFile file, string rootPath)
        {
            if (file == null || file.Length == 0) return null;

            // Ensure rootPath points to wwwroot
            string wwwrootPath = string.IsNullOrEmpty(rootPath)
                ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                : rootPath;

            // Create 'uploads' folder inside wwwroot
            string uploadsFolder = Path.Combine(wwwrootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generate a unique file name
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadsFolder, fileName);

            // Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // Return relative path (for client use)
            return "/uploads/" + fileName;
        }
    }
}
