using System.ComponentModel.DataAnnotations;

namespace HelloApi.Models.Requests
{
    public class OrderProductRequest
    {
        public int ProductId { get; set; }

        [Range(1, uint.MaxValue)]
        public uint Amount { get; set; }

        public int? OrderId { get; set; }
    }
}
