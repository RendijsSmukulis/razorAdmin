namespace RazorAdmin.Configuration;

/// <summary>
/// Application settings configuration class.
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Database connection string.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
    
    /// <summary>
    /// Application name.
    /// </summary>
    public string ApplicationName { get; set; } = "RazorAdmin";
    
    /// <summary>
    /// Application version.
    /// </summary>
    public string Version { get; set; } = "1.0.0";
    
    /// <summary>
    /// Whether to enable detailed error messages in development.
    /// </summary>
    public bool EnableDetailedErrors { get; set; } = false;
} 