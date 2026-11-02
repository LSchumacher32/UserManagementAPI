using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Models;

public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Der Vorname ist erforderlich.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Der Nachname ist erforderlich.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Die E-Mail-Adresse ist erforderlich.")]
    [EmailAddress(ErrorMessage = "Die E-Mail-Adresse ist ungültig.")]
    public string Email { get; set; } = string.Empty;
}