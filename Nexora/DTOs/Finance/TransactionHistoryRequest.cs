using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Nexora.DTOs.Finance
{
    public class TransactionHistoryRequest
    {
        [FromQuery]
        [Required(ErrorMessage = "Поле token обязательно")]
        public string Token { get; set; }
        
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "Значение limit должно быть не меньше 1")]
        public int Limit { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Значение offset не может быть отрицательным")]
        public int Offset { get; set; }
    }
}