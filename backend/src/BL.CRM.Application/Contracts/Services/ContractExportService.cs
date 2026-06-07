using System.Globalization;
using BL.CRM.Application.Common.Interfaces;
using BL.CRM.Application.Contracts.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;

namespace BL.CRM.Application.Contracts.Services;

// ──────────────────────────────────────────────
// Row & Map
// ──────────────────────────────────────────────

internal sealed record ContractCsvRow(
    string RegistrationNumber,
    string Institution,
    string ClientId,
    string ClientFirstName,
    string ClientLastName,
    string ManagerId,
    string ManagerFirstName,
    string ManagerLastName,
    string StartDate,
    string ValidityDate,
    string EndDate,
    string Participants      // "FirstName LastName (<guid>); ..."
);

internal sealed class ContractCsvRowMap : ClassMap<ContractCsvRow>
{
    public ContractCsvRowMap()
    {
        Map(r => r.RegistrationNumber).Index(0).Name("RegistrationNumber");
        Map(r => r.Institution).Index(1).Name("Institution");
        Map(r => r.ClientId).Index(2).Name("ClientId");
        Map(r => r.ClientFirstName).Index(3).Name("ClientFirstName");
        Map(r => r.ClientLastName).Index(4).Name("ClientLastName");
        Map(r => r.ManagerId).Index(5).Name("ManagerId");
        Map(r => r.ManagerFirstName).Index(6).Name("ManagerFirstName");
        Map(r => r.ManagerLastName).Index(7).Name("ManagerLastName");
        Map(r => r.StartDate).Index(8).Name("StartDate");
        Map(r => r.ValidityDate).Index(9).Name("ValidityDate");
        Map(r => r.EndDate).Index(10).Name("EndDate");
        Map(r => r.Participants).Index(11).Name("Participants");
    }
}

// ──────────────────────────────────────────────
// Service
// ──────────────────────────────────────────────

public class ContractExportService(IApplicationDbContext dbContext) : IContractExportService
{
    public async Task<byte[]> ExportContractsToCsvAsync()
    {
        var contracts = await dbContext.Contracts
            .Include(c => c.Client)
            .Include(c => c.ContractManager)
            .Include(c => c.Participants)
            .OrderBy(c => c.RegistrationNumber)
            .ToListAsync();

        var rows = contracts.Select(c =>
        {
            var participants = c.Participants
                .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
                .Select(p => $"{p.FirstName} {p.LastName} ({p.Id})")
                .ToList();

            return new ContractCsvRow(
                c.RegistrationNumber,
                c.Institution,
                c.ClientId.ToString(),
                c.Client.FirstName,
                c.Client.LastName,
                c.ContractManagerId.ToString(),
                c.ContractManager.FirstName,
                c.ContractManager.LastName,
                c.StartDate.ToString("yyyy-MM-dd"),
                c.ValidityDate.ToString("yyyy-MM-dd"),
                c.EndDate?.ToString("yyyy-MM-dd") ?? string.Empty,
                string.Join("; ", participants)
            );
        }).ToList();

        await using var memoryStream = new MemoryStream();
        await using var writer = new StreamWriter(memoryStream, System.Text.Encoding.UTF8, leaveOpen: true);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            NewLine = "\r\n",
            ShouldQuote = _ => true,
        };

        await using (var csv = new CsvWriter(writer, config))
        {
            csv.Context.RegisterClassMap<ContractCsvRowMap>();
            await csv.WriteRecordsAsync(rows);
        }

        return memoryStream.ToArray();
    }
}
