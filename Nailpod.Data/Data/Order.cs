#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nailpod.Data
{
    [Table("orders")]
    public partial class Order
    {
        [Key]
        [DatabaseGenerat‌​ed(DatabaseGeneratedOption.None)]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long order_id { get; set; }

        [Required]
        public long place_id { get; set; }

        [Required]
        public int order_status { get; set; }

        public int? payment_type { get; set; }

        [Required]
        public DateTimeOffset order_dt { get; set; }
        public DateTimeOffset? shipped_dt { get; set; }
        public DateTimeOffset? delivered_dt { get; set; }

        [MaxLength(20)]
        public string shipped_tel_no { get; set; }

        [MaxLength(120)]
        public string shipped_addr { get; set; }

        [MaxLength(30)]
        public string shipped_city { get; set; }

        [MaxLength(50)]
        public string shipped_region { get; set; }

        [MaxLength(2)]
        public string shipped_country_cd { get; set; }

        [MaxLength(15)]
        public string shipped_postalcode { get; set; }

        public int? shipper_id { get; set; }

        [MaxLength(50)]
        public string tracking_no { get; set; }

        public string cancel_yn { get; set; }

        public string del_yn { get; set; }

        [Required]
        public DateTimeOffset reg_dt { get; set; }

        [Required]
        public DateTimeOffset upd_dt { get; set; }
        public string searchterms { get; set; }

        public virtual Place Place { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public string BuildSearchTerms() => $"{order_id} {place_id} {shipped_city} {shipped_region}".ToLower();
    }
}
