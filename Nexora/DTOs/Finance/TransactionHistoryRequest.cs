using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Nexora.DTOs.Finance
{
    public class TransactionHistoryRequest
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        
        [Range(1, 100, ErrorMessage = "Значение limit должно быть от 1 до 100")]
        public int Limit { get; set; } = 20;
        
        [Range(0, int.MaxValue, ErrorMessage = "Значение offset не может быть отрицательным")]
        public int Offset { get; set; } = 0;
    }
}