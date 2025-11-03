using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.PayOs
{
    public record CreatePaymentLinkRequest
    (
    int userId,
    int supscriptionId,
    string supscriptionName,
    string description,
    int amount,
    string returnUrl,
    string cancelUrl
    );
   
}
