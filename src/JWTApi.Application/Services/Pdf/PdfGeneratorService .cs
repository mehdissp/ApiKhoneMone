//////////////////// سرویس PDF را تغییر دهید تا همان نوع RealEstateDetails را بپذیرد
//////////////////using JWTApi.Domain.Dtos.RealEstate;
//////////////////using QRCoder;
//////////////////using QuestPDF.Fluent;
//////////////////using QuestPDF.Helpers;
//////////////////using QuestPDF.Infrastructure;

//////////////////public interface IPdfGeneratorService
//////////////////{
//////////////////    Task<byte[]> GeneratePropertyPdf(RealEstateDetails property, CancellationToken cancellationToken);
//////////////////}

//////////////////public class PdfGeneratorService : IPdfGeneratorService
//////////////////{
//////////////////    public Task<byte[]> GeneratePropertyPdf(RealEstateDetails property, CancellationToken cancellationToken)
//////////////////    {
//////////////////        return Task.FromResult(GeneratePdf(property));
//////////////////    }

//////////////////    private byte[] GeneratePdf(RealEstateDetails property)
//////////////////    {
//////////////////        QuestPDF.Settings.License = LicenseType.Community;

//////////////////        var document = Document.Create(container =>
//////////////////        {
//////////////////            container.Page(page =>
//////////////////            {
//////////////////                page.Size(PageSizes.A4);
//////////////////                page.Margin(2, Unit.Centimetre);
//////////////////                page.PageColor(Colors.White);
//////////////////                page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black));

//////////////////                #region Header
//////////////////                page.Header()
//////////////////                    .Row(row =>
//////////////////                    {
//////////////////                        row.RelativeItem()
//////////////////                            .Column(col =>
//////////////////                            {
//////////////////                                col.Item().Text("مشاور املاک")
//////////////////                                    .FontSize(16)
//////////////////                                    .Bold()
//////////////////                                    .FontColor(Colors.Blue.Darken2)
//////////////////                                    .AlignRight();

//////////////////                                col.Item().Text("گزارش جزئیات ملک")
//////////////////                                    .FontSize(12)
//////////////////                                    .FontColor(Colors.Grey.Darken1)
//////////////////                                    .AlignRight();
//////////////////                            });

//////////////////                        row.ConstantItem(120)
//////////////////                            .Image(GenerateQrCode(property.Id))
//////////////////                            .FitArea();
//////////////////                    });
//////////////////                #endregion

//////////////////                #region Content
//////////////////                page.Content()
//////////////////                    .PaddingVertical(1, Unit.Centimetre)
//////////////////                    .Column(col =>
//////////////////                    {
//////////////////                        // اطلاعات اصلی
//////////////////                        col.Item().PaddingBottom(10).BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
//////////////////                            .Row(row =>
//////////////////                            {
//////////////////                                row.RelativeItem().Column(col2 =>
//////////////////                                {
//////////////////                                    col2.Item().Text($"عنوان: {property.Title}").Bold();
//////////////////                                    col2.Item().Text($"نوع: {GetCategoryType(property.CategoryType)}");
//////////////////                                    col2.Item().Text($"منطقه: {property.RegionName}");
//////////////////                                });
//////////////////                                row.RelativeItem().Column(col2 =>
//////////////////                                {
//////////////////                                    col2.Item().Text($"قیمت: {property.Price:N0} تومان").Bold().FontColor(Colors.Green.Darken2);
//////////////////                                    col2.Item().Text($"قیمت هر متر: {property.PriceMeter:N0} تومان");
//////////////////                                    col2.Item().Text($"متراژ: {property.SquareMeter} متر مربع");
//////////////////                                });
//////////////////                            });

//////////////////                        // عکس اصلی
//////////////////                        if (property.Images != null && property.Images.Any())
//////////////////                        {
//////////////////                            col.Item().PaddingVertical(10)
//////////////////                                .Height(200, Unit.Point)
//////////////////                                .Image(GetImageFromUrl(property.Images.First()))
//////////////////                                .FitArea();
//////////////////                        }

//////////////////                        // مشخصات فنی
//////////////////                        col.Item().PaddingVertical(10)
//////////////////                            .Row(row =>
//////////////////                            {
//////////////////                                row.RelativeItem().Column(col2 =>
//////////////////                                {
//////////////////                                    col2.Item().Text("مشخصات فنی").Bold().Underline();
//////////////////                                    col2.Item().Text($"تعداد طبقات: {property.CountFloor}");
//////////////////                                    col2.Item().Text($"طبقه: {property.Floor}");
//////////////////                                    col2.Item().Text($"سال ساخت: {property.ConstructionYear} (شمسی)");
//////////////////                                });
//////////////////                                row.RelativeItem().Column(col2 =>
//////////////////                                {
//////////////////                                    col2.Item().Text("امکانات").Bold().Underline();
//////////////////                                    col2.Item().Text($"آسانسور: {(property.IsHasElevator ? "دارد" : "ندارد")}");
//////////////////                                    col2.Item().Text($"پارکینگ: {(property.IsHasParking ? "دارد" : "ندارد")}");
//////////////////                                    col2.Item().Text($"استخر: {(property.IsHasPool ? "دارد" : "ندارد")}");
//////////////////                                    col2.Item().Text($"انباری: {(property.IsHasStoreRoom ? "دارد" : "ندارد")}");
//////////////////                                });
//////////////////                            });

//////////////////                        // آدرس
//////////////////                        col.Item().PaddingVertical(10)
//////////////////                            .Column(col2 =>
//////////////////                            {
//////////////////                                col2.Item().Text("آدرس").Bold().Underline();
//////////////////                                col2.Item().Text(property.Address);
//////////////////                                col2.Item().Text($"طول جغرافیایی: {property.lng}");
//////////////////                                col2.Item().Text($"عرض جغرافیایی: {property.lat}");
//////////////////                            });

//////////////////                        // هشدارها
//////////////////                        if (property.Warnings != null && property.Warnings.Any())
//////////////////                        {
//////////////////                            col.Item().PaddingVertical(10)
//////////////////                                .Column(col2 =>
//////////////////                                {
//////////////////                                    col2.Item().Text("هشدارهای مهم").Bold().Underline().FontColor(Colors.Red.Darken2);
//////////////////                                    foreach (var warning in property.Warnings)
//////////////////                                    {
//////////////////                                        col2.Item().Text($"• {warning}").FontColor(Colors.Red.Medium);
//////////////////                                    }
//////////////////                                });
//////////////////                        }

//////////////////                        // اطلاعات مشاور
//////////////////                        if (property.Agents != null)
//////////////////                        {
//////////////////                            col.Item().PaddingVertical(10).BorderTop(1).BorderColor(Colors.Grey.Lighten2)
//////////////////                                .Row(row =>
//////////////////                                {
//////////////////                                    row.RelativeItem().Column(col2 =>
//////////////////                                    {
//////////////////                                        col2.Item().Text("اطلاعات مشاور").Bold();
//////////////////                                        col2.Item().Text($"نام: {property.Agents.Name ?? "نامشخص"}");
//////////////////                                        col2.Item().Text($"تلفن: {property.Agents.Phone ?? "نامشخص"}");
//////////////////                                    });
//////////////////                                    row.RelativeItem().Column(col2 =>
//////////////////                                    {
//////////////////                                        col2.Item().Text("آدرس دفتر").Bold();
//////////////////                                        col2.Item().Text(property.Agents.Address ?? "نامشخص");
//////////////////                                    });
//////////////////                                });
//////////////////                        }
//////////////////                    });
//////////////////                #endregion

//////////////////                #region Footer
//////////////////                page.Footer()
//////////////////                    .AlignRight()
//////////////////                    .Text(text =>
//////////////////                    {
//////////////////                        text.Span("تاریخ چاپ: ");
//////////////////                        text.Span($"{DateTime.Now:yyyy/MM/dd HH:mm}");
//////////////////                        text.Span(" | ");
//////////////////                        text.Span($"کد ملک: {property.Id}");
//////////////////                    });
//////////////////                #endregion
//////////////////            });
//////////////////        });

//////////////////        return document.GeneratePdf();
//////////////////    }

//////////////////    private string GetCategoryType(int categoryType)
//////////////////    {
//////////////////        return categoryType switch
//////////////////        {
//////////////////            1 => "فروش",
//////////////////            2 => "رهن",
//////////////////            3 => "اجاره",
//////////////////            _ => "نامشخص"
//////////////////        };
//////////////////    }

//////////////////    private byte[] GenerateQrCode(int id)
//////////////////    {
//////////////////        try
//////////////////        {
//////////////////            using var qrGenerator = new QRCodeGenerator();
//////////////////            var qrCodeData = qrGenerator.CreateQrCode($"PropertyId:{id}", QRCodeGenerator.ECCLevel.Q);
//////////////////            var qrCode = new PngByteQRCode(qrCodeData);
//////////////////            return qrCode.GetGraphic(20);
//////////////////        }
//////////////////        catch
//////////////////        {
//////////////////            return Array.Empty<byte>();
//////////////////        }
//////////////////    }

//////////////////    private byte[] GetImageFromUrl(string imageUrl)
//////////////////    {
//////////////////        try
//////////////////        {
//////////////////            using var client = new HttpClient();
//////////////////            client.Timeout = TimeSpan.FromSeconds(10);
//////////////////            return client.GetByteArrayAsync(imageUrl).GetAwaiter().GetResult();
//////////////////        }
//////////////////        catch
//////////////////        {
//////////////////            return Array.Empty<byte>();
//////////////////        }
//////////////////    }
//////////////////}

////////////////using JWTApi.Domain.Dtos.RealEstate;
////////////////using QRCoder;
////////////////using QuestPDF;
////////////////using QuestPDF.Fluent;
////////////////using QuestPDF.Helpers;
////////////////using QuestPDF.Infrastructure;

////////////////namespace JWTApi.Services.Pdf;

////////////////public interface IPdfGeneratorService
////////////////{
////////////////    Task<byte[]> GeneratePropertyPdf(RealEstateDetails property, CancellationToken cancellationToken);
////////////////}

////////////////public class PdfGeneratorService : IPdfGeneratorService
////////////////{
////////////////    public PdfGeneratorService()
////////////////    {
////////////////        Settings.License = LicenseType.Community;
////////////////    }

////////////////    public Task<byte[]> GeneratePropertyPdf(RealEstateDetails property, CancellationToken cancellationToken)
////////////////    {
////////////////        return Task.FromResult(GeneratePdf(property));
////////////////    }

////////////////    private byte[] GeneratePdf(RealEstateDetails property)
////////////////    {
////////////////        return Document.Create(container =>
////////////////        {
////////////////            container.Page(page =>
////////////////            {
////////////////                page.Size(PageSizes.A4);
////////////////                page.Margin(2, Unit.Centimetre);
////////////////                page.PageColor(Colors.White);
////////////////                page.DefaultTextStyle(x => x.FontSize(11).FontColor(Colors.Black));

////////////////                page.Header().Element(x => BuildHeader(x, property));
////////////////                page.Content().Element(x => BuildContent(x, property));
////////////////                page.Footer().Element(x => BuildFooter(x, property));
////////////////            });
////////////////        }).GeneratePdf();
////////////////    }

////////////////    private void BuildHeader(IContainer container, RealEstateDetails property)
////////////////    {
////////////////        container.Row(row =>
////////////////        {
////////////////            row.RelativeItem(2).Column(col =>
////////////////            {
////////////////                col.Item().Text("مشاور املاک")
////////////////                    .FontSize(20)
////////////////                    .Bold()
////////////////                    .FontColor(Colors.Blue.Darken2)
////////////////                    .AlignRight();

////////////////                col.Item().Text("گزارش کامل مشخصات ملک")
////////////////                    .FontSize(13)
////////////////                    .FontColor(Colors.Grey.Darken1)
////////////////                    .AlignRight();
////////////////            });

////////////////            // QR Code در ستون سمت چپ
////////////////            row.RelativeItem(1).Column(col =>
////////////////            {
////////////////                col.Item().AlignCenter().Image(GenerateQrCode(property.Id)).FitArea();
////////////////            });
////////////////        });
////////////////    }

////////////////    private void BuildContent(IContainer container, RealEstateDetails property)
////////////////    {
////////////////        container.PaddingVertical(0.8f, Unit.Centimetre).Column(col =>
////////////////        {
////////////////            // ============================================================
////////////////            // عنوان و قیمت (وسط‌چین)
////////////////            // ============================================================
////////////////            col.Item().PaddingBottom(12).Column(c =>
////////////////            {
////////////////                c.Item().AlignCenter().Text(property.Title)
////////////////                    .FontSize(18)
////////////////                    .Bold()
////////////////                    .FontColor(Colors.Blue.Darken2);

////////////////                c.Item().AlignCenter().Text($"{property.Price:N0} تومان")
////////////////                    .FontSize(22)
////////////////                    .Bold()
////////////////                    .FontColor(Colors.Green.Darken2);

////////////////                c.Item().AlignCenter().Text($"متراژ: {property.SquareMeter} متر مربع | قیمت هر متر: {property.PriceMeter:N0} تومان")
////////////////                    .FontSize(12)
////////////////                    .FontColor(Colors.Grey.Darken1);
////////////////            });

////////////////            // ============================================================
////////////////            // خط جداکننده
////////////////            // ============================================================
////////////////            col.Item().PaddingVertical(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

////////////////            // ============================================================
////////////////            // اطلاعات اصلی در سه ستون
////////////////            // ============================================================
////////////////            col.Item().PaddingVertical(6).Row(row =>
////////////////            {
////////////////                row.RelativeItem().Column(c =>
////////////////                {
////////////////                    c.Item().AlignRight().Text($"نوع: {GetCategoryType(property.CategoryType)}");
////////////////                    c.Item().AlignRight().Text($"منطقه: {property.RegionName}");
////////////////                });

////////////////                row.RelativeItem().Column(c =>
////////////////                {
////////////////                    c.Item().AlignRight().Text($"طبقه: {property.Floor}");
////////////////                    c.Item().AlignRight().Text($"تعداد طبقات: {property.CountFloor}");
////////////////                });

////////////////                row.RelativeItem().Column(c =>
////////////////                {
////////////////                    c.Item().AlignRight().Text($"سال ساخت: {property.ConstructionYear} (شمسی)");
////////////////                });
////////////////            });

////////////////            // ============================================================
////////////////            // عکس (وسط‌چین)
////////////////            // ============================================================
////////////////            if (property.Images?.Any() == true)
////////////////            {
////////////////                col.Item().PaddingVertical(8)
////////////////                    .AlignCenter()
////////////////                    .Height(250)
////////////////                    .Image(LoadImage(property.Images.First()))
////////////////                    .FitArea();
////////////////            }

////////////////            // ============================================================
////////////////            // امکانات (به صورت جدولی)
////////////////            // ============================================================
////////////////            col.Item().PaddingVertical(8).Column(c =>
////////////////            {
////////////////                c.Item().AlignRight().Text("امکانات ملک")
////////////////                    .FontSize(14)
////////////////                    .Bold()
////////////////                    .Underline();

////////////////                c.Item().PaddingTop(6).Row(row =>
////////////////                {
////////////////                    row.RelativeItem().Column(c2 =>
////////////////                    {
////////////////                        c2.Item().AlignRight().Text($"{(property.IsHasElevator ? "✅" : "❌")} آسانسور");
////////////////                        c2.Item().AlignRight().Text($"{(property.IsHasParking ? "✅" : "❌")} پارکینگ");
////////////////                    });

////////////////                    row.RelativeItem().Column(c2 =>
////////////////                    {
////////////////                        c2.Item().AlignRight().Text($"{(property.IsHasPool ? "✅" : "❌")} استخر");
////////////////                        c2.Item().AlignRight().Text($"{(property.IsHasStoreRoom ? "✅" : "❌")} انباری");
////////////////                    });
////////////////                });
////////////////            });

////////////////            // ============================================================
////////////////            // آدرس
////////////////            // ============================================================
////////////////            col.Item().PaddingVertical(8).Column(c =>
////////////////            {
////////////////                c.Item().AlignRight().Text("آدرس ملک")
////////////////                    .FontSize(14)
////////////////                    .Bold()
////////////////                    .Underline();

////////////////                c.Item().AlignRight().Text(property.Address)
////////////////                    .FontSize(12);

////////////////                c.Item().AlignRight().Text($"مختصات: {property.lat} , {property.lng}")
////////////////                    .FontSize(10)
////////////////                    .FontColor(Colors.Grey.Darken1);
////////////////            });

////////////////            // ============================================================
////////////////            // هشدارها
////////////////            // ============================================================
////////////////            if (property.Warnings?.Any() == true)
////////////////            {
////////////////                col.Item().PaddingVertical(8).Column(c =>
////////////////                {
////////////////                    c.Item().AlignRight().Text("⚠️ نکات مهم")
////////////////                        .FontSize(14)
////////////////                        .Bold()
////////////////                        .Underline()
////////////////                        .FontColor(Colors.Red.Darken2);

////////////////                    foreach (var warning in property.Warnings)
////////////////                    {
////////////////                        c.Item().AlignRight().Text($"• {warning}")
////////////////                            .FontColor(Colors.Red.Medium);
////////////////                    }
////////////////                });
////////////////            }

////////////////            // ============================================================
////////////////            // مشاور
////////////////            // ============================================================
////////////////            if (property.Agents != null)
////////////////            {
////////////////                col.Item().PaddingVertical(8).BorderTop(1).BorderColor(Colors.Grey.Lighten2).Column(c =>
////////////////                {
////////////////                    c.Item().AlignRight().Text("اطلاعات مشاور")
////////////////                        .FontSize(14)
////////////////                        .Bold()
////////////////                        .Underline();

////////////////                    c.Item().PaddingTop(4).Row(row =>
////////////////                    {
////////////////                        row.RelativeItem().Column(c2 =>
////////////////                        {
////////////////                            c2.Item().AlignRight().Text($"نام: {property.Agents.Name ?? "نامشخص"}");
////////////////                            c2.Item().AlignRight().Text($"تلفن: {property.Agents.Phone ?? "نامشخص"}");
////////////////                        });

