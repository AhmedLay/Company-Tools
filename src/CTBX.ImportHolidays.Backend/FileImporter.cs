
using Microsoft.Extensions.Logging;

namespace CTBX.ImportHolidays.Backend
{
    public class FileImporter
    {
        private readonly FileImportService _fileImportService;
        private readonly ILogger<FileImportService> _logger;


        public FileImporter(FileImportService fileImportService, ILogger<FileImportService> logger)
        {
            _fileImportService = fileImportService;
            _logger = logger;
        }
        public async Task ImportHolidayFromFile()
        {
            _logger.LogInformation("Start processing files");
            var pendingFiles = await _fileImportService.GetPendingFiles();
            foreach (var file in pendingFiles)
            {
                await _fileImportService.UpdateFileStatus(file.Id, "In Progress");
                _logger.LogDebug("Processing file {fileName}.", file.FileName);

                try
                {
                    await _fileImportService.ImportHolidayFromFile(file.FilePath);
                    await _fileImportService.DeleteFileFromFolder(file.FilePath);
                    await _fileImportService.UpdateFileStatus(file.Id, "In Progress");
                    _logger.LogInformation("{fileName} successfully processed.", file.FileName);
                }
                catch (Exception ex)
                {
                    await _fileImportService.UpdateFileStatus(file.Id, "Failed");
                    await _fileImportService.DeleteFileFromFolder(file.FilePath);
                    _logger.LogError(ex, "{fileName} failed.", file.FileName);
                }

            }

        }

    }
}
