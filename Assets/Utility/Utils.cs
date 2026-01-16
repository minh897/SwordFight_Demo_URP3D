using System.Collections;
using UnityEngine;

namespace Assets.Utility
{    
    public static class Utils
    {
        /// <summary>
        /// Smoothly interpolates a Vector3 value from a starting point to a target point over a given duration. 
        /// The interpolated value is applied each frame via a callback (delegate), and the final target is
        /// guaranteed to be applied when the interpolation completes.
        /// </summary>
        /// <param name="duration"> The time in seconds over which the interpolation occurs </param>
        /// <param name="start"> The starting Vector3 value </param>
        /// <param name="end"> The target Vector3 value </param>
        /// <param name="applyVector3To"> A delegate invoked each frame with the interpolated Vector3 value </param>
        /// <returns> Coroutine </returns>
        public static IEnumerator LerpVector3(
            float duration, 
            Vector3 start, 
            Vector3 end, 
            System.Action<Vector3> applyVector3To)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                Vector3 lerp = Vector3.Lerp(start, end, t);
                applyVector3To(lerp);
                yield return null;
            }
            // making sure the interpolated value reaches its target
            applyVector3To(end); 
        }

        public static Vector3 SwapVector3(ref Vector3 from, ref Vector3 to)
        {
            Vector3 tmp = from;
            from = to;
            return tmp;
        }
    }
}
