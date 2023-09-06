using System.Collections.Generic;
using Godot;
using SlidingPuzzleMono.Globals;

namespace SlidingPuzzleMono
{
    public class Puzzle : Container
    {
        [Export()] private PackedScene _blockScene;
        [Export()] private PackedScene _effectScene;
        [Export()] private int _blocksPerLine = 4;

        private Block _emptyBlock;
        private Tween _tween = new Tween();
        private RandomNumberGenerator _rndmGen = new RandomNumberGenerator();
        private float _tweenSpeed = 0.4f;
        private List<int> _randomNumbers = new List<int>();
        private int _movesAmount = 0;
        private GlobalEvents _globalEvents;
        private Dictionary<int, Vector2> _gridPositions = new Dictionary<int, Vector2>();

        public override void _Ready()
        {
            _globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");

            foreach (var child in GetChild(0).GetChildren())
            {
                var grid = child as Label;
                if (grid == null) return;
                _gridPositions.Add(grid.GetIndex(), grid.RectPosition);
                // GD.Print(_gridPositions[grid.GetIndex()]);
            }
            
            
            AddChild(_tween);
            _tween?.Connect("tween_completed", this, nameof(_on_Tween_completed));
            _rndmGen.Randomize();
            GenerateRandomNumbers();
            CreatePuzzle();
        }

        private void GenerateRandomNumbers()
        {
            _randomNumbers.Clear();
            var counter = 0;
            for (int i = 0; i < 15; i = counter)
            {
                var randomNumber = _rndmGen.RandiRange(1, 15);
                if (!_randomNumbers.Contains(randomNumber))
                {
                    _randomNumbers.Add(randomNumber);
                    counter++;
                }
            }
        }

        private void CreatePuzzle()
        {
            var counter = 0;
            for (int y = 0; y < _blocksPerLine; y++)
            {
                for (int x = 0; x < _blocksPerLine; x++)
                {
                    var block = _blockScene.Instance() as Block;
                    if (block == null) return;
                    AddChild(block);
                    block.Connect("BlockPressed", this, nameof(OnBlockPressed));
                    block.RectPosition= Vector2.One * 32 * new Vector2(x, y);
                    
                    if (y == 0 && x == 0)
                    {
                        block.Visible = false;
                        _emptyBlock = block;
                    }
                    else
                    {
                        block.SetNumber(_randomNumbers[counter]);
                        counter++;
                    }
                }
            }
            // GD.Print("puzzle created");
        }

        private void RemoveBlocks()
        {
            if (GetChildren().Count <= 1) return;
            for (int i = 0; i < GetChildren().Count; i++)
            {
                if (GetChild(i) is Block) GetChild(i).QueueFree();
            }
        }

        private void OnBlockPressed(Block blockToMove)
        {
            if (blockToMove.RectPosition.DistanceTo(_emptyBlock.RectPosition) != 32) return;
            if (_tween.IsActive()) return;
            Vector2 targetPosition = _emptyBlock.RectPosition;
            _emptyBlock.RectPosition = blockToMove.RectPosition;
            MoveTween(blockToMove, blockToMove.RectPosition, targetPosition);
            _movesAmount++;
            _globalEvents.EmitSignal("BlockMoved", blockToMove, _movesAmount);
        }

        private void _on_Tween_completed(Object _object, NodePath _key)
        {
            var movedBlock = _object as Block;
            foreach (var grid in _gridPositions)
            {
                if (grid.Value == movedBlock?.RectPosition)
                {
                    if (grid.Key == movedBlock.Number)
                    {
                        PlayEffect(grid.Value);
                    }
                }
            }
        }

        public void _on_Button_button_down()
        {
            _movesAmount = 0;
            _globalEvents.EmitSignal("ResetPuzzle");
            GenerateRandomNumbers();
            RemoveBlocks();
            CreatePuzzle();
        }

        private void PlayEffect(Vector2 position)
        {
            var effect = _effectScene.Instance() as Effect;
            AddChild(effect);
            effect.RectPosition = position;
            effect?.PlayEffect();
        }

        private void MoveTween(Block blockToMove, Vector2 startPos, Vector2 endPos)
        {
            _tween.InterpolateProperty(blockToMove, "rect_position", startPos, endPos, _tweenSpeed,
                Tween.TransitionType.Bounce, Tween.EaseType.Out);
            _tween.Start();
        }
    }
}