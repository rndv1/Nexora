using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Nexora.DTOs.Finance
{
    public class TransferRequest
    {
        [Required(ErrorMessage = "Поле receiverLogin обязательно ")]
        public required string ReceiverLogin { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "Сумма должна быть больше 0")]
        public decimal Amount { get; set; }
    }
}