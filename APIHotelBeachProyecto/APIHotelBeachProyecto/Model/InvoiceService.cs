using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using QuestPDF.Fluent;
using MailKit.Net.Smtp;
using QuestPDF.Helpers;


namespace APIHotelBeachProyecto.Model
{
    public class InvoiceService
    {
        private readonly DbContextHotel _context;
        private readonly IConfiguration _config;
        private readonly Customer _customer = new Customer();

        public InvoiceService(DbContextHotel context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public string GenerateAndSendInvoice(int invoiceId)
        {
            try
            {
                var factura = _context.Invoices.FirstOrDefault(f => f.InvoiceId == invoiceId);
                var cliente = _context.Customers.FirstOrDefault(c => c.CustomerId == factura.CustomerId);
                var reserva = _context.Reservations.FirstOrDefault(r => r.ReservationId == factura.ReservationId);
                var package = _context.Packages.FirstOrDefault(r => r.PackageId == reserva.PackageId);

                //si se paga con cheque: se obtienen los datos del cheque por medio de a quien le pertenece (el cheque ya tiene que estar creado)
                //En la aplicacion a la hora usar el boton de pagar, primero llamar a guardar cheque y luego a guardar reserva
                //var check = _context.Checks.FirstOrDefault(r => r.CustomerId == cliente.CustomerId);
                //var card = _context.Cards.FirstOrDefault(r => r.CustomerId == cliente.CustomerId);


                if (factura == null)
                    return "Factura no encontrada.";

                if (cliente == null)
                    return "Cliente no encontrado.";

                var usuario = _context.Users.FirstOrDefault(u => u.email == cliente.Email);
                if (usuario == null)
                    return "Usuario no encontrado.";

                using var stream = new MemoryStream();

                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);//el tamaño de la hoja
                        page.Margin(50); // el margen toda la pagina
                        page.PageColor(Colors.White);//el color de fondo de la pagina
                        page.DefaultTextStyle(x => x.FontSize(12));//tamaño de letra en toda la api

                        //Esto es para acomodar el encabezado
                        page.Header()
                        .Element(header =>
                        {
                            header
                                .Background(Colors.Orange.Lighten3)
                                .Padding(13)
                                .Row(row =>
                                {
                                    row.RelativeItem()
                                        .Text("Factura")
                                        .SemiBold()
                                        .FontSize(60)
                                        .FontColor(Colors.White);//Agregar a factura alienado a la izquierda

                                    row.ConstantItem(200)
                                        .Column(column =>
                                        {
                                            column.Item().Text($"# {factura.InvoiceId}")
                                                .FontSize(30)
                                                .FontColor(Colors.White); //agrega el numero de id de la factura alineada a la derecha

                                            column.Item().Text($"{factura.IssueDate:dd/MM/yyyy}")
                                                .FontSize(30)
                                                .FontColor(Colors.White);//agrega la fecha de emision de la factura alineada a la derecha
                                        });
                                });
                        });

                        //El contenido del medio, acomodado en tabla

                        page.Content()
                        .Element(container => container.PaddingTop(20))
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);//columna de campo
                                columns.RelativeColumn(2);//columna para los valores
                            });

                            table.Header(header =>
                            {
                                header.Cell()
                                    .Border(1)
                                    .BorderColor(Colors.Black)
                                    .Padding(5)
                                    .Background(Colors.Grey.Lighten3)
                                    .Text("Campo").SemiBold(); //se coloca el campo en color gris

                                header.Cell()
                                    .Border(1)
                                    .BorderColor(Colors.Black)
                                    .Padding(5)
                                    .Background(Colors.Grey.Lighten3)
                                    .Text("Valor").SemiBold();
                            });

                            void AddRow(string campo, string valor)
                            {
                                table.Cell()
                                    .Border(1)
                                    .BorderColor(Colors.Black)
                                    .Padding(5)
                                    .Text(campo);

                                table.Cell()
                                    .Border(1)
                                    .BorderColor(Colors.Black)
                                    .Padding(5)
                                    .Text(valor);
                            }
                            //se agregan las filas 

                            AddRow("Cliente", $"{factura.CustomerId}");
                            AddRow("Método de Pago", $"{factura.PaymentMethod}");
                            AddRow("Nombre", $"{cliente.Name}");
                            AddRow("Primer Apellido", $"{cliente.LastName}");
                            AddRow("Segundo Apellido", $"{cliente.SecondLastName}");
                            AddRow("Fecha de reservación", $"{reserva.ReservationDate:dd/MM/yyyy}");
                            AddRow("Paquete", $"{package.Name}");
                            AddRow("Costo por Persona", $"{package.CostPerPersonPerNight}");
                            AddRow("Descripción del Paquete", $"{package.Description}");
                            AddRow("Número de Personas", $"{reserva.NumberOfPeople}");
                            AddRow("Número de Noches", $"{reserva.NumberOfNights}");
                            AddRow("Descuento", $"{reserva.DiscountApplied}");
                            AddRow("IVA 13%", $"{reserva.IVA}");
                            AddRow("Total en Colones", $"{reserva.TotalColones}");
                            AddRow("Total en Dólares", $"{reserva.TotalDollars}");
                            AddRow("Tipo de Cambio $", $"{reserva.ExchangeRate}");
                            //Si se paga con cheque
                            // if (factura.PaymentMethod?.ToLower() == "cheque")
                            // {
                            //   AddRow("Número de Cheque", $"{check.CheckNumber}");//identificador
                            //   AddRow("Duenno de cheque", $"{check.CheckOwner}");
                            //   AddRow("Banco Emisor", $"{check.CheckBank}");
                            // AddRow("Cantidad", $"{check.Amount}");

                            //}
                            // if (factura.PaymentMethod?.ToLower() == "tarjeta")
                            //{
                            // AddRow("Número de tarjeta", $"{card.CardNumber}");//identificador
                            //  AddRow("Duenno de tarjeta", $"{card.CardOwner}");
                            // AddRow("Banco", $"{card.CardBank}");
                            // AddRow("Identificador de Pago", $"{card.PayId}");

                            //}
                        });



                        page.Footer()
                            .AlignCenter()
                            .Column(column =>
                            {
                                column.Item().Text("¡Gracias por su compra!").FontSize(15).SemiBold();
                                column.Item().Text("   HOTEL BEACH SA").FontSize(15).SemiBold();
                            });

                    });
                }).GeneratePdf(stream);

                stream.Position = 0;
                var pdfBytes = stream.ToArray();

                EnviarCorreo(usuario.email, pdfBytes, $"Factura_{factura.InvoiceId}.pdf");

                return "Factura generada y enviada correctamente.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private void EnviarCorreo(string destinatario, byte[] archivoPdf, string nombreArchivo)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config["Email:UserName"]));
            email.To.Add(MailboxAddress.Parse(destinatario));
            email.Subject = "Factura Generada";

            var body = new TextPart(TextFormat.Plain)
            {
                Text = "Adjunto encontrarás la factura generada."
            };

            var attachment = new MimePart("application", "pdf")
            {
                Content = new MimeContent(new MemoryStream(archivoPdf)),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = nombreArchivo
            };

            var multipart = new Multipart("mixed")
        {
            body,
            attachment
        };

            email.Body = multipart;

            using var smtp = new SmtpClient();
            smtp.Connect(_config["Email:Host"], int.Parse(_config["Email:Port"]), SecureSocketOptions.StartTls);
            smtp.Authenticate(_config["Email:UserName"], _config["Email:PassWord"]);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }

}