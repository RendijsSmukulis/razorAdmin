namespace RazorAdmin.Services;

/// <summary>
/// Service interface for database initialization and seeding operations.
/// </summary>
public interface IDatabaseInitializationService
{
    Task InitializeDatabaseAsync();
} 