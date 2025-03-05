using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MovieStoreMvc.Repositories.Abstract;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MovieStoreMvc.Repositories.Implementation
{
    public class MovieImageService : IMovieImageService
    {
        private readonly Cloudinary _cloudinary;

        // Конструкторът получава зависимостта от Cloudinary
        public MovieImageService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        // Реализация на метода за качване на изображение в Cloudinary
        public async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return null;  // Ако файлът е празен или не съществува, връщаме null
            }

            try
            {
                // Подготвяме параметрите за качване
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream())
                    // Може да добавиш параметри като размери или формат
                };

                // Качване на изображението
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                // Проверка дали качването е успешно и връщаме URL на изображението
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return uploadResult.SecureUrl.ToString(); // Връща URL на каченото изображение
                }

                return null;
            }
            catch (Exception ex)
            {
                // Логване на грешката
                // Вместо Console.WriteLine може да използвате специфично логване
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        // Реализация на метода за изтриване на изображение от Cloudinary
        public async Task<bool> DeleteImageAsync(string imagePublicId)
        {
            if (string.IsNullOrEmpty(imagePublicId))
            {
                return false;  // Ако идентификаторът е празен, не изтриваме нищо
            }

            try
            {
                // Параметри за изтриване на изображение
                var deleteParams = new DeletionParams(imagePublicId)
                {
                    ResourceType = ResourceType.Image // Указваме типа на ресурса
                };

                // Изтриваме изображението от Cloudinary
                var deleteResult = await _cloudinary.DestroyAsync(deleteParams);

                return deleteResult.StatusCode == System.Net.HttpStatusCode.OK; // Проверка дали е изтрито
            }
            catch (Exception ex)
            {
                // Логване на грешката
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // Получаване на URL за изображение
        public string GetImageUrl(string imagePublicId)
        {
            if (string.IsNullOrEmpty(imagePublicId))
            {
                return null;  // Ако публичният идентификатор е празен, не генерираме URL
            }

            var url = _cloudinary.Api.UrlImgUp
                        .Transform(new Transformation().Width(300).Height(300).Crop("fit"))  // Пример за трансформация
                        .BuildUrl(imagePublicId); // Връща URL на изображението по публичен идентификатор

            return url;
        }
    }
}
