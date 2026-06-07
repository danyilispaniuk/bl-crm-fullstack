namespace BL.CRM.Application.Users.Interfaces;

public interface IUserExportService
{
    /// <summary>
    /// Generates a UTF-8 CSV byte array for all clients.
    /// </summary>
    Task<byte[]> ExportClientsToCsvAsync();

    /// <summary>
    /// Generates a UTF-8 CSV byte array for all advisors.
    /// </summary>
    Task<byte[]> ExportAdvisorsToCsvAsync();
}
