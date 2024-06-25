using UnityEngine;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This component allows you to create a joint between the current Rigidbody2D, and a separate destructible Rigidbody2D. The joint will automatically be broken when the <b>ConnectedFixture</b> is detached, or the <b>Fixture</b> if the current Rigidbody is also destructible.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dFixtureJoint")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Fixture Joint")]
	public class D2dFixtureJoint : MonoBehaviour
	{
		/// <summary>This is the joint whose <b>ConnectedBody</b> will be updated based on the <b>ConnectedFixture</b>, and will be destroyed when either of the fixtures detaches.</summary>
		public Joint2D Joint { set { joint = value; } get { return joint; } } [SerializeField] private Joint2D joint;

		/// <summary>This is the fixture associated with the joint.</summary>
		public D2dFixture Fixture { set { fixture = value; } get { return fixture; } } [SerializeField] private D2dFixture fixture;

		/// <summary>This is the fixture associated with the object the current joint attaches to.</summary>
		public D2dFixture ConnectedFixture { set { connectedFixture = value; } get { return connectedFixture; } } [SerializeField] private D2dFixture connectedFixture;

		/// <summary>Automatically destroy this joint if detached?</summary>
		public bool AutoDestroyJoint { set { autoDestroyJoint = value; } get { return autoDestroyJoint; } } [SerializeField] private bool autoDestroyJoint = true;

		/// <summary>Automatically destroy this component if detached?</summary>
		public bool AutoDestroyThis { set { autoDestroyThis = value; } get { return autoDestroyThis; } } [SerializeField] private bool autoDestroyThis = true;

		private bool IsAttached
		{
			get
			{
				if (connectedFixture == null)
				{
					return false;
				}

				var destructible = GetComponent<D2dDestructible>();

				if (destructible != null)
				{
					if (fixture == null)
					{
						return false;
					}

					if (destructible != fixture.GetComponentInParent<D2dDestructible>())
					{
						return false;
					}
				}

				return true;
			}
		}

		/// <summary>This method can be used to immediately update this component.</summary>
		[ContextMenu("Update Fixtures")]
		public void UpdateFixtures()
		{
			if (enabled == true && joint != null && joint.enabled == true)
			{
				// Detach?
				if (IsAttached == false)
				{
					if (autoDestroyJoint == true)
					{
						Destroy(joint);
					}
					else
					{
						joint.enabled = false;
					}

					if (autoDestroyThis == true)
					{
						Destroy(this);
					}
				}
				// Keep connected?
				else
				{
					var connectedBody = default(Rigidbody2D);

					if (connectedFixture != null)
					{
						connectedBody = connectedFixture.GetComponentInParent<Rigidbody2D>();
					}

					// This is required to stop the joint 'drifting'.
					if (joint.connectedBody != connectedBody)
					{
						// Due to splitting the joints connected body may have changed.
						joint.connectedBody = connectedBody;
					}
				}
			}
		}

		protected virtual void Update()
		{
			UpdateFixtures();
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Inspector
{
	using UnityEditor;
	using TARGET = D2dFixtureJoint;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dFixtureJoint_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			BeginError(Any(tgts, t => t.Joint == null));
				Draw("joint", "This is the joint whose ConnectedBody will be updated based on the ConnectedFixture, and will be destroyed when either of the fixtures detaches.");
			EndError();
			if (Any(tgts, t => t.GetComponent<D2dDestructible>() != null))
			{
				BeginError(Any(tgts, t => t.Fixture == null));
					Draw("fixture", "This is the fixture associated with the joint.");
				EndError();
			}
			BeginError(Any(tgts, t => t.ConnectedFixture == null));
				Draw("connectedFixture", "This is the fixture associated with the object the current joint attaches to.");
			EndError();

			Separator();

			Draw("autoDestroyJoint", "Automatically destroy this joint if detached?");
			Draw("autoDestroyThis", "Automatically destroy this component if detached?");
		}
	}
}
#endif