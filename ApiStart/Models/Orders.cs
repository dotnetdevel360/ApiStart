namespace ApiStart.Models
{
    public class Orders
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Product { get; set; }
    }
}
