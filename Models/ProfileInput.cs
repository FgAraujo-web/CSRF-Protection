using System.ComponentModel.DataAnnotations;

namespace CSRFProtectionDemo.Models;

public class ProfileInput
{
    [Required(ErrorMessage = "Indica o nome.")]
    [StringLength(80)]
    public string Name { get; set; } = "Perfil de demonstração";
}
