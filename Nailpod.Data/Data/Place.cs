using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nailpod.Data
{
    [Table("place")]
    public partial class Place
    {
        [Key]
        [DatabaseGenerat‌​ed(DatabaseGeneratedOption.None)]
        public long place_id { get; set; }

        public long customer_id { get; set; }

        [Required]
        [MaxLength(120)]
        public string addr { get; set; }
        [MaxLength(120)]
        public string addr2 { get; set; }
        [Required]
        [MaxLength(30)]
        public string city { get; set; }
        [Required]
        [MaxLength(50)]
        public string region { get; set; }
        [Required]
        [MaxLength(2)]
        public string country_cd { get; set; }
        [Required]
        [MaxLength(15)]
        public string postalcode { get; set; }

        [MaxLength(20)]
        public string place_tel_no { get; set; }

        public DateTimeOffset? install_dt { get; set; }

        [MaxLength(1)]
        public string del_yn { get; set; }

        [Required]
        public DateTimeOffset reg_dt { get; set; }
        [Required]
        public DateTimeOffset? upd_dt { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

        public string searchterms { get; set; }
        public string BuildSearchTerms() => $"{place_id} {customer_id} {city} {region}".ToLower();
    }
}
