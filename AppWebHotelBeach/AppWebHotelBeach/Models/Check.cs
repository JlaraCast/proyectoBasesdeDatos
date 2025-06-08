using Microsoft.EntityFrameworkCore;

namespace AppWebHotelBeach.Models
{
    [PrimaryKey(nameof(CheckNumber), nameof(CheckOwner))]
    public class Check
    {
        public int CheckNumber { get; set; }
        public string CheckOwner { get; set; }
        public string CheckBank { get; set; }
        public int OperationNumber { get; set; }
        public string DebitAccount { get; set; } // e.g., "5 xxxx-x-xxx-x"

        public string IssuedTo { get; set; } // e.g., "R Sas"

        public string CUITDocument { get; set; } // e.g., "CUIT - 30xxxxxxxxx"

        public decimal Amount { get; set; } // e.g., 10000.00

        public DateTime PaymentDate { get; set; } // e.g., 31/03/2020

        public string Concept { get; set; } // e.g., "Various"


        public string Mode { get; set; } // e.g., "Crossed"

        public string Nature { get; set; } // e.g., "To the order"

        public string PaymentReason { get; set; } // e.g., "Merchandise"

        public int CustomerId { get; set; }


    }
}
      