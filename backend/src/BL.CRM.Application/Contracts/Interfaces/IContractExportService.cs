namespace BL.CRM.Application.Contracts.Interfaces;

public interface IContractExportService
{
    /// <summary>
    /// Generates a UTF-8 CSV byte array for all contracts,
    /// including client, manager (id + name) and a semicolon-separated
    /// list of participating advisors.
    /// </summary>
    Task<byte[]> ExportContractsToCsvAsync();
}
