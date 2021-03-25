using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Nailpod.Models
{
    public partial class MachineModel : ObservableObject
    {
        static public MachineModel CreateEmpty() => new MachineModel { MachineID = -1, IsEmpty = true };

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

        public override void Merge(ObservableObject source)
        {
            if (source is MachineModel model)
            {
                Merge(model);
            }
        }

        public void Merge(MachineModel source)
        {
            if (source != null)
            {
                MachineID = source.MachineID;
                PlaceID = source.PlaceID;
                MacAddr = source.MacAddr;
                IpAddr = source.IpAddr;
                CardReaderProductID = source.CardReaderProductID;
                EpsonPrinterProductID = source.EpsonPrinterProductID;
                ReceiptPrinterProductID = source.ReceiptPrinterProductID;
                Cpu = source.Cpu;
                Hdd = source.Hdd;
                Ram = source.Ram;
                UsbYn = source.UsbYn;
                ScannerYn = source.ScannerYn;
                MachineStatus = source.MachineStatus;
                MfLoc = source.MfLoc;
                MfDt = source.MfDt;
                UseYn = source.UseYn;
                DelYn = source.DelYn;
                RegDt = source.RegDt;
                UpdDt = source.UpdDt;
            }
        }

        public override string ToString()
        {
            return MachineID.ToString();
        }
    }

    public partial class MachineModel
    {
        public static MachineModel[] FromJson(string json) => JsonConvert.DeserializeObject<MachineModel[]>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this MachineModel[] self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

}
