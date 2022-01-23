namespace EasyTLV
{
    public class TLV
    {
        private readonly Dictionary<string, TLVTagValue> tagValues = new();

        public void Add(string tag, string hexValue)
        {
            tagValues.Add(tag, new TLVTagValue(tag,))
        }
    }
}