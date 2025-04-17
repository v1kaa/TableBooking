namespace TableBooking.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }



        public Table Table { get; set; }
    }
}
