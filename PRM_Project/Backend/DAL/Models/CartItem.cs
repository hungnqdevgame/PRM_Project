using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DAL.Models;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int? CartId { get; set; }

    public int? ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }


    [JsonIgnore]
    public virtual Cart? Cart { get; set; }

    public virtual Product? Product { get; set; }
}
