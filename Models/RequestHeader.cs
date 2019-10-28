namespace Models
{
    public class RequestHeader : Header
    {
        public Identifier Identifier { get; set; }
        public User User { get; set; }

        public RequestHeader() { MessageNumber = DateTime.Now.Ticks + " - " + new Random().Next(0, 999999) + ""; }

    }
}
