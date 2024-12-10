using System.ComponentModel.DataAnnotations;

namespace airbnbb.Models
{
    public class PropertyViewModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal PricePerNight { get; set; }


        public string Address { get; set; }


        public string City { get; set; }


        public string Country { get; set; }

  
        public int Capacity { get; set; }

        public string Category { get; set; }
        public string ImageUrl { get; set; }
    }

}
