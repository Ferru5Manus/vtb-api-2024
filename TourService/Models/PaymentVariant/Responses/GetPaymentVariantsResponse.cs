using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourService.Models.PaymentVariant.Responses
{
    public class GetPaymentVariantsResponse
    {
        public List<PaymentVariantDto>? PaymentVariants { get; set; }
    }
}