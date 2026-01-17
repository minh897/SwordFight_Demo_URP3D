using System.Collections;
using UnityEngine;

namespace Assets.Utility
{    
    public static class Utils
    {
        /// <summary>
        /// Smoothly interpolates a value from a starting point to a target point over a given duration. 
        /// The interpolated value is applied each frame via a callback (delegate), and the final target is
        /// guaranteed to be applied when the interpolation completes.
        /// </summary>
        /// <param name="duration"> The time in seconds over which the interpolation occurs </param>
        /// <param name="start"> The starting value </param>
        /// <param name="end"> The target value </param>
        /// <param name="applyLerpTo"> A delegate invoked each frame with the interpolated value </param>
        /// <returns> Coroutine </returns>
        public static IEnumerator LerpOverTime(
            float duration, 
            float start, 
            float end, 
            System.Action<float> applyLerpTo)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.fixedDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float newValue = Mathf.Lerp(start, end, t);
                applyLerpTo(newValue);
                yield return null;
            }
            // making sure the interpolated value reaches its target
            applyLerpTo(end); 
        }

        public static Vector3 SwapVector3(ref Vector3 from, ref Vector3 to)
        {
            Vector3 tmp = from;
            from = to;
            return tmp;
        }
    }
}
