using System.Globalization;
using BL.CRM.Application.Common.Interfaces;
using BL.CRM.Application.Users.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;

namespace BL.CRM.Application.Users.Services;

// ──────────────────────────────────────────────
// Clients
// ──────────────────────────────────────────────

internal sealed record ClientCsvRow(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string PersonalId,
    string BirthDate,
    string CreatedAt
);

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

// ──────────────────────────────────────────────
// Advisors
// ──────────────────────────────────────────────

internal sealed record AdvisorCsvRow(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string PersonalId,
    string BirthDate,
    string CreatedAt
);

internal sealed class AdvisorCsvRowMap : ClassMap<AdvisorCsvRow>
{
    public AdvisorCsvRowMap()
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

// ──────────────────────────────────────────────
// Service
// ──────────────────────────────────────────────

public class UserExportService(IApplicationDbContext dbContext) : IUserExportService
{
    private static CsvConfiguration BuildConfig() => new(CultureInfo.InvariantCulture)
    {
        HasHeaderRecord = true,
        NewLine = "\r\n",
        // Quote everything except Excel formula fields (="..")
        ShouldQuote = args => !(args.Field?.StartsWith('=') == true),
    };

    private static string PhoneFormula(string? phone) =>
        $"=\"{(phone ?? string.Empty).Replace("\"", "\"\"")}\"";

    public async Task<byte[]> ExportClientsToCsvAsync()
    {
        var rows = await dbContext.Clients
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .Select(c => new ClientCsvRow(
                c.Id,
                c.FirstName,
                c.LastName,
                c.Email ?? string.Empty,
                PhoneFormula(c.PhoneNumber),
                c.PersonalId ?? string.Empty,
                c.BirthDate.ToString("yyyy-MM-dd"),
                c.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            ))
            .ToListAsync();

        return await WriteCsvAsync(rows, new ClientCsvRowMap());
    }

    public async Task<byte[]> ExportAdvisorsToCsvAsync()
    {
        var rows = await dbContext.Advisors
            .OrderBy(a => a.LastName)
            .ThenBy(a => a.FirstName)
            .Select(a => new AdvisorCsvRow(
                a.Id,
                a.FirstName,
                a.LastName,
                a.Email ?? string.Empty,
                PhoneFormula(a.PhoneNumber),
                a.PersonalId ?? string.Empty,
                a.BirthDate.ToString("yyyy-MM-dd"),
                a.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            ))
            .ToListAsync();

        return await WriteCsvAsync(rows, new AdvisorCsvRowMap());
    }

    private static async Task<byte[]> WriteCsvAsync<T>(IEnumerable<T> rows, ClassMap<T> map)
    {
        await using var memoryStream = new MemoryStream();
        await using var writer = new StreamWriter(memoryStream, System.Text.Encoding.UTF8, leaveOpen: true);
        await using (var csv = new CsvWriter(writer, BuildConfig()))
        {
            csv.Context.RegisterClassMap(map);
            await csv.WriteRecordsAsync(rows);
        }
        return memoryStream.ToArray();
    }
}
