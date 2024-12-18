

namespace CTBX.EmployeesImport.Backend
{
    public class FileImporter
    {
        private readonly FileImportService _fileImportService;
        public FileImporter(FileImportService fileImportService)
        {
            _fileImportService = fileImportService;
        }

        public async Task ImportEmployeeFromFile()
        {
            Console.WriteLine($"Start processing file");
            var pendingFiles = await _fileImportService.GetPendingFiles();
            foreach (var file in pendingFiles)
            {
                await _fileImportService.UpdateFileStatus(file.Id, "In Progress");
                await _fileImportService.ImportEmployeeFromFile(file.FilePath);
                await _fileImportService.UpdateFileStatus(file.Id, "Completed");
                await _fileImportService.DeleteFileFromFolder(file.FilePath);
            }

        }

    }
}
