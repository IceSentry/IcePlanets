using UnityEngine;

namespace DefaultNamespace
{
    public interface INoiseFilter
    {
        float Evaluate(Vector3 point);
    }
}