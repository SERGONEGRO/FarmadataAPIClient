namespace FarmadataAPIClient
{
    public class Good
    {
        public object refId { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string producer { get; set; }
        public string country { get; set; }
        public List<string> barcodes { get; set; }
        public object goodsGroupCodes { get; set; }
        public object mnnEn { get; set; }
        public object mnnRu { get; set; }
        public string rv { get; set; }
        public DateTime created { get; set; }
    }
}
