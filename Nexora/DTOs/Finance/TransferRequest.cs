using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Nexora.DTOs.Finance
{
    public class TransferRequest
    {
        [Required(ErrorMessage = "Receiver login is required")]
        public required string ReceiverLogin { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
    }
}