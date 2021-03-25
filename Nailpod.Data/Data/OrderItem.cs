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
    [Table("order_items")]
    public partial class OrderItem
    {
        [Key]
        [DatabaseGenerat‌​ed(DatabaseGeneratedOption.None)]
        public long order_id { get; set; }

        [Key, Column(Order = 1)]
        [DatabaseGenerat‌​ed(DatabaseGeneratedOption.None)]
        public int seq { get; set; }

        [Required]
        [MaxLength(16)]
        public string product_id { get; set; }

        [Required]
        public int qty { get; set; }
        [Required]
        public decimal unitprice { get; set; }
        [Required]
        public decimal discount { get; set; }
        [Required]
        public int tax_type { get; set; }
        [MaxLength(1)]
        public string del_yn { get; set; }
        public virtual Product Product { get; set; }
    }
}
