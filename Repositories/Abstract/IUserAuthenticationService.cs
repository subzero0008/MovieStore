using MovieStoreMvc.Models.DTO;

namespace MovieStoreMvc.Repositories.Abstract
{
    /// <summary>
    /// Интерфейс за управление на потребителска автентикация.
    /// </summary>
    public interface IUserAuthenticationService
    {
        /// <summary> Вход на потребител. </summary>
        Task<Status> LoginAsync(LoginModel model);

        /// <summary> Изход на потребител. </summary>
        Task LogoutAsync();

        /// <summary> Регистрация на нов потребител. </summary>
        Task<Status> RegisterAsync(RegistrationModel model);
    }
}
