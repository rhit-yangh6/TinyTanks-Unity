using UnityEngine;
using UnityEngine.Events;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component acts as a timer that invokes an event when the time runs out.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dTimer")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Timer")]
	public class D2dTimer : MonoBehaviour
	{
		/// <summary>When this reaches 0, the <b>OnFinished</b> event will be invoked.</summary>
		public float SecondsRemaining { set { secondsRemaining = value; } get { return secondsRemaining; } } [SerializeField] private float secondsRemaining = 1.0f;

		/// <summary>If you pause the game, should this timer keep running?</summary>
		public bool IgnoreTimeScale { set { ignoreTimeScale = value; } get { return ignoreTimeScale; } } [SerializeField] private bool ignoreTimeScale;

		/// <summary>When <b>SecondsRemaining</b> reaches 0, this event will be invoked.</summary>
		public UnityEvent OnFinished { get { return onFinished; } } [SerializeField] private UnityEvent onFinished = null;
		
		protected virtual void Update()
		{
			if (secondsRemaining > 0.0f)
			{
				var delta = ignoreTimeScale == true ? Time.unscaledDeltaTime : Time.deltaTime;

				secondsRemaining -= delta;

				if (secondsRemaining <= 0.0f)
				{
					secondsRemaining = 0.0f;

					if (onFinished != null)
					{
						onFinished.Invoke();
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dTimer;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dTimer_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("secondsRemaining", "When this reaches 0, the <b>OnFinished</b> event will be invoked.");
			Draw("ignoreTimeScale", "If you pause the game, should this timer keep running?");

			Separator();

			Draw("onFinished");
		}
	}
}
#endif