using System.ComponentModel.DataAnnotations;

namespace Nexora.DTOs.Finance
{
public class DepositRequest
{
    [Required(ErrorMessage ="Поле token обязательно")] 
    public string Token { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Сумма должна быть больше 0")]
    public decimal Amount { get; set; }
}
}