using Game.Models.InventoryManagement;
using Game.UI.GameLayers;
using Game.UI.GameModels.Widgets;
using Game.UI.GameModels.Widgets.Inventory;
using Game.UI.Models.Window;

namespace Game.UI.GameModels.Windows
{
    public interface IBaseGameplayWindowModel : IWindowModel
    {
        IButtonWidgetModel BackButton { get; }
        IActivePositionalInventoryModel Inventory { get; }
        void AttachInventory(IPlayerActiveInventory inventory);
    }
    
    public class BaseGameplayWindowModel : IBaseGameplayWindowModel
    {
        private readonly IInventoryVisualDataProvider _inventoryVisualDataProvider;
        
        public IButtonWidgetModel BackButton { get; }
        
        private IPlayerActiveInventory _inventory;
        
        public BaseGameplayWindowModel(IInventoryVisualDataProvider inventoryVisualDataProvider)
        {
            _inventoryVisualDataProvider = inventoryVisualDataProvider;
        }
        
        public IActivePositionalInventoryModel Inventory
        {
            get
            {
                if (_inventory == null) return null;

                return new ActivePositionalInventoryModel(_inventory, _inventoryVisualDataProvider);
            }
        }
        
        public void AttachInventory(IPlayerActiveInventory inventory)
        {
            _inventory = inventory;
        }

        public void OnOpen()
        {
            
        }

        public void OnClose()
        {
            
        }
    }
}