////////////////                        row.RelativeItem().Column(c2 =>
////////////////                        {
////////////////                            c2.Item().AlignRight().Text("آدرس دفتر:").Bold();
////////////////                            c2.Item().AlignRight().Text(property.Agents.Address ?? "نامشخص");
////////////////                        });
////////////////                    });
////////////////                });
////////////////            }
////////////////        });
////////////////    }

////////////////    private void BuildFooter(IContainer container, RealEstateDetails property)
////////////////    {
////////////////        container.AlignCenter().Text(t =>
////////////////        {
////////////////            t.Span("تاریخ چاپ: ");
////////////////            t.Span($"{DateTime.Now:yyyy/MM/dd HH:mm}");

////////////////            t.Span("    |    ");

////////////////            t.Span("کد ملک: ");
////////////////            t.Span($"{property.Id}");

////////////////            t.Span("    |    ");

////////////////            t.Span("صفحه ");
////////////////            t.Span("1");
////////////////            t.Span(" از ");
////////////////            t.Span("1");
////////////////        });
////////////////    }

////////////////    private string GetCategoryType(int type) => type switch
////////////////    {
////////////////        1 => "فروش",
////////////////        2 => "رهن",
////////////////        3 => "اجاره",
////////////////        _ => "نامشخص"
////////////////    };

////////////////    private byte[] GenerateQrCode(int id)
////////////////    {
////////////////        try
////////////////        {
////////////////            using var gen = new QRCodeGenerator();
////////////////            var data = gen.CreateQrCode($"PropertyId:{id}", QRCodeGenerator.ECCLevel.Q);
////////////////            return new PngByteQRCode(data).GetGraphic(20);
////////////////        }
////////////////        catch
////////////////        {
////////////////            return Array.Empty<byte>();
////////////////        }
////////////////    }

////////////////    private byte[] LoadImage(string url)
////////////////    {
////////////////        try
////////////////        {
////////////////            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
////////////////            return client.GetByteArrayAsync(url).GetAwaiter().GetResult();
////////////////        }
////////////////        catch
////////////////        {
////////////////            return Array.Empty<byte>();
////////////////        }
////////////////    }
////////////////}

//////////////using JWTApi.Domain.Dtos.RealEstate;
//////////////using QRCoder;
//////////////using QuestPDF;
//////////////using QuestPDF.Fluent;
//////////////using QuestPDF.Helpers;
//////////////using QuestPDF.Infrastructure;

//////////////namespace JWTApi.Services.Pdf;

//////////////public interface IPdfGeneratorService
//////////////{
//////////////    Task<byte[]> GeneratePropertyPdf(RealEstateDetails property, CancellationToken cancellationToken);
//////////////}

//////////////public class PdfGeneratorService : IPdfGeneratorService
//////////////{
//////////////    public PdfGeneratorService()
//////////////    {
//////////////        Settings.License = LicenseType.Community;
//////////////        // غیرفعال کردن بررسی گلیف‌ها (اگر ایموجی نداشته باشیم)
//////////////        Settings.CheckIfAllTextGlyphsAreAvailable = false;
//////////////    }

//////////////    public Task<byte[]> GeneratePropertyPdf(RealEstateDetails property, CancellationToken cancellationToken)
//////////////    {
//////////////        return Task.FromResult(GeneratePdf(property));
//////////////    }

//////////////    private byte[] GeneratePdf(RealEstateDetails property)
//////////////    {
//////////////        return Document.Create(container =>
//////////////        {
//////////////            container.Page(page =>
//////////////            {
//////////////                page.Size(PageSizes.A4);
//////////////                page.Margin(1.5f, Unit.Centimetre);
//////////////                page.PageColor(Colors.White);
//////////////                page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black));

//////////////                page.Header().Element(x => BuildHeader(x, property));
//////////////                page.Content().Element(x => BuildContent(x, property));
//////////////                page.Footer().Element(x => BuildFooter(x, property));
//////////////            });
//////////////        }).GeneratePdf();
//////////////    }

//////////////    private void BuildHeader(IContainer container, RealEstateDetails property)
//////////////    {
//////////////        container.Row(row =>
//////////////        {
//////////////            row.RelativeItem(3).Column(col =>
//////////////            {
//////////////                col.Item().Text("مشاور املاک")
//////////////                    .FontSize(18)
//////////////                    .Bold()
//////////////                    .FontColor(Colors.Blue.Darken2)
//////////////                    .AlignRight();

//////////////                col.Item().Text("گزارش کامل مشخصات ملک")
//////////////                    .FontSize(12)
//////////////                    .FontColor(Colors.Grey.Darken1)
//////////////                    .AlignRight();
//////////////            });

//////////////            row.RelativeItem(1).Column(col =>
//////////////            {
//////////////                col.Item().AlignCenter().Image(GenerateQrCode(property.Id)).FitArea();
//////////////            });
//////////////        });
//////////////    }

//////////////    private void BuildContent(IContainer container, RealEstateDetails property)
//////////////    {
//////////////        container.PaddingVertical(0.5f, Unit.Centimetre).Column(col =>
//////////////        {
//////////////            // ============================================================
//////////////            // کد ملک و عنوان (وسط‌چین)
//////////////            // ============================================================
//////////////            col.Item().PaddingBottom(8).Column(c =>
//////////////            {
//////////////                // کد ملک
//////////////                c.Item().AlignCenter().Text($"کد: {property.Id:D4}")
//////////////                    .FontSize(11)
//////////////                    .FontColor(Colors.Grey.Darken2)
//////////////                    .Bold();

//////////////                // عنوان اصلی
//////////////                c.Item().AlignCenter().Text(property.Title)
//////////////                    .FontSize(20)
//////////////                    .Bold()
//////////////                    .FontColor(Colors.Blue.Darken2);

//////////////                // قیمت
//////////////                c.Item().AlignCenter().Text($"{property.Price:N0} تومان")
//////////////                    .FontSize(24)
//////////////                    .Bold()
//////////////                    .FontColor(Colors.Green.Darken2);

//////////////                // متراژ و قیمت هر متر
//////////////                c.Item().AlignCenter().Text($"متراژ: {property.SquareMeter} متر مربع  |  قیمت هر متر: {property.PriceMeter:N0} تومان")
//////////////                    .FontSize(11)
//////////////                    .FontColor(Colors.Grey.Darken1);
//////////////            });

//////////////            // ============================================================
//////////////            // خط جداکننده
//////////////            // ============================================================
//////////////            col.Item().PaddingVertical(4).LineHorizontal(1).LineColor(Colors.Blue.Lighten3);

//////////////            // ============================================================
//////////////            // اطلاعات اصلی در 4 ستون با باکس
//////////////            // ============================================================
//////////////            col.Item().PaddingVertical(6).Column(c =>
//////////////            {
//////////////                c.Item().Background(Colors.Grey.Lighten4).Padding(8).Row(row =>
//////////////                {
//////////////                    row.RelativeItem().Column(c2 =>
//////////////                    {
//////////////                        c2.Item().Text("نوع ملک").Bold().FontSize(9).FontColor(Colors.Grey.Darken2).AlignRight();
//////////////                        c2.Item().Text(GetCategoryType(property.CategoryType)).FontSize(11).AlignRight();
//////////////                    });

//////////////                    row.RelativeItem().Column(c2 =>
//////////////                    {
//////////////                        c2.Item().Text("منطقه").Bold().FontSize(9).FontColor(Colors.Grey.Darken2).AlignRight();
//////////////                        c2.Item().Text(property.RegionName).FontSize(11).AlignRight();
//////////////                    });

//////////////                    row.RelativeItem().Column(c2 =>
//////////////                    {
//////////////                        c2.Item().Text("سال ساخت").Bold().FontSize(9).FontColor(Colors.Grey.Darken2).AlignRight();
//////////////                        c2.Item().Text($"{property.ConstructionYear} (شمسی)").FontSize(11).AlignRight();
//////////////                    });

//////////////                    row.RelativeItem().Column(c2 =>
//////////////                    {
//////////////                        c2.Item().Text("طبقه").Bold().FontSize(9).FontColor(Colors.Grey.Darken2).AlignRight();
//////////////                        c2.Item().Text($"{property.Floor} از {property.CountFloor}").FontSize(11).AlignRight();
//////////////                    });
//////////////                });
//////////////            });

//////////////            // ============================================================
//////////////            // عکس
//////////////            // ============================================================
//////////////            if (property.Images?.Any() == true)
//////////////            {
//////////////                //col.Item().PaddingVertical(6)
//////////////                //    .AlignCenter()
//////////////                //    .Height(220)
//////////////                //    .Image(LoadImage(property.Images.First()))
//////////////                //    .FitArea();
//////////////            }

//////////////            // ============================================================
//////////////            // امکانات
//////////////            // ============================================================
//////////////            col.Item().PaddingVertical(6).Column(c =>
//////////////            {
//////////////                c.Item().Text("امکانات ملک")
//////////////                    .FontSize(13)
//////////////                    .Bold()
//////////////                    .FontColor(Colors.Blue.Darken2)
//////////////                    .AlignRight();

//////////////                c.Item().PaddingTop(4).Row(row =>
//////////////                {
//////////////                    row.RelativeItem().Column(c2 =>
//////////////                    {
//////////////                        AddAmenity(c2, "آسانسور", property.IsHasElevator);
//////////////                        AddAmenity(c2, "پارکینگ", property.IsHasParking);
//////////////                    });

//////////////                    row.RelativeItem().Column(c2 =>
//////////////                    {
//////////////                        AddAmenity(c2, "استخر", property.IsHasPool);
//////////////                        AddAmenity(c2, "انباری", property.IsHasStoreRoom);
//////////////                    });
//////////////                });
//////////////            });

//////////////            // ============================================================
//////////////            // آدرس
//////////////            // ============================================================
//////////////            col.Item().PaddingVertical(6).Column(c =>
//////////////            {
//////////////                c.Item().Text("آدرس ملک")
//////////////                    .FontSize(13)
//////////////                    .Bold()
//////////////                    .FontColor(Colors.Blue.Darken2)
//////////////                    .AlignRight();

//////////////                c.Item().PaddingTop(2).Background(Colors.Grey.Lighten4).Padding(8).Text(property.Address)
//////////////                    .FontSize(11)
//////////////                    .AlignRight();

//////////////                c.Item().PaddingTop(2).Text($"مختصات: {property.lat} , {property.lng}")
//////////////                    .FontSize(9)
//////////////                    .FontColor(Colors.Grey.Darken1)
//////////////                    .AlignRight();
//////////////            });

//////////////            // ============================================================
//////////////            // هشدارها
//////////////            // ============================================================
//////////////            if (property.Warnings?.Any() == true)
//////////////            {
//////////////                col.Item().PaddingVertical(6).Column(c =>
//////////////                {
//////////////                    c.Item().Text("نکات مهم")
//////////////                        .FontSize(13)
//////////////                        .Bold()
//////////////                        .FontColor(Colors.Red.Darken2)
//////////////                        .AlignRight();

//////////////                    c.Item().PaddingTop(2).Background(Colors.Red.Lighten5).Padding(8).Column(c2 =>
//////////////                    {
//////////////                        foreach (var warning in property.Warnings)
//////////////                        {
//////////////                            c2.Item().Text($"- {warning}")
//////////////                                .FontColor(Colors.Red.Darken2)
//////////////                                .AlignRight();
//////////////                        }
//////////////                    });
//////////////                });
//////////////            }

//////////////            // ============================================================
//////////////            // مشاور
//////////////            // ============================================================
//////////////            if (property.Agents != null)
//////////////            {
//////////////                col.Item().PaddingVertical(6).Column(c =>
//////////////                {
//////////////                    c.Item().Text("اطلاعات مشاور")
//////////////                        .FontSize(13)
//////////////                        .Bold()
//////////////                        .FontColor(Colors.Blue.Darken2)
//////////////                        .AlignRight();

//////////////                    c.Item().PaddingTop(2).Background(Colors.Grey.Lighten4).Padding(8).Row(row =>
//////////////                    {
//////////////                        row.RelativeItem().Column(c2 =>
//////////////                        {
//////////////                            c2.Item().Text($"نام: {property.Agents.Name ?? "نامشخص"}").AlignRight();
//////////////                            c2.Item().Text($"تلفن: {property.Agents.Phone ?? "نامشخص"}").AlignRight();
//////////////                        });

//////////////                        row.RelativeItem().Column(c2 =>
//////////////                        {
//////////////                            c2.Item().Text("آدرس دفتر:").Bold().AlignRight();
//////////////                            c2.Item().Text(property.Agents.Address ?? "نامشخص").AlignRight();
//////////////                        });
//////////////                    });
//////////////                });
//////////////            }
//////////////        });
//////////////    }

//////////////    private void BuildFooter(IContainer container, RealEstateDetails property)
//////////////    {
//////////////        container.BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(6).AlignCenter().Text(t =>
//////////////        {
//////////////            t.Span("تاریخ چاپ: ");
//////////////            t.Span($"{DateTime.Now:yyyy/MM/dd HH:mm}");

//////////////            t.Span("    |    ");

//////////////            t.Span("کد ملک: ");
//////////////            t.Span($"{property.Id:D4}");

//////////////            t.Span("    |    ");

//////////////            t.Span("صفحه ");
//////////////            t.Span("1");
//////////////            t.Span(" از ");
//////////////            t.Span("1");
//////////////        });
//////////////    }

//////////////    private void AddAmenity(ColumnDescriptor col, string name, bool has)
//////////////    {
//////////////        var status = has ? "✓ دارد" : "✗ ندارد";
//////////////        var color = has ? Colors.Green.Darken2 : Colors.Red.Darken2;

//////////////        col.Item().Padding(4)
//////////////            .Background(has ? Colors.Green.Lighten5 : Colors.Red.Lighten5)
//////////////            .Padding(4)
//////////////            .Text($"{name}: {status}")
//////////////            .FontColor(color)
//////////////            .AlignRight();
//////////////    }

//////////////    private string GetCategoryType(int type) => type switch
//////////////    {
//////////////        1 => "فروش",
//////////////        2 => "رهن",
//////////////        3 => "اجاره",
//////////////        _ => "نامشخص"
//////////////    };

//////////////    private byte[] GenerateQrCode(int id)
//////////////    {
//////////////        try
//////////////        {
//////////////            using var gen = new QRCodeGenerator();
//////////////            var data = gen.CreateQrCode($"PropertyId:{id}", QRCodeGenerator.ECCLevel.Q);
//////////////            return new PngByteQRCode(data).GetGraphic(20);
//////////////        }
//////////////        catch
//////////////        {
//////////////            return Array.Empty<byte>();
//////////////        }
//////////////    }

//////////////    private byte[] LoadImage(string url)
//////////////    {
//////////////        try
//////////////        {
//////////////            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
//////////////            return client.GetByteArrayAsync(url).GetAwaiter().GetResult();
//////////////        }
//////////////        catch
//////////////        {
//////////////            return Array.Empty<byte>();
//////////////        }
//////////////    }
//////////////}

////////////using JWTApi.Domain.Dtos.RealEstate;
////////////using QRCoder;
////////////using QuestPDF;
////////////using QuestPDF.Fluent;
////////////using QuestPDF.Helpers;
////////////using QuestPDF.Infrastructure;

////////////namespace JWTApi.Services.Pdf;

////////////public interface IPdfGeneratorService
////////////{
////////////    Task<byte[]> GeneratePropertyPdf(RealEstateDetails property, CancellationToken cancellationToken);
////////////}

////////////public class PdfGeneratorService : IPdfGeneratorService
////////////{
////////////    private const string WatermarkText = "مشاور املاک";

////////////    public PdfGeneratorService()
////////////    {
////////////        Settings.License = LicenseType.Community;
////////////        Settings.CheckIfAllTextGlyphsAreAvailable = false;
////////////    }

////////////    public Task<byte[]> GeneratePropertyPdf(RealEstateDetails property, CancellationToken cancellationToken)
////////////    {
////////////        return Task.FromResult(GeneratePdf(property));
////////////    }

////////////    private byte[] GeneratePdf(RealEstateDetails property)
////////////    {
////////////        return Document.Create(container =>
////////////        {
////////////            container.Page(page =>
////////////            {
////////////                page.Size(PageSizes.A4);
////////////                page.Margin(1.5f, Unit.Centimetre);
////////////                page.PageColor(Colors.White);
////////////                page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black));

////////////                page.Header().Element(x => BuildHeader(x, property));
////////////                page.Content().Element(x => BuildContent(x, property));
////////////                page.Footer().Element(x => BuildFooter(x, property));
////////////            });
////////////        }).GeneratePdf();
////////////    }

////////////    private void BuildHeader(IContainer container, RealEstateDetails property)
////////////    {
////////////        container.Row(row =>
////////////        {
////////////            row.RelativeItem(3).Column(col =>
////////////            {
////////////                col.Item().Text("مشاور املاک")
////////////                    .FontSize(20)
////////////                    .Bold()
////////////                    .FontColor(Colors.Blue.Darken2)
////////////                    .AlignRight();

////////////                col.Item().Text("گزارش کامل مشخصات ملک")
////////////                    .FontSize(13)
////////////                    .FontColor(Colors.Grey.Darken1)
////////////                    .AlignRight();

////////////                col.Item().PaddingTop(4).LineHorizontal(2).LineColor(Colors.Blue.Lighten2);
////////////            });

////////////            row.RelativeItem(1).Column(col =>
////////////            {
////////////                col.Item().AlignCenter().Image(GenerateQrCode(property.Id)).FitArea();
////////////            });
////////////        });
////////////    }

////////////    private void BuildContent(IContainer container, RealEstateDetails property)
////////////    {
////////////        container.PaddingVertical(0.5f, Unit.Centimetre).Column(col =>
////////////        {
////////////            // ============================================================
////////////            // کد ملک و عنوان
////////////            // ============================================================
////////////            col.Item().PaddingBottom(10).Column(c =>
////////////            {
////////////                c.Item().AlignCenter().Text($"شماره ملک: {property.Id:D4}")
////////////                    .FontSize(12)
////////////                    .FontColor(Colors.Grey.Darken2)
////////////                    .Bold();

////////////                c.Item().AlignCenter().Text(property.Title)
////////////                    .FontSize(22)
////////////                    .Bold()
////////////                    .FontColor(Colors.Blue.Darken2);

