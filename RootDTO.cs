using System.Collections.Generic;

namespace ServerInventory
{
    public class RootDTO
    {
        public List<InventoryDTO> InventoryDTOList { get; set; }
        public List<SkillDTO> SkillDTOList { get; set; }
    }
}
