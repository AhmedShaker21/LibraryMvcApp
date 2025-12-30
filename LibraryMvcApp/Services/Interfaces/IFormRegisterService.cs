using LibraryMvcApp.Models;

namespace LibraryMvcApp.Services.Interfaces
{
    public interface IFormRegisterService
    {
        Task AddFormAsync(FormEntry entry);
        Task<int> GetLastFormNumberAsync(int departmentId);
        Task<List<FormEntry>> GetByDepartmentAsync(int departmentId);
        Task<List<FormEntry>> GetAllAsync();
        Task DeleteAsync(int id);
    }


}
