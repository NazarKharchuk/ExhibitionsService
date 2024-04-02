namespace ExhibitionsService.BLL.DTO
{
    public class PaintingDTO
    {
        public int PaintingId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CretionDate { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string ImagePath { get; set; }
        public string? Location { get; set; }

        public int PainterId { get; set; }
    }
}
