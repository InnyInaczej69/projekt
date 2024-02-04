using System.ComponentModel.DataAnnotations;

namespace CustomIdentity.ViewModels;

public class AccountVM
{
    [Required]
    public string? Name { get; set; }
    [Display(Name = "Nazwa użytkownika")]
    [Required]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string? Hasło { get; set; }

    [Compare("Hasło", ErrorMessage = "Hasła nie pasują")]
    [Display(Name = "Potwierdź Hasło")]
    [DataType(DataType.Password)]
    public string? ConfirmPassword { get; set; }
}