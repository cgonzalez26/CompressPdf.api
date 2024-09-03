using Microsoft.AspNetCore.Mvc;
using CompressPdf.Services;

namespace CompressPdf.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfController : Controller
    {
        private readonly PdfCompressorService _pdfCompressorService;

        public PdfController(PdfCompressorService pdfCompressorService)
        {
            _pdfCompressorService = pdfCompressorService;
        }

        [HttpPost("compress")]
        public IActionResult CompressPdf([FromBody] byte[] pdfBytes)
        {
            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                return BadRequest("El archivo PDF no puede estar vacío.");
            }

            try
            {
                byte[] compressedPdf = _pdfCompressorService.CompressPdf(pdfBytes);
                return File(compressedPdf, "application/pdf", "compressed.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error durante la compresión: {ex.Message}");
            }
        }
    }
}
