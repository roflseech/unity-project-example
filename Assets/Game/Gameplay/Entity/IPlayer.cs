using UnityEngine;

namespace Game.Gameplay.Entity
{
    public interface IPlayer : IEntity
    {
        void Move(Vector2 dir);
        void LookAt(Vector3 point);
    }
}