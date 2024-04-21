using System.ComponentModel.DataAnnotations;

namespace DiscountGeneratorClient.Models
{
    public class UseCodeViewModel
    {
        [Length(7,8)]
        public string DiscountCode { get; set; }    
    }
}
