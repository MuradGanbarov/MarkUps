namespace MarkUps.Models
{
    public class Position
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Chef> Chefs { get; set; }
    }
}
