using Godot;

namespace SlidingPuzzleMono.Globals
{
    public class GlobalEvents : Node
    {
        [Signal]public delegate void BlockMoved(Block block, int moveAmount);

        [Signal]
        public delegate void ResetPuzzle();
    }
}