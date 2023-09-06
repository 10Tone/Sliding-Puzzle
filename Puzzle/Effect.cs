using System;
using Godot;

namespace SlidingPuzzleMono
{
    public class Effect : ColorRect
    {
        private AnimationPlayer _animationPlayer;

        public override void _Ready()
        {
            _animationPlayer = GetNode("AnimationPlayer") as AnimationPlayer;
        }

        public void PlayEffect()
        {
            _animationPlayer?.Play("effect");
        }

        private void _on_AnimationPlayer_animation_finished(String _anim_name)
        {
            QueueFree();
        }
    }
}
