namespace ServerInventory
{
    public class InventoryDTO
    {
        public string InventoryName { get; set; }
        public string Name { get; set; }
        public int Stack { get; set; }
        public int Quality { get; set; }
        public int Variant { get; set; }
        public long CrafterID { get; set; }
        public string CrafterName { get; set; }
        public bool Equiped { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public float Durability { get; set; }
    }
}
