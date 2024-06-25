#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

namespace Destructible2D
{
	/// <summary>This class is used to add context menu options to various components for integration with Destructible2D.</summary>
	public static class D2dContext
	{
		[MenuItem("CONTEXT/SpriteRenderer/Make Destructible", true)]
		private static bool MakeDestructibleValidate(MenuCommand menuCommand)
		{
			var gameObject = GetGameObject(menuCommand);

			return gameObject.GetComponent<SpriteRenderer>() != null && gameObject.GetComponent<D2dDestructibleSprite>() == null;
		}

		[MenuItem("CONTEXT/SpriteRenderer/Make Destructible", false)]
		private static void MakeDestructible(MenuCommand menuCommand)
		{
			var gameObject = GetGameObject(menuCommand);

			AddSingleComponent<D2dDestructibleSprite>(gameObject, d => { d.ChangeMaterial(); d.Rebuild(); });
		}

		[MenuItem("CONTEXT/SpriteRenderer/Make Destructible (Preset: Dynamic Splittable)", true)]
		private static bool MakeDestructibleDynSplValidate(MenuCommand menuCommand)
		{
			return AddSingleComponentValidate<D2dDestructibleSprite>(GetGameObject(menuCommand));
		}

		[MenuItem("CONTEXT/SpriteRenderer/Make Destructible (Preset: Dynamic Splittable)", false)]
		private static void MakeDestructibleDynSpl(MenuCommand menuCommand)
		{
			var gameObject = GetGameObject(menuCommand);

			AddSingleComponent<D2dDestructibleSprite>(gameObject, d => { d.ChangeMaterial(); d.Rebuild(); });
			AddSingleComponent<D2dSplitter>(gameObject);
			AddSingleComponent<D2dPolygonCollider>(gameObject);
			AddSingleComponent<Rigidbody2D>(gameObject);
			AddSingleComponent<D2dRetainVelocity>(gameObject);
			AddSingleComponent<D2dCalculateMass>(gameObject);
			AddSingleComponent<D2dDestroyer>(gameObject, d =>
				{
					d.enabled = false;
					d.Fade    = true;
				});
			AddSingleComponent<D2dRequirements>(gameObject, r =>
				{
					var enableMethod   = typeof(D2dDestroyer).GetProperty("enabled").GetSetMethod();
					var enableDelegate = (UnityAction<bool>)System.Delegate.CreateDelegate(typeof(UnityAction<bool>), r.GetComponent<D2dDestroyer>(), enableMethod);

					UnityEditor.Events.UnityEventTools.AddBoolPersistentListener(r.OnRequirementsMet, enableDelegate, true);

					r.AlphaCount    = true;
					r.AlphaCountMin = 0;
					r.AlphaCountMax = 10;
				});
		}

		[MenuItem("CONTEXT/SpriteRenderer/Make Destructible (Preset: Dynamic Detachable)", true)]
		private static bool MakeDestructibleDynDetValidate(MenuCommand menuCommand)
		{
			return AddSingleComponentValidate<D2dDestructibleSprite>(GetGameObject(menuCommand));
		}

