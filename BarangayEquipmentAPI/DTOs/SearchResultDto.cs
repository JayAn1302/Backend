namespace BarangayEquipmentAPI.DTOs.Search
{
    public class SearchResultDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Subtitle { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;
    }
}