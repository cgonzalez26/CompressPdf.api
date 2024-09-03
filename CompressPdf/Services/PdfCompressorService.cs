using System.IO;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using Microsoft.AspNetCore.Mvc;

namespace CompressPdf.Services
{
    public class PdfCompressorService
    {
        public byte[] CompressPdf(byte[] pdfBytes)
        {
            using (MemoryStream inputStream = new MemoryStream(pdfBytes))
            using (MemoryStream outputStream = new MemoryStream())
            {
                PdfReader reader = new PdfReader(inputStream);
                PdfWriter writer = new PdfWriter(outputStream, new WriterProperties().SetCompressionLevel(CompressionConstants.BEST_COMPRESSION));
                PdfDocument pdfDoc = new PdfDocument(reader, writer);

                int numberOfPages = pdfDoc.GetNumberOfPages();
                for (int i = 1; i <= numberOfPages; i++)
                {
                    PdfPage page = pdfDoc.GetPage(i);
                    PdfDictionary resources = page.GetResources().GetPdfObject();
                    PdfDictionary xObjectDictionary = resources.GetAsDictionary(PdfName.XObject);

                    if (xObjectDictionary != null)
                    {
                        foreach (PdfName imgName in xObjectDictionary.KeySet())
                        {
                            PdfObject pdfObj = xObjectDictionary.Get(imgName);
                            if (pdfObj is PdfStream pdfStream && pdfStream.Get(PdfName.Subtype).Equals(PdfName.Image))
                            {
                                // Obtener los datos de la imagen original
                                byte[] originalImageBytes = pdfStream.GetBytes();
                                ImageData imageData = ImageDataFactory.Create(originalImageBytes);

                                // Redimensionar la imagen y reducir su calidad
                                imageData.SetWidth(imageData.GetWidth() * 0.75f);
                                imageData.SetHeight(imageData.GetHeight() * 0.75f);

                                // Reemplazar la imagen en el PDF
                                PdfImageXObject compressedImageObject = new PdfImageXObject(imageData);
                                pdfStream.SetData(compressedImageObject.GetPdfObject().GetBytes());
                            }
                        }
                    }
                }

                pdfDoc.Close();
                return outputStream.ToArray();
            }
        }
    }
}
