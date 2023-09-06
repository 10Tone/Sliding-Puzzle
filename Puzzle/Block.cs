using Godot;

namespace SlidingPuzzleMono
{
    public class Block : ColorRect
    {
        [Signal]
        public delegate void BlockPressed(Block block);

        [Export()] private NodePath _labelPath;

        private Label _numberLabel;
        private int _number;

        public int Number => _number;

        public override void _Ready()
        {
            _numberLabel = GetNode<Label>(_labelPath);
        }

        public void _on_Block_gui_input(InputEvent inputEvent)
        {
            if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
            {
                EmitSignal(nameof(BlockPressed), this);
            }
        }

        public void SetNumber(int number)
        {
            _number = number;
            if (_numberLabel == null) return;
            _numberLabel.Text = number.ToString();
        }
        
    }
}
