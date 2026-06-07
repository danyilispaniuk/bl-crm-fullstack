namespace BL.CRM.Application.Users.Interfaces;

public interface IExportService
{
    /// <summary>
    /// Generates a UTF-8 CSV byte array for all clients.
    /// </summary>
    Task<byte[]> ExportClientsToCsvAsync();
}
