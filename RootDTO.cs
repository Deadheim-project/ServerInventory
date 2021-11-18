using System.Collections.Generic;

namespace ServerInventory
{
    public class RootDTO
    {
        public List<InventoryDTO> InventoryDTOList { get; set; }
        public List<SkillDTO> SkillDTOList { get; set; }
        public List<KnownTextDTO> KnownTextDTOList { get; set; }

        public class KnownTextDTO
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}
