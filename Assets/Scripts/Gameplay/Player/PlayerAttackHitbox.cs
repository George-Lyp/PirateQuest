using System;
using UnityEngine;

namespace PirateQuest.Gameplay.Player
{
    public sealed class PlayerAttackHitbox:MonoBehaviour
    {
        [SerializeField] private Collider2D hitboxCollider;
        [SerializeField] private LayerMask targetLayers;

        public event Action<Collider2D> Hit;
        private void Awake()
        {
            hitboxCollider.enabled=false;
        }
        public void Open()
        {
            hitboxCollider.enabled=true;
        }
        public void Close()
        {
            hitboxCollider.enabled=false;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(((1<<other.gameObject.layer) & targetLayers)==0)
            {
                return;
            }
            Hit?.Invoke(other);
        }
    }
}
