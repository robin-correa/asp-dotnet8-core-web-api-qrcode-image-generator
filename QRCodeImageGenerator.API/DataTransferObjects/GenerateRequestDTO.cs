namespace QRCodeImageGenerator.API.DataTransferObjects
{
    public class GenerateRequestDTO
    {
        public string Text { get; set; }
        public string QRLogoColorHex { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
