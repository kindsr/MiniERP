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
    [Table("customer")]
    public partial class Customer
    {
        [Key]
        [DatabaseGenerat‌​ed(DatabaseGeneratedOption.None)]
        public long customer_id { get; set; }

        [Required]
        [MaxLength(50)]
        public string firstname { get; set; }

        [MaxLength(50)]
        public string lastname { get; set; }

        [MaxLength(1)]
        public string gender { get; set; }

        public byte[] picture { get; set; }
        public byte[] thumbnail { get; set; }

        [MaxLength(1)]
        public string pe_corp_gb { get; set; }

        [MaxLength(20)]
        public string identify_no { get; set; }

        [MaxLength(20)]
        public string phone_no { get; set; }

        [Required]
        [MaxLength(50)]
        public string email { get; set; }
        
        public string country_cd { get; set; }

        public DateTimeOffset? birthdate { get; set; }

        [MaxLength(200)]
        public string remark { get; set; }

        [MaxLength(1)]
        public string del_yn { get; set; }

        [Required]
        public DateTimeOffset reg_dt { get; set; }
        [Required]
        public DateTimeOffset? upd_dt { get; set; }
        public string searchterms { get; set; }

        public virtual ICollection<Place> Places { get; set; }

        public string BuildSearchTerms() => $"{customer_id} {firstname} {email}".ToLower();
    }
}
