namespace BarangayEquipmentAPI.DTOs
{
    public class DashboardResponseDto
    {
        // Cards
        public int TotalEquipment { get; set; }
        public int AvailableEquipment { get; set; }
        public int BorrowedEquipment { get; set; }
        public int MaintenanceEquipment { get; set; }
        public int ReservedEquipment { get; set; }

        public int TotalBorrowers { get; set; }

        public int ActiveBorrowings { get; set; }
        public int ReturnedBorrowings { get; set; }
        public int OverdueBorrowings { get; set; }



        // Dashboard Table
        public List<BorrowingResponseDto> RecentBorrowings { get; set; } = new();
    }
}