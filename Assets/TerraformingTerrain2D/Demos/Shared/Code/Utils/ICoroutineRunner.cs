using System.Collections;
using UnityEngine;

namespace DemosShared
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator routine);
        void StopCoroutine(Coroutine coroutine);
        void StopAllCoroutines();
    }
}