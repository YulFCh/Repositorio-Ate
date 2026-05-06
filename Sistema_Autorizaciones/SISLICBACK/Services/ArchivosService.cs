using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using OfficeOpenXml;
using QuestPDF.Fluent;
using System.Globalization;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SISLICBACK.Services.Utils;
using SistemaLicencias.SHARED.DTOs;
using System.Data;

using System.Text.Json;
using System.Text.Json.Nodes;

using System.Text.RegularExpressions;
using Xceed.Words.NET;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace SISLICBACK.Services {
    public class ArchivosService {

        private static readonly HttpClient client = new HttpClient();
        private string _connDB;
        private readonly ConsultaService _cs;

        public ArchivosService(IConfiguration _configuration, ConsultaService cs) {
            _cs = cs ?? throw new ArgumentNullException(nameof(cs));
            _connDB = _configuration.GetSection("ConexionSQL:Licencia:Conexion").Value;            
        }

        public async Task<byte[]> GenerarPdfBytesDesdeStream(int id, Stream? imgStream, string tipo) {
            try {
                var datos = await getSolicitudxId3(id);
                var solicitud = JsonConvert.DeserializeObject<SolicitudesAPI>(
                    JsonConvert.SerializeObject(datos));

                if(imgStream != null) {
                    imgStream.Position = 0;
                    Console.WriteLine($"📷 Imagen cargada para solicitud ID {id}. Tamaño: {imgStream.Length} bytes");
                }

                if(tipo == "BLANCO") {
                    var doc = ConstAutorizacionFormatoBlanco(solicitud, imgStream);
                    return doc.GeneratePdf();
                }
                else {
                    //var doc = ConstAutorizacionFormato(solicitud, imgStream);
                    string anio = solicitud.vigencia_hasta.Year.ToString();
                    if(anio.StartsWith("2025")) {
                        Console.WriteLine($"{anio}");
                        var doc = ConstAutorizacionFormato(solicitud, imgStream);
                        return doc.GeneratePdf();
                    }
                    else {
                        var doc = NewConstAutorizacionFormato(solicitud, imgStream);
                        return doc.GeneratePdf();
                    }
                    
                }
                
            }
            catch(Exception ex) {
                Console.WriteLine($"Error - GenerarPdfBytesDesdeStream: {ex.Message}");
                throw;
            }
        }
        // Esta función convierte los bytes a Base64
        public async Task<string> ObtenerPdfBase64Async(int id, Stream? imgStream, string tipoPdf) {
            try {
                var pdfBytes = await GenerarPdfBytesDesdeStream(id, imgStream, tipoPdf);
                return Convert.ToBase64String(pdfBytes);
            }
            catch(Exception ex) {
                Console.WriteLine($"Error al convertir a base64: {ex.Message}");
                throw;
            }
        }

        public async Task<string> ObPdfBase64EPE(int id, int tipo) {
            try {

                var datos = await _cs.getAnuncioxID(id, 1);

                var solicitud = JsonConvert.DeserializeObject<ApiOpc>(
                    JsonConvert.SerializeObject(datos));


                var doc = CertificadoEPEFormato(solicitud.Resultado, tipo);

                var pdfBytes = doc.GeneratePdf();

                return Convert.ToBase64String(pdfBytes);
            }
            catch(Exception ex) {
                Console.WriteLine($"Error al convertir a base64: {ex.Message}");
                throw;
            }
        }

        //FORMATO CERTIFICADO COMERCIO AMBULATORIO
        QuestPDF.Infrastructure.IDocument ConstAutorizacionFormato(SolicitudesAPI sol, Stream? img) {
            DateTime today = DateTime.Now;
            var horarioLimpio = sol.aHorario?.Replace("{", "").Replace("}", "").Replace("\"", "").Trim();
            //var agreg = string.IsNullOrWhiteSpace(sol.observacion) ? "" : $" - {sol.observacion.Trim()}";
            //var limpio = sol.giros.Replace("-", "").Replace("* N0", "").Trim();

            return Document.Create(document => {
                //Pg 1
                document.Page(page => {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(15);
                    page.DefaultTextStyle(x => x.FontSize(12));


                    AddWatermark(page.Background());


                    page.Content().PaddingLeft(2, Unit.Centimetre).PaddingRight(1, Unit.Centimetre).Column(col2 => {
                        col2.Item().Height(135);

                        col2.Item().PaddingLeft(5, Unit.Centimetre).Text($"N° {sol.nroAutorizacion}-{sol.fechaAutorizacion:yyyy}-{sol.siglas_resolucion}").AlignCenter().FontSize(15).Bold();
                        col2.Item().PaddingLeft(5, Unit.Centimetre).Text($"Resolución de SubGerencia N° {sol.nroResolucion}").FontSize(10).AlignCenter();
                        col2.Item().PaddingLeft(5, Unit.Centimetre).Text($"Expediente N° {sol.nroExpediente}-{sol.fecha_expediente:yyyy}").FontSize(10).AlignCenter();

                        col2.Item().PaddingLeft(7, Unit.Centimetre).PaddingRight(2, Unit.Centimetre).PaddingTop(10).AlignCenter().Text(t => {
                            t.Span("Habiendo cumplido con los requisitos exigidos para la obtención de la ").FontSize(10);
                            t.Span("AUTORIZACIÓN MUNICIPAL").Bold().FontSize(10);
                            t.Span(" para el uso comercial del espacio público según la Ord. 1787-MML; Ord. 403-MDA y sus modificatorias, se otorga la presente a:").FontSize(10);
                        });

                        col2.Item().Height(25);
                        col2.Item().Row(r => {

                            // Cuadro  — lado izquierdo con imagen o texto
                            r.ConstantItem(30, Unit.Millimetre).Height(40, Unit.Millimetre)
                            .AlignCenter().AlignMiddle()
                             .Element(e => {
                                 if(img != null) {
                                     try {
                                         img.Position = 0;
                                         e.Border(0.5f).Image(img).FitArea();
                                     }
                                     catch(Exception ex) {
                                         Console.WriteLine("⚠ Error al insertar imagen: " + ex.Message);
                                         e.Text("Error img").FontSize(8).Italic();
                                     }
                                 }
                                 else {
                                     e.Text("").FontSize(9).Italic();
                                 }
                             });


                            // Razon Social centrado
                            r.RelativeItem(1).AlignMiddle().Text(sol.razon_social ?? "NOMBRE NO DISPONIBLE")
                             .Bold().FontSize(24).AlignCenter().FontColor(Color.FromHex("#4F81BD"));

                            // Imagen QR — lado derecho
                            var qr = Path.Combine(AppContext.BaseDirectory, "img", "consultalic2.png");
                            r.ConstantItem(80).Height(80).Width(80).Image(qr);
                        });


                        col2.Item().Height(15);

                        col2.Item().Text(t => {
                            t.Span("DNI/RUC: ").FontSize(9).FontColor(Color.FromHex("#4F81BD"));
                            t.Span(sol.nrodni ?? "NO REGISTRADO").FontSize(9).Bold();
                        });

                        col2.Item().Height(5);
                        col2.Item().Text(t => {
                            t.Span("PUNTO DE VENTA: ").FontSize(9).FontColor(Color.FromHex("#4F81BD"));
                            t.Span(sol.punto_local ?? "NO REGISTRADO").FontSize(9).Bold();
                        });

                        col2.Item().Height(5);
                        col2.Item().Text(t => {
                            t.Span("GIRO AUTORIZADO: ").FontSize(9).FontColor(Color.FromHex("#4F81BD"));
                            t.Span(sol.NombreComputado ?? "NO DEFINIDO").FontSize(9).Bold();
                            //t.Span(limpio ?? "NO DEFINIDO").FontSize(9).Bold();
                            //t.Span($" {agreg}" ?? "").FontSize(9).Bold();
                        });

                        col2.Item().Height(5);
                        col2.Item().Text(t => {
                            t.Span("HORARIO AUTORIZADO: ").FontSize(9).FontColor(Color.FromHex("#4F81BD"));
                            t.Span(string.IsNullOrWhiteSpace(horarioLimpio) ? "SIN HORARIO" : horarioLimpio).FontSize(9).Bold();
                        });

                        col2.Item().Height(5);

                        col2.Item().PaddingTop(9).Row(r => {
                            r.RelativeItem().Text($"Fecha de Vigencia: {sol.vigencia_hasta:dd 'de' MMMM 'del' yyyy}").AlignLeft().FontSize(9).Bold();
                            r.RelativeItem().Text($"Ate, {today:dd 'de' MMMM 'del' yyyy}").AlignRight().FontSize(9).Bold();
                        });
                    });

                    //page.Footer().AlignRight().Text(txt => txt.CurrentPageNumber());
                });
                //Pg 2

                // Página 2: nueva página agregada
                document.Page(page => {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(15);
                    page.DefaultTextStyle(x => x.FontSize(12));


                    AddWatermark2(page.Background());


                    page.Content().PaddingLeft(2, Unit.Centimetre).PaddingRight(1, Unit.Centimetre).Column(col2 => {
                        col2.Item().Height(150);

                    });

                    //page.Footer().AlignRight().Text(txt => txt.CurrentPageNumber());
                });

                void AddWatermark(QuestPDF.Infrastructure.IContainer container) {
                    container.Element(elem => {
                        var ruteImg = Path.Combine(AppContext.BaseDirectory, "img", "autov1.jpeg");

                        if(File.Exists(ruteImg)) {
                            byte[] imageData = File.ReadAllBytes(ruteImg);
                            elem.Image(imageData).FitArea();
                        }
                        else {
                            Console.WriteLine($"⚠ Imagen no encontrada: {ruteImg}");
                        }
                    });
                }

                void AddWatermark2(QuestPDF.Infrastructure.IContainer container) {
                    container.Element(elem => {
                        var ruteImg = Path.Combine(AppContext.BaseDirectory, "img", "autov2.jpeg");

                        if(File.Exists(ruteImg)) {
                            byte[] imageData = File.ReadAllBytes(ruteImg);
                            elem.Image(imageData).FitArea();
                        }
                        else {
                            Console.WriteLine($"Imagen no encontrada: {ruteImg}");
                        }
                    });
                }
            });
        }

        QuestPDF.Infrastructure.IDocument NewConstAutorizacionFormato(SolicitudesAPI sol, Stream? img) {
            DateTime today = DateTime.Now;
            var horarioLimpio = sol.aHorario?.Replace("{", "").Replace("}", "").Replace("\"", "").Trim();
            //var agreg = string.IsNullOrWhiteSpace(sol.observacion) ? "" : $" - {sol.observacion.Trim()}";
            //var limpio = sol.giros.Replace("-", "").Replace("* N0", "").Trim();

            return Document.Create(document => {
                //Pg 1
                document.Page(page => {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(15);
                    page.DefaultTextStyle(x => x.FontSize(12));


                    AddWatermark(page.Background());


                    page.Content().PaddingLeft(3.5f, Unit.Centimetre).PaddingRight(1, Unit.Centimetre).Column(col2 => {
                        col2.Item().Height(135);

                        col2.Item().PaddingLeft(5, Unit.Centimetre).Text($"N° {sol.nroAutorizacion}-{sol.fechaAutorizacion:yyyy}-{sol.siglas_resolucion}").AlignCenter().FontSize(15).Bold();
                        col2.Item().PaddingLeft(5, Unit.Centimetre).Text($"Resolución de SubGerencia N° {sol.nroResolucion}").FontSize(10).AlignCenter();
                        col2.Item().PaddingLeft(5, Unit.Centimetre).Text($"Expediente N° {sol.nroExpediente}-{sol.fecha_expediente:yyyy}").FontSize(10).AlignCenter();

                        col2.Item().PaddingLeft(7, Unit.Centimetre).PaddingRight(2, Unit.Centimetre).PaddingTop(10).AlignCenter().Text(t => {
                            t.Span("Habiendo cumplido con los requisitos exigidos para la obtención de la ").FontSize(10);
                            t.Span("AUTORIZACIÓN MUNICIPAL").Bold().FontSize(10);
                            t.Span(" para el uso comercial del espacio público según la Ord. 1787-MML; Ord. 403-MDA y sus modificatorias, se otorga la presente a:").FontSize(10);
                        });

                        col2.Item().Height(25);
                        col2.Item().Row(r => {

                            // Cuadro  — lado izquierdo con imagen o texto
                            r.ConstantItem(30, Unit.Millimetre).Height(40, Unit.Millimetre)
                            .AlignCenter().AlignMiddle()
                             .Element(e => {
                                 if(img != null) {
                                     try {
                                         img.Position = 0;
                                         e.Border(0.5f).Image(img).FitArea();
                                     }
                                     catch(Exception ex) {
                                         Console.WriteLine("⚠ Error al insertar imagen: " + ex.Message);
                                         e.Text("Error img").FontSize(8).Italic();
                                     }
                                 }
                                 else {
                                     e.Text("").FontSize(9).Italic();
                                 }
                             });


                            // Razon Social centrado
                            r.RelativeItem(1).AlignMiddle().Text(sol.razon_social ?? "NOMBRE NO DISPONIBLE")
                             .Bold().FontSize(24).AlignCenter().FontColor(Color.FromHex("#4F81BD"));

                            // Imagen QR — lado derecho
                            var qr = Path.Combine(AppContext.BaseDirectory, "img", "consultalic2.png");
                            r.ConstantItem(80).Height(80).Width(80).Image(qr);
                        });


                        col2.Item().Height(15);

                        col2.Item().Text(t => {
                            t.Span("DNI/RUC: ").FontSize(9).FontColor(Color.FromHex("#4F81BD"));
                            t.Span(sol.nrodni ?? "NO REGISTRADO").FontSize(9).Bold();
                        });

                        col2.Item().Height(5);
                        col2.Item().Text(t => {
                            t.Span("PUNTO DE VENTA: ").FontSize(9).FontColor(Color.FromHex("#4F81BD"));
                            t.Span(sol.punto_local ?? "NO REGISTRADO").FontSize(9).Bold();
                        });

                        col2.Item().Height(5);
                        col2.Item().Text(t => {
                            t.Span("GIRO AUTORIZADO: ").FontSize(9).FontColor(Color.FromHex("#4F81BD"));
                            t.Span(sol.NombreComputado ?? "NO DEFINIDO").FontSize(9).Bold();
                            //t.Span(limpio ?? "NO DEFINIDO").FontSize(9).Bold();
                            //t.Span($" {agreg}" ?? "").FontSize(9).Bold();
                        });

                        col2.Item().Height(5);
                        col2.Item().Text(t => {
                            t.Span("HORARIO AUTORIZADO: ").FontSize(9).FontColor(Color.FromHex("#4F81BD"));
                            t.Span(string.IsNullOrWhiteSpace(horarioLimpio) ? "SIN HORARIO" : horarioLimpio).FontSize(9).Bold();
                        });

                        col2.Item().Height(5);

                        col2.Item().PaddingTop(9).Row(r => {
                            r.RelativeItem().Text($"Fecha de Vigencia: {sol.vigencia_hasta:dd 'de' MMMM 'del' yyyy}").AlignLeft().FontSize(9).Bold();
                            r.RelativeItem().Text($"Ate, {today:dd 'de' MMMM 'del' yyyy}").AlignRight().FontSize(9).Bold();
                        });
                    });

                    //page.Footer().AlignRight().Text(txt => txt.CurrentPageNumber());
                });
                //Pg 2

                // Página 2: nueva página agregada
                document.Page(page => {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(15);
                    page.DefaultTextStyle(x => x.FontSize(12));


                    AddWatermark2(page.Background());


                    page.Content().PaddingLeft(2, Unit.Centimetre).PaddingRight(1, Unit.Centimetre).Column(col2 => {
                        col2.Item().Height(150);

                    });

                    //page.Footer().AlignRight().Text(txt => txt.CurrentPageNumber());
                });

                void AddWatermark(QuestPDF.Infrastructure.IContainer container) {
                    container.Element(elem => {
                        //var ruteImg = Path.Combine(AppContext.BaseDirectory, "img", "autov1.jpeg");
                        var ruteImg = Path.Combine(AppContext.BaseDirectory, "img", "ambv2_cara.png");


                        if(File.Exists(ruteImg)) {
                            byte[] imageData = File.ReadAllBytes(ruteImg);
                            elem.Image(imageData).FitArea();
                        }
                        else {
                            Console.WriteLine($"⚠ Imagen no encontrada: {ruteImg}");
                        }
                    });
                }

                void AddWatermark2(QuestPDF.Infrastructure.IContainer container) {
                    container.Element(elem => {
                        //var ruteImg = Path.Combine(AppContext.BaseDirectory, "img", "autov2.jpeg");
                        var ruteImg = Path.Combine(AppContext.BaseDirectory, "img", "ambv2.png");


                        if(File.Exists(ruteImg)) {
                            byte[] imageData = File.ReadAllBytes(ruteImg);
                            elem.Image(imageData).FitArea();
                        }
                        else {
                            Console.WriteLine($"Imagen no encontrada: {ruteImg}");
                        }
                    });
                }
            });
        }

        QuestPDF.Infrastructure.IDocument ConstAutorizacionFormatoBlanco(SolicitudesAPI sol, Stream? img) {
            DateTime today = DateTime.Now;
            var horarioLimpio = sol.aHorario?.Replace("{", "").Replace("}", "").Replace("\"", "").Trim();
            //var agreg = string.IsNullOrWhiteSpace(sol.observacion) ? "" : $" - {sol.observacion.Trim()}";
            //var limpio = sol.giros.Replace("-", "").Replace("* N0", "").Trim();

            return Document.Create(document => {
                //Pg 1
                document.Page(page => {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(15);
                    page.DefaultTextStyle(x => x.FontSize(12));



                    page.Content().PaddingLeft(2, Unit.Centimetre).PaddingRight(1, Unit.Centimetre).Column(col2 => {
                        col2.Item().Height(135);

                        col2.Item().PaddingLeft(5, Unit.Centimetre).Text($"N° {sol.nroAutorizacion}-{sol.fechaAutorizacion:yyyy}-{sol.siglas_resolucion}").AlignCenter().FontSize(15).Bold();
                        col2.Item().PaddingLeft(5, Unit.Centimetre).Text($"Resolución de SubGerencia N° {sol.nroResolucion}").FontSize(10).AlignCenter();
                        col2.Item().PaddingLeft(5, Unit.Centimetre).Text($"Expediente N° {sol.nroExpediente}-{sol.fecha_expediente:yyyy}").FontSize(10).AlignCenter();

                        col2.Item().PaddingLeft(7, Unit.Centimetre).PaddingRight(2, Unit.Centimetre).PaddingTop(10).AlignCenter().Text(t => {
                            t.Span("Habiendo cumplido con los requisitos exigidos para la obtención de la ").FontSize(10);
                            t.Span("AUTORIZACIÓN MUNICIPAL").Bold().FontSize(10);
                            t.Span(" para el uso comercial del espacio público según la Ord. 1787-MML; Ord. 403-MDA y sus modificatorias, se otorga la presente a:").FontSize(10);
                        });

                        col2.Item().Height(25);
                        col2.Item().Row(r => {

                            // Cuadro  — lado izquierdo con imagen o texto
                            r.ConstantItem(30, Unit.Millimetre).Height(40, Unit.Millimetre)
                            .AlignCenter().AlignMiddle()
                             .Element(e => {
                                 if(img != null) {
                                     try {
                                         img.Position = 0;
                                         e.Border(0.5f).Image(img).FitArea();
                                     }
                                     catch(Exception ex) {
                                         Console.WriteLine("⚠ Error al insertar imagen: " + ex.Message);
                                         e.Text("Error img").FontSize(8).Italic();
                                     }
                                 }
                                 else {
                                     e.Text("").FontSize(9).Italic();
                                 }
                             });


                            // Razon Social centrado
                            r.RelativeItem(1).AlignMiddle().Text(sol.razon_social ?? "NOMBRE NO DISPONIBLE")
                             .Bold().FontSize(24).AlignCenter().FontColor(Color.FromHex("#4F81BD"));

                            // Imagen QR — lado derecho
                            var qr = Path.Combine(AppContext.BaseDirectory, "img", "consultalic2.png");
                            r.ConstantItem(80).Height(80).Width(80).Image(qr);
                        });


                        col2.Item().Height(15);

                        col2.Item().Text(t => {
                            t.Span("DNI/RUC: ").FontSize(9).FontColor(Color.FromHex("#4F81BD"));
                            t.Span(sol.nrodni ?? "NO REGISTRADO").FontSize(9).Bold();
                        });

                        col2.Item().Height(5);
                        col2.Item().Text(t => {
                            t.Span("PUNTO DE VENTA: ").FontSize(9).FontColor(Color.FromHex("#4F81BD"));
                            t.Span(sol.punto_local ?? "NO REGISTRADO").FontSize(9).Bold();
                        });

                        col2.Item().Height(5);
                        col2.Item().Text(t => {
                            t.Span("GIRO AUTORIZADO: ").FontSize(9).FontColor(Color.FromHex("#4F81BD"));
                            t.Span(sol.NombreComputado ?? "NO DEFINIDO").FontSize(9).Bold();
                            //t.Span(limpio ?? "NO DEFINIDO").FontSize(9).Bold();
                            //t.Span($" {agreg}" ?? "").FontSize(9).Bold();
                        });

                        col2.Item().Height(5);
                        col2.Item().Text(t => {
                            t.Span("HORARIO AUTORIZADO: ").FontSize(9).FontColor(Color.FromHex("#4F81BD"));
                            t.Span(string.IsNullOrWhiteSpace(horarioLimpio) ? "SIN HORARIO" : horarioLimpio).FontSize(9).Bold();
                        });

                        col2.Item().Height(5);

                        col2.Item().PaddingTop(9).Row(r => {
                            r.RelativeItem().Text($"Fecha de Vigencia: {sol.vigencia_hasta:dd 'de' MMMM 'del' yyyy}").AlignLeft().FontSize(9).Bold();
                            r.RelativeItem().Text($"Ate, {today:dd 'de' MMMM 'del' yyyy}").AlignRight().FontSize(9).Bold();
                        });
                    });

                    //page.Footer().AlignRight().Text(txt => txt.CurrentPageNumber());
                });




            });
        }

        //FORMATO CERTIFICADO DE PANEL
        QuestPDF.Infrastructure.IDocument CertificadoEPEFormato2(List<AnuncioDetalleDto> an, int tipo) {
            DateTime today = DateTime.Now;
            var horarioLimpio = an[0].Horario?.Replace("{", "").Replace("}", "").Replace("\"", "").Trim();
            //var agreg = string.IsNullOrWhiteSpace(sol.observacion) ? "" : $" - {sol.observacion.Trim()}";
            //var limpio = sol.giros.Replace("-", "").Replace("* N0", "").Trim();

            return Document.Create(document => {
                //Pg 1
                document.Page(page => {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(15);
                    page.DefaultTextStyle(x => x.FontSize(12));


                    if(an.Count == 1) {
                        AddWatermark(page.Background());
                    }



                    page.Content().PaddingLeft(2, Unit.Centimetre).PaddingRight(1, Unit.Centimetre).Column(col2 => {
                        col2.Item().Height(180);

                        col2.Item().PaddingLeft(5, Unit.Centimetre).Text($"N° {an[0].NroAutorizacion}-{GetYear(an[0].FechaAutorizacion)}-SGCL").AlignCenter().FontSize(15).Bold();
                        col2.Item().PaddingLeft(5, Unit.Centimetre).Text($"Resolución de SubGerencia N° {an[0].NroResolucion}").FontSize(10).AlignCenter();
                        col2.Item().PaddingLeft(5, Unit.Centimetre).Text($"Expediente N° {an[0].NroExpediente}-{GetYear(an[0].FechaExpediente)}").FontSize(10).AlignCenter();

                        col2.Item().PaddingLeft(7, Unit.Centimetre).PaddingRight(2, Unit.Centimetre).PaddingTop(10).AlignCenter().Text(t => {
                            t.Span("Habiendo cumplido con los requisitos para obtener la ").FontSize(10);
                            t.Span("AUTORIZACION DE INSTALCION DEL ELEMENTO DE PUBLICIDAD EXTERIOR,").Bold().FontSize(10);
                            t.Span(" de acuerdo a la Ord. Municipal N° 2682-MML, Ord. 414-MDA, Ley 27972, se concede el presente certificado a:").FontSize(10);
                        });

                        col2.Item().Height(25);


                        col2.Item().Height(15);

                        col2.Item().Text(an[0].Nombre).FontSize(15);

                        col2.Item().Text(t => {
                            t.Span("DNI").FontSize(9).FontColor(Color.FromHex("#4F81BD"));
                            t.Span(an[0].NroDni ?? "NO REGISTRADO").FontSize(9).Bold();
                            t.Span(" - ").FontSize(9);
                            t.Span("RUC").FontSize(9).FontColor(Color.FromHex("#4F81BD"));
                            t.Span(an[0].NroRuc ?? "NO REGISTRADO").FontSize(9).Bold();
                        });

                        col2.Item().Height(5);
                        col2.Item().Text(t => {
                            t.Span("DIRECCION: ").FontSize(9).FontColor(Color.FromHex("#4F81BD"));
                            t.Span(an[0].Direccion ?? "NO REGISTRADO").FontSize(9).Bold();
                        });



                        col2.Item().Height(5);
                        col2.Item().Text(t => {
                            t.Span("HORARIO AUTORIZADO: ").FontSize(9).FontColor(Color.FromHex("#4F81BD"));
                            t.Span(string.IsNullOrWhiteSpace(horarioLimpio) ? "SIN HORARIO" : horarioLimpio).FontSize(9).Bold();
                        });

                        col2.Item().Text("INICIO: 03 de Setiembre 2025").FontSize(9).Bold();
                        col2.Item().Text("VENCE: SIN VENCIMIENTO");
                        col2.Item().Text("PROCEDIMIENTO: INDETEERMINADO");

                        col2.Item().Height(5);
                        col2.Item().Text(t => {
                            t.Span("DETALLE EPE ").FontSize(9).FontColor(Color.FromHex("#4F81BD"));
                            t.Span(an[0].TipoUbicacion ?? "SIN DATOS").FontSize(9);
                            t.Span(" - ").FontSize(9);
                            t.Span(an[0].TipoCaracteristica ?? "SIN DATOS").FontSize(9);
                            t.Span(" - ").FontSize(9);
                            t.Span(an[0].TipoElemento ?? "SIN DATOS").FontSize(9);
                            t.Span(" - ").FontSize(9);
                            t.Span(an[0].TipoEstructura ?? "SIN DATOS").FontSize(9);

                        });

                        col2.Item().Height(5);

                        col2.Item().PaddingTop(9).Row(r => {
                            r.RelativeItem().Text($"Fecha de Vigencia: {an[0].FechaVigencia:dd 'de' MMMM 'del' yyyy}").AlignLeft().FontSize(9).Bold();
                            r.RelativeItem().Text($"Ate, {today:dd 'de' MMMM 'del' yyyy}").AlignRight().FontSize(9).Bold();
                        });
                    });

                    //page.Footer().AlignRight().Text(txt => txt.CurrentPageNumber());
                });
                //Pg 2

                // Página 2: nueva página agregada
                document.Page(page => {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(15);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    if(tipo == 1) {
                        AddWatermark2(page.Background());
                    }

                    page.Content().PaddingLeft(2, Unit.Centimetre).PaddingRight(1, Unit.Centimetre).Column(col2 => {
                        col2.Item().Height(150);

                    });

                    //page.Footer().AlignRight().Text(txt => txt.CurrentPageNumber());
                });

                void AddWatermark(QuestPDF.Infrastructure.IContainer container) {
                    container.Element(elem => {
                        var ruteImg = Path.Combine(AppContext.BaseDirectory, "img", "EPEc1.jpg");


                        if(File.Exists(ruteImg)) {
                            byte[] imageData = File.ReadAllBytes(ruteImg);
                            elem.Image(imageData).FitArea();
                        }
                        else {
                            Console.WriteLine($"⚠ Imagen no encontrada: {ruteImg}");
                        }
                    });
                }

                void AddWatermark2(QuestPDF.Infrastructure.IContainer container) {
                    container.Element(elem => {
                        var ruteImg = Path.Combine(AppContext.BaseDirectory, "img", "EPEc2.jpg");


                        if(File.Exists(ruteImg)) {
                            byte[] imageData = File.ReadAllBytes(ruteImg);
                            elem.Image(imageData).FitArea();
                        }
                        else {
                            Console.WriteLine($"⚠ Imagen no encontrada: {ruteImg}");
                        }
                    });
                }

                string GetYear(string fecha) {
                    if(DateTime.TryParseExact(fecha, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)) {
                        return dt.ToString("yyyy");
                    }
                    return "";
                }
            });
        }
        QuestPDF.Infrastructure.IDocument CertificadoEPEFormato(List<AnuncioDetalleDto> an, int tipo) {
            DateTime today = DateTime.Now;

            return Document.Create(document => {
                // Página 1
                document.Page(page => {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // HEADER
                    page.Header().PaddingTop(40).Row(row => {
                        row.RelativeItem().Column(col => {
                            col.Item().Text("");
                        });

                        row.ConstantItem(180).PaddingRight(15).Border(1).BorderColor(Colors.Blue.Darken4).Padding(5).Column(col => {
                            col.Item().AlignCenter().Text("AUTORIZACION").Bold().FontSize(12);
                            col.Item().AlignCenter().Text($"N° {an[0].NroAutorizacion}-{today.Year}").Bold();
                            col.Item().AlignCenter().Text($"RSG N° {an[0].NroResolucion}");
                            col.Item().AlignCenter().Text($"EXP. N° {an[0].NroExpediente}");
                        });
                    });

                    if(an.Count == 1) {
                        AddWatermark(page.Background());
                    }


                    // CONTENT
                    page.Content().PaddingTop(80).Row(row => {
                        row.RelativeItem(); // aire izquierda

                        // Ajusta 740 según tu gusto; 720–780 suele verse bien en A4 horizontal
                        row.ConstantItem(740).Column(col => {
                            col.Spacing(10);
                            col.Item().Height(0);
                            col.Item().PaddingHorizontal(50).Text(t => {
                                t.Span("Habiendo cumplido con los requisitos para obtener la ").FontSize(10);
                                t.Span("AUTORIZACION DE INSTALACION DEL ELEMENTO DE PUBLICIDAD EXTERIOR, ").FontSize(10).Bold();
                                t.Span("de acuerdo a la Ord. Municipal N° 2682-MML, Ord. 414-MDA, Ley 27972, se concede el presente certificado a:")
                                 .FontSize(10);
                            });
                            col.Item().Height(3);
                            col.Item().PaddingTop(10).AlignCenter().Text(an[0].Nombre ?? "RAZON SOCIAL NO REGISTRADA").Bold().FontSize(14);
                            
                            col.Item().Height(5);

                            col.Item().PaddingTop(6).Column(c2 => {
                                c2.Spacing(2);
                                c2.Item().Text($"UBICACION GEOGRAFICA: {an[0].Direccion}").FontSize(10).Bold();
                                c2.Item().Text(t => {
                                    t.Span("RUC N°: ").FontSize(10).Bold();
                                    t.Span("NO REGISTRADO   ").FontSize(10);
                                    t.Span("UBICACION FISICA: ").FontSize(10).Bold();
                                    t.Span("SIN DATOS").FontSize(10);
                                });

                                c2.Item().Text(t => {
                                    t.Span("LEYENDA: ").FontSize(10).Bold();
                                    t.Span("SIN DATOS").FontSize(10);
                                });

                                c2.Item().Text(t => {
                                    t.Span("CLASIFICACION: ").FontSize(10).Bold();
                                    t.Span($"{an[0].TipoElemento}   ").FontSize(10);
                                    t.Span("TIPO: ").FontSize(10).Bold();
                                    t.Span($"{an[0].TipoCaracteristica}   ").FontSize(10);
                                    t.Span("MATERIAL: ").FontSize(10).Bold();
                                    t.Span($"{an[0].TipoEstructura} | {an[0].TipoMaterial}").FontSize(10);
                                });

                                c2.Item().Text(t => {
                                    t.Span("DIMENSIONES: ").FontSize(10).Bold();
                                    t.Span($"{an[0].Altoa} x {an[0].Ancho} = {an[0].Area} M2      ").FontSize(10);
                                    t.Span("CARA(S): ").FontSize(10).Bold(); 
                                    t.Span($"{an[0].Nrocaras}").FontSize(10);
                                });
                            });

                            col.Item().Height(5);
                            col.Item().PaddingTop(14).Row(r => {

                                r.RelativeItem().Column(c3 => {
                                    c3.Item().Text($"INICIO: {an[0].FechaAutorizacion:dd 'de' MMMM yyyy}").FontSize(10).Bold();
                                    c3.Item().Text("VENCE: SIN VENCIMIENTO").FontSize(10).Bold();
                                    c3.Item().Text("PROCEDIMIENTO: INDETERMINADO").FontSize(10).Bold();
                                });
                                r.RelativeItem().AlignBottom().AlignCenter().Text($"Ate, {today:dd 'de' MMMM yyyy}").FontSize(10).Bold();
                            });
                        });

                        row.RelativeItem();
                    });
                });

                // Página 2
                document.Page(page => {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(15);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    if(tipo == 1) {
                        AddWatermark2(page.Background());
                    }

                    page.Content().PaddingLeft(2, Unit.Centimetre).PaddingRight(1, Unit.Centimetre).Column(col2 => {
                        col2.Item().Height(150);

                    });

                    //page.Footer().AlignRight().Text(txt => txt.CurrentPageNumber());
                });
                void AddWatermark(QuestPDF.Infrastructure.IContainer container) {
                    container.Element(elem => {
                        var ruteImg = Path.Combine(AppContext.BaseDirectory, "img", "EPEc1.jpg");


                        if(File.Exists(ruteImg)) {
                            byte[] imageData = File.ReadAllBytes(ruteImg);
                            elem.Image(imageData).FitArea();
                        }
                        else {
                            Console.WriteLine($"⚠ Imagen no encontrada: {ruteImg}");
                        }
                    });
                }

                void AddWatermark2(QuestPDF.Infrastructure.IContainer container) {
                    container.Element(elem => {
                        var ruteImg = Path.Combine(AppContext.BaseDirectory, "img", "EPEc2.jpg");


                        if(File.Exists(ruteImg)) {
                            byte[] imageData = File.ReadAllBytes(ruteImg);
                            elem.Image(imageData).FitArea();
                        }
                        else {
                            Console.WriteLine($"⚠ Imagen no encontrada: {ruteImg}");
                        }
                    });
                }

                string GetYear(string fecha) {
                    if(DateTime.TryParseExact(fecha, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)) {
                        return dt.ToString("yyyy");
                    }
                    return "";
                }
            });
        }


        public async Task<object> getSolicitudxId3(int id) {
            Dictionary<string, object> resultado = null;

            using(var connection = new SqlConnection(_connDB)) {
                try {
                    await connection.OpenAsync();

                    string query = @"
                SELECT 
                 sa.*, 
				 sl.nrodni AS nrodni,
                 gsa.NombreComputado,
                 sz.Nombre as 'nombSZ'    
             FROM Autorizacion.Solicitud_AUT sa
             LEFT JOIN Autorizacion.vw_GiroSolicitudAutorizacion gsa 
                 ON TRY_CAST(sa.giros AS INT) = gsa.IdGiroSolicitud
             LEFT JOIN Autorizacion.TB_SUBZONAS_AUTORIZACION_COORD sz
              on sa.idSudZona = sz.IdSubzona
			   LEFT JOIN Autorizacion.Solicitante_AUT sl
              on sa.id_solicitante = sl.idSolicitante
             WHERE sa.estado = '1' AND sa.estadoTramite = 5 AND sa.idSolicitud = @id
             ORDER BY sa.fechaRegistro DESC";

                    using(var command = new SqlCommand(query, connection)) {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@id", id);

                        using(var reader = await command.ExecuteReaderAsync()) {
                            if(await reader.ReadAsync()) // ← solo un resultado
                            {
                                resultado = new Dictionary<string, object>();

                                for(int i = 0; i < reader.FieldCount; i++) {
                                    resultado[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                }
                            }
                        }
                    }
                }
                catch(Exception ex) {
                    Console.WriteLine($"Error en la consulta getSolicitudxId3: {ex.Message}");
                    return null;
                }
            }

            return resultado; // ← ya no es lista, solo un objeto
        }

        public async Task<DocumentoDTO?> getDataxDocumento(int idSolicitud) {
            DocumentoDTO dto = null;

            var sql = @"
                 SELECT
                      sol.nroExpediente,
                      sol.fecha_expediente,
                      s.nombre,
                      s.nrodni,
                      sol.punto_local as 'direcPred',
                      gsa.IdGiroPrincipal as 'giros',
                      gsa.NombreComputado as 'nombreGiro',
                      sol.observacion,
                      sol.vigencia_hasta,
                      sol.aHorario,
                      ISNULL(tsac.Nombre, '') as 'subZona'
                FROM Autorizacion.Solicitud_AUT sol
                JOIN Autorizacion.Solicitante_AUT s ON sol.id_solicitante = s.idSolicitante
                FULL JOIN Autorizacion.vw_GiroSolicitudAutorizacion gsa ON TRY_CAST(sol.giros AS INT) = gsa.IdGiroSolicitud
                LEFT JOIN Autorizacion.TB_SUBZONAS_AUTORIZACION_COORD tsac ON TRY_CAST(SOL.idSudZona as INT) = tsac.IdSubzona
                WHERE Desc_solicitud = 'AUTORIZACION' AND sol.idSolicitud =  @idSolicitud

            ";

            using(var connection = new SqlConnection(_connDB)) {
                await connection.OpenAsync();

                using(var command = new SqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@idSolicitud", idSolicitud);

                    using(var reader = await command.ExecuteReaderAsync()) {
                        if(await reader.ReadAsync()) {
                            dto = new DocumentoDTO {
                                nroExpediente = reader["nroExpediente"]?.ToString(),
                                FecExpediente = Convert.ToDateTime(reader["fecha_expediente"]).ToString("dd/MM/yyyy"),
                                RazSocial = reader["nombre"]?.ToString(),
                                Dni = reader["nrodni"]?.ToString(),
                                Direccion = reader["direcPred"]?.ToString(),
                                giro = reader["giros"]?.ToString(),
                                nombreGiro = reader["nombreGiro"]?.ToString(),
                                obs = reader["observacion"]?.ToString(),
                                fecVencimineto = reader["vigencia_hasta"] != DBNull.Value ? Convert.ToDateTime(reader["vigencia_hasta"]).ToString("dd/MM/yyyy") : "",
                                aHorario = reader["aHorario"]?.ToString(),
                                subZona = reader["subZona"]?.ToString()
                                
                            };
                        }
                    }
                }
            }

            return dto;
        }



        public byte[] GenerarResoluciones(DocumentoDTO datosSolicitud) {
            var path = "";
            var giro = datosSolicitud.giro?.ToString()?.Trim();

            if(giro == "3") { //ORDENANZA N° 302-2012-MDA Y SUS MODIFICATORIAS - GOLOSINAS Y FLORES
                path = Path.Combine(AppContext.BaseDirectory, "document", "PATRON_RESOL_DE_GOLOSINAS.docx");//77154
            }
            else if(giro == "1") {//ORDENANZA N° 317-2013-MDA - VENTA DE ALIMENTOS
                path = Path.Combine(AppContext.BaseDirectory, "document", "PATRON_RESOL_DE_ALIMENTOS.docx");//77156
            }
            else if(giro == "4") {//ORDENANZA N° 354-2014-MDA - DIARIOS, REVISTAS, BILLETES DE LOTERIA Y SUS PRODUCTOS COMPLEMENTARIOS
                path = Path.Combine(AppContext.BaseDirectory, "document", "PATRON_RESOL_DE_CANILLITAS_FINAL.docx");//
            }
            else if(giro == "5") {//ORDENANZA N° 387-2014-MDA - CERRAJERIA FINA (LLAVEROS)
                path = Path.Combine(AppContext.BaseDirectory, "document", "PATRON_RESOL_DE_LLAVEROS.docx");//
            }
            else if(giro == "2") {//ORDENANZA N° 301-2012-MDA Y SUS MODIFICATORIAS - BEBIDAS CALIENTA Y COMPLEMENTOS
                path = Path.Combine(AppContext.BaseDirectory, "document", "PATRON_RESOL_DE_EMOLIENTEROS.docx");//
            }
            //new Giro { Value = "5", Text = " - LUSTRADORES DE CALZADO * N0" },
            else {
                path = Path.Combine(AppContext.BaseDirectory, "document", "PATRON_RESOL_OTROS.docx");//
            }

            using var doc = DocX.Load(path);

            var reemplazos = new Dictionary<string, string>{
                { "N_DEL_EXPEDIENTE",datosSolicitud.nroExpediente },
                { "FECHA_DEL_EXPEDIENTE", datosSolicitud.FecExpediente },
                { "NOMBRE", datosSolicitud.RazSocial?.ToString() ?? "" },
                { "DNI", datosSolicitud.Dni?.ToString() ?? "" },
                { "DIRECCION_DEL_MODULO", datosSolicitud.Direccion?.ToString() ?? "" },
                { "GIRO", datosSolicitud.nombreGiro?.ToString() ?? ""},
                { "OBS", string.IsNullOrWhiteSpace(datosSolicitud.obs?.ToString()) ? "" : $" - {datosSolicitud.obs?.ToString().Trim()}"},
                { "FECHA_VENCIMIENTO",  datosSolicitud.fecVencimineto},
                { "COORD", datosSolicitud.subZona?.ToString() ?? ""},
                { "HORARIO", datosSolicitud.aHorario?.ToString() ?? "" }
             };

            foreach(var par in doc.Paragraphs) {
                foreach(var campo in reemplazos) {
                    par.ReplaceText($"«{campo.Key}»", campo.Value);
                }
            }

            using var stream = new MemoryStream();
            doc.SaveAs(stream);
            return stream.ToArray();
        }

        //EXPORTAR RESOLUCION WORD

        public async Task<byte[]> GenResWorEPE(int idanunc, IWebHostEnvironment env) {
            // 0) Guard clauses
            if(idanunc <= 0) throw new ArgumentException("Id de anuncio inválido.", nameof(idanunc));
            if(env == null) throw new ArgumentNullException(nameof(env), "IWebHostEnvironment es null (no se inyectó).");
            //if(cs == null) throw new NullReferenceException("El servicio 'cs' es null (no fue inyectado en ArchivosService).");

            // 1) Ejecuta SP
            var envObj = await _cs.getAnuncioxID(idanunc, 1);
            if(envObj == null) throw new NullReferenceException("getAnuncioxID devolvió null.");

            // 2) Serializa objeto anónimo -> JSON
            var json = System.Text.Json.JsonSerializer.Serialize(envObj);
            if(string.IsNullOrWhiteSpace(json)) throw new NullReferenceException("Serialización JSON vacía.");

            // 3) Parse JSON
            var node = JsonNode.Parse(json);
            if(node is null) throw new NullReferenceException("JsonNode.Parse devolvió null.");

            var arr = node["resultado"]?.AsArray();
            if(arr is null) throw new NullReferenceException("El nodo 'resultado' no existe o no es un array.");
            if(arr.Count == 0) throw new InvalidOperationException("El array 'resultado' está vacío.");

            var it = arr[0] as JsonObject;
            if(it is null) throw new NullReferenceException("El primer elemento de 'resultado' no es un objeto JSON.");

            static string S(JsonObject o, string k)
                => o.ContainsKey(k) && o[k] is not null ? (o[k]!.GetValue<string>() ?? "") : "";

            var reemplazos = new Dictionary<string, string> {
                ["N_EXPEDIENTE"] = S(it, "nroExpediente"),
                ["FECHA_EXPEDIENTE"] = S(it, "fechaExpediente"),
                ["TIPO_UBICACION"] = S(it, "tipoUbicacion"),
                ["TIPO_ELEMENTO"] = S(it, "tipoElemento"),
                ["TIPO_CARACTERISTICA"] = S(it, "tipoCaracteristica"),
                ["TIPO_ESTRUCTURA"] = S(it, "tipoEstructura"),
                ["TIPO_MATERIAL"] = S(it, "tipoMaterial"),
                ["NOMBRES"] = S(it, "nombre"),
                ["DNI"] = S(it, "nrodni"),
                ["RUC"] = S(it, "nroruc"),
                ["ALTURA"] = S(it, "altoa"),
                ["ANCHO"] = S(it, "ancho"),
                ["AREA"] = S(it, "area"),
                ["NROCARAS"] = S(it, "nrocaras"),
                ["ZONA_URB"] = S(it, "zonaUrbana"),
                ["ZONIFICACION"] = S(it, "zonificacion"),
                ["HORARIO"] = LimpiarTexto(S(it, "horario")),
                ["FECHA_VIGENCIA"] = S(it, "fechaVigencia"),
                ["NRO_LICENCIA"] = S(it, "nroLicencia"),

            };

            // 6) Ubicar la plantilla con IWebHostEnvironment
            var path = Path.Combine(env.ContentRootPath ?? AppContext.BaseDirectory, "document", "PATRON_RESOL_EPE.docx");
            if(!File.Exists(path))
                throw new FileNotFoundException($"No se encontró la plantilla: {path}", path);


            using var doc = DocX.Load(path);

            foreach(var kv in reemplazos) {
                doc.ReplaceText($"«{kv.Key}»", kv.Value ?? string.Empty, false, RegexOptions.None);
            }

            using var ms = new MemoryStream();
            doc.SaveAs(ms);
            return ms.ToArray();
        }


        public static string LimpiarTexto(string input) {
            if(string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = input.Replace("{", "")
                         .Replace("}", "")
                         .Replace("*", "")
                         .Replace("N0", "")
                         .Replace("-", "")
                         .Replace("\"", "")
                         .Replace(":", ": ")
                         .Replace("\r", "")
                         .Replace("\n", "")
                         .Replace("\t", "");

            input = Regex.Replace(input, @"\s+", " ");

            return input.Trim();
        }


        public async Task<(string nombreNumerico, string nombreOriginal)> GuardarArchivoEnRed(IFormFile archivo) {
            string rutaRed = @"\\192.168.0.143\tmpauth";
            string usuario = @"muniate\jyacolcac";
            string contrasena = "soportecmd2025";

            using(new NetworkShareAccesser(rutaRed, usuario, contrasena)) {
                var extension = Path.GetExtension(archivo.FileName).ToLower();
                string nuevoNombre = GenerarNombreNumericoSeguro(extension);
                var rutaDestino = Path.Combine(rutaRed, nuevoNombre);

                using var stream = new FileStream(rutaDestino, FileMode.Create);
                await archivo.CopyToAsync(stream);

                return (nuevoNombre, archivo.FileName);
            }
        }

        private string GenerarNombreNumericoSeguro(string extension) {
            var random = new Random();
            var numero = random.Next(1000000000, int.MaxValue);
            var timestamp = DateTime.Now.ToString("fff"); // milisegundos

            return $"{numero}{timestamp}{extension}";
        }

        public async Task InsertarAnexoAsync(AnexoDTO a) {
            using var conn = new SqlConnection(_connDB);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("[Autorizacion].[usp_insert_doc_solicitud_anexo_aut]", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@idSolicitud", a.IdSolicitud);
            cmd.Parameters.AddWithValue("@id_anexos_doc", a.IdAnexosDoc);
            cmd.Parameters.AddWithValue("@id_documento", 0);
            cmd.Parameters.AddWithValue("@numero_doc", a.NumeroDoc);
            cmd.Parameters.AddWithValue("@fecha_doc", a.FechaDoc);
            cmd.Parameters.AddWithValue("@siglas_doc", a.SiglasDoc);
            cmd.Parameters.AddWithValue("@operador_registro", a.OperadorRegistro);
            cmd.Parameters.AddWithValue("@estacion_registro", "");
            cmd.Parameters.AddWithValue("@RutaDoc", a.RutaDoc);
            cmd.Parameters.AddWithValue("@NombreDoc", a.NombreDoc);
            cmd.Parameters.AddWithValue("@flag_valida", a.FlagValida);

            await cmd.ExecuteNonQueryAsync();
        }


        public async Task<byte[]?> GenerarDocExcel() {
            var data = await ObtenerSolicitudesConGiroAsync();

            if(data == null || !data.Any())
                return null;



            using(var package = new ExcelPackage()) {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                var worksheet = package.Workbook.Worksheets.Add("Reporte");

                // Encabezados
                worksheet.Cells[1, 1].Value = "ID Solicitud";
                worksheet.Cells[1, 2].Value = "N° Solicitud";
                worksheet.Cells[1, 3].Value = "N° Autorización";
                worksheet.Cells[1, 4].Value = "Fec. Autorización";
                worksheet.Cells[1, 5].Value = "N° Expediente";
                worksheet.Cells[1, 6].Value = "Fec. Expediente";
                worksheet.Cells[1, 7].Value = "N° Resolución";
                worksheet.Cells[1, 8].Value = "Fec. Resolución";
                worksheet.Cells[1, 9].Value = "Estado Trámite";
                worksheet.Cells[1, 10].Value = "Vigencia Hasta";
                worksheet.Cells[1, 11].Value = "Fec. Registro";
                worksheet.Cells[1, 12].Value = "Giro";
                worksheet.Cells[1, 13].Value = "Nombre Giro";
                worksheet.Cells[1, 14].Value = "Siglas Resolución";
                worksheet.Cells[1, 15].Value = "Solicitante";
                worksheet.Cells[1, 16].Value = "Punto Local";
                worksheet.Cells[1, 17].Value = "Horario";
                worksheet.Cells[1, 18].Value = "Zona";

                // etc.


                int row = 2;
                foreach(var x in data) {
                    worksheet.Cells[row, 1].Value = x.idSolicitud;
                    worksheet.Cells[row, 2].Value = x.nroSolicitud;
                    worksheet.Cells[row, 3].Value = x.nroAutorizacion;
                    worksheet.Cells[row, 4].Value = x.fechaAutorizacion?.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 5].Value = x.nroExpediente;
                    worksheet.Cells[row, 6].Value = x.fecha_expediente?.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 7].Value = x.nroResolucion;
                    worksheet.Cells[row, 8].Value = x.fechaResolucion?.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 9].Value = x.estadoTramite;
                    worksheet.Cells[row, 10].Value = x.vigencia_hasta?.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 11].Value = x.fechaRegistro.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 12].Value = x.giros;
                    worksheet.Cells[row, 13].Value = x.NombreComputado;
                    worksheet.Cells[row, 14].Value = x.siglas_resolucion;
                    worksheet.Cells[row, 15].Value = x.razon_social;
                    worksheet.Cells[row, 16].Value = x.punto_local;
                    worksheet.Cells[row, 17].Value = x.aHorario;
                    worksheet.Cells[row, 18].Value = x.nombSZ ?? "N/R";
                    row++;
                }

                return package.GetAsByteArray();
            }
        }

        //funcion consulta tabla
        public async Task<List<ExcelDTO>> ObtenerSolicitudesConGiroAsync() {
            var resultados = new List<ExcelDTO>();

            using(var connection = new SqlConnection(_connDB)) {
                await connection.OpenAsync();

                using(var command = new SqlCommand("Autorizacion.sp_ConsultarSolicitudesConGiro", connection)) {
                    command.CommandType = CommandType.StoredProcedure;

                    using(var reader = await command.ExecuteReaderAsync()) {
                        while(await reader.ReadAsync()) {
                            resultados.Add(new ExcelDTO {
                                idSolicitud = reader.GetInt32(reader.GetOrdinal("idSolicitud")),
                                nroAutorizacion = reader["nroAutorizacion"]?.ToString(),
                                fechaAutorizacion = reader["fechaAutorizacion"] as DateTime?,
                                nroExpediente = reader["nroExpediente"]?.ToString(),
                                fecha_expediente = reader["fecha_expediente"] as DateTime?,
                                nroResolucion = reader["nroResolucion"]?.ToString(),
                                fechaResolucion = reader["fechaResolucion"] as DateTime?,
                                estadoTramite = reader["estadoTramite"] != DBNull.Value ? Convert.ToInt32(reader["estadoTramite"]) : 0,
                                vigencia_hasta = reader["vigencia_hasta"] as DateTime?,
                                fechaRegistro = Convert.ToDateTime(reader["fechaRegistro"]),
                                giros = reader["IdGiroSolicitud"]?.ToString(),
                                NombreComputado = reader["NombreGiro"]?.ToString(),
                                siglas_resolucion = reader["siglas_resolucion"]?.ToString(),
                                razon_social = reader["Solicitante"]?.ToString(),
                                punto_local = reader["punto_local"]?.ToString(),
                                aHorario = reader["aHorario"]?.ToString(),
                                nombSZ = reader["NombreSubzona"]?.ToString()
                            });
                        }

                    }
                }
            }

            return resultados;
        }


    }
}
