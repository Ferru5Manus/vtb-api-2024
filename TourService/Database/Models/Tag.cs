using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TourService.Database.Models
{
    public class Tag
    {
        //TODO: Add validation attributes
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
    }
}