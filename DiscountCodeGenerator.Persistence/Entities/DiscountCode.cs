using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscountCodeGenerator.Model.Entities
{
    public class DiscountCode
    {
        public Guid Id { get; set; }
        [Length(7,8)]
        public string Code {  get; set; }
        public bool IsUsed {  get; set; }
    }
}
