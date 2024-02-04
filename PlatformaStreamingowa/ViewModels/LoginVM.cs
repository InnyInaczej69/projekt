using System.ComponentModel.DataAnnotations;

namespace CustomIdentity.ViewModels;

public class LoginVM
{
    [Required(ErrorMessage = "Wymagana nazwa u�ytkownika")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Wymagane jest has�o")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Display(Name = "Zapami�taj mnie")]
    public bool RememberMe { get; set; }
}
