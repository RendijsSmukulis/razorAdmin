using Microsoft.Data.Sqlite;
using RazorAdmin.Models;
using Dapper;

namespace RazorAdmin.Services;

public interface IDatabaseService
{
    Task<IEnumerable<Feature>> GetAllFeaturesAsync();
    Task<Feature?> GetFeatureByNameAsync(string featureName);
    Task<int> CreateFeatureAsync(Feature feature);
    Task<bool> UpdateFeatureAsync(Feature feature);
    Task<bool> DeleteFeatureAsync(int id);
    Task InitializeDatabaseAsync();
}

public class DatabaseService : IDatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=razoradmin.db";
    }

    public async Task<IEnumerable<Feature>> GetAllFeaturesAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryAsync<Feature>("SELECT * FROM Features ORDER BY Name");
    }

    public async Task<Feature?> GetFeatureByNameAsync(string featureName)
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Feature>(
            "SELECT * FROM Features WHERE Name = @FeatureName", 
            new { FeatureName = featureName });
    }

    public async Task<int> CreateFeatureAsync(Feature feature)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = @"
            INSERT INTO Features (Name, Description, Status, Category, Icon, LastUpdated, UsageCount, SuccessRate, ErrorCount, LastUsed)
            VALUES (@Name, @Description, @Status, @Category, @Icon, @LastUpdated, @UsageCount, @SuccessRate, @ErrorCount, @LastUsed);
            SELECT last_insert_rowid();";
        
        return await connection.ExecuteScalarAsync<int>(sql, feature);
    }

    public async Task<bool> UpdateFeatureAsync(Feature feature)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = @"
            UPDATE Features 
            SET Description = @Description, Status = @Status, Category = @Category, 
                Icon = @Icon, LastUpdated = @LastUpdated, UsageCount = @UsageCount, 
                SuccessRate = @SuccessRate, ErrorCount = @ErrorCount, LastUsed = @LastUsed
            WHERE Id = @Id";
        
        var rowsAffected = await connection.ExecuteAsync(sql, feature);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteFeatureAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        var rowsAffected = await connection.ExecuteAsync("DELETE FROM Features WHERE Id = @Id", new { Id = id });
        return rowsAffected > 0;
    }

    public async Task InitializeDatabaseAsync()
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
        }
    }

    private async Task SeedFeaturesAsync(SqliteConnection connection)
    {
        var features = new[]
        {
            new Feature
            {
                Name = "AdBlock",
                Description = "Block unwanted advertisements and tracking scripts",
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
                Description = "Developer tools and debugging utilities",
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
                Description = "Database administration and monitoring tools",
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
                Description = "User accounts, roles, and permissions management",
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
                Description = "System analytics and reporting dashboard",
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
                Description = "System backup and data recovery tools",
                Status = "Active",
                Category = "System",
                Icon = "mdi mdi-backup-restore mdi-24px text-indigo-500",
                LastUpdated = DateTime.Now.AddDays(-22),
                UsageCount = 2340,
                SuccessRate = 100,
                ErrorCount = 0,
                LastUsed = DateTime.Now.AddDays(-1)
            },
            new Feature
            {
                Name = "Notifications",
                Description = "System notification and alert management",
                Status = "Disabled",
                Category = "Communication",
                Icon = "mdi mdi-bell mdi-24px text-pink-500",
                LastUpdated = DateTime.Now.AddDays(-27),
                UsageCount = 0,
                SuccessRate = 0,
                ErrorCount = 0,
                LastUsed = null
            }
        };

        var insertSql = @"
            INSERT INTO Features (Name, Description, Status, Category, Icon, LastUpdated, UsageCount, SuccessRate, ErrorCount, LastUsed)
            VALUES (@Name, @Description, @Status, @Category, @Icon, @LastUpdated, @UsageCount, @SuccessRate, @ErrorCount, @LastUsed)";

        foreach (var feature in features)
        {
            await connection.ExecuteAsync(insertSql, feature);
        }
    }
} 