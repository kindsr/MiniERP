using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Nailpod.Http.Endpoints.PaymentInfoEndpoint.Models
{
    public class PaymentInfoRequestObj
    {
        [JsonProperty("p_machine_id")]
        public int MachineID { get; set; }

        //[JsonProperty("p_customer_id")]
        //public int CustomerID { get; set; }

        [JsonProperty("p_price")]
        public double Price { get; set; }

        [JsonProperty("p_approval_no")]
        public string ApprovalNo { get; set; }

        [JsonProperty("p_card_owner")]
        public string CardOwner { get; set; }

        [JsonProperty("p_card_type")]
        public string CardType { get; set; }

        [JsonProperty("p_card_comp")]
        public string CardComp { get; set; }

        [JsonProperty("p_payment_dt")]
        public string PaymentDt  { get; set; }

        [JsonProperty("p_error_cd")]
        public string ErrorCd { get; set; }

        [JsonProperty("p_error_msg")]
        public string ErrorMsg { get; set; }
    }
}
