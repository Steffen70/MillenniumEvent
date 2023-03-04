using System;
using SkiaSharp;
using System.IO;
using API.Services;
using MimeKit;
using ZXing;
using ZXing.Common;
using ZXing.SkiaSharp;

namespace API.Logic
{
    public class Ticket : Entities.Ticket
    {
        public SKImage GenerateTicketBitmap(SKBitmap flyer)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_39,
                Options = new EncodingOptions {
                    Height = 400,
                    Width = 800,
                    PureBarcode = false,
                    Margin = 10
                }
            };

            var barcode = writer.Write(ToString());
            var flyerHeight = (int)Math.Round((double)flyer.Height / flyer.Width * barcode.Width, MidpointRounding.AwayFromZero);

            var bitmapImage = new SKBitmap(barcode.Width, barcode.Height + flyerHeight);

            using var g = new SKCanvas(bitmapImage);

            //g.DrawImage(SKImage.FromBitmap(flyer), 0, 0, barcode.Width, flyerHeight);
            g.DrawImage(SKImage.FromBitmap(flyer),0, 0);
            g.DrawImage(SKImage.FromBitmap(barcode), 0, flyerHeight);

            return SKImage.FromBitmap(bitmapImage);
        }

        public void SendViaMail(EmailService service, SKImage image)
        {
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var ms = new MemoryStream(data.ToArray());

            var builder = new BodyBuilder
            {
                HtmlBody = $"<p>Dein Millennium Event Ticket</p><img src=\"cid:{TicketKey}\"/>"
            };

            var att = new MimePart()
            {
                Content = new MimeContent(ms),
                ContentDisposition = new ContentDisposition(ContentDisposition.Inline),
                ContentTransferEncoding = ContentEncoding.Base64,
                ContentId = TicketKey.ToString(),
                FileName = $"{TicketKey}.png"
            };

            builder.Attachments.Add(att);

            service.SendEmail(new Message(new[] { Email }, $"Millennium Event Ticket", builder.ToMessageBody()));
        }
    }
}