////////////                c.Item().AlignCenter().Text($"{property.Price:N0} تومان")
////////////                    .FontSize(26)
////////////                    .Bold()
////////////                    .FontColor(Colors.Green.Darken2);

////////////                c.Item().AlignCenter().Text($"متراژ: {property.SquareMeter} متر مربع  ●  قیمت هر متر: {property.PriceMeter:N0} تومان")
////////////                    .FontSize(12)
////////////                    .FontColor(Colors.Grey.Darken1);
////////////            });

////////////            // ============================================================
////////////            // اطلاعات اصلی با آیتم‌های کارتی
////////////            // ============================================================
////////////            col.Item().PaddingVertical(8).Column(c =>
////////////            {
////////////                c.Item().Text("اطلاعات اصلی")
////////////                    .FontSize(14)
////////////                    .Bold()
////////////                    .FontColor(Colors.Blue.Darken2)
////////////                    .AlignRight();

////////////                c.Item().PaddingTop(4).Row(row =>
////////////                {
////////////                    row.RelativeItem().Column(c2 =>
////////////                    {
////////////                        c2.Item().Background(Colors.Grey.Lighten4)
////////////                            .Padding(8)
////////////                            .Border(1)
////////////                            .BorderColor(Colors.Grey.Lighten2)
////////////                            .Column(c3 =>
////////////                            {
////////////                                c3.Item().Text("نوع ملک")
////////////                                    .FontSize(9)
////////////                                    .FontColor(Colors.Grey.Darken2)
////////////                                    .AlignCenter();
////////////                                c3.Item().Text(GetCategoryType(property.CategoryType))
////////////                                    .FontSize(13)
////////////                                    .Bold()
////////////                                    .AlignCenter();
////////////                            });
////////////                    });

////////////                    row.RelativeItem().Column(c2 =>
////////////                    {
////////////                        c2.Item().Background(Colors.Grey.Lighten4)
////////////                            .Padding(8)
////////////                            .Border(1)
////////////                            .BorderColor(Colors.Grey.Lighten2)
////////////                            .Column(c3 =>
////////////                            {
////////////                                c3.Item().Text("منطقه")
////////////                                    .FontSize(9)
////////////                                    .FontColor(Colors.Grey.Darken2)
////////////                                    .AlignCenter();
////////////                                c3.Item().Text(property.RegionName)
////////////                                    .FontSize(13)
////////////                                    .Bold()
////////////                                    .AlignCenter();
////////////                            });
////////////                    });

////////////                    row.RelativeItem().Column(c2 =>
////////////                    {
////////////                        c2.Item().Background(Colors.Grey.Lighten4)
////////////                            .Padding(8)
////////////                            .Border(1)
////////////                            .BorderColor(Colors.Grey.Lighten2)
////////////                            .Column(c3 =>
////////////                            {
////////////                                c3.Item().Text("سال ساخت")
////////////                                    .FontSize(9)
////////////                                    .FontColor(Colors.Grey.Darken2)
////////////                                    .AlignCenter();
////////////                                c3.Item().Text($"{property.ConstructionYear}")
////////////                                    .FontSize(13)
////////////                                    .Bold()
////////////                                    .AlignCenter();
////////////                            });
////////////                    });

////////////                    row.RelativeItem().Column(c2 =>
////////////                    {
////////////                        c2.Item().Background(Colors.Grey.Lighten4)
////////////                            .Padding(8)
////////////                            .Border(1)
////////////                            .BorderColor(Colors.Grey.Lighten2)
////////////                            .Column(c3 =>
////////////                            {
////////////                                c3.Item().Text("طبقه")
////////////                                    .FontSize(9)
////////////                                    .FontColor(Colors.Grey.Darken2)
////////////                                    .AlignCenter();
////////////                                c3.Item().Text($"{property.Floor} از {property.CountFloor}")
////////////                                    .FontSize(13)
////////////                                    .Bold()
////////////                                    .AlignCenter();
////////////                            });
////////////                    });
////////////                });
////////////            });

////////////            // ============================================================
////////////            // عکس
////////////            // ============================================================
////////////            if (property.Images?.Any() == true)
////////////            {
////////////                try
////////////                {
////////////                    var imageBytes = LoadImage(property.Images.First());
////////////                    if (imageBytes != null && imageBytes.Length > 0)
////////////                    {
////////////                        col.Item().PaddingVertical(8)
////////////                            .AlignCenter()
////////////                            .Height(230)
////////////                            .Image(imageBytes)
////////////                            .FitArea();
////////////                    }
////////////                }
////////////                catch
////////////                {
////////////                    // اگر تصویر قابل نمایش نبود، نادیده بگیر
////////////                }
////////////            }

////////////            // ============================================================
////////////            // امکانات با کارت‌های رنگی
////////////            // ============================================================
////////////            col.Item().PaddingVertical(8).Column(c =>
////////////            {
////////////                c.Item().Text("امکانات ملک")
////////////                    .FontSize(14)
////////////                    .Bold()
////////////                    .FontColor(Colors.Blue.Darken2)
////////////                    .AlignRight();

////////////                c.Item().PaddingTop(4).Row(row =>
////////////                {
////////////                    row.RelativeItem().Column(c2 =>
////////////                    {
////////////                        AddAmenityCard(c2, "آسانسور", property.IsHasElevator);
////////////                        AddAmenityCard(c2, "پارکینگ", property.IsHasParking);
////////////                    });

////////////                    row.RelativeItem().Column(c2 =>
////////////                    {
////////////                        AddAmenityCard(c2, "استخر", property.IsHasPool);
////////////                        AddAmenityCard(c2, "انباری", property.IsHasStoreRoom);
////////////                    });
////////////                });
////////////            });

////////////            // ============================================================
////////////            // آدرس
////////////            // ============================================================
////////////            col.Item().PaddingVertical(8).Column(c =>
////////////            {
////////////                c.Item().Text("آدرس ملک")
////////////                    .FontSize(14)
////////////                    .Bold()
////////////                    .FontColor(Colors.Blue.Darken2)
////////////                    .AlignRight();

////////////                c.Item().PaddingTop(4)
////////////                    .Background(Colors.Grey.Lighten4)
////////////                    .Padding(10)
////////////                    .Border(1)
////////////                    .BorderColor(Colors.Grey.Lighten2)
////////////                    .Column(c2 =>
////////////                    {
////////////                        c2.Item().Text(property.Address)
////////////                            .FontSize(12)
////////////                            .AlignRight();

////////////                        c2.Item().PaddingTop(4)
////////////                            .Text($"مختصات: {property.lat} , {property.lng}")
////////////                            .FontSize(9)
////////////                            .FontColor(Colors.Grey.Darken1)
////////////                            .AlignRight();
////////////                    });
////////////            });

////////////            // ============================================================
////////////            // هشدارها
////////////            // ============================================================
////////////            if (property.Warnings?.Any() == true)
////////////            {
////////////                col.Item().PaddingVertical(8).Column(c =>
////////////                {
////////////                    c.Item().Text("نکات مهم")
////////////                        .FontSize(14)
////////////                        .Bold()
////////////                        .FontColor(Colors.Red.Darken2)
////////////                        .AlignRight();

////////////                    c.Item().PaddingTop(4)
////////////                        .Background(Colors.Red.Lighten5)
////////////                        .Padding(10)
////////////                        .Border(1)
////////////                        .BorderColor(Colors.Red.Lighten2)
////////////                        .Column(c2 =>
////////////                        {
////////////                            foreach (var warning in property.Warnings)
////////////                            {
////////////                                c2.Item().PaddingBottom(2).Text($"• {warning}")
////////////                                    .FontColor(Colors.Red.Darken2)
////////////                                    .FontSize(11)
////////////                                    .AlignRight();
////////////                            }
////////////                        });
////////////                });
////////////            }

////////////            // ============================================================
////////////            // مشاور
////////////            // ============================================================
////////////            if (property.Agents != null)
////////////            {
////////////                col.Item().PaddingVertical(8).Column(c =>
////////////                {
////////////                    c.Item().Text("اطلاعات مشاور")
////////////                        .FontSize(14)
////////////                        .Bold()
////////////                        .FontColor(Colors.Blue.Darken2)
////////////                        .AlignRight();

////////////                    c.Item().PaddingTop(4)
////////////                        .Background(Colors.Grey.Lighten4)
////////////                        .Padding(10)
////////////                        .Border(1)
////////////                        .BorderColor(Colors.Grey.Lighten2)
////////////                        .Row(row =>
////////////                        {
////////////                            row.RelativeItem().Column(c2 =>
////////////                            {
////////////                                c2.Item().Text($"نام: {property.Agents.Name ?? "نامشخص"}")
////////////                                    .FontSize(12)
////////////                                    .AlignRight();
////////////                                c2.Item().Text($"تلفن: {property.Agents.Phone ?? "نامشخص"}")
////////////                                    .FontSize(12)
////////////                                    .AlignRight();
////////////                            });

////////////                            row.RelativeItem().Column(c2 =>
////////////                            {
////////////                                c2.Item().Text("آدرس دفتر:")
////////////                                    .FontSize(11)
////////////                                    .Bold()
////////////                                    .AlignRight();
////////////                                c2.Item().Text(property.Agents.Address ?? "نامشخص")
////////////                                    .FontSize(12)
////////////                                    .AlignRight();
////////////                            });
////////////                        });
////////////                });
////////////            }
////////////        });
////////////    }

////////////    private void BuildFooter(IContainer container, RealEstateDetails property)
////////////    {
////////////        container.BorderTop(1.5f)
////////////            .BorderColor(Colors.Blue.Lighten2)
////////////            .PaddingTop(8)
////////////            .AlignCenter()
////////////            .Text(t =>
////////////            {
////////////                t.Span("تاریخ چاپ: ");
////////////                t.Span($"{DateTime.Now:yyyy/MM/dd HH:mm}");

////////////                t.Span("    ●    ");

////////////                t.Span("کد ملک: ");
////////////                t.Span($"#{property.Id:D4}");

////////////                t.Span("    ●    ");

////////////                t.Span("صفحه ");
////////////                t.Span("1");
////////////                t.Span(" از ");
////////////                t.Span("1");
////////////            });
////////////    }

////////////    private void AddAmenityCard(ColumnDescriptor col, string name, bool has)
////////////    {
////////////        var icon = has ? "✓" : "✗";
////////////        var color = has ? Colors.Green.Darken2 : Colors.Red.Darken2;
////////////        var bgColor = has ? Colors.Green.Lighten5 : Colors.Red.Lighten5;
////////////        var borderColor = has ? Colors.Green.Lighten2 : Colors.Red.Lighten2;

////////////        col.Item().Padding(4)
////////////            .Background(bgColor)
////////////            .Padding(8)
////////////            .Border(1)
////////////            .BorderColor(borderColor)
////////////            .Text($"{icon} {name}")
////////////            .FontColor(color)
////////////            .FontSize(12)
////////////            .Bold()
////////////            .AlignCenter();
////////////    }

////////////    private string GetCategoryType(int type) => type switch
////////////    {
////////////        1 => "فروش",
////////////        2 => "رهن",
////////////        3 => "اجاره",
////////////        _ => "نامشخص"
////////////    };

////////////    private byte[] GenerateQrCode(int id)
////////////    {
////////////        try
////////////        {
////////////            using var gen = new QRCodeGenerator();
////////////            var data = gen.CreateQrCode($"PropertyId:{id}", QRCodeGenerator.ECCLevel.Q);
////////////            return new PngByteQRCode(data).GetGraphic(20);
////////////        }
////////////        catch
////////////        {
////////////            return Array.Empty<byte>();
////////////        }
////////////    }

////////////    private byte[] LoadImage(string url)
////////////    {
////////////        try
////////////        {
////////////            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
////////////            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
////////////            return client.GetByteArrayAsync(url).GetAwaiter().GetResult();
////////////        }
////////////        catch
////////////        {
////////////            return Array.Empty<byte>();
////////////        }
////////////    }
////////////}

//////////using JWTApi.Domain.Dtos.RealEstate;
//////////using QRCoder;
//////////using QuestPDF;
//////////using QuestPDF.Fluent;
//////////using QuestPDF.Helpers;
//////////using QuestPDF.Infrastructure;

//////////namespace JWTApi.Services.Pdf;

//////////public interface IPdfGeneratorService
//////////{
//////////    Task<byte[]> GeneratePropertyPdf(RealEstateDetails property, CancellationToken cancellationToken);
//////////}

//////////public class PdfGeneratorService : IPdfGeneratorService
//////////{
//////////    public PdfGeneratorService()
//////////    {
//////////        Settings.License = LicenseType.Community;
//////////        Settings.CheckIfAllTextGlyphsAreAvailable = false;
//////////    }

//////////    public Task<byte[]> GeneratePropertyPdf(RealEstateDetails property, CancellationToken cancellationToken)
//////////    {
//////////        return Task.FromResult(GeneratePdf(property));
//////////    }

//////////    private byte[] GeneratePdf(RealEstateDetails property)
//////////    {
//////////        return Document.Create(container =>
//////////        {
//////////            container.Page(page =>
//////////            {
//////////                page.Size(PageSizes.A4);
//////////                page.Margin(1.5f, Unit.Centimetre);
//////////                page.PageColor(Colors.White);
//////////                page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black));

//////////                page.Header().Element(x => BuildHeader(x, property));
//////////                page.Content().Element(x => BuildContentWithWatermark(x, property));
//////////                page.Footer().Element(x => BuildFooter(x, property));
//////////            });
//////////        }).GeneratePdf();
//////////    }

//////////    private void BuildContentWithWatermark(IContainer container, RealEstateDetails property)
//////////    {
//////////        container.Layers(layers =>
//////////        {
//////////            // لایه اصلی (محتوا) - باید اول باشد
//////////            layers.PrimaryLayer()
//////////                .PaddingVertical(0.3f, Unit.Centimetre)
//////////                .Column(col =>
//////////                {
//////////                    BuildMainContent(col, property);
//////////                });

//////////            // لایه واترمارک (لایه اضافی)
//////////            layers.Layer()
//////////                .AlignCenter()
//////////                .AlignMiddle()
//////////                .Rotate(-30)
//////////                .Text("مشاور املاک")
//////////                .FontSize(60)
//////////                .Bold()
//////////                .FontColor(Colors.Grey.Lighten2);
//////////        });
//////////    }

//////////    private void BuildHeader(IContainer container, RealEstateDetails property)
//////////    {
//////////        container.Row(row =>
//////////        {
//////////            row.RelativeItem(3).Column(col =>
//////////            {
//////////                col.Item().Text("مشاور املاک")
//////////                    .FontSize(20)
//////////                    .Bold()
//////////                    .FontColor(Colors.Blue.Darken2)
//////////                    .AlignRight();

//////////                col.Item().Text("گزارش کامل مشخصات ملک")
//////////                    .FontSize(13)
//////////                    .FontColor(Colors.Grey.Darken1)
//////////                    .AlignRight();

//////////                col.Item().PaddingTop(4).LineHorizontal(2).LineColor(Colors.Blue.Lighten2);
//////////            });

//////////            row.RelativeItem(1).Column(col =>
//////////            {
//////////                col.Item().AlignCenter().Image(GenerateQrCode(property.Id)).FitArea();
//////////            });
//////////        });
//////////    }

//////////    private void BuildMainContent(ColumnDescriptor col, RealEstateDetails property)
//////////    {
//////////        // ============================================================
//////////        // کد ملک
//////////        // ============================================================
//////////        col.Item().AlignCenter().Text($"شماره ملک: {property.Id:D4}")
//////////            .FontSize(12)
//////////            .FontColor(Colors.Grey.Darken2)
//////////            .Bold();

//////////        // ============================================================
//////////        // عنوان و قیمت
//////////        // ============================================================
//////////        col.Item().PaddingVertical(4).Column(c =>
//////////        {
//////////            c.Item().AlignCenter().Text(property.Title)
//////////                .FontSize(22)
//////////                .Bold()
//////////                .FontColor(Colors.Blue.Darken2);

//////////            c.Item().AlignCenter().Text($"{property.Price:N0} تومان")
//////////                .FontSize(26)
//////////                .Bold()
//////////                .FontColor(Colors.Green.Darken2);

//////////            c.Item().AlignCenter().Text($"متراژ: {property.SquareMeter} متر مربع  ●  قیمت هر متر: {property.PriceMeter:N0} تومان")
//////////                .FontSize(12)
//////////                .FontColor(Colors.Grey.Darken1);
//////////        });

//////////        // ============================================================
//////////        // توضیحات ملک
//////////        // ============================================================
//////////        if (!string.IsNullOrEmpty(property.DescriptionRows))
//////////        {
//////////            col.Item().PaddingVertical(4).Column(c =>
//////////            {
//////////                c.Item().Text("توضیحات")
//////////                    .FontSize(13)
//////////                    .Bold()
//////////                    .FontColor(Colors.Blue.Darken2)
//////////                    .AlignRight();

//////////                c.Item().PaddingTop(4)
//////////                    .Background(Colors.Grey.Lighten4)
//////////                    .Padding(10)
//////////                    .Border(1)
//////////                    .BorderColor(Colors.Grey.Lighten2)
//////////                    .Text(property.DescriptionRows)
//////////                    .FontSize(11)
//////////                    .AlignRight();
//////////            });
//////////        }

//////////        // ============================================================
//////////        // اطلاعات اصلی با کارت‌ها
//////////        // ============================================================
//////////        col.Item().PaddingVertical(4).Column(c =>
//////////        {
//////////            c.Item().Text("اطلاعات اصلی")
//////////                .FontSize(13)
//////////                .Bold()
//////////                .FontColor(Colors.Blue.Darken2)
//////////                .AlignRight();

//////////            c.Item().PaddingTop(4).Row(row =>
//////////            {
//////////                row.RelativeItem().Column(c2 =>
//////////                {
//////////                    c2.Item().Background(Colors.Grey.Lighten4)
//////////                        .Padding(8)
//////////                        .Border(1)
//////////                        .BorderColor(Colors.Grey.Lighten2)
//////////                        .Column(c3 =>
//////////                        {
//////////                            c3.Item().Text("نوع ملک")
//////////                                .FontSize(9)
//////////                                .FontColor(Colors.Grey.Darken2)
//////////                                .AlignCenter();
//////////                            c3.Item().Text(GetCategoryType(property.CategoryType))
//////////                                .FontSize(13)
//////////                                .Bold()
//////////                                .AlignCenter();
//////////                        });
//////////                });

