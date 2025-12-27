namespace quantity_move_api.Common.Exceptions;

public class ItemNotFoundException : Exception
{
    public string ItemCode { get; }

    public ItemNotFoundException(string itemCode) 
        : base($"Item '{itemCode}' not found.")
    {
        ItemCode = itemCode;
    }

    public ItemNotFoundException(string itemCode, string message) 
        : base(message)
    {
        ItemCode = itemCode;
    }
}

