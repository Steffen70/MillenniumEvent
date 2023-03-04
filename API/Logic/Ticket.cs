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
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = 200,
                    Width = 200,
                    PureBarcode = false,
                    NoPadding = true
                }
            };

            using var qrcode = writer.Write(ToString());
            const int width = 1000;

            var flyerHeight = (int)Math.Round((double)flyer.Height / flyer.Width * width, MidpointRounding.AwayFromZero);

            using var bitmapImage = new SKBitmap(width, qrcode.Height + flyerHeight);
            using var g = new SKCanvas(bitmapImage);

            using var scaledBitmap =
                flyer.Resize(new SKImageInfo(width, flyerHeight), SKFilterQuality.High);


            g.DrawImage(SKImage.FromBitmap(scaledBitmap), 0, 0);

            var anz = (int)Math.Ceiling((double)width / qrcode.Width);

            for (var i = 0; i < anz; i++)
            {
                g.DrawImage(SKImage.FromBitmap(qrcode), i * qrcode.Width, flyerHeight);
            }


            return SKImage.FromBitmap(bitmapImage);
        }

        public void SendViaMail(EmailService service, SKImage image)
        {
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var ms = new MemoryStream(data.ToArray());

            var builder = new BodyBuilder
            {
                HtmlBody = $"<p>Dein Millennium Event Ticket</p><img src=\"cid:{TicketKey}\" width=\"350\"/>"
            };

            var att = new MimePart
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
