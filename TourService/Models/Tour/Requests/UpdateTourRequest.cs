using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TourService.Attributes.Validation;

namespace TourService.Models.Tour.Requests
{
    public class UpdateTourRequest
    {
        [Required]
        public long TourId { get; set; }
        
        [Required]
        [TourName]
        public string Name { get; set; } = null!;

        [Required]
        [TourDescription]
        public string Description { get; set; } = null!;

        [Required]
        [TourPrice]
        public double Price { get; set; }

        [Required]
        [TourAddress]
        public string Address { get; set; } = null!;

        [Coordinates]
        public string? Coordinates { get; set; }

        [Required]
        public bool IsActive { get; set; }

    }
}