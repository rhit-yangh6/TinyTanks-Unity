using System.Collections.Generic;
using System.Linq;
using DemosShared;
using UnityEngine;
using UnityEngine.UI;

namespace ExplosionGame
{
    public class ServicesRunner : MonoBehaviourWrapper
    {
        [SerializeField] private Button _restartButton;

        public void Compose(DieState dieState)
        {
            IEnumerable<IRestart> restartable = Utils.FindObjects<MonoBehaviour>().OfType<IRestart>();
            RestartComposite restartComposite = new(restartable);
            
            SetDependencies(new IUnityCallback[]
            {
                new ButtonRestartListener(_restartButton, restartComposite),
                new DieRestart(dieState, restartComposite),
            });
        }
    }
}