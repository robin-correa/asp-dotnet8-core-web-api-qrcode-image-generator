using System.Drawing;
using System.Drawing.Imaging;

using Microsoft.AspNetCore.Mvc;

using QRCodeImageGenerator.API.DataTransferObjects;

using QRCoder;

namespace QRCodeImageGenerator.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QRCodeImageGeneratorController : ControllerBase
    {
        private static readonly string LogoPath = "./Images/logo.png";

        [HttpPost]
        [Route("generate")]
        public IActionResult Generate([FromBody] GenerateRequestDTO request)
        {
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(request.Text, QRCodeGenerator.ECCLevel.Q);

                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeImage = qrCode.GetGraphic(20);

                // Load the logo image
                using (var ms = new MemoryStream(qrCodeImage))
                using (var qrCodeBitmap = new Bitmap(ms))
                {
                    // Resize the QR code to specified dimensions
                    Bitmap resizedQrCodeBitmap = new Bitmap(qrCodeBitmap, new Size(request.Width, request.Height));

                    // Load the main logo image
                    var logo = Image.FromFile(LogoPath);

                    // Calculate logo size and position
                    int logoSize = resizedQrCodeBitmap.Height / 5;
                    var logoPosition = new Point((resizedQrCodeBitmap.Width - logoSize) / 2, (resizedQrCodeBitmap.Height - logoSize) / 2);

                    // Add main logo to QR code
                    using (var graphics = Graphics.FromImage(resizedQrCodeBitmap))
                    {
                        graphics.DrawImage(logo, new Rectangle(logoPosition, new Size(logoSize, logoSize)));
                    }

                    // Change QR code logo color
                    if (!string.IsNullOrEmpty(request.QRLogoColorHex))
                    {
                        // Convert hex color to Color object
                        Color qrLogoColor = ColorTranslator.FromHtml(request.QRLogoColorHex);

                        // Change color of QR code logo
                        ChangeQRCodeColor(resizedQrCodeBitmap, qrLogoColor);
                    }

                    // Convert the final image to a base64 string
                    using (var stream = new MemoryStream())
                    {
                        resizedQrCodeBitmap.Save(stream, ImageFormat.Png);
                        var base64String = Convert.ToBase64String(stream.ToArray());
                        return Ok(new { base64Image = base64String });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private void ChangeQRCodeColor(Bitmap qrCodeBitmap, Color color)
        {
            for (int x = 0; x < qrCodeBitmap.Width; x++)
            {
                for (int y = 0; y < qrCodeBitmap.Height; y++)
                {
                    if (qrCodeBitmap.GetPixel(x, y).R < 128)
                    {
                        qrCodeBitmap.SetPixel(x, y, color);
                    }
                }
            }
        }
    }
}