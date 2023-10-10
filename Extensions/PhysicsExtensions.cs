using UnityEngine;

namespace stoogebag.Extensions
{
    public static class PhysicsExtensions
    {
        
        public static void SetFixedDeltaTime(float step)
        {
            if (Time.fixedDeltaTime == step) return;
            Time.fixedDeltaTime = step;
            Debug.Log($"Setting physics timestep {Time.fixedDeltaTime}.");
        }

    }
}