		[MenuItem("CONTEXT/SpriteRenderer/Make Destructible (Preset: Dynamic Detachable)", false)]
		private static void MakeDestructibleDetSpl(MenuCommand menuCommand)
		{
			var gameObject = GetGameObject(menuCommand);
			var child      = new GameObject("Fixture");
			var fixture    = child.AddComponent<D2dFixture>();

			child.transform.SetParent(gameObject.transform, false);

			AddSingleComponent<D2dDestructibleSprite>(gameObject, d => { d.ChangeMaterial(); d.Rebuild(); });
			AddSingleComponent<D2dSplitter>(gameObject);
			AddSingleComponent<D2dPolygonCollider>(gameObject);
			AddSingleComponent<Rigidbody2D>(gameObject, r => r.isKinematic = true);
			AddSingleComponent<D2dRetainVelocity>(gameObject);
			AddSingleComponent<D2dCalculateMass>(gameObject);
			AddSingleComponent<D2dDestroyer>(gameObject, d =>
				{
					d.enabled = false;
					d.Fade    = true;
				});
			AddSingleComponent<D2dRequirements>(gameObject, r =>
				{
					var enableMethod   = typeof(D2dDestroyer).GetProperty("enabled").GetSetMethod();
					var enableDelegate = (UnityAction<bool>)System.Delegate.CreateDelegate(typeof(UnityAction<bool>), r.GetComponent<D2dDestroyer>(), enableMethod);

					UnityEditor.Events.UnityEventTools.AddBoolPersistentListener(r.OnRequirementsMet, enableDelegate, true);

					r.AlphaCount    = true;
					r.AlphaCountMin = 0;
					r.AlphaCountMax = 10;
				});
			AddSingleComponent<D2dFixtureGroup>(gameObject, r =>
				{
					var isKinematicMethod   = typeof(Rigidbody2D).GetProperty("isKinematic").GetSetMethod();
					var isKinematicDelegate = (UnityAction<bool>)System.Delegate.CreateDelegate(typeof(UnityAction<bool>), r.GetComponent<Rigidbody2D>(), isKinematicMethod);

					UnityEditor.Events.UnityEventTools.AddBoolPersistentListener(r.OnAllDetached, isKinematicDelegate, false);

					r.Fixtures.Add(fixture);
				});
		}

		[MenuItem("CONTEXT/SpriteRenderer/Make Destructible (Preset: Dynamic Solid)", true)]
		private static bool MakeDestructibleDynSolValidate(MenuCommand menuCommand)
		{
			return AddSingleComponentValidate<D2dDestructibleSprite>(GetGameObject(menuCommand));
		}

		[MenuItem("CONTEXT/SpriteRenderer/Make Destructible (Preset: Dynamic Solid)", false)]
		private static void MakeDestructibleDynSol(MenuCommand menuCommand)
		{
			var gameObject = GetGameObject(menuCommand);

			AddSingleComponent<D2dDestructibleSprite>(gameObject, c => { c.ChangeMaterial(); c.Rebuild(); });
			AddSingleComponent<D2dEdgeCollider>(gameObject);
		}

		[MenuItem("CONTEXT/SpriteRenderer/Make Destructible (Preset: Static)", true)]
		private static bool MakeDestructibleStaValidate(MenuCommand menuCommand)
		{
			return AddSingleComponentValidate<D2dDestructibleSprite>(GetGameObject(menuCommand));
		}

		[MenuItem("CONTEXT/SpriteRenderer/Make Destructible (Preset: Static)", false)]
		private static void MakeDestructibleSta(MenuCommand menuCommand)
		{
			var gameObject = GetGameObject(menuCommand);

			AddSingleComponent<D2dDestructibleSprite>(gameObject, c => { c.ChangeMaterial(); c.Rebuild(); });
			AddSingleComponent<D2dEdgeCollider>(gameObject);
		}

		[MenuItem("CONTEXT/D2dDestructible/Add Fixture")]
		private static void AddFixture(MenuCommand menuCommand)
		{
			var gameObject = GetGameObject(menuCommand);
			var child      = new GameObject("Fixture");

			child.transform.SetParent(gameObject.transform, false);

			child.AddComponent<D2dFixture>();

			Selection.activeGameObject = child;

			EditorGUIUtility.PingObject(child);
		}

		private static bool AddSingleComponentValidate<T>(GameObject gameObject)
			where T : Component
		{
			if (gameObject != null)
			{
				return gameObject.GetComponent<T>() == null;
			}

			return false;
		}

		private static void AddSingleComponent<T>(GameObject gameObject, System.Action<T> action = null)
			where T : Component
		{
			if (gameObject != null)
			{
				if (gameObject.GetComponent<T>() == null)
				{
					var component = Undo.AddComponent<T>(gameObject);

					if (action != null)
					{
						action(component);
					}
				}
			}
		}

		private static GameObject GetGameObject(MenuCommand menuCommand)
		{
			if (menuCommand != null)
			{
				var component = menuCommand.context as Component;

				if (component != null)
				{
					return component.gameObject;
				}
			}

			return null;
		}
	}
}
#endif