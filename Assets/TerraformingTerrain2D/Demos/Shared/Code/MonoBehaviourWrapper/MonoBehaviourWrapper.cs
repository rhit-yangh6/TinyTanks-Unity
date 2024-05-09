using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DemosShared
{
    public abstract class MonoBehaviourWrapper : MonoBehaviour, IRestart, ICoroutineRunner
    {
        private IEnumerable<IOnTriggerEnter2D> _onTriggerEnter;
        private IEnumerable<IFixedUpdate> _fixedUpdates;
        private IEnumerable<ISubscriber> _subscribers;
        private IEnumerable<ILateUpdate> _lateUpdates;
        private IEnumerable<IRestart> _restarts;
        private IEnumerable<IUpdate> _updates;

        protected void SetDependencies(IUnityCallback[] dependencies)
        {
            dependencies.OfType<IInitializable>().ForEach(initializable => initializable.Initialize());
            _fixedUpdates = dependencies.OfType<IFixedUpdate>();
            _subscribers = dependencies.OfType<ISubscriber>();
            _lateUpdates = dependencies.OfType<ILateUpdate>();
            _updates = dependencies.OfType<IUpdate>();
            _restarts = dependencies.OfType<IRestart>();
            _onTriggerEnter = dependencies.OfType<IOnTriggerEnter2D>();
        }

        private void OnEnable()
        {
            _subscribers.ForEach(subscriber => subscriber.Subscribe());
        }

        private void OnDisable()
        {
            _subscribers.ForEach(subscriber => subscriber.Unsubscribe());
        }

        private void FixedUpdate()
        {
            _fixedUpdates.ForEach(fixedUpdate => fixedUpdate.FixedUpdate());
        }

        private void Update()
        {
            _updates.ForEach(update => update.Update());
        }

        private void LateUpdate()
        {
            _lateUpdates.ForEach(lateUpdate => lateUpdate.LateUpdate());
        }

        public void Restart()
        {
            _restarts.ForEach(restart => restart.Restart());
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _onTriggerEnter.ForEach(onTriggerEnter => onTriggerEnter.Entered(other));
        }
    }
}