using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models;

public class EcommerceConnection
{
    public int Id { get; set; }

    [Required]
    public string Platform { get; set; } = "Shopify";

    [Required]
    public string StoreName { get; set; } = "";

    public string StoreUrl { get; set; } = "";

    public string SyncStatus { get; set; } = "Pret a configurer";

    public bool SyncProducts { get; set; } = true;

    public bool SyncOrders { get; set; } = true;

    public DateTime? LastSyncAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
