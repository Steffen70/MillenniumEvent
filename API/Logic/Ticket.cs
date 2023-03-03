using System;
using System.Drawing;
using System.IO;
using API.Services;
using MimeKit;
using Spire.Barcode;

namespace API.Logic
{
    public class Ticket : Entities.Ticket
    {
        public Bitmap GenerateTicketBitmap(Image flyer)
        {
            var bs = new BarcodeSettings
            {
                Type = BarCodeType.Code39,
                Data = ToString()
            };

            var bg = new BarCodeGenerator(bs);
            var barcode = bg.GenerateImage();


            //Combine flyer and generated barcode

            var flyerHeight = (int)Math.Round((double)flyer.Height / flyer.Width * barcode.Width, MidpointRounding.AwayFromZero);

            var bitmapImage = new Bitmap(barcode.Width, barcode.Height + flyerHeight);

            using var g = Graphics.FromImage(bitmapImage);

            g.DrawImage(flyer, 0, 0, barcode.Width, flyerHeight);
            g.DrawImage(barcode, 0, flyerHeight);

            return bitmapImage;
        }

        public void SendViaMail(EmailService service, Bitmap bitmap)
        {
            using var ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

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
