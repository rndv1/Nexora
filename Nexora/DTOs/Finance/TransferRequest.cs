using System.ComponentModel.DataAnnotations;

namespace Nexora.DTOs.Finance
{
    public class TransferRequest
    {
        [Required(ErrorMessage = "Поле token обязательно")]
        public string Token { get; set; }
        
        [Required(ErrorMessage = "Поле receiverLogin обязательно ")]
        public string ReceiverLogin { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "Сумма должна быть больше 0")]
        public decimal Amount { get; set; }
    }
}