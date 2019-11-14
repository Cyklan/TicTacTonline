namespace Models
{
    public class Document
    {
        public string Type { get; set; }

        public Document() { Type = GetType().Name; }
  
    }
}
