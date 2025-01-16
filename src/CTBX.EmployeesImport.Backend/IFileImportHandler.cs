using CTBX.EmployeesImport.Shared;

namespace CTBX.EmployeesImport.Backend
{
    public interface IFileImportHandler
    {
        Task UpdateFileStatus(int id, string status);
        Task<List<Employee>> ConvertFileToEmployees(string filepath);
        Task DeleteFileFromFolder(string filepath);
        Task ImportEmployeeFromFile(string filepath);
        Task<IEnumerable<FileRecord>> GetPendingFiles();
    }
}
