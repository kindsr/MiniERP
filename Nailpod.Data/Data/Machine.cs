using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nailpod.Data
{
    [Table("master")]
    public partial class Machine
    {
        [Key]
        [DatabaseGenerat‌​ed(DatabaseGeneratedOption.None)]
        [JsonProperty("machine_id")]
        public long MachineID { get; set; }

        [JsonProperty("place_id")]
        public long PlaceID { get; set; }

        [JsonProperty("mac_addr")]
        public string MacAddr { get; set; }

        [JsonProperty("ip_addr")]
        public string IpAddr { get; set; }

        [JsonProperty("card_reader_product_id")]
        public string CardReaderProductID { get; set; }

        [JsonProperty("epson_printer_product_id")]
        public string EpsonPrinterProductID { get; set; }

        [JsonProperty("receipt_printer_product_id")]
        public string ReceiptPrinterProductID { get; set; }

        [JsonProperty("cpu")]
        public string Cpu { get; set; }

        [JsonProperty("hdd")]
        public string Hdd { get; set; }

        [JsonProperty("ram")]
        public string Ram { get; set; }

        [JsonProperty("usb_yn")]
        public string UsbYn { get; set; }

        [JsonProperty("scanner_yn")]
        public string ScannerYn { get; set; }

        [JsonProperty("machine_status")]
        public string MachineStatus { get; set; }

        [JsonProperty("mf_loc")]
        public string MfLoc { get; set; }

        [JsonProperty("mf_dt")]
        public DateTimeOffset? MfDt { get; set; }

        [JsonProperty("use_yn")]
        public string UseYn { get; set; }

        [JsonProperty("del_yn")]
        public string DelYn { get; set; }

        [JsonProperty("reg_dt")]
        public DateTimeOffset? RegDt { get; set; }

        [JsonProperty("upd_dt")]
        public DateTimeOffset? UpdDt { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public string BuildSearchTerms() => $"{MachineID} {PlaceID}".ToLower();
    }
}
