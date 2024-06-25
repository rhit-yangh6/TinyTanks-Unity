using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This component allows you to perform an action when all the specified fixtures all become detached from the current GameObject.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dFixtureGroup")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Fixture Group")]
	public class D2dFixtureGroup : MonoBehaviour
	{
		/// <summary>Automatically destroy this component if all fixtures are removed?</summary>
		public bool AutoDestroy { set { autoDestroy = value; } get { return autoDestroy; } } [SerializeField] private bool autoDestroy;

		/// <summary>This allows you to set the fixtures that will be tracked by this group.</summary>
		public List<D2dFixture> Fixtures { set { fixtures = value; } get { if (fixtures == null) fixtures = new List<D2dFixture>(); return fixtures; } } [SerializeField] public List<D2dFixture> fixtures;

		/// <summary>This event will be invoked when all entries in the Fixtures list are destroyed.</summary>
		public UnityEvent OnAllDetached { get { if (onAllDetached == null) onAllDetached = new UnityEvent(); return onAllDetached; } } [SerializeField] public UnityEvent onAllDetached;

		[System.NonSerialized]
		private D2dDestructible cachedDestructible;

		public void UpdateFixtures()
		{
			if (fixtures != null && fixtures.Count > 0)
			{
				if (cachedDestructible == null) cachedDestructible = GetComponentInParent<D2dDestructible>();

				if (cachedDestructible != null)
				{
					for (var i = fixtures.Count - 1; i >= 0; i--)
					{
						var fixture = fixtures[i];

						if (FixtureIsConnected(fixture) == false)
						{
							fixtures.RemoveAt(i);
						}
					}

					if (fixtures.Count == 0)
					{
						if (OnAllDetached != null)
						{
							OnAllDetached.Invoke();
						}

						if (AutoDestroy == true)
						{
							CwHelper.Destroy(this);
						}
					}
				}
			}
		}

		protected virtual void Update()
		{
			UpdateFixtures();
		}

		private bool FixtureIsConnected(D2dFixture fixture)
		{
			if (fixture != null)
			{
				var checkTransform = fixture.transform;

				while (checkTransform != null)
				{
					if (checkTransform == cachedDestructible.transform)
					{
						return true;
					}

					checkTransform = checkTransform.parent;
				}
			}

			return false;
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Inspector
{
	using UnityEditor;
	using TARGET = D2dFixtureGroup;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dFixtureGroup_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("autoDestroy", "Automatically destroy this component if all fixtures are removed?");
			BeginError(Any(tgts, t => t.Fixtures.Count == 0));
				Draw("fixtures");
			EndError();

			Separator();

			Draw("onAllDetached");
		}
	}
}
#endif