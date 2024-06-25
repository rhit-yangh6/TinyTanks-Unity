using UnityEngine;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component will automatically heal the attached D2dDestructible to the specified snapshot.</summary>
	[RequireComponent(typeof(D2dDestructible))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dHealCuts")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Heal Cuts")]
	public class D2dHealCuts : MonoBehaviour
	{
		/// <summary>The amount of pixel change required to paint.</summary>
		public float Threshold { set { threshold = value; } get { return threshold; } } [Range(0.0f, 1.0f)] [SerializeField] private float threshold = 0.1f;

		/// <summary>The speed of healing.\n1 = 1 second\n2 = 0.5 second</summary>
		public float Speed { set { speed = value; } get { return speed; } } [SerializeField] private float speed = 1.0f;

		[System.NonSerialized]
		private D2dDestructible cachedDestructible;

		[SerializeField]
		private float current;

		protected virtual void OnEnable()
		{
			if (cachedDestructible == null) cachedDestructible = GetComponent<D2dDestructible>();
		}

		protected virtual void Update()
		{
			if (speed > 0.0f)
			{
				current += speed * Time.deltaTime;
			}

			if (current >= threshold)
			{
				var step = Mathf.FloorToInt(current * 255.0f);

				if (step > 0)
				{
					var change = step / 255.0f;

					current -= change;

					Heal(step);
				}
			}
		}

		private void Heal(int healAmount)
		{
			if (cachedDestructible.CanHeal == true)
			{
				var w         = cachedDestructible.AlphaWidth;
				var h         = cachedDestructible.AlphaHeight;
				var t         = w * h;
				var healData  = cachedDestructible.HealSnapshot.DataRaw.AlphaData;
				var alphaData = cachedDestructible.AlphaData;

				for (var i = 0; i < t; i++)
				{
					var alphaPixel = alphaData[i];
					var healPixel  = healData[i];

					alphaPixel.a = (byte)Mathf.MoveTowards(alphaPixel.a, healPixel.a, healAmount);

					alphaData[i] = alphaPixel;
				}

				cachedDestructible.AlphaModified.Set(0, w, 0, h);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dHealCuts;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dHealCuts_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("threshold", "The amount of pixel change required to paint.");
			BeginError(Any(tgts, t => t.Speed <= 0));
				Draw("speed", "The speed of healing.\n1 = 1 second\n2 = 0.5 second");
			EndError();
		}
	}
}
#endif