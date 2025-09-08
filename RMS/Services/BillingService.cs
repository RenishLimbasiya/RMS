using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using RMS.Data;
using RMS.Models.DTOs.Billing;
using RMS.Models.Entities;

namespace RMS.Services
{
    public class BillingService
    {
        private readonly RmsDbContext _ctx;

        public BillingService(RmsDbContext ctx) { _ctx = ctx; }

        public async Task<BillPreviewDto?> PreviewAsync(int orderId)
        {
            var order = await _ctx.Orders.Include(o => o.Items).ThenInclude(i => i.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return null;

            var subtotal = order.Items.Sum(i => i.UnitPrice * i.Quantity);
            var tax = Math.Round(subtotal * (order.TaxPercent / 100m), 2);
            var total = subtotal - order.Discount + tax;

            return new BillPreviewDto
            {
                OrderId = order.Id,
                Subtotal = subtotal,
                Discount = order.Discount,
                Tax = tax,
                Total = total
            };
        }

        public async Task<BillDto?> FinalizeAsync(BillFinalizeDto dto)
        {
            var preview = await PreviewAsync(dto.OrderId);
            if (preview == null) return null;

            var bill = new Bill
            {
                OrderId = dto.OrderId,
                Subtotal = preview.Subtotal,
                Discount = preview.Discount,
                Tax = preview.Tax,
                Total = preview.Total,
                PaymentType = dto.PaymentType,
                BillNumber = $"BILL-{DateTime.UtcNow:yyyyMMddHHmmss}"
            };
            _ctx.Bills.Add(bill);

            var order = await _ctx.Orders.FindAsync(dto.OrderId);
            if (order != null)
            {
                order.Status = "Billed";
            }

            await _ctx.SaveChangesAsync();

            return new BillDto
            {
                Id = bill.Id,
                BillNumber = bill.BillNumber ?? "",
                OrderId = bill.OrderId,
                Subtotal = bill.Subtotal,
                Discount = bill.Discount,
                Tax = bill.Tax,
                Total = bill.Total,
                PaymentType = bill.PaymentType,
                CreatedAt = bill.CreatedAt
            };
        }

        public async Task<List<BillDto>> GetHistoryAsync()
        {
            return await _ctx.Bills
                .Include(b => b.Order)!.ThenInclude(o => o!.Table)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new BillDto
                {
                    Id = b.Id,
                    BillNumber = b.BillNumber ?? "",
                    OrderId = b.OrderId,
                    Subtotal = b.Subtotal,
                    Discount = b.Discount,
                    Tax = b.Tax,
                    Total = b.Total,
                    PaymentType = b.PaymentType,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();
        }

        // Export finalized bill PDF
        public async Task<byte[]?> ExportBillPdfAsync(int billId)
        {
            var bill = await _ctx.Bills
                .Include(b => b.Order)!.ThenInclude(o => o!.Items)
                .ThenInclude(i => i.MenuItem)
                .Include(b => b.Order)!.ThenInclude(o => o!.Table)
                .FirstOrDefaultAsync(b => b.Id == billId);
            if (bill == null) return null;

            QuestPDF.Settings.License = LicenseType.Community;

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text($"Restaurant Bill - {bill.BillNumber}").FontSize(18).SemiBold();

                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Date: {bill.CreatedAt:yyyy-MM-dd HH:mm}");
                        col.Item().Text($"Table: {bill.Order!.Table!.Name}");

                        col.Item().LineHorizontal(1);

                        col.Item().Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(40);
                                c.RelativeColumn(4);
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                            });

                            t.Header(h =>
                            {
                                h.Cell().Text("#");
                                h.Cell().Text("Item");
                                h.Cell().Text("Qty");
                                h.Cell().Text("Amount");
                            });

                            int idx = 1;
                            foreach (var it in bill.Order!.Items)
                            {
                                t.Cell().Text(idx.ToString());
                                t.Cell().Text(it.MenuItem!.Name);
                                t.Cell().Text(it.Quantity.ToString());
                                t.Cell().Text((it.Quantity * it.UnitPrice).ToString("0.00"));
                                idx++;
                            }
                        });

                        col.Item().LineHorizontal(1);
                        col.Item().AlignRight().Text($"Subtotal: {bill.Subtotal:0.00}");
                        col.Item().AlignRight().Text($"Discount: {bill.Discount:0.00}");
                        col.Item().AlignRight().Text($"Tax: {bill.Tax:0.00}");
                        col.Item().AlignRight().Text($"Total: {bill.Total:0.00}").Bold();
                        col.Item().Text($"Payment: {bill.PaymentType}");
                    });

                    page.Footer().AlignCenter().Text("Thank you for dining with us!");
                });
            });

            using var ms = new MemoryStream();
            doc.GeneratePdf(ms);
            return ms.ToArray();
        }


        // Export bill PDF from preview (no billId needed)
        public async Task<byte[]?> ExportPreviewPdfAsync(int orderId)
        {
            var preview = await PreviewAsync(orderId);
            if (preview == null) return null;

            var order = await _ctx.Orders
                .Include(o => o.Items).ThenInclude(i => i.MenuItem)
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return null;

            QuestPDF.Settings.License = LicenseType.Community;

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text($"Restaurant Bill").FontSize(18).SemiBold();

                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Date: {DateTime.Now:yyyy-MM-dd HH:mm}");
                        col.Item().Text($"Table: {order.Table!.Name}");
                        col.Item().LineHorizontal(1);

                        col.Item().Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(40);
                                c.RelativeColumn(4);
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                            });

                            t.Header(h =>
                            {
                                h.Cell().Text("#");
                                h.Cell().Text("Item");
                                h.Cell().Text("Qty");
                                h.Cell().Text("Amount");
                            });

                            int idx = 1;
                            foreach (var it in order.Items)
                            {
                                t.Cell().Text(idx.ToString());
                                t.Cell().Text(it.MenuItem!.Name);
                                t.Cell().Text(it.Quantity.ToString());
                                t.Cell().Text((it.Quantity * it.UnitPrice).ToString("0.00"));
                                idx++;
                            }
                        });

                        col.Item().LineHorizontal(1);
                        col.Item().AlignRight().Text($"Subtotal: {preview.Subtotal:0.00}");
                        col.Item().AlignRight().Text($"Discount: {preview.Discount:0.00}");
                        col.Item().AlignRight().Text($"Tax: {preview.Tax:0.00}");
                        col.Item().AlignRight().Text($"Total: {preview.Total:0.00}").Bold();
                    });

                    page.Footer().AlignCenter().Text("Thank you for dining with us!");
                });
            });

            using var ms = new MemoryStream();
            doc.GeneratePdf(ms);
            return ms.ToArray();
        }

    }
}
