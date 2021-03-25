using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Nailpod.Http.Endpoints.PrintInfoEndPoint.Models
{
    public class PrintInfoRequestObj
    {
        [JsonProperty("p_machine_id")]
        public int MachineID { get; set; }

        [JsonProperty("p_design_type")]
        public int DesignType { get; set; }

        [JsonProperty("p_total_print_cnt")]
        public int TotalPrintCnt { get; set; }

        [JsonProperty("p_print_cnt")]
        public int PrintCnt { get; set; }

        [JsonProperty("p_print_file_name")]
        public string PrintFileName { get; set; }

        [JsonProperty("p_file_name_1")]
        public string FileName1 { get; set; }

        [JsonProperty("p_file_name_2")]
        public string FileName2 { get; set; }

        [JsonProperty("p_file_name_3")]
        public string FileName3 { get; set; }

        [JsonProperty("p_file_name_4")]
        public string FileName4 { get; set; }

        [JsonProperty("p_file_name_5")]
        public string FileName5 { get; set; }

        [JsonProperty("p_file_name_6")]
        public string FileName6 { get; set; }

        [JsonProperty("p_file_name_7")]
        public string FileName7 { get; set; }

        [JsonProperty("p_file_name_8")]
        public string FileName8 { get; set; }

        [JsonProperty("p_file_name_9")]
        public string FileName9 { get; set; }

        [JsonProperty("p_file_name_10")]
        public string FileName10 { get; set; }
    }
}
