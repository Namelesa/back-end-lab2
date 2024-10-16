namespace RESTAPI.Models;

public class Record : BaseModel
{
    public int UserId { get; set; }
    public int CategoryId { get; set; }

    public DateTime Date { get; set; }

    public decimal TotalPrice { get; set; }
}