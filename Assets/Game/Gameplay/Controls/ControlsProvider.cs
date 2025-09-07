using Game.Input;

namespace Game.Gameplay.Controls
{
    //TODO: make abstraction, so it is possible to migrate from input system
    public class ControlsProvider
    {
        public MainControls Controls { get; private set; }

        public void Init()
        {
            Controls = new MainControls();
            Controls.Enable();
        }
    }
}