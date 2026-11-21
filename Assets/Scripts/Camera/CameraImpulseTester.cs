using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

namespace PirateQuest.Camera
{
    public sealed class CameraImpulseTester : MonoBehaviour
    {
        [SerializeField] private CinemachineImpulseSource impulseSource;
        [SerializeField] private float hurtForce = 0.6f;
        [SerializeField] private float hitForce = 0.3f;
        [SerializeField] private float explodeForce = 1.2f;
        [SerializeField] private float pickupForce = 0.25f;
        private void Update()
        {
            if (Keyboard.current == null) return;
            if (Keyboard.current.digit1Key.wasPressedThisFrame) Fire(hurtForce);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) Fire(hitForce);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) Fire(explodeForce);
            if (Keyboard.current.digit4Key.wasPressedThisFrame) Fire(pickupForce);
        }
        private void Fire(float force)
        {
            impulseSource.GenerateImpulseWithForce(force);
        }
    }
}
