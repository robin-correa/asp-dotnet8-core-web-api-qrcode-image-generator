using System.ComponentModel;

namespace QRCodeImageGenerator.API.DataTransferObjects
{
    public class GenerateRequestDTO
    {
        [DefaultValue("Robin Correa <robin.correa21@gmail.com>")]
        public string Text { get; set; } = "Robin Correa <robin.correa21@gmail.com>";

        [DefaultValue("#000000")]
        public string QRLogoColorHex { get; set; } = "#000000";
        [DefaultValue(250)]
        public int Width { get; set; } = 250;
        [DefaultValue(250)]
        public int Height { get; set; } = 250;
    }
}
