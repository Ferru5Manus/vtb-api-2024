using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourService.Models.Benefits
{
    public class BenefitDto
    {
        public long BenefitId { get; set; }
        public string BenefitName { get; set; } = null!;
    }
}