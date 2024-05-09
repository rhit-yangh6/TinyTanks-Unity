using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DemosShared
{
    public class ServicesRunner : MonoBehaviourWrapper
    {
        [SerializeField] private Button _restartButton;
        [SerializeField] private UIStarsPanel _uiStarsPanel;
        [SerializeField] private WinPanel _winPanel;
        [SerializeField] private AudioSource _pickupSound;

        public void Compose(Star[] stars)
        {
            IEnumerable<IRestart> restartable = Utils.FindObjects<MonoBehaviour>().OfType<IRestart>();
            StarFlyCoordinator starFlyCoordinator = new(this, _uiStarsPanel, _winPanel, stars, _pickupSound);

            SetDependencies(new IUnityCallback[]
            {
                new ButtonRestartListener(_restartButton, new RestartComposite(restartable)),
                starFlyCoordinator
            });
        }
    }
}