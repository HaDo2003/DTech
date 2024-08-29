using DTech.Models.EF;

namespace DTech.Library
{
    public class SettingImage
    {
        private readonly IWebHostEnvironment _environment;

        public SettingImage(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string filepath)
        {
            string imageName;
            if(file == null || file.Length == 0)
            {

                return imageName = "noimage.png";
            }

            // Define the directory to save the image using the provided relative path
            string uploadDir = Path.Combine(_environment.WebRootPath, filepath);

            // Ensure the directory exists
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            // Get the image name
            imageName = Path.GetFileName(file.FileName);

            // Combine the directory and image name to get the full file path
            string filePath = Path.Combine(uploadDir, imageName);

            // Save the image to the specified path
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            return imageName;
        }

        public async Task<string> ChangeImageAsync(string oldfile, IFormFile newfile, string filepath)
        {
            //Delete old image
            await DeleteImageAsync(oldfile, filepath + "/");

            //Upload new image
            string imageName = await UploadImageAsync(newfile, filepath);
            
            return imageName;
        }

        public async Task DeleteImageAsync(string file, string filepath)
        {
            if (!string.Equals(file, "noimage.png"))
            {
                string PhotoPath = Path.Combine(_environment.WebRootPath, filepath + file);

                if (System.IO.File.Exists(PhotoPath))
                {
                    System.IO.File.Delete(PhotoPath);
                }
            }

            await Task.CompletedTask;
        }
    }
}
