using Godot;
using SlidingPuzzleMono.Globals;

namespace SlidingPuzzleMono
{
    public class MovesCounter : Label
    {
        private GlobalEvents _globalEvents;
    
        public override void _Ready()
        {
            _globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
            if(_globalEvents == null) GD.Print("global events not found");
            _globalEvents?.Connect(nameof(GlobalEvents.BlockMoved), this, nameof(OnBlockMoved));
            _globalEvents?.Connect(nameof(GlobalEvents.ResetPuzzle), this, nameof(OnResetPuzzle));
            
        }

        private void OnBlockMoved(Block _block, int movesAmount)
        {
            Text = movesAmount.ToString();
        }

        private void OnResetPuzzle()
        {
            Text = "0";
        }
    }
}
