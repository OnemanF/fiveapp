using System.ComponentModel.DataAnnotations;

namespace api;

public class AppOptions
{
    public string DbConnectionString { get; set; } = string.Empty;
    public string jwtSecret { get; set; } = string.Empty;
}