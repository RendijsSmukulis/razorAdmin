using Microsoft.Data.Sqlite;
using RazorAdmin.Models;
using RazorAdmin.Configuration;
using Dapper;
using Microsoft.Extensions.Options;

namespace RazorAdmin.Repositories;

/// <summary>
/// Repository implementation for feature data access using Dapper and SQLite.
/// </summary>
public class FeatureRepository(IOptions<AppSettings> appSettings) : IFeatureRepository
{
    private readonly string _connectionString = appSettings.Value.ConnectionString;

    public async Task<IEnumerable<Feature>> GetAllAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryAsync<Feature>("SELECT * FROM Features ORDER BY Name");
    }

    public async Task<Feature?> GetByIdAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Feature>(
            "SELECT * FROM Features WHERE Id = @Id",
            new { Id = id });
    }

    public async Task<Feature?> GetByNameAsync(string name)
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Feature>(
            "SELECT * FROM Features WHERE Name = @Name",
            new { Name = name });
    }

    public async Task<int> CreateAsync(Feature feature)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = @"
            INSERT INTO Features (Name, Description, Status, Category, Icon, LastUpdated, UsageCount, SuccessRate, ErrorCount, LastUsed)
            VALUES (@Name, @Description, @Status, @Category, @Icon, @LastUpdated, @UsageCount, @SuccessRate, @ErrorCount, @LastUsed);
            SELECT last_insert_rowid();";

        return await connection.ExecuteScalarAsync<int>(sql, feature);
    }

    public async Task<bool> UpdateAsync(Feature feature)
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

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        var rowsAffected = await connection.ExecuteAsync("DELETE FROM Features WHERE Id = @Id", new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<bool> ExistsAsync(string name, int? excludeId = null)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = excludeId.HasValue
            ? "SELECT COUNT(*) FROM Features WHERE Name = @Name AND Id != @ExcludeId"
            : "SELECT COUNT(*) FROM Features WHERE Name = @Name";

        object parameters = excludeId.HasValue
            ? new { Name = name, ExcludeId = excludeId.Value }
            : new { Name = name };

        var count = await connection.ExecuteScalarAsync<int>(sql, parameters);
        return count > 0;
    }
}