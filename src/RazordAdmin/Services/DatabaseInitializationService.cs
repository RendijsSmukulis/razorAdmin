using Microsoft.Data.Sqlite;
using RazorAdmin.Models;
using RazorAdmin.Repositories;
using Dapper;

namespace RazorAdmin.Services;

/// <summary>
/// Service for database initialization and seeding operations.
/// </summary>
public class DatabaseInitializationService : IDatabaseInitializationService
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseInitializationService> _logger;

    public DatabaseInitializationService(IConfiguration configuration, ILogger<DatabaseInitializationService> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=razoradmin2.db";
        _logger = logger;
    }

    public async Task InitializeDatabaseAsync()
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            // Create Features table
            var createTableSql = @"
                CREATE TABLE IF NOT EXISTS Features (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE,
                    Description TEXT NOT NULL,
                    Status TEXT NOT NULL,
                    Category TEXT NOT NULL,
                    Icon TEXT NOT NULL,
                    LastUpdated TEXT NOT NULL,
                    UsageCount INTEGER NOT NULL DEFAULT 0,
                    SuccessRate INTEGER NOT NULL DEFAULT 0,
                    ErrorCount INTEGER NOT NULL DEFAULT 0,
                    LastUsed TEXT
                )";

            await connection.ExecuteAsync(createTableSql);

            // Check if we need to seed data
            var count = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Features");
            if (count == 0)
            {
                await SeedFeaturesAsync(connection);
                _logger.LogInformation("Database seeded with initial data");
            }
            else
            {
                _logger.LogInformation("Database already contains {FeatureCount} features", count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing database");
            throw;
        }
    }

    private async Task SeedFeaturesAsync(SqliteConnection connection)
    {
        var features = new[]
        {
            new Feature
            {
                Name = "AdBlock",
                Description = "# Ad Block Feature\n\n**Blocks unwanted advertisements** and tracking scripts from loading on your system.\n\n## Features\n- *Real-time filtering*\n- `Custom filter lists`\n- **Performance optimized**\n\n> Provides enhanced privacy and faster browsing experience.",
                Status = "Active",
                Category = "Security",
                Icon = "mdi mdi-shield-check mdi-24px text-blue-500",
                LastUpdated = DateTime.Now.AddDays(-10),
                UsageCount = 15420,
                SuccessRate = 98,
                ErrorCount = 12,
                LastUsed = DateTime.Now.AddHours(-2)
            },
            new Feature
            {
                Name = "DevTools",
                Description = "# Developer Tools\n\nEssential **debugging utilities** and development aids.\n\n## Available Tools\n1. *Code Inspector*\n2. `Console Logger`\n3. **Performance Monitor**\n4. Network Analyzer\n\nSupports multiple programming languages and frameworks.",
                Status = "Active",
                Category = "Development",
                Icon = "mdi mdi-tools mdi-24px text-green-500",
                LastUpdated = DateTime.Now.AddDays(-13),
                UsageCount = 8920,
                SuccessRate = 95,
                ErrorCount = 45,
                LastUsed = DateTime.Now.AddHours(-1)
            },
            new Feature
            {
                Name = "Database",
                Description = "# Database Management\n\n**Comprehensive database administration** and monitoring tools.\n\n## Capabilities\n- *Query Builder*\n- `Schema Management`\n- **Performance Tuning**\n- Backup & Recovery\n\n> Supports SQLite, MySQL, PostgreSQL, and MongoDB.",
                Status = "Active",
                Category = "Data",
                Icon = "mdi mdi-database mdi-24px text-purple-500",
                LastUpdated = DateTime.Now.AddDays(-15),
                UsageCount = 5670,
                SuccessRate = 99,
                ErrorCount = 3,
                LastUsed = DateTime.Now.AddMinutes(-30)
            },
            new Feature
            {
                Name = "UserManagement",
                Description = "# User Management System\n\n**Complete user account management** with role-based access control.\n\n## Features\n- *User Registration*\n- `Role Assignment`\n- **Permission Management**\n- Audit Logging\n\nSupports LDAP integration and SSO authentication.",
                Status = "Active",
                Category = "Administration",
                Icon = "mdi mdi-account-group mdi-24px text-orange-500",
                LastUpdated = DateTime.Now.AddDays(-17),
                UsageCount = 12340,
                SuccessRate = 97,
                ErrorCount = 23,
                LastUsed = DateTime.Now.AddMinutes(-15)
            },
            new Feature
            {
                Name = "Analytics",
                Description = "# Analytics Dashboard\n\n**Advanced system analytics** and reporting capabilities.\n\n## Metrics Tracked\n- *User Activity*\n- `System Performance`\n- **Error Rates**\n- Resource Usage\n\n> Real-time data visualization with customizable reports.",
                Status = "Pending",
                Category = "Reporting",
                Icon = "mdi mdi-chart-line mdi-24px text-red-500",
                LastUpdated = DateTime.Now.AddDays(-20),
                UsageCount = 0,
                SuccessRate = 0,
                ErrorCount = 0,
                LastUsed = null
            },
            new Feature
            {
                Name = "Backup",
                Description = "# Backup & Restore\n\n**Automated backup system** with data recovery capabilities.\n\n## Backup Types\n1. *Full System Backup*\n2. `Incremental Backup`\n3. **Differential Backup**\n4. File-level Backup\n\nSupports cloud storage integration and encryption.",
                Status = "Active",
                Category = "Data",
                Icon = "mdi mdi-cloud-upload mdi-24px text-indigo-500",
                LastUpdated = DateTime.Now.AddDays(-5),
                UsageCount = 3420,
                SuccessRate = 99,
                ErrorCount = 5,
                LastUsed = DateTime.Now.AddHours(-6)
            }
        };

        var sql = @"
            INSERT INTO Features (Name, Description, Status, Category, Icon, LastUpdated, UsageCount, SuccessRate, ErrorCount, LastUsed)
            VALUES (@Name, @Description, @Status, @Category, @Icon, @LastUpdated, @UsageCount, @SuccessRate, @ErrorCount, @LastUsed)";

        foreach (var feature in features)
        {
            await connection.ExecuteAsync(sql, feature);
        }
    }
} 