//////////                row.RelativeItem().Column(c2 =>
//////////                {
//////////                    c2.Item().Background(Colors.Grey.Lighten4)
//////////                        .Padding(8)
//////////                        .Border(1)
//////////                        .BorderColor(Colors.Grey.Lighten2)
//////////                        .Column(c3 =>
//////////                        {
//////////                            c3.Item().Text("منطقه")
//////////                                .FontSize(9)
//////////                                .FontColor(Colors.Grey.Darken2)
//////////                                .AlignCenter();
//////////                            c3.Item().Text(property.RegionName)
//////////                                .FontSize(13)
//////////                                .Bold()
//////////                                .AlignCenter();
//////////                        });
//////////                });

//////////                row.RelativeItem().Column(c2 =>
//////////                {
//////////                    c2.Item().Background(Colors.Grey.Lighten4)
//////////                        .Padding(8)
//////////                        .Border(1)
//////////                        .BorderColor(Colors.Grey.Lighten2)
//////////                        .Column(c3 =>
//////////                        {
//////////                            c3.Item().Text("سال ساخت")
//////////                                .FontSize(9)
//////////                                .FontColor(Colors.Grey.Darken2)
//////////                                .AlignCenter();
//////////                            c3.Item().Text($"{property.ConstructionYear}")
//////////                                .FontSize(13)
//////////                                .Bold()
//////////                                .AlignCenter();
//////////                        });
//////////                });

//////////                row.RelativeItem().Column(c2 =>
//////////                {
//////////                    c2.Item().Background(Colors.Grey.Lighten4)
//////////                        .Padding(8)
//////////                        .Border(1)
//////////                        .BorderColor(Colors.Grey.Lighten2)
//////////                        .Column(c3 =>
//////////                        {
//////////                            c3.Item().Text("طبقه")
//////////                                .FontSize(9)
//////////                                .FontColor(Colors.Grey.Darken2)
//////////                                .AlignCenter();
//////////                            c3.Item().Text($"{property.Floor} از {property.CountFloor}")
//////////                                .FontSize(13)
//////////                                .Bold()
//////////                                .AlignCenter();
//////////                        });
//////////                });
//////////            });
//////////        });

//////////        // ============================================================
//////////        // عکس
//////////        // ============================================================
//////////        if (property.Images?.Any() == true)
//////////        {
//////////            try
//////////            {
//////////                var imageBytes = LoadImage(property.Images.First());
//////////                if (imageBytes != null && imageBytes.Length > 0)
//////////                {
//////////                    col.Item().PaddingVertical(6)
//////////                        .AlignCenter()
//////////                        .Height(200)
//////////                        .Image(imageBytes)
//////////                        .FitArea();
//////////                }
//////////            }
//////////            catch
//////////            {
//////////                // خطا را نادیده بگیر
//////////            }
//////////        }

//////////        // ============================================================
//////////        // امکانات
//////////        // ============================================================
//////////        col.Item().PaddingVertical(4).Column(c =>
//////////        {
//////////            c.Item().Text("امکانات ملک")
//////////                .FontSize(13)
//////////                .Bold()
//////////                .FontColor(Colors.Blue.Darken2)
//////////                .AlignRight();

//////////            c.Item().PaddingTop(4).Row(row =>
//////////            {
//////////                row.RelativeItem().Column(c2 =>
//////////                {
//////////                    AddAmenityCard(c2, "آسانسور", property.IsHasElevator);
//////////                    AddAmenityCard(c2, "پارکینگ", property.IsHasParking);
//////////                });

//////////                row.RelativeItem().Column(c2 =>
//////////                {
//////////                    AddAmenityCard(c2, "استخر", property.IsHasPool);
//////////                    AddAmenityCard(c2, "انباری", property.IsHasStoreRoom);
//////////                });
//////////            });
//////////        });

//////////        // ============================================================
//////////        // آدرس
//////////        // ============================================================
//////////        col.Item().PaddingVertical(4).Column(c =>
//////////        {
//////////            c.Item().Text("آدرس ملک")
//////////                .FontSize(13)
//////////                .Bold()
//////////                .FontColor(Colors.Blue.Darken2)
//////////                .AlignRight();

//////////            c.Item().PaddingTop(4)
//////////                .Background(Colors.Grey.Lighten4)
//////////                .Padding(10)
//////////                .Border(1)
//////////                .BorderColor(Colors.Grey.Lighten2)
//////////                .Column(c2 =>
//////////                {
//////////                    c2.Item().Text(property.Address)
//////////                        .FontSize(12)
//////////                        .AlignRight();

//////////                    c2.Item().PaddingTop(4)
//////////                        .Text($"مختصات: {property.lat} , {property.lng}")
//////////                        .FontSize(9)
//////////                        .FontColor(Colors.Grey.Darken1)
//////////                        .AlignRight();
//////////                });
//////////        });

//////////        // ============================================================
//////////        // هشدارها
//////////        // ============================================================
//////////        if (property.Warnings?.Any() == true)
//////////        {
//////////            col.Item().PaddingVertical(4).Column(c =>
//////////            {
//////////                c.Item().Text("نکات مهم")
//////////                    .FontSize(13)
//////////                    .Bold()
//////////                    .FontColor(Colors.Red.Darken2)
//////////                    .AlignRight();

//////////                c.Item().PaddingTop(4)
//////////                    .Background(Colors.Red.Lighten5)
//////////                    .Padding(10)
//////////                    .Border(1)
//////////                    .BorderColor(Colors.Red.Lighten2)
//////////                    .Column(c2 =>
//////////                    {
//////////                        foreach (var warning in property.Warnings)
//////////                        {
//////////                            c2.Item().PaddingBottom(2).Text($"• {warning}")
//////////                                .FontColor(Colors.Red.Darken2)
//////////                                .FontSize(11)
//////////                                .AlignRight();
//////////                        }
//////////                    });
//////////            });
//////////        }

//////////        // ============================================================
//////////        // مشاور
//////////        // ============================================================
//////////        if (property.Agents != null)
//////////        {
//////////            col.Item().PaddingVertical(4).Column(c =>
//////////            {
//////////                c.Item().Text("اطلاعات مشاور")
//////////                    .FontSize(13)
//////////                    .Bold()
//////////                    .FontColor(Colors.Blue.Darken2)
//////////                    .AlignRight();

//////////                c.Item().PaddingTop(4)
//////////                    .Background(Colors.Grey.Lighten4)
//////////                    .Padding(10)
//////////                    .Border(1)
//////////                    .BorderColor(Colors.Grey.Lighten2)
//////////                    .Row(row =>
//////////                    {
//////////                        row.RelativeItem().Column(c2 =>
//////////                        {
//////////                            c2.Item().Text($"نام: {property.Agents.Name ?? "نامشخص"}")
//////////                                .FontSize(12)
//////////                                .AlignRight();
//////////                            c2.Item().Text($"تلفن: {property.Agents.Phone ?? "نامشخص"}")
//////////                                .FontSize(12)
//////////                                .AlignRight();
//////////                        });

//////////                        row.RelativeItem().Column(c2 =>
//////////                        {
//////////                            c2.Item().Text("آدرس دفتر:")
//////////                                .FontSize(11)
//////////                                .Bold()
//////////                                .AlignRight();
//////////                            c2.Item().Text(property.Agents.Address ?? "نامشخص")
//////////                                .FontSize(12)
//////////                                .AlignRight();
//////////                        });
//////////                    });
//////////            });
//////////        }
//////////    }

//////////    private void BuildFooter(IContainer container, RealEstateDetails property)
//////////    {
//////////        container.BorderTop(1.5f)
//////////            .BorderColor(Colors.Blue.Lighten2)
//////////            .PaddingTop(8)
//////////            .AlignCenter()
//////////            .Text(t =>
//////////            {
//////////                t.Span("تاریخ چاپ: ");
//////////                t.Span($"{DateTime.Now:yyyy/MM/dd HH:mm}");

//////////                t.Span("    ●    ");

//////////                t.Span("کد ملک: ");
//////////                t.Span($"#{property.Id:D4}");

//////////                t.Span("    ●    ");

//////////                t.Span("صفحه ");
//////////                t.Span("1");
//////////                t.Span(" از ");
//////////                t.Span("1");
//////////            });
//////////    }

//////////    private void AddAmenityCard(ColumnDescriptor col, string name, bool has)
//////////    {
//////////        var icon = has ? "✓" : "✗";
//////////        var color = has ? Colors.Green.Darken2 : Colors.Red.Darken2;
//////////        var bgColor = has ? Colors.Green.Lighten5 : Colors.Red.Lighten5;
//////////        var borderColor = has ? Colors.Green.Lighten2 : Colors.Red.Lighten2;

//////////        col.Item().Padding(3)
//////////            .Background(bgColor)
//////////            .Padding(6)
//////////            .Border(1)
//////////            .BorderColor(borderColor)
//////////            .Text($"{icon} {name}")
//////////            .FontColor(color)
//////////            .FontSize(12)
//////////            .Bold()
//////////            .AlignCenter();
//////////    }

//////////    private string GetCategoryType(int type) => type switch
//////////    {
//////////        1 => "فروش",
//////////        2 => "رهن",
//////////        3 => "اجاره",
//////////        _ => "نامشخص"
//////////    };

//////////    private byte[] GenerateQrCode(int id)
//////////    {
//////////        try
//////////        {
//////////            using var gen = new QRCodeGenerator();
//////////            var data = gen.CreateQrCode($"PropertyId:{id}", QRCodeGenerator.ECCLevel.Q);
//////////            return new PngByteQRCode(data).GetGraphic(20);
//////////        }
//////////        catch
//////////        {
//////////            return Array.Empty<byte>();
//////////        }
//////////    }

//////////    private byte[] LoadImage(string url)
//////////    {
//////////        try
//////////        {
//////////            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
//////////            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
//////////            return client.GetByteArrayAsync(url).GetAwaiter().GetResult();
//////////        }
//////////        catch
//////////        {
//////////            return Array.Empty<byte>();
//////////        }
//////////    }
//////////}

////////using JWTApi.Domain.Dtos.RealEstate;
////////using QRCoder;
////////using QuestPDF;
////////using QuestPDF.Fluent;
////////using QuestPDF.Helpers;
////////using QuestPDF.Infrastructure;
////////using System.Text.RegularExpressions;

////////namespace JWTApi.Services.Pdf;

////////public interface IPdfGeneratorService
////////{
////////    Task<byte[]> GeneratePropertyPdf(RealEstateDetails property, CancellationToken cancellationToken);
////////}

////////public class PdfGeneratorService : IPdfGeneratorService
////////{
////////    public PdfGeneratorService()
////////    {
////////        Settings.License = LicenseType.Community;
////////        Settings.CheckIfAllTextGlyphsAreAvailable = false;
////////    }

////////    public Task<byte[]> GeneratePropertyPdf(RealEstateDetails property, CancellationToken cancellationToken)
////////    {
////////        return Task.FromResult(GeneratePdf(property));
////////    }

////////    private byte[] GeneratePdf(RealEstateDetails property)
////////    {
////////        return Document.Create(container =>
////////        {
////////            container.Page(page =>
////////            {
////////                page.Size(PageSizes.A4);
////////                page.Margin(1.5f, Unit.Centimetre);
////////                page.PageColor(Colors.White);
////////                page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black));

////////                page.Header().Element(x => BuildHeader(x, property));
////////                page.Content().Element(x => BuildContentWithWatermark(x, property));
////////                page.Footer().Element(x => BuildFooter(x, property));
////////            });
////////        }).GeneratePdf();
////////    }

////////    private void BuildContentWithWatermark(IContainer container, RealEstateDetails property)
////////    {
////////        container.Layers(layers =>
////////        {
////////            // لایه اصلی (محتوا)
////////            layers.PrimaryLayer()
////////                .PaddingVertical(0.3f, Unit.Centimetre)
////////                .Column(col =>
////////                {
////////                    BuildMainContent(col, property);
////////                });

////////            // لایه واترمارک (کم‌رنگ‌تر)
////////            layers.Layer()
////////                .AlignCenter()
////////                .AlignMiddle()
////////                .Rotate(-30)
////////                .Text("خونه یاب")
////////                .FontSize(60)
////////                .Bold()
////////                .FontColor(Colors.Grey.Lighten3)  // رنگ کم‌رنگ‌تر
////////                .Light(); // شفافیت
////////        });
////////    }

////////    private void BuildHeader(IContainer container, RealEstateDetails property)
////////    {
////////        container.Row(row =>
////////        {
////////            row.RelativeItem(3).Column(col =>
////////            {
////////                col.Item().Text("خونه یاب")
////////                    .FontSize(20)
////////                    .Bold()
////////                    .FontColor(Colors.Blue.Darken2)
////////                    .AlignRight();

////////                col.Item().Text("گزارش کامل مشخصات ملک")
////////                    .FontSize(13)
////////                    .FontColor(Colors.Grey.Darken1)
////////                    .AlignRight();

////////                col.Item().PaddingTop(4).LineHorizontal(2).LineColor(Colors.Blue.Lighten2);
////////            });

////////            row.RelativeItem(1).Column(col =>
////////            {
////////                col.Item().AlignCenter().Image(GenerateQrCode(property.Id)).FitArea();
////////            });
////////        });
////////    }

////////    private void BuildMainContent(ColumnDescriptor col, RealEstateDetails property)
////////    {
////////        // ============================================================
////////        // کد ملک
////////        // ============================================================
////////        col.Item().AlignCenter().Text($"شماره ملک: {property.Id:D4}")
////////            .FontSize(12)
////////            .FontColor(Colors.Grey.Darken2)
////////            .Bold();

////////        // ============================================================
////////        // عنوان و قیمت
////////        // ============================================================
////////        col.Item().PaddingVertical(4).Column(c =>
////////        {
////////            c.Item().AlignCenter().Text(property.Title)
////////                .FontSize(22)
////////                .Bold()
////////                .FontColor(Colors.Blue.Darken2);

////////            c.Item().AlignCenter().Text($"{property.Price:N0} تومان")
////////                .FontSize(26)
////////                .Bold()
////////                .FontColor(Colors.Green.Darken2);

////////            c.Item().AlignCenter().Text($"متراژ: {property.SquareMeter} متر مربع  ●  قیمت هر متر: {property.PriceMeter:N0} تومان")
////////                .FontSize(12)
////////                .FontColor(Colors.Grey.Darken1);
////////        });

////////        // ============================================================
////////        // توضیحات ملک (با حذف تگ‌های HTML)
////////        // ============================================================
////////        if (!string.IsNullOrEmpty(property.DescriptionRows))
////////        {
////////            var cleanDescription = StripHtmlTags(property.DescriptionRows);

////////            col.Item().PaddingVertical(4).Column(c =>
////////            {
////////                c.Item().Text("توضیحات")
////////                    .FontSize(13)
////////                    .Bold()
////////                    .FontColor(Colors.Blue.Darken2)
////////                    .AlignRight();

////////                c.Item().PaddingTop(4)
////////                    .Background(Colors.Grey.Lighten4)
////////                    .Padding(10)
////////                    .Border(1)
////////                    .BorderColor(Colors.Grey.Lighten2)
////////                    .Text(cleanDescription)
////////                    .FontSize(11)
////////                    .AlignRight();
////////            });
////////        }

////////        // ============================================================
////////        // اطلاعات اصلی با کارت‌ها
////////        // ============================================================
////////        col.Item().PaddingVertical(4).Column(c =>
////////        {
////////            c.Item().Text("اطلاعات اصلی")
////////                .FontSize(13)
////////                .Bold()
////////                .FontColor(Colors.Blue.Darken2)
////////                .AlignRight();

////////            c.Item().PaddingTop(4).Row(row =>
////////            {
////////                row.RelativeItem().Column(c2 =>
////////                {
////////                    c2.Item().Background(Colors.Grey.Lighten4)
////////                        .Padding(8)
////////                        .Border(1)
////////                        .BorderColor(Colors.Grey.Lighten2)
////////                        .Column(c3 =>
////////                        {
////////                            c3.Item().Text("نوع ملک")
////////                                .FontSize(9)
////////                                .FontColor(Colors.Grey.Darken2)
////////                                .AlignCenter();
////////                            c3.Item().Text(GetCategoryType(property.CategoryType))
////////                                .FontSize(13)
////////                                .Bold()
////////                                .AlignCenter();
////////                        });
////////                });

////////                row.RelativeItem().Column(c2 =>
////////                {
////////                    c2.Item().Background(Colors.Grey.Lighten4)
////////                        .Padding(8)
////////                        .Border(1)
////////                        .BorderColor(Colors.Grey.Lighten2)
////////                        .Column(c3 =>
////////                        {
////////                            c3.Item().Text("منطقه")
////////                                .FontSize(9)
////////                                .FontColor(Colors.Grey.Darken2)
////////                                .AlignCenter();
////////                            c3.Item().Text(property.RegionName)
////////                                .FontSize(13)
////////                                .Bold()
////////                                .AlignCenter();
////////                        });
////////                });

////////                row.RelativeItem().Column(c2 =>
////////                {
////////                    c2.Item().Background(Colors.Grey.Lighten4)
////////                        .Padding(8)
////////                        .Border(1)
////////                        .BorderColor(Colors.Grey.Lighten2)
////////                        .Column(c3 =>
////////                        {
////////                            c3.Item().Text("سال ساخت")
////////                                .FontSize(9)
////////                                .FontColor(Colors.Grey.Darken2)
////////                                .AlignCenter();
////////                            c3.Item().Text($"{property.ConstructionYear}")
////////                                .FontSize(13)
////////                                .Bold()
////////                                .AlignCenter();
////////                        });
////////                });

////////                row.RelativeItem().Column(c2 =>
////////                {
////////                    c2.Item().Background(Colors.Grey.Lighten4)
////////                        .Padding(8)
////////                        .Border(1)
////////                        .BorderColor(Colors.Grey.Lighten2)
////////                        .Column(c3 =>
////////                        {
////////                            c3.Item().Text("طبقه")
////////                                .FontSize(9)
////////                                .FontColor(Colors.Grey.Darken2)
////////                                .AlignCenter();
////////                            c3.Item().Text($"{property.Floor} از {property.CountFloor}")
////////                                .FontSize(13)
////////                                .Bold()
////////                                .AlignCenter();
////////                        });
////////                });
////////            });
////////        });

////////        // ============================================================
////////        // عکس
////////        // ============================================================
////////        if (property.Images?.Any() == true)
////////        {
////////            try
////////            {
////////                var imageBytes = LoadImage(property.Images.First());
////////                if (imageBytes != null && imageBytes.Length > 0)
////////                {
////////                    col.Item().PaddingVertical(6)
////////                        .AlignCenter()
////////                        .Height(200)
////////                        .Image(imageBytes)
////////                        .FitArea();
////////                }
////////            }
////////            catch
////////            {
////////                // خطا را نادیده بگیر
////////            }
////////        }

////////        // ============================================================
////////        // امکانات
////////        // ============================================================
////////        col.Item().PaddingVertical(4).Column(c =>
////////        {
////////            c.Item().Text("امکانات ملک")
////////                .FontSize(13)
////////                .Bold()
////////                .FontColor(Colors.Blue.Darken2)
////////                .AlignRight();

////////            c.Item().PaddingTop(4).Row(row =>
////////            {
////////                row.RelativeItem().Column(c2 =>
////////                {
////////                    AddAmenityCard(c2, "آسانسور", property.IsHasElevator);
////////                    AddAmenityCard(c2, "پارکینگ", property.IsHasParking);
////////                });

////////                row.RelativeItem().Column(c2 =>
////////                {
////////                    AddAmenityCard(c2, "استخر", property.IsHasPool);
////////                    AddAmenityCard(c2, "انباری", property.IsHasStoreRoom);
////////                });
////////            });
////////        });

////////        // ============================================================
////////        // آدرس
////////        // ============================================================
////////        col.Item().PaddingVertical(4).Column(c =>
////////        {
////////            c.Item().Text("آدرس ملک")
////////                .FontSize(13)
////////                .Bold()
////////                .FontColor(Colors.Blue.Darken2)
////////                .AlignRight();

////////            c.Item().PaddingTop(4)
////////                .Background(Colors.Grey.Lighten4)
////////                .Padding(10)
////////                .Border(1)
////////                .BorderColor(Colors.Grey.Lighten2)
////////                .Column(c2 =>
////////                {
////////                    c2.Item().Text(property.Address)
////////                        .FontSize(12)
////////                        .AlignRight();

////////                    c2.Item().PaddingTop(4)
////////                        .Text($"مختصات: {property.lat} , {property.lng}")
////////                        .FontSize(9)
////////                        .FontColor(Colors.Grey.Darken1)
////////                        .AlignRight();
////////                });
////////        });

////////        // ============================================================
////////        // هشدارها
////////        // ============================================================
////////        if (property.Warnings?.Any() == true)
////////        {
////////            col.Item().PaddingVertical(4).Column(c =>
////////            {
////////                c.Item().Text("نکات مهم")
////////                    .FontSize(13)
////////                    .Bold()
////////                    .FontColor(Colors.Red.Darken2)
////////                    .AlignRight();

////////                c.Item().PaddingTop(4)
////////                    .Background(Colors.Red.Lighten5)
////////                    .Padding(10)
////////                    .Border(1)
////////                    .BorderColor(Colors.Red.Lighten2)
////////                    .Column(c2 =>
////////                    {
////////                        foreach (var warning in property.Warnings)
////////                        {
////////                            var cleanWarning = StripHtmlTags(warning);
////////                            c2.Item().PaddingBottom(2).Text($"• {cleanWarning}")
////////                                .FontColor(Colors.Red.Darken2)
////////                                .FontSize(11)
////////                                .AlignRight();
////////                        }
////////                    });
////////            });
////////        }

////////        // ============================================================
////////        // مشاور
////////        // ============================================================
////////        if (property.Agents != null)
////////        {
////////            col.Item().PaddingVertical(4).Column(c =>
////////            {
////////                c.Item().Text("اطلاعات مشاور")
////////                    .FontSize(13)
////////                    .Bold()
////////                    .FontColor(Colors.Blue.Darken2)
////////                    .AlignRight();

////////                c.Item().PaddingTop(4)
////////                    .Background(Colors.Grey.Lighten4)
////////                    .Padding(10)
////////                    .Border(1)
////////                    .BorderColor(Colors.Grey.Lighten2)
////////                    .Row(row =>
////////                    {
////////                        row.RelativeItem().Column(c2 =>
////////                        {
////////                            c2.Item().Text($"نام: {property.Agents.Name ?? "نامشخص"}")
////////                                .FontSize(12)
////////                                .AlignRight();
////////                            c2.Item().Text($"تلفن: {property.Agents.Phone ?? "نامشخص"}")
////////                                .FontSize(12)
////////                                .AlignRight();
////////                        });

////////                        row.RelativeItem().Column(c2 =>
////////                        {
////////                            c2.Item().Text("آدرس دفتر:")
////////                                .FontSize(11)
////////                                .Bold()
////////                                .AlignRight();
////////                            c2.Item().Text(property.Agents.Address ?? "نامشخص")
////////                                .FontSize(12)
////////                                .AlignRight();
////////                        });
////////                    });
////////            });
////////        }
////////    }

////////    private void BuildFooter(IContainer container, RealEstateDetails property)
////////    {
////////        container.BorderTop(1.5f)
////////            .BorderColor(Colors.Blue.Lighten2)
////////            .PaddingTop(8)
////////            .AlignCenter()
////////            .Text(t =>
////////            {
////////                t.Span("تاریخ چاپ: ");
////////                t.Span($"{DateTime.Now:yyyy/MM/dd HH:mm}");

////////                t.Span("    ●    ");

////////                t.Span("کد ملک: ");
////////                t.Span($"#{property.Id:D4}");

////////                t.Span("    ●    ");

////////                t.Span("صفحه ");
////////                t.Span("1");
////////                t.Span(" از ");
////////                t.Span("1");
////////            });
////////    }

////////    private void AddAmenityCard(ColumnDescriptor col, string name, bool has)
////////    {
////////        var icon = has ? "✓" : "✗";
////////        var color = has ? Colors.Green.Darken2 : Colors.Red.Darken2;
////////        var bgColor = has ? Colors.Green.Lighten5 : Colors.Red.Lighten5;
////////        var borderColor = has ? Colors.Green.Lighten2 : Colors.Red.Lighten2;

////////        col.Item().Padding(3)
////////            .Background(bgColor)
////////            .Padding(6)
////////            .Border(1)
////////            .BorderColor(borderColor)
////////            .Text($"{icon} {name}")
////////            .FontColor(color)
////////            .FontSize(12)
////////            .Bold()
////////            .AlignCenter();
////////    }

////////    private string GetCategoryType(int type) => type switch
////////    {
////////        1 => "فروش",
////////        2 => "رهن",
////////        3 => "اجاره",
////////        _ => "نامشخص"
////////    };

////////    /// <summary>
////////    /// حذف تگ‌های HTML از متن
////////    /// </summary>
////////    private string StripHtmlTags(string html)
////////    {
////////        if (string.IsNullOrEmpty(html))
////////            return html;

////////        // حذف تگ‌های HTML
////////        var clean = Regex.Replace(html, @"<[^>]*>", string.Empty);

////////        // تبدیل &nbsp; به فاصله
////////        clean = clean.Replace("&nbsp;", " ");

////////        // تبدیل &amp; به &
////////        clean = clean.Replace("&amp;", "&");

////////        // تبدیل &lt; به <
////////        clean = clean.Replace("&lt;", "<");

////////        // تبدیل &gt; به >
////////        clean = clean.Replace("&gt;", ">");

////////        // تبدیل &quot; به "
////////        clean = clean.Replace("&quot;", "\"");

////////        // حذف فاصله‌های اضافی
////////        clean = Regex.Replace(clean, @"\s+", " ");

////////        return clean.Trim();
////////    }

////////    private byte[] GenerateQrCode(int id)
////////    {
////////        try
////////        {
////////            using var gen = new QRCodeGenerator();
////////            var data = gen.CreateQrCode($"PropertyId:{id}", QRCodeGenerator.ECCLevel.Q);
////////            return new PngByteQRCode(data).GetGraphic(20);
////////        }
////////        catch
////////        {
////////            return Array.Empty<byte>();
////////        }
////////    }

////////    private byte[] LoadImage(string url)
////////    {
////////        try
////////        {
////////            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
////////            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
////////            return client.GetByteArrayAsync(url).GetAwaiter().GetResult();
////////        }
////////        catch
////////        {
////////            return Array.Empty<byte>();
////////        }
////////    }
////////}

//////using JWTApi.Domain.Dtos.RealEstate;
//////using QRCoder;
//////using QuestPDF;
//////using QuestPDF.Fluent;
//////using QuestPDF.Helpers;
//////using QuestPDF.Infrastructure;
//////using System.Text.RegularExpressions;

//////namespace JWTApi.Services.Pdf;

//////public interface IPdfGeneratorService
//////{
//////    Task<byte[]> GeneratePropertyPdf(RealEstateDetails property, CancellationToken cancellationToken);
//////}

//////public class PdfGeneratorService : IPdfGeneratorService
//////{
//////    public PdfGeneratorService()
//////    {
//////        Settings.License = LicenseType.Community;
//////        Settings.CheckIfAllTextGlyphsAreAvailable = false;
//////    }

//////    public Task<byte[]> GeneratePropertyPdf(RealEstateDetails property, CancellationToken cancellationToken)
//////    {
//////        return Task.FromResult(GeneratePdf(property));
//////    }

//////    private byte[] GeneratePdf(RealEstateDetails property)
//////    {
//////        return Document.Create(container =>
//////        {
//////            container.Page(page =>
//////            {
//////                page.Size(PageSizes.A4);
//////                page.Margin(1.5f, Unit.Centimetre);
//////                page.PageColor(Colors.White);
//////                page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black));

//////                page.Header().Element(x => BuildHeader(x, property));
//////                page.Content().Element(x => BuildContentWithWatermark(x, property));
//////                page.Footer().Element(x => BuildFooter(x, property));
//////            });
//////        }).GeneratePdf();
//////    }

//////    private void BuildContentWithWatermark(IContainer container, RealEstateDetails property)
//////    {
//////        container.Layers(layers =>
//////        {
//////            // لایه اصلی (محتوا)
//////            layers.PrimaryLayer()
//////                .PaddingVertical(0.2f, Unit.Centimetre)
//////                .Column(col =>
//////                {
//////                    BuildMainContent(col, property);
//////                });

//////            // لایه واترمارک
//////            layers.Layer()
//////                .AlignCenter()
//////                .AlignMiddle()
//////                .Rotate(-30)
//////                .Text("مشاور املاک")
//////                .FontSize(60)
//////                .Bold()
//////                .FontColor(Colors.Grey.Lighten4)
//////                .Light();
//////        });
//////    }

//////    private void BuildHeader(IContainer container, RealEstateDetails property)
//////    {
//////        container.Row(row =>
//////        {
//////            row.RelativeItem(3).Column(col =>
//////            {
//////                col.Item().Text("مشاور املاک")
//////                    .FontSize(20)
//////                    .Bold()
//////                    .FontColor(Colors.Blue.Darken2)
//////                    .AlignRight();

//////                col.Item().Text("گزارش کامل مشخصات ملک")
//////                    .FontSize(13)
//////                    .FontColor(Colors.Grey.Darken1)
//////                    .AlignRight();

//////                col.Item().PaddingTop(4).LineHorizontal(2).LineColor(Colors.Blue.Lighten2);
//////            });

//////            row.RelativeItem(1).Column(col =>
//////            {
//////                col.Item().AlignCenter().Image(GenerateQrCode(property.Id)).FitArea();
//////            });
//////        });
//////    }

//////    private void BuildMainContent(ColumnDescriptor col, RealEstateDetails property)
//////    {
//////        // ============================================================
//////        // کد ملک
//////        // ============================================================
//////        col.Item().AlignCenter().Text($"شماره ملک: {property.Id:D4}")
//////            .FontSize(12)
//////            .FontColor(Colors.Grey.Darken2)
//////            .Bold();

//////        // ============================================================
//////        // عنوان و قیمت
//////        // ============================================================
//////        col.Item().PaddingVertical(3).Column(c =>
//////        {
//////            c.Item().AlignCenter().Text(property.Title)
//////                .FontSize(22)
//////                .Bold()
//////                .FontColor(Colors.Blue.Darken2);

//////            c.Item().AlignCenter().Text($"{property.Price:N0} تومان")
//////                .FontSize(26)
//////                .Bold()
//////                .FontColor(Colors.Green.Darken2);

//////            c.Item().AlignCenter().Text($"متراژ: {property.SquareMeter} متر مربع  ●  قیمت هر متر: {property.PriceMeter:N0} تومان")
//////                .FontSize(12)
//////                .FontColor(Colors.Grey.Darken1);
//////        });

//////        // ============================================================
//////        // توضیحات ملک
//////        // ============================================================
//////        if (!string.IsNullOrEmpty(property.DescriptionRows))
//////        {
//////            var cleanDescription = StripHtmlTags(property.DescriptionRows);

//////            col.Item().PaddingVertical(3).Column(c =>
//////            {
//////                c.Item().Text("توضیحات")
//////                    .FontSize(13)
//////                    .Bold()
//////                    .FontColor(Colors.Blue.Darken2)
//////                    .AlignRight();

//////                c.Item().PaddingTop(3)
//////                    .Background(Colors.Grey.Lighten4)
//////                    .Padding(8)
//////                    .Border(1)
//////                    .BorderColor(Colors.Grey.Lighten2)
//////                    .Text(cleanDescription)
//////                    .FontSize(11)
//////                    .AlignRight();
//////            });
//////        }

//////        // ============================================================
//////        // اطلاعات اصلی با کارت‌ها
//////        // ============================================================
//////        col.Item().PaddingVertical(3).Column(c =>
//////        {
//////            c.Item().Text("اطلاعات اصلی")
//////                .FontSize(13)
//////                .Bold()
//////                .FontColor(Colors.Blue.Darken2)
//////                .AlignRight();

//////            c.Item().PaddingTop(3).Row(row =>
//////            {
//////                row.RelativeItem().Column(c2 =>
//////                {
//////                    c2.Item().Background(Colors.Grey.Lighten4)
//////                        .Padding(6)
//////                        .Border(1)
//////                        .BorderColor(Colors.Grey.Lighten2)
//////                        .Column(c3 =>
//////                        {
//////                            c3.Item().Text("نوع ملک")
//////                                .FontSize(9)
//////                                .FontColor(Colors.Grey.Darken2)
//////                                .AlignCenter();
//////                            c3.Item().Text(GetCategoryType(property.CategoryType))
//////                                .FontSize(13)
//////                                .Bold()
//////                                .AlignCenter();
//////                        });
//////                });

//////                row.RelativeItem().Column(c2 =>
//////                {
//////                    c2.Item().Background(Colors.Grey.Lighten4)
//////                        .Padding(6)
//////                        .Border(1)
//////                        .BorderColor(Colors.Grey.Lighten2)
//////                        .Column(c3 =>
//////                        {
//////                            c3.Item().Text("منطقه")
//////                                .FontSize(9)
//////                                .FontColor(Colors.Grey.Darken2)
//////                                .AlignCenter();
//////                            c3.Item().Text(property.RegionName)
//////                                .FontSize(13)
//////                                .Bold()
//////                                .AlignCenter();
//////                        });
//////                });

//////                row.RelativeItem().Column(c2 =>
//////                {
//////                    c2.Item().Background(Colors.Grey.Lighten4)
//////                        .Padding(6)
//////                        .Border(1)
//////                        .BorderColor(Colors.Grey.Lighten2)
//////                        .Column(c3 =>
//////                        {
//////                            c3.Item().Text("سال ساخت")
//////                                .FontSize(9)
//////                                .FontColor(Colors.Grey.Darken2)
//////                                .AlignCenter();
//////                            c3.Item().Text(property.ConstructionYear.ToString())
//////                                .FontSize(13)
//////                                .Bold()
//////                                .AlignCenter();
//////                        });
//////                });

//////                row.RelativeItem().Column(c2 =>
//////                {
//////                    c2.Item().Background(Colors.Grey.Lighten4)
//////                        .Padding(6)
//////                        .Border(1)
//////                        .BorderColor(Colors.Grey.Lighten2)
//////                        .Column(c3 =>
//////                        {
//////                            c3.Item().Text("طبقه")
//////                                .FontSize(9)
//////                                .FontColor(Colors.Grey.Darken2)
//////                                .AlignCenter();
//////                            c3.Item().Text($"{property.Floor} از {property.CountFloor}")
//////                                .FontSize(13)
//////                                .Bold()
//////                                .AlignCenter();
//////                        });
//////                });
//////            });
//////        });

//////        // ============================================================
//////        // عکس
//////        // ============================================================
//////        if (property.Images?.Any() == true)
//////        {
//////            try
//////            {
//////                var imageBytes = LoadImage(property.Images.First());
//////                if (imageBytes != null && imageBytes.Length > 0)
//////                {
//////                    col.Item().PaddingVertical(4)
//////                        .AlignCenter()
//////                        .Height(180)
//////                        .Image(imageBytes)
//////                        .FitArea();
//////                }
//////            }
//////            catch
//////            {
//////                // خطا را نادیده بگیر
//////            }
//////        }

//////        // ============================================================
//////        // امکانات (با کارت‌های رنگی)
//////        // ============================================================
//////        col.Item().PaddingVertical(3).Column(c =>
//////        {
//////            c.Item().Text("امکانات ملک")
//////                .FontSize(13)
//////                .Bold()
//////                .FontColor(Colors.Blue.Darken2)
//////                .AlignRight();

//////            c.Item().PaddingTop(3).Row(row =>
//////            {
//////                row.RelativeItem().Column(c2 =>
//////                {
//////                    AddAmenityCard(c2, "آسانسور", property.IsHasElevator);
//////                    AddAmenityCard(c2, "پارکینگ", property.IsHasParking);
//////                });

//////                row.RelativeItem().Column(c2 =>
//////                {
//////                    AddAmenityCard(c2, "استخر", property.IsHasPool);
//////                    AddAmenityCard(c2, "انباری", property.IsHasStoreRoom);
//////                });
//////            });
//////        });

//////        // ============================================================
//////        // آدرس
//////        // ============================================================
//////        col.Item().PaddingVertical(3).Column(c =>
//////        {
//////            c.Item().Text("آدرس ملک")
//////                .FontSize(13)
//////                .Bold()
//////                .FontColor(Colors.Blue.Darken2)
//////                .AlignRight();

//////            c.Item().PaddingTop(3)
//////                .Background(Colors.Grey.Lighten4)
//////                .Padding(8)
//////                .Border(1)
//////                .BorderColor(Colors.Grey.Lighten2)
//////                .Column(c2 =>
//////                {
//////                    c2.Item().Text(property.Address)
//////                        .FontSize(12)
//////                        .AlignRight();

//////                    c2.Item().PaddingTop(3)
//////                        .Text($"مختصات: {property.lat} , {property.lng}")
//////                        .FontSize(9)
//////                        .FontColor(Colors.Grey.Darken1)
//////                        .AlignRight();
//////                });
//////        });

//////        // ============================================================
//////        // هشدارها
//////        // ============================================================
//////        if (property.Warnings?.Any() == true)
//////        {
//////            col.Item().PaddingVertical(3).Column(c =>
//////            {
//////                c.Item().Text("نکات مهم")
//////                    .FontSize(13)
//////                    .Bold()
//////                    .FontColor(Colors.Red.Darken2)
//////                    .AlignRight();

//////                c.Item().PaddingTop(3)
//////                    .Background(Colors.Red.Lighten5)
//////                    .Padding(8)
//////                    .Border(1)
//////                    .BorderColor(Colors.Red.Lighten2)
//////                    .Column(c2 =>
//////                    {
//////                        foreach (var warning in property.Warnings)
//////                        {
//////                            var cleanWarning = StripHtmlTags(warning);
//////                            c2.Item().PaddingBottom(2).Text($"• {cleanWarning}")
//////                                .FontColor(Colors.Red.Darken2)
//////                                .FontSize(11)
//////                                .AlignRight();
//////                        }
//////                    });
//////            });
//////        }

//////        // ============================================================
//////        // مشاور
//////        // ============================================================
//////        if (property.Agents != null)
//////        {
//////            col.Item().PaddingVertical(3).Column(c =>
//////            {
//////                c.Item().Text("اطلاعات مشاور")
//////                    .FontSize(13)
//////                    .Bold()
//////                    .FontColor(Colors.Blue.Darken2)
//////                    .AlignRight();

//////                c.Item().PaddingTop(3)
//////                    .Background(Colors.Grey.Lighten4)
//////                    .Padding(8)
//////                    .Border(1)
//////                    .BorderColor(Colors.Grey.Lighten2)
//////                    .Row(row =>
//////                    {
//////                        row.RelativeItem().Column(c2 =>
//////                        {
//////                            c2.Item().Text($"نام: {property.Agents.Name ?? "نامشخص"}")
//////                                .FontSize(12)
//////                                .AlignRight();
//////                            c2.Item().Text($"تلفن: {property.Agents.Phone ?? "نامشخص"}")
//////                                .FontSize(12)
//////                                .AlignRight();
//////                        });

//////                        row.RelativeItem().Column(c2 =>
//////                        {
//////                            c2.Item().Text("آدرس دفتر:")
//////                                .FontSize(11)
//////                                .Bold()
//////                                .AlignRight();
//////                            c2.Item().Text(property.Agents.Address ?? "نامشخص")
//////                                .FontSize(12)
//////                                .AlignRight();
//////                        });
//////                    });
//////            });
//////        }
//////    }

//////    private void BuildFooter(IContainer container, RealEstateDetails property)
//////    {
//////        container.BorderTop(1.5f)
//////            .BorderColor(Colors.Blue.Lighten2)
//////            .PaddingTop(6)
//////            .AlignCenter()
//////            .Text(t =>
//////            {
//////                t.Span("تاریخ چاپ: ");
//////                t.Span($"{DateTime.Now:yyyy/MM/dd HH:mm}");

//////                t.Span("    ●    ");

//////                t.Span("کد ملک: ");
//////                t.Span($"#{property.Id:D4}");

//////                t.Span("    ●    ");

//////                t.Span("صفحه ");
//////                t.Span("1");
//////                t.Span(" از ");
//////                t.Span("1");
//////            });
//////    }

//////    private void AddAmenityCard(ColumnDescriptor col, string name, bool has)
//////    {
//////        var icon = has ? "✓" : "✗";
//////        var color = has ? Colors.Green.Darken2 : Colors.Red.Darken2;
//////        var bgColor = has ? Colors.Green.Lighten5 : Colors.Red.Lighten5;
//////        var borderColor = has ? Colors.Green.Lighten2 : Colors.Red.Lighten2;

//////        col.Item().Padding(3)
//////            .Background(bgColor)
//////            .Padding(6)
//////            .Border(1)
//////            .BorderColor(borderColor)
//////            .Text($"{icon} {name}")
//////            .FontColor(color)
//////            .FontSize(12)
//////            .Bold()
//////            .AlignCenter();
//////    }

//////    private string GetCategoryType(int type) => type switch
//////    {
//////        1 => "فروش",
//////        2 => "رهن",
//////        3 => "اجاره",
//////        _ => "نامشخص"
//////    };

//////    private string StripHtmlTags(string html)
//////    {
//////        if (string.IsNullOrEmpty(html))
//////            return html;

//////        var clean = Regex.Replace(html, @"<[^>]*>", string.Empty);
//////        clean = clean.Replace("&nbsp;", " ");
//////        clean = clean.Replace("&amp;", "&");
//////        clean = clean.Replace("&lt;", "<");
//////        clean = clean.Replace("&gt;", ">");
//////        clean = clean.Replace("&quot;", "\"");
//////        clean = Regex.Replace(clean, @"\s+", " ");

//////        return clean.Trim();
//////    }

//////    private byte[] GenerateQrCode(int id)
//////    {
//////        try
//////        {
//////            using var gen = new QRCodeGenerator();
//////            var data = gen.CreateQrCode($"PropertyId:{id}", QRCodeGenerator.ECCLevel.Q);
//////            return new PngByteQRCode(data).GetGraphic(20);
//////        }
//////        catch
//////        {
//////            return Array.Empty<byte>();
//////        }
//////    }

//////    private byte[] LoadImage(string url)
//////    {
//////        try
//////        {
//////            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
//////            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
//////            return client.GetByteArrayAsync(url).GetAwaiter().GetResult();
//////        }
//////        catch
//////        {
//////            return Array.Empty<byte>();
//////        }
//////    }
//////}

////using JWTApi.Domain.Dtos.RealEstate;
////using QRCoder;
////using QuestPDF;
////using QuestPDF.Fluent;
////using QuestPDF.Helpers;
////using QuestPDF.Infrastructure;
////using System.Text.RegularExpressions;

////namespace JWTApi.Services.Pdf;

////public interface IPdfGeneratorService
////{
////    Task<byte[]> GeneratePropertyPdf(RealEstateDetails property, CancellationToken cancellationToken);
////}

////public class PdfGeneratorService : IPdfGeneratorService
////{
////    public PdfGeneratorService()
////    {
////        Settings.License = LicenseType.Community;
////        Settings.CheckIfAllTextGlyphsAreAvailable = false;
////    }

////    public Task<byte[]> GeneratePropertyPdf(RealEstateDetails property, CancellationToken cancellationToken)
////    {
////        return Task.FromResult(GeneratePdf(property));
////    }

////    private byte[] GeneratePdf(RealEstateDetails property)
////    {
////        return Document.Create(container =>
////        {
////            container.Page(page =>
////            {
////                page.Size(PageSizes.A4);
////                page.Margin(1.5f, Unit.Centimetre);
////                page.PageColor(Colors.White);
////                page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black));

////                page.Header().Element(x => BuildHeader(x, property));
////                page.Content().Element(x => BuildContentWithWatermark(x, property));
////                page.Footer().Element(x => BuildFooter(x, property));
////            });
////        }).GeneratePdf();
////    }

////    private void BuildContentWithWatermark(IContainer container, RealEstateDetails property)
////    {
////        container.Layers(layers =>
////        {
////            // لایه اصلی (محتوا)
////            layers.PrimaryLayer()
////                .PaddingVertical(0.2f, Unit.Centimetre)
////                .Column(col =>
////                {
////                    BuildMainContent(col, property);
////                });

////            // لایه واترمارک
////            layers.Layer()
////                .AlignCenter()
////                .AlignMiddle()
////                .Rotate(-30)
////                .Text("مشاور املاک")
////                .FontSize(60)
////                .Bold()
////                .FontColor(Colors.Grey.Lighten5)
////                .Light();
////        });
////    }

////    private void BuildHeader(IContainer container, RealEstateDetails property)
////    {
////        container.Row(row =>
////        {
////            row.RelativeItem(3).Column(col =>
////            {
////                col.Item().Text("مشاور املاک")
////                    .FontSize(20)
////                    .Bold()
////                    .FontColor(Colors.Blue.Darken2)
////                    .AlignRight();

////                col.Item().Text("گزارش کامل مشخصات ملک")
////                    .FontSize(13)
////                    .FontColor(Colors.Grey.Darken1)
////                    .AlignRight();

////                col.Item().PaddingTop(4).LineHorizontal(2).LineColor(Colors.Blue.Lighten2);
////            });

////            row.RelativeItem(1).Column(col =>
////            {
////                col.Item().AlignCenter().Image(GenerateQrCode(property.Id)).FitArea();
////            });
////        });
////    }

////    private void BuildMainContent(ColumnDescriptor col, RealEstateDetails property)
////    {
////        // ============================================================
////        // کد ملک
////        // ============================================================
////        col.Item().AlignCenter().Text($"شماره ملک: {property.Id:D4}")
////            .FontSize(12)
////            .FontColor(Colors.Grey.Darken2)
////            .Bold();

////        // ============================================================
////        // عنوان و قیمت
////        // ============================================================
////        col.Item().PaddingVertical(3).Column(c =>
////        {
////            c.Item().AlignCenter().Text(property.Title ?? "بدون عنوان")
////                .FontSize(20)
////                .Bold()
////                .FontColor(Colors.Blue.Darken2);

////            c.Item().AlignCenter().Text($"{property.Price:N0} تومان")
////                .FontSize(24)
////                .Bold()
////                .FontColor(Colors.Green.Darken2);

////            c.Item().AlignCenter().Text($"متراژ: {property.SquareMeter} متر مربع  ●  قیمت هر متر: {property.PriceMeter:N0} تومان")
////                .FontSize(11)
////                .FontColor(Colors.Grey.Darken1);
////        });

////        // ============================================================
////        // توضیحات ملک
////        // ============================================================
////        if (!string.IsNullOrEmpty(property.DescriptionRows))
////        {
////            var cleanDescription = StripHtmlTags(property.DescriptionRows);

////            col.Item().PaddingVertical(3).Column(c =>
////            {
////                c.Item().Text("توضیحات")
////                    .FontSize(13)
////                    .Bold()
////                    .FontColor(Colors.Blue.Darken2)
////                    .AlignRight();

////                c.Item().PaddingTop(3)
////                    .Background(Colors.Grey.Lighten4)
////                    .Padding(8)
////                    .Border(1)
////                    .BorderColor(Colors.Grey.Lighten2)
////                    .Text(cleanDescription)
////                    .FontSize(11)
////                    .AlignRight();
////            });
////        }

////        // ============================================================
////        // اطلاعات اصلی با کارت‌ها
////        // ============================================================
////        col.Item().PaddingVertical(3).Column(c =>
////        {
////            c.Item().Text("اطلاعات اصلی")
////                .FontSize(13)
////                .Bold()
////                .FontColor(Colors.Blue.Darken2)
////                .AlignRight();

////            c.Item().PaddingTop(3).Row(row =>
////            {
////                // نوع ملک
////                row.RelativeItem().Column(c2 =>
////                {
////                    c2.Item().Background(Colors.Grey.Lighten4)
////                        .Padding(6)
////                        .Border(1)
////                        .BorderColor(Colors.Grey.Lighten2)
////                        .Column(c3 =>
////                        {
////                            c3.Item().Text("نوع ملک")
////                                .FontSize(9)
////                                .FontColor(Colors.Grey.Darken2)
////                                .AlignCenter();
////                            c3.Item().Text(GetCategoryType(property.CategoryType))
////                                .FontSize(13)
////                                .Bold()
////                                .AlignCenter();
////                        });
////                });

////                // منطقه
////                row.RelativeItem().Column(c2 =>
////                {
////                    c2.Item().Background(Colors.Grey.Lighten4)
////                        .Padding(6)
////                        .Border(1)
////                        .BorderColor(Colors.Grey.Lighten2)
////                        .Column(c3 =>
////                        {
////                            c3.Item().Text("منطقه")
////                                .FontSize(9)
////                                .FontColor(Colors.Grey.Darken2)
////                                .AlignCenter();
////                            c3.Item().Text(property.RegionName ?? "نامشخص")
////                                .FontSize(13)
////                                .Bold()
////                                .AlignCenter();
////                        });
////                });

////                // سال ساخت
////                row.RelativeItem().Column(c2 =>
////                {
////                    c2.Item().Background(Colors.Grey.Lighten4)
////                        .Padding(6)
////                        .Border(1)
////                        .BorderColor(Colors.Grey.Lighten2)
////                        .Column(c3 =>
////                        {
////                            c3.Item().Text("سال ساخت")
////                                .FontSize(9)
////                                .FontColor(Colors.Grey.Darken2)
////                                .AlignCenter();
////                            c3.Item().Text(property.ConstructionYear > 0 ? property.ConstructionYear.ToString() : "نامشخص")
////                                .FontSize(13)
////                                .Bold()
////                                .AlignCenter();
////                        });
////                });

////                // طبقه
////                row.RelativeItem().Column(c2 =>
////                {
////                    c2.Item().Background(Colors.Grey.Lighten4)
////                        .Padding(6)
////                        .Border(1)
////                        .BorderColor(Colors.Grey.Lighten2)
////                        .Column(c3 =>
////                        {
////                            c3.Item().Text("طبقه")
////                                .FontSize(9)
////                                .FontColor(Colors.Grey.Darken2)
////                                .AlignCenter();
////                            c3.Item().Text($"{property.Floor} از {property.CountFloor}")
////                                .FontSize(13)
////                                .Bold()
////                                .AlignCenter();
////                        });
////                });
////            });
////        });

////        // ============================================================
////        // عکس
////        // ============================================================
////        if (property.Images?.Any() == true)
////        {
////            try
////            {
////                var imageBytes = LoadImage(property.Images.First());
////                if (imageBytes != null && imageBytes.Length > 0)
////                {
////                    col.Item().PaddingVertical(4)
////                        .AlignCenter()
////                        .Height(180)
////                        .Image(imageBytes)
////                        .FitArea();
////                }
////            }
////            catch
////            {
////                // خطا را نادیده بگیر
////            }
////        }

////        // ============================================================
////        // امکانات (با کارت‌های رنگی و آیکون)
////        // ============================================================
////        col.Item().PaddingVertical(3).Column(c =>
////        {
////            c.Item().Text("امکانات ملک")
////                .FontSize(13)
////                .Bold()
////                .FontColor(Colors.Blue.Darken2)
////                .AlignRight();

////            c.Item().PaddingTop(3).Row(row =>
////            {
////                row.RelativeItem().Column(c2 =>
////                {
////                    AddAmenityCard(c2, "آسانسور", property.IsHasElevator);
////                    AddAmenityCard(c2, "پارکینگ", property.IsHasParking);
////                });

////                row.RelativeItem().Column(c2 =>
////                {
////                    AddAmenityCard(c2, "استخر", property.IsHasPool);
////                    AddAmenityCard(c2, "انباری", property.IsHasStoreRoom);
////                });
////            });
////        });

////        // ============================================================
////        // آدرس
////        // ============================================================
////        col.Item().PaddingVertical(3).Column(c =>
////        {
////            c.Item().Text("آدرس ملک")
////                .FontSize(13)
////                .Bold()
////                .FontColor(Colors.Blue.Darken2)
////                .AlignRight();

////            c.Item().PaddingTop(3)
////                .Background(Colors.Grey.Lighten4)
////                .Padding(8)
////                .Border(1)
////                .BorderColor(Colors.Grey.Lighten2)
////                .Column(c2 =>
////                {
////                    c2.Item().Text(property.Address ?? "آدرسی ثبت نشده")
////                        .FontSize(12)
////                        .AlignRight();

////                    c2.Item().PaddingTop(3)
////                        .Text($"مختصات: {property.lat} , {property.lng}")
////                        .FontSize(9)
////                        .FontColor(Colors.Grey.Darken1)
////                        .AlignRight();
////                });
////        });

////        // ============================================================
////        // هشدارها
////        // ============================================================
////        if (property.Warnings?.Any() == true)
////        {
////            col.Item().PaddingVertical(3).Column(c =>
////            {
////                c.Item().Text("نکات مهم")
////                    .FontSize(13)
////                    .Bold()
////                    .FontColor(Colors.Red.Darken2)
////                    .AlignRight();

////                c.Item().PaddingTop(3)
////                    .Background(Colors.Red.Lighten5)
////                    .Padding(8)
////                    .Border(1)
////                    .BorderColor(Colors.Red.Lighten2)
////                    .Column(c2 =>
////                    {
////                        foreach (var warning in property.Warnings)
////                        {
////                            var cleanWarning = StripHtmlTags(warning);
////                            c2.Item().PaddingBottom(2).Text($"• {cleanWarning}")
////                                .FontColor(Colors.Red.Darken2)
////                                .FontSize(11)
////                                .AlignRight();
////                        }
////                    });
////            });
////        }

////        // ============================================================
////        // مشاور
////        // ============================================================
////        if (property.Agents != null)
////        {
////            col.Item().PaddingVertical(3).Column(c =>
////            {
////                c.Item().Text("اطلاعات مشاور")
////                    .FontSize(13)
////                    .Bold()
////                    .FontColor(Colors.Blue.Darken2)
////                    .AlignRight();

////                c.Item().PaddingTop(3)
////                    .Background(Colors.Grey.Lighten4)
////                    .Padding(8)
////                    .Border(1)
////                    .BorderColor(Colors.Grey.Lighten2)
////                    .Row(row =>
////                    {
////                        row.RelativeItem().Column(c2 =>
////                        {
////                            c2.Item().Text($"نام: {property.Agents.Name ?? "نامشخص"}")
////                                .FontSize(12)
////                                .AlignRight();
////                            c2.Item().Text($"تلفن: {property.Agents.Phone ?? "نامشخص"}")
////                                .FontSize(12)
////                                .AlignRight();
////                        });

////                        row.RelativeItem().Column(c2 =>
////                        {
////                            c2.Item().Text("آدرس دفتر:")
////                                .FontSize(11)
////                                .Bold()
////                                .AlignRight();
////                            c2.Item().Text(property.Agents.Address ?? "نامشخص")
////                                .FontSize(12)
////                                .AlignRight();
////                        });
////                    });
////            });
////        }
////    }

////    private void BuildFooter(IContainer container, RealEstateDetails property)
////    {
////        container.BorderTop(1.5f)
////            .BorderColor(Colors.Blue.Lighten2)
////            .PaddingTop(6)
////            .AlignCenter()
////            .Text(t =>
////            {
////                t.Span("تاریخ چاپ: ");
////                t.Span($"{DateTime.Now:yyyy/MM/dd HH:mm}");

////                t.Span("    ●    ");

////                t.Span("کد ملک: ");
////                t.Span($"#{property.Id:D4}");

////                t.Span("    ●    ");

////                t.Span("صفحه ");
////                t.Span("1");
////                t.Span(" از ");
////                t.Span("1");
////            });
////    }

////    private void AddAmenityCard(ColumnDescriptor col, string name, bool has)
////    {
////        // استفاده از آیکون‌های ساده
////        var icon = has ? "✓" : "✗";
////        var color = has ? Colors.Green.Darken2 : Colors.Red.Darken2;
////        var bgColor = has ? Colors.Green.Lighten5 : Colors.Red.Lighten5;
////        var borderColor = has ? Colors.Green.Lighten2 : Colors.Red.Lighten2;

////        col.Item().Padding(3)
////            .Background(bgColor)
////            .Padding(6)
////            .Border(1)
////            .BorderColor(borderColor)
////            .Text($"{icon} {name}")
////            .FontColor(color)
////            .FontSize(12)
////            .Bold()
////            .AlignCenter();
////    }

////    private string GetCategoryType(int type) => type switch
////    {
////        1 => "فروش",
////        2 => "رهن",
////        3 => "اجاره",
////        _ => "نامشخص"
////    };

////    private string StripHtmlTags(string html)
////    {
////        if (string.IsNullOrEmpty(html))
////            return html;

////        var clean = Regex.Replace(html, @"<[^>]*>", string.Empty);
////        clean = clean.Replace("&nbsp;", " ");
////        clean = clean.Replace("&amp;", "&");
////        clean = clean.Replace("&lt;", "<");
////        clean = clean.Replace("&gt;", ">");
////        clean = clean.Replace("&quot;", "\"");
////        clean = Regex.Replace(clean, @"\s+", " ");

////        return clean.Trim();
////    }

////    private byte[] GenerateQrCode(int id)
////    {
////        try
////        {
////            using var gen = new QRCodeGenerator();
////            var data = gen.CreateQrCode($"PropertyId:{id}", QRCodeGenerator.ECCLevel.Q);
////            return new PngByteQRCode(data).GetGraphic(20);
////        }
////        catch
////        {
////            return Array.Empty<byte>();
////        }
////    }

////    private byte[] LoadImage(string url)
////    {
////        try
////        {
////            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
////            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
////            return client.GetByteArrayAsync(url).GetAwaiter().GetResult();
////        }
////        catch
////        {
////            return Array.Empty<byte>();
////        }
////    }
////}

//using JWTApi.Domain.Dtos.RealEstate;
//using QRCoder;
//using QuestPDF;
//using QuestPDF.Fluent;
//using QuestPDF.Helpers;
//using QuestPDF.Infrastructure;
//using System.Text.RegularExpressions;

//namespace JWTApi.Services.Pdf;

//public interface IPdfGeneratorService
//{
//    Task<byte[]> GeneratePropertyPdf(RealEstateDetails property, CancellationToken cancellationToken);
//}

//public class PdfGeneratorService : IPdfGeneratorService
//{
//    public PdfGeneratorService()
//    {
//        Settings.License = LicenseType.Community;
//        Settings.CheckIfAllTextGlyphsAreAvailable = false;
//    }

//    public Task<byte[]> GeneratePropertyPdf(RealEstateDetails property, CancellationToken cancellationToken)
//    {
//        return Task.FromResult(GeneratePdf(property));
//    }

//    private byte[] GeneratePdf(RealEstateDetails property)
//    {
//        return Document.Create(container =>
//        {
//            container.Page(page =>
//            {
//                page.Size(PageSizes.A4);
//                page.Margin(1.5f, Unit.Centimetre);
//                page.PageColor(Colors.White);
//                page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black));

//                page.Header().Element(x => BuildHeader(x, property));
//                page.Content().Element(x => BuildContentWithWatermark(x, property));
//                page.Footer().Element(x => BuildFooter(x, property));
//            });
//        }).GeneratePdf();
//    }

//    private void BuildContentWithWatermark(IContainer container, RealEstateDetails property)
//    {
//        container.Layers(layers =>
//        {
//            layers.PrimaryLayer()
//                .PaddingVertical(0.2f, Unit.Centimetre)
//                .Column(col =>
//                {
//                    BuildMainContent(col, property);
//                });

//            layers.Layer()
//                .AlignCenter()
//                .AlignMiddle()
//                .Rotate(-30)
//                .Text("مشاور املاک")
//                .FontSize(60)
//                .Bold()
//                .FontColor(Colors.Grey.Lighten4)
//                .Light();
//        });
//    }

//    private void BuildHeader(IContainer container, RealEstateDetails property)
//    {
//        container.Row(row =>
//        {
//            row.RelativeItem(3).Column(col =>
//            {
//                col.Item().Text("مشاور املاک")
//                    .FontSize(20)
//                    .Bold()
//                    .FontColor(Colors.Blue.Darken2)
//                    .AlignRight();

//                col.Item().Text("گزارش کامل مشخصات ملک")
//                    .FontSize(13)
//                    .FontColor(Colors.Grey.Darken1)
//                    .AlignRight();

//                col.Item().PaddingTop(4).LineHorizontal(2).LineColor(Colors.Blue.Lighten2);
//            });

//            row.RelativeItem(1).Column(col =>
//            {
//                col.Item().AlignCenter().Image(GenerateQrCode(property.Id)).FitArea();
//            });
//        });
//    }

//    private void BuildMainContent(ColumnDescriptor col, RealEstateDetails property)
//    {
//        // ============================================================
//        // کد ملک
//        // ============================================================
//        col.Item().AlignCenter().Text($"شماره ملک: {property.Id:D4}")
//            .FontSize(12)
//            .FontColor(Colors.Grey.Darken2)
//            .Bold();

//        // ============================================================
//        // عنوان و قیمت
//        // ============================================================
//        col.Item().PaddingVertical(3).Column(c =>
//        {
//            c.Item().AlignCenter().Text(property.Title ?? "بدون عنوان")
//                .FontSize(20)
//                .Bold()
//                .FontColor(Colors.Blue.Darken2);

//            c.Item().AlignCenter().Text($"{property.Price:N0} تومان")
//                .FontSize(24)
//                .Bold()
//                .FontColor(Colors.Green.Darken2);

//            c.Item().AlignCenter().Text($"متراژ: {property.SquareMeter} متر مربع  ●  قیمت هر متر: {property.PriceMeter:N0} تومان")
//                .FontSize(11)
//                .FontColor(Colors.Grey.Darken1);
//        });

//        // ============================================================
//        // توضیحات ملک
//        // ============================================================
//        if (!string.IsNullOrEmpty(property.DescriptionRows))
//        {
//            var cleanDescription = StripHtmlTags(property.DescriptionRows);

//            col.Item().PaddingVertical(3).Column(c =>
//            {
//                c.Item().Text("توضیحات")
//                    .FontSize(13)
//                    .Bold()
//                    .FontColor(Colors.Blue.Darken2)
//                    .AlignRight();

//                c.Item().PaddingTop(3)
//                    .Background(Colors.Grey.Lighten4)
//                    .Padding(8)
//                    .Border(1)
//                    .BorderColor(Colors.Grey.Lighten2)
//                    .Text(cleanDescription)
//                    .FontSize(11)
//                    .AlignRight();
//            });
//        }

//        // ============================================================
//        // اطلاعات اصلی با کارت‌ها
//        // ============================================================
//        col.Item().PaddingVertical(3).Column(c =>
//        {
//            c.Item().Text("اطلاعات اصلی")
//                .FontSize(13)
//                .Bold()
//                .FontColor(Colors.Blue.Darken2)
//                .AlignRight();

//            c.Item().PaddingTop(3).Row(row =>
//            {
//                row.RelativeItem().Column(c2 =>
//                {
//                    c2.Item().Background(Colors.Grey.Lighten4)
//                        .Padding(6)
//                        .Border(1)
//                        .BorderColor(Colors.Grey.Lighten2)
//                        .Column(c3 =>
//                        {
//                            c3.Item().Text("نوع ملک")
//                                .FontSize(9)
//                                .FontColor(Colors.Grey.Darken2)
//                                .AlignCenter();
//                            c3.Item().Text(GetCategoryType(property.CategoryType))
//                                .FontSize(13)
//                                .Bold()
//                                .AlignCenter();
//                        });
//                });

//                row.RelativeItem().Column(c2 =>
//                {
//                    c2.Item().Background(Colors.Grey.Lighten4)
//                        .Padding(6)
//                        .Border(1)
//                        .BorderColor(Colors.Grey.Lighten2)
//                        .Column(c3 =>
//                        {
//                            c3.Item().Text("منطقه")
//                                .FontSize(9)
//                                .FontColor(Colors.Grey.Darken2)
//                                .AlignCenter();
//                            c3.Item().Text(property.RegionName ?? "نامشخص")
//                                .FontSize(13)
//                                .Bold()
//                                .AlignCenter();
//                        });
//                });

//                row.RelativeItem().Column(c2 =>
//                {
//                    c2.Item().Background(Colors.Grey.Lighten4)
//                        .Padding(6)
//                        .Border(1)
//                        .BorderColor(Colors.Grey.Lighten2)
//                        .Column(c3 =>
//                        {
//                            c3.Item().Text("سال ساخت")
//                                .FontSize(9)
//                                .FontColor(Colors.Grey.Darken2)
//                                .AlignCenter();
//                            c3.Item().Text(property.ConstructionYear > 0 ? property.ConstructionYear.ToString() : "نامشخص")
//                                .FontSize(13)
//                                .Bold()
//                                .AlignCenter();
//                        });
//                });

//                row.RelativeItem().Column(c2 =>
//                {
//                    c2.Item().Background(Colors.Grey.Lighten4)
//                        .Padding(6)
//                        .Border(1)
//                        .BorderColor(Colors.Grey.Lighten2)
//                        .Column(c3 =>
//                        {
//                            c3.Item().Text("طبقه")
//                                .FontSize(9)
//                                .FontColor(Colors.Grey.Darken2)
//                                .AlignCenter();
//                            c3.Item().Text($"{property.Floor} از {property.CountFloor}")
//                                .FontSize(13)
//                                .Bold()
//                                .AlignCenter();
//                        });
//                });
//            });
//        });

//        // ============================================================
//        // عکس
//        // ============================================================
//        if (property.Images?.Any() == true)
//        {
//            try
//            {
//                var imageBytes = LoadImage(property.Images.First());
//                if (imageBytes != null && imageBytes.Length > 0)
//                {
//                    col.Item().PaddingVertical(4)
//                        .AlignCenter()
//                        .Height(180)
//                        .Image(imageBytes)
//                        .FitArea();
//                }
//            }
//            catch
//            {
//                // خطا را نادیده بگیر
//            }
//        }

//        // ============================================================
//        // امکانات (از پراپرتی Facilities)
//        // ============================================================
//        col.Item().PaddingVertical(3).Column(c =>
//        {
//            c.Item().Text("امکانات ملک")
//                .FontSize(13)
//                .Bold()
//                .FontColor(Colors.Blue.Darken2)
//                .AlignRight();

//            // امکانات اصلی (Boolean)
//            c.Item().PaddingTop(3).Row(row =>
//            {
//                row.RelativeItem().Column(c2 =>
//                {
//                    AddAmenityCard(c2, "آسانسور", property.IsHasElevator);
//                    AddAmenityCard(c2, "پارکینگ", property.IsHasParking);
//                });

//                row.RelativeItem().Column(c2 =>
//                {
//                    AddAmenityCard(c2, "استخر", property.IsHasPool);
//                    AddAmenityCard(c2, "انباری", property.IsHasStoreRoom);
//                });
//            });

//            // ============================================================
//            // Facilities اضافی (از لیست Facilities)
//            // ============================================================
//            if (property.Facilities != null && property.Facilities.Any())
//            {
//                c.Item().Text("ویژگی ملک")
//             .FontSize(13)
//             .Bold()
//             .FontColor(Colors.Blue.Darken2)
//             .AlignRight();
//                c.Item().PaddingTop(4).Row(row =>
//                {
//                    var facilities = property.Facilities.ToList();
//                    var mid = (int)Math.Ceiling(facilities.Count / 2.0);

//                    // ستون اول
//                    row.RelativeItem().Column(c2 =>
//                    {
//                        for (int i = 0; i < mid && i < facilities.Count; i++)
//                        {
//                            AddFacilityItem(c2, facilities[i]);
//                        }
//                    });

//                    // ستون دوم
//                    row.RelativeItem().Column(c2 =>
//                    {
//                        for (int i = mid; i < facilities.Count; i++)
//                        {
//                            AddFacilityItem(c2, facilities[i]);
//                        }
//                    });
//                });
//            }
//        });

//        // ============================================================
//        // آدرس
//        // ============================================================
//        col.Item().PaddingVertical(3).Column(c =>
//        {
//            c.Item().Text("آدرس ملک")
//                .FontSize(13)
//                .Bold()
//                .FontColor(Colors.Blue.Darken2)
//                .AlignRight();

//            c.Item().PaddingTop(3)
//                .Background(Colors.Grey.Lighten4)
//                .Padding(8)
//                .Border(1)
//                .BorderColor(Colors.Grey.Lighten2)
//                .Column(c2 =>
//                {
//                    c2.Item().Text(property.Address ?? "آدرسی ثبت نشده")
//                        .FontSize(12)
//                        .AlignRight();

//                    c2.Item().PaddingTop(3)
//                        .Text($"مختصات: {property.lat} , {property.lng}")
//                        .FontSize(9)
//                        .FontColor(Colors.Grey.Darken1)
//                        .AlignRight();
//                });
//        });

//        // ============================================================
//        // هشدارها
//        // ============================================================
//        //if (property.Warnings?.Any() == true)
//        //{
//        //    col.Item().PaddingVertical(3).Column(c =>
//        //    {
//        //        c.Item().Text("نکات مهم")
//        //            .FontSize(13)
//        //            .Bold()
//        //            .FontColor(Colors.Red.Darken2)
//        //            .AlignRight();

//        //        c.Item().PaddingTop(3)
//        //            .Background(Colors.Red.Lighten5)
//        //            .Padding(8)
//        //            .Border(1)
//        //            .BorderColor(Colors.Red.Lighten2)
//        //            .Column(c2 =>
//        //            {
//        //                foreach (var warning in property.Warnings)
//        //                {
//        //                    var cleanWarning = StripHtmlTags(warning);
//        //                    c2.Item().PaddingBottom(2).Text($"• {cleanWarning}")
//        //                        .FontColor(Colors.Red.Darken2)
//        //                        .FontSize(11)
//        //                        .AlignRight();
//        //                }
//        //            });
//        //    });
//        //}

//        // ============================================================
//        // مشاور
//        // ============================================================
//        if (property.Agents != null)
//        {
//            col.Item().PaddingVertical(3).Column(c =>
//            {
//                c.Item().Text("اطلاعات مشاور")
//                    .FontSize(13)
//                    .Bold()
//                    .FontColor(Colors.Blue.Darken2)
//                    .AlignRight();

//                c.Item().PaddingTop(3)
//                    .Background(Colors.Grey.Lighten4)
//                    .Padding(8)
//                    .Border(1)
//                    .BorderColor(Colors.Grey.Lighten2)
//                    .Row(row =>
//                    {
//                        row.RelativeItem().Column(c2 =>
//                        {
//                            c2.Item().Text($"نام: {property.Agents.Name ?? "نامشخص"}")
//                                .FontSize(12)
//                                .AlignRight();
//                            c2.Item().Text($"تلفن: {property.Agents.Phone ?? "نامشخص"}")
//                                .FontSize(12)
//                                .AlignRight();
//                        });

//                        row.RelativeItem().Column(c2 =>
//                        {
//                            c2.Item().Text("آدرس دفتر:")
//                                .FontSize(11)
//                                .Bold()
//                                .AlignRight();
//                            c2.Item().Text(property.Agents.Address ?? "نامشخص")
//                                .FontSize(12)
//                                .AlignRight();
//                        });
//                    });
//            });
//        }
//    }

//    private void BuildFooter(IContainer container, RealEstateDetails property)
//    {
//        container.BorderTop(1.5f)
//            .BorderColor(Colors.Blue.Lighten2)
//            .PaddingTop(6)
//            .AlignCenter()
//            .Text(t =>
//            {
//                t.Span("تاریخ چاپ: ");
//                t.Span($"{DateTime.Now:yyyy/MM/dd HH:mm}");

//                t.Span("    ●    ");

//                t.Span("کد ملک: ");
//                t.Span($"#{property.Id:D4}");

//                t.Span("    ●    ");

//                t.Span("صفحه ");
//                t.Span("1");
//                t.Span(" از ");
//                t.Span("1");
//            });
//    }

//    private void AddAmenityCard(ColumnDescriptor col, string name, bool has)
//    {
//        var icon = has ? "✓" : "✗";
//        var color = has ? Colors.Green.Darken2 : Colors.Red.Darken2;
//        var bgColor = has ? Colors.Green.Lighten5 : Colors.Red.Lighten5;
//        var borderColor = has ? Colors.Green.Lighten2 : Colors.Red.Lighten2;

//        col.Item().Padding(3)
//            .Background(bgColor)
//            .Padding(6)
//            .Border(1)
//            .BorderColor(borderColor)
//            .Text($"{icon} {name}")
//            .FontColor(color)
//            .FontSize(12)
//            .Bold()
//            .AlignCenter();
//    }

//    private void AddFacilityItem(ColumnDescriptor col, string facilityName)
//    {
//        col.Item().Padding(3)
//            .Background(Colors.Blue.Lighten5)
//            .Padding(6)
//            .Border(1)
//            .BorderColor(Colors.Blue.Lighten2)
//            .Text($"• {facilityName}")
//            .FontColor(Colors.Blue.Darken2)
//            .FontSize(11)
//            .AlignRight();
//    }

//    private string GetCategoryType(int type) => type switch
//    {
//        1 => "فروش",
//        2 => "رهن",
//        3 => "اجاره",
//        _ => "نامشخص"
//    };

//    private string StripHtmlTags(string html)
//    {
//        if (string.IsNullOrEmpty(html))
//            return html;

//        var clean = Regex.Replace(html, @"<[^>]*>", string.Empty);
//        clean = clean.Replace("&nbsp;", " ");
//        clean = clean.Replace("&amp;", "&");
//        clean = clean.Replace("&lt;", "<");
//        clean = clean.Replace("&gt;", ">");
//        clean = clean.Replace("&quot;", "\"");
//        clean = Regex.Replace(clean, @"\s+", " ");

//        return clean.Trim();
//    }

//    private byte[] GenerateQrCode(int id)
//    {
//        try
//        {
//            using var gen = new QRCodeGenerator();
//            var data = gen.CreateQrCode($"PropertyId:{id}", QRCodeGenerator.ECCLevel.Q);
//            return new PngByteQRCode(data).GetGraphic(20);
//        }
//        catch
//        {
//            return Array.Empty<byte>();
//        }
//    }

//    private byte[] LoadImage(string url)
//    {
//        try
//        {
//            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
//            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
//            return client.GetByteArrayAsync(url).GetAwaiter().GetResult();
//        }
//        catch
//        {
//            return Array.Empty<byte>();
//        }
//    }
//}

using JWTApi.Domain.Dtos.RealEstate;
using QRCoder;
using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text.RegularExpressions;

namespace JWTApi.Services.Pdf;

public interface IPdfGeneratorService
{
    Task<byte[]> GeneratePropertyPdf(RealEstateDetails property, CancellationToken cancellationToken);
}

public class PdfGeneratorService : IPdfGeneratorService
{
    public PdfGeneratorService()
    {
        Settings.License = LicenseType.Community;
        Settings.CheckIfAllTextGlyphsAreAvailable = false;
    }

    public Task<byte[]> GeneratePropertyPdf(RealEstateDetails property, CancellationToken cancellationToken)
    {
        return Task.FromResult(GeneratePdf(property));
    }

    private byte[] GeneratePdf(RealEstateDetails property)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black));

                page.Header().Element(x => BuildHeader(x, property));
                page.Content().Element(x => BuildContentWithWatermark(x, property));
                page.Footer().Element(x => BuildFooter(x, property));
            });
        }).GeneratePdf();
    }

    private void BuildContentWithWatermark(IContainer container, RealEstateDetails property)
    {
        container.Layers(layers =>
        {
            layers.PrimaryLayer()
                .PaddingVertical(0.2f, Unit.Centimetre)
                .Column(col =>
                {
                    BuildMainContent(col, property);
                });

            layers.Layer()
                .AlignCenter()
                .AlignMiddle()
                .Rotate(-30)
               .Text("سایت تخصصی املاک خونه یاب")
                .FontSize(60)
                .Bold()
                .FontColor(Colors.Brown.Lighten4)
                .Light();
        });
    }

    private void BuildHeader(IContainer container, RealEstateDetails property)
    {
        container.Row(row =>
        {
            row.RelativeItem(3).Column(col =>
            {
                col.Item().Text("سایت تخصصی املاک خونه یاب")
                    .FontSize(20)
                    .Bold()
                    .FontColor(Colors.Blue.Darken2)
                    .AlignRight();

                col.Item().Text("گزارش کامل مشخصات ملک")
                    .FontSize(13)
                    .FontColor(Colors.Grey.Darken1)
                    .AlignRight();

                col.Item().PaddingTop(4).LineHorizontal(2).LineColor(Colors.Blue.Lighten2);
            });

            row.RelativeItem(1).Column(col =>
            {
                col.Item().AlignCenter().Image(GenerateQrCode(property.Id,property.Title)).FitArea();
            });
        });
    }

    private void BuildMainContent(ColumnDescriptor col, RealEstateDetails property)
    {
        // ============================================================
        // کد ملک
        // ============================================================
        col.Item().AlignCenter().Text($"شماره ملک: {property.Id:D4}")
            .FontSize(12)
            .FontColor(Colors.Grey.Darken2)
            .Bold();

        // ============================================================
        // عنوان و قیمت
        // ============================================================
        col.Item().PaddingVertical(3).Column(c =>
        {
            c.Item().AlignCenter().Text(property.Title ?? "بدون عنوان")
                .FontSize(20)
                .Bold()
                .FontColor(Colors.Blue.Darken2);

            c.Item().AlignCenter().Text($"{property.Price:N0} تومان")
                .FontSize(24)
                .Bold()
                .FontColor(Colors.Green.Darken2);

            c.Item().AlignCenter().Text($"متراژ: {property.SquareMeter} متر مربع  ●  قیمت هر متر: {property.PriceMeter:N0} تومان")
                .FontSize(11)
                .FontColor(Colors.Grey.Darken1);
        });



        // ============================================================
        // اطلاعات اصلی با کارت‌ها
        // ============================================================
        col.Item().PaddingVertical(3).Column(c =>
        {
            c.Item().Text("اطلاعات اصلی")
                .FontSize(13)
                .Bold()
                .FontColor(Colors.Blue.Darken2)
                .AlignRight();

            c.Item().PaddingTop(3).Row(row =>
            {
                row.RelativeItem().Column(c2 =>
                {
                    c2.Item().Background(Colors.Grey.Lighten4)
                        .Padding(6)
                        .Border(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .Column(c3 =>
                        {
                            c3.Item().Text("نوع ملک")
                                .FontSize(9)
                                .FontColor(Colors.Grey.Darken2)
                                .AlignCenter();
                            c3.Item().Text(GetCategoryType(property.CategoryType))
                                .FontSize(13)
                                .Bold()
                                .AlignCenter();
                        });
                });

                row.RelativeItem().Column(c2 =>
                {
                    c2.Item().Background(Colors.Grey.Lighten4)
                        .Padding(6)
                        .Border(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .Column(c3 =>
                        {
                            c3.Item().Text("منطقه")
                                .FontSize(9)
                                .FontColor(Colors.Grey.Darken2)
                                .AlignCenter();
                            c3.Item().Text(property.RegionName ?? "نامشخص")
                                .FontSize(13)
                                .Bold()
                                .AlignCenter();
                        });
                });

                row.RelativeItem().Column(c2 =>
                {
                    c2.Item().Background(Colors.Grey.Lighten4)
                        .Padding(6)
                        .Border(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .Column(c3 =>
                        {
                            c3.Item().Text("سال ساخت")
                                .FontSize(9)
                                .FontColor(Colors.Grey.Darken2)
                                .AlignCenter();
                            c3.Item().Text(property.ConstructionYear > 0 ? property.ConstructionYear.ToString() : "نامشخص")
                                .FontSize(13)
                                .Bold()
                                .AlignCenter();
                        });
                });

                row.RelativeItem().Column(c2 =>
                {
                    c2.Item().Background(Colors.Grey.Lighten4)
                        .Padding(6)
                        .Border(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .Column(c3 =>
                        {
                            c3.Item().Text("طبقه")
                                .FontSize(9)
                                .FontColor(Colors.Grey.Darken2)
                                .AlignCenter();
                            c3.Item().Text($"{property.Floor} ==> {property.CountFloor}")
                                .FontSize(13)
                                .Bold()
                                .AlignCenter();
                        });
                });
            });
        });


        // ============================================================
        // عکس
        // ============================================================
        if (property.Images?.Any() == true)
        {
            try
            {
                var imageBytes = LoadImage(property.Images.First());
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    col.Item().PaddingVertical(4)
                        .AlignCenter()
                        .Height(180)
                        .Image(imageBytes)
                        .FitArea();
                }
            }
            catch
            {
                // خطا را نادیده بگیر
            }
        }

        // ============================================================
        // امکانات (از پراپرتی Facilities)
        // ============================================================
        col.Item().PaddingVertical(3).Column(c =>
        {
            c.Item().Text("امکانات ملک")
                .FontSize(13)
                .Bold()
                .FontColor(Colors.Blue.Darken2)
                .AlignRight();

            // امکانات اصلی (Boolean)
            c.Item().PaddingTop(3).Row(row =>
            {
                row.RelativeItem().Column(c2 =>
                {
                    AddAmenityCard(c2, "آسانسور", property.IsHasElevator);
                    AddAmenityCard(c2, "پارکینگ", property.IsHasParking);
                });

                row.RelativeItem().Column(c2 =>
                {
                    AddAmenityCard(c2, "استخر", property.IsHasPool);
                    AddAmenityCard(c2, "انباری", property.IsHasStoreRoom);
                });
            });

            // ============================================================
            // Facilities اضافی (از لیست Facilities)
            // ============================================================
            if (property.Facilities != null && property.Facilities.Any())
            {
                c.Item().Text("ویژگی ملک")
                    .FontSize(13)
                    .Bold()
                    .FontColor(Colors.Blue.Darken2)
                    .AlignRight();

                c.Item().PaddingTop(4).Row(row =>
                {
                    var facilities = property.Facilities.ToList();
                    var mid = (int)Math.Ceiling(facilities.Count / 2.0);

                    row.RelativeItem().Column(c2 =>
                    {
                        for (int i = 0; i < mid && i < facilities.Count; i++)
                        {
                            AddFacilityItem(c2, facilities[i]);
                        }
                    });

                    row.RelativeItem().Column(c2 =>
                    {
                        for (int i = mid; i < facilities.Count; i++)
                        {
                            AddFacilityItem(c2, facilities[i]);
                        }
                    });
                });
            }
        });
        // ============================================================
        // توضیحات ملک (با پشتیبانی از HTML)
        // ============================================================
        if (!string.IsNullOrEmpty(property.DescriptionRows))
        {
            col.Item().PaddingVertical(3).Column(c =>
            {
                c.Item().Text("توضیحات")
                    .FontSize(13)
                    .Bold()
                    .FontColor(Colors.Blue.Darken2)
                    .AlignRight();

                c.Item().PaddingTop(3)
                    .Background(Colors.Grey.Lighten4)
                    .Padding(8)
                    .Border(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .Column(c2 =>
                    {
                        RenderHtmlToQuestPdf(c2, property.DescriptionRows);
                    });
            });
        }
        // ============================================================
        // آدرس
        // ============================================================
        col.Item().PaddingVertical(3).Column(c =>
        {
            c.Item().Text("آدرس ملک")
                .FontSize(13)
                .Bold()
                .FontColor(Colors.Blue.Darken2)
                .AlignRight();

            c.Item().PaddingTop(3)
                .Background(Colors.Grey.Lighten4)
                .Padding(8)
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Column(c2 =>
                {
                    c2.Item().Text(property.Address ?? "آدرسی ثبت نشده")
                        .FontSize(12)
                        .AlignRight();

                    c2.Item().PaddingTop(3)
                        .Text($"مختصات: {property.lat} , {property.lng}")
                        .FontSize(9)
                        .FontColor(Colors.Grey.Darken1)
                        .AlignRight();
                });
        });

        // ============================================================
        // مشاور
        // ============================================================
        if (property.Agents != null)
        {
            col.Item().PaddingVertical(3).Column(c =>
            {
                c.Item().Text("اطلاعات مشاور")
                    .FontSize(13)
                    .Bold()
                    .FontColor(Colors.Blue.Darken2)
                    .AlignRight();

                c.Item().PaddingTop(3)
                    .Background(Colors.Grey.Lighten4)
                    .Padding(8)
                    .Border(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .Row(row =>
                    {
                        row.RelativeItem().Column(c2 =>
                        {
                            c2.Item().Text($"نام: {property.Agents.Name ?? "نامشخص"}")
                                .FontSize(12)
                                .AlignRight();
                            c2.Item().Text($"تلفن: {property.Agents.Phone ?? "نامشخص"}")
                                .FontSize(12)
                                .AlignRight();
                        });

                        row.RelativeItem().Column(c2 =>
                        {
                            c2.Item().Text("آدرس دفتر ")
                                .FontSize(11)
                                .Bold()
                                .AlignRight();
                            c2.Item().Text(property.Agents.Address ?? "نامشخص")
                                .FontSize(12)
                                .AlignRight();
                        });
                    });
            });
        }
    }

    private void BuildFooter(IContainer container, RealEstateDetails property)
    {
        container.BorderTop(1.5f)
            .BorderColor(Colors.Blue.Lighten2)
            .PaddingTop(6)
            .AlignCenter()
            .Text(t =>
            {
                t.Span("تاریخ چاپ: ");
                t.Span($"{DateTime.Now:yyyy/MM/dd HH:mm}");

                t.Span("    ●    ");

                t.Span("کد ملک: ");
                t.Span($"#{property.Id:D4}");

                t.Span("    ●    ");

                t.Span("صفحه ");
                t.Span("1");
                t.Span(" از ");
                t.Span("1");
                t.Span("    ●    ");

                t.Span(" www.khoneyab.ir ");
            });
    }

    /// <summary>
    /// تبدیل HTML به المان‌های QuestPDF
    /// </summary>
    private void RenderHtmlToQuestPdf(ColumnDescriptor col, string html)
    {
        if (string.IsNullOrEmpty(html))
            return;

        // پیدا کردن تمام تگ‌های p با محتوایشان
        var pTagRegex = new Regex(@"<p[^>]*>(.*?)</p>", RegexOptions.Singleline);
        var matches = pTagRegex.Matches(html);

        if (matches.Count == 0)
        {
            // اگر تگ p نبود، کل متن رو نمایش بده
            var cleanText = StripHtmlTags(html);
            col.Item().Text(cleanText).FontSize(11).AlignRight();
            return;
        }

        foreach (Match match in matches)
        {
            var content = match.Groups[1].Value;
            var tag = match.Value;

            // بررسی کلاس‌های CSS
            var isCenter = tag.Contains("ql-align-center");
            var isBold = content.Contains("<strong>") || content.Contains("<b>");

            // حذف تگ‌های strong/b از محتوا
            var cleanContent = Regex.Replace(content, @"</?strong>|</?b>", string.Empty);

            // تبدیل &nbsp; به فاصله
            cleanContent = cleanContent.Replace("&nbsp;", " ");

            // ساخت متن در QuestPDF
            var textBlock = col.Item().Text(cleanContent)
                .FontSize(11);

            if (isBold)
                textBlock.Bold();

            if (isCenter)
                textBlock.AlignCenter();
            else
                textBlock.AlignRight();

            // اضافه کردن فاصله بین پاراگراف‌ها
            if (match != matches.Last())
            {
                col.Item().PaddingBottom(3);
            }
        }
    }

    private void AddAmenityCard(ColumnDescriptor col, string name, bool has)
    {
        var icon = has ? "✓" : "✗";
        var color = has ? Colors.Green.Darken2 : Colors.Red.Darken2;
        var bgColor = has ? Colors.Green.Lighten5 : Colors.Red.Lighten5;
        var borderColor = has ? Colors.Green.Lighten2 : Colors.Red.Lighten2;

        col.Item().Padding(3)
            .Background(bgColor)
            .Padding(6)
            .Border(1)
            .BorderColor(borderColor)
            .Text($"{icon} {name}")
            .FontColor(color)
            .FontSize(12)
            .Bold()
            .AlignCenter();
    }

    private void AddFacilityItem(ColumnDescriptor col, string facilityName)
    {
        col.Item().Padding(3)
            .Background(Colors.Blue.Lighten5)
            .Padding(6)
            .Border(1)
            .BorderColor(Colors.Blue.Lighten2)
            .Text($"• {facilityName}")
            .FontColor(Colors.Blue.Darken2)
            .FontSize(11)
            .AlignRight();
    }

    private string GetCategoryType(int type) => type switch
    {
        1 => "فروش",
        2 => "رهن",
        3 => "اجاره",
        _ => "نامشخص"
    };

    private string StripHtmlTags(string html)
    {
        if (string.IsNullOrEmpty(html))
            return html;

        var clean = Regex.Replace(html, @"<[^>]*>", string.Empty);
        clean = clean.Replace("&nbsp;", " ");
        clean = clean.Replace("&amp;", "&");
        clean = clean.Replace("&lt;", "<");
        clean = clean.Replace("&gt;", ">");
        clean = clean.Replace("&quot;", "\"");
        clean = Regex.Replace(clean, @"\s+", " ");

        return clean.Trim();
    }

    private byte[] GenerateQrCode(int id,string name)
    {
        try
        {
            using var gen = new QRCodeGenerator();
            var data = gen.CreateQrCode($"http://localhost:3000/property/{id}/{name}", QRCodeGenerator.ECCLevel.Q);
            return new PngByteQRCode(data).GetGraphic(20);
        }
        catch
        {
            return Array.Empty<byte>();
        }
    }

    private byte[] LoadImage(string url)
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
            return client.GetByteArrayAsync(url).GetAwaiter().GetResult();
        }
        catch
        {
            return Array.Empty<byte>();
        }
    }
    private string GetFloorText(int floor, int countFloor)
    {
        if (countFloor <= 0)
            return floor > 0 ? $"طبقه {floor}" : "نامشخص";

        return $"{floor} از {countFloor}";
    }
}