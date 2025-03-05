namespace MovieStoreMvc.Repositories.Abstract
{
    public interface IMovieImageService
    {
        /// <summary>
        /// Качва изображение в Cloudinary и връща URL на каченото изображение.
        /// </summary>
        /// <param name="imageFile">Файлът, който да бъде качен.</param>
        /// <returns>URL на каченото изображение или null, ако не е успешен качването.</returns>
        Task<string> UploadImageAsync(IFormFile imageFile);

        /// <summary>
        /// Изтрива изображение от Cloudinary по публичен идентификатор.
        /// </summary>
        /// <param name="imagePublicId">Публичният идентификатор на изображението, което да бъде изтрито.</param>
        /// <returns>Връща true ако изтриването е успешно, false в противен случай.</returns>
        Task<bool> DeleteImageAsync(string imagePublicId);

        /// <summary>
        /// Получава URL на изображение по публичен идентификатор.
        /// </summary>
        /// <param name="imagePublicId">Публичният идентификатор на изображението.</param>
        /// <returns>URL на изображението.</returns>
        string GetImageUrl(string imagePublicId);
    }
}
