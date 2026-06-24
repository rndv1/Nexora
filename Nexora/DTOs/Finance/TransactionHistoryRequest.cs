using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Nexora.DTOs.Finance
{
    public class TransactionHistoryRequest
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100")]
        public int Limit { get; set; } = 20;

        [Range(0, int.MaxValue, ErrorMessage = "Offset cannot be negative")]
        public int Offset { get; set; } = 0;
    }
}