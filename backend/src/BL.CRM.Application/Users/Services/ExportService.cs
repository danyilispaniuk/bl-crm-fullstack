using System.Globalization;
using BL.CRM.Application.Common.Interfaces;
using BL.CRM.Application.Users.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;

namespace BL.CRM.Application.Users.Services;

/// <summary>
/// A record that represents a single row in the clients CSV export.
/// PhoneNumber is stored as an Excel formula string (="value") so that
/// Excel/Sheets treats it as text and preserves the leading + sign.
/// </summary>
internal sealed record ClientCsvRow(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,   // formatted as ="<phone>" for Excel text forcing
    string PersonalId,
    string BirthDate,
    string CreatedAt
);

/// <summary>
/// Explicit CsvHelper class map — controls column order and header names.
/// </summary>
internal sealed class ClientCsvRowMap : ClassMap<ClientCsvRow>
{
    public ClientCsvRowMap()
    {
        Map(r => r.Id).Index(0).Name("Id");
        Map(r => r.FirstName).Index(1).Name("FirstName");
        Map(r => r.LastName).Index(2).Name("LastName");
        Map(r => r.Email).Index(3).Name("Email");
        Map(r => r.PhoneNumber).Index(4).Name("PhoneNumber");
        Map(r => r.PersonalId).Index(5).Name("PersonalId");
        Map(r => r.BirthDate).Index(6).Name("BirthDate");
        Map(r => r.CreatedAt).Index(7).Name("CreatedAt");
    }
}

public class ExportService(IApplicationDbContext dbContext) : IExportService
{
    public async Task<byte[]> ExportClientsToCsvAsync()
    {
        var clients = await dbContext.Clients
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .Select(c => new ClientCsvRow(
                c.Id,
                c.FirstName,
                c.LastName,
                c.Email ?? string.Empty,
                // Excel formula: ="<value>" forces the cell to be treated as text,
                // preserving the leading + and preventing scientific notation.
                $"=\"{(c.PhoneNumber ?? string.Empty).Replace("\"", "\"\"")}\"",
                c.PersonalId ?? string.Empty,
                c.BirthDate.ToString("yyyy-MM-dd"),
                c.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            ))
            .ToListAsync();

        await using var memoryStream = new MemoryStream();
        await using var streamWriter = new StreamWriter(memoryStream, System.Text.Encoding.UTF8, leaveOpen: true);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            NewLine = "\r\n",
            // Quote all fields EXCEPT those starting with = (Excel formulas must be unquoted)
            ShouldQuote = args => !(args.Field?.StartsWith('=') == true),
        };

        await using (var csvWriter = new CsvWriter(streamWriter, config))
        {
            csvWriter.Context.RegisterClassMap<ClientCsvRowMap>();
            await csvWriter.WriteRecordsAsync(clients);
        }

        return memoryStream.ToArray();
    }
}
