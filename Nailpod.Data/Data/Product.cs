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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nailpod.Data
{
    [Table("products")]
    public partial class Product
    {
        [MaxLength(16)]
        [Key, Column(Order = 0)]
        public string product_id { get; set; }

        [Required]
        public int category_id { get; set; }

        [Required]
        [MaxLength(50)]
        public string name { get; set; }

        [Required]
        public decimal list_price { get; set; }
        [Required]
        public decimal dealer_price { get; set; }

        [Required]
        public decimal discount { get; set; }

        public DateTimeOffset? discount_begin_dt { get; set; }
        public DateTimeOffset? discount_end_dt { get; set; }

        [Required]
        public int stock_unit { get; set; }
        [Required]
        public int safety_stock_level { get; set; }

        public byte[] picture { get; set; }
        public byte[] thumbnail { get; set; }

        [Required]
        public int tax_type { get; set; }

        [MaxLength(400)]
        public string desc { get; set; }

        public long machine_id { get; set; }

        [MaxLength(50)]
        public string color { get; set; }

        [MaxLength(4)]
        public string size { get; set; }

        //public string search_terms { get; set; }

        public string use_yn { get; set; }

        [Required]
        public DateTimeOffset reg_dt { get; set; }
        [Required]
        public DateTimeOffset upd_dt { get; set; }

        //public string BuildSearchTerms() => $"{product_id} {name} {color}".ToLower();
    }
}
