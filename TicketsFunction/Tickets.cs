using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketFunction
{
    public class Tickets
    {
        [Required]
        public int ConcertId { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^[0-9\s\+\-\(\)]*$", ErrorMessage = "Phone must contain only digits and phone formatting characters")]
        public string Phone { get; set; }

        [Required, Range(1, 10)]
        public int Quantity { get; set; }


        [Required]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Credit card must be 16 digits")]
        public string CreditCard { get; set; }

        [Required, RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Expiration must be in MM/YY format")]
        public string Expiration { get; set; }

        [Required, StringLength(4, MinimumLength = 3)]
        public string SecurityCode { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Province { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]{6}$")]
        public string PostalCode { get; set; }

        [Required]
        public string Country { get; set; }
    }
}