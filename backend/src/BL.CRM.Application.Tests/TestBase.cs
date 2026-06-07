using System;
using BL.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BL.CRM.Application.Tests;

public abstract class TestBase
{
    protected static ApplicationDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
