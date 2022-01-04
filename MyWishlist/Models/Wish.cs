using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MyWishlist.Models
{
    public class Wish
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public bool IsCompleted { get; set; }
        [AllowNull]
        public string Link { get; set; }
        [AllowNull]
        public string PriceAmount { get; set; }
        [AllowNull]
        [MaxLength(3)]
        public string PriceCurrency { get; set; }
        [AllowNull]
        public string ImageLink { get; set; }
    }
}