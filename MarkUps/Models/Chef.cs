namespace MarkUps.Models
{
    public class Chef
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ImageURL { get; set; }
        public int? PositionId { get; set; }
        public Position? Position { get; set; }
    }
}
