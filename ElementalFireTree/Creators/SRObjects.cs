using System;
using System.Collections.Generic;
using SRML.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SRML
{
	public static class SRObjects
	{

		public static Sprite MissingIcon { get; private set; } = Get<Sprite>("unknownSmall");

		public static Sprite MissingImg { get; private set; } = Get<Sprite>("unknownLarge");

		public static GameObject BaseLargo { get; internal set; } = PrefabUtils.CopyPrefab(Get<GameObject>("slimePinkTabby"));

		static SRObjects()
		{
			Object.Destroy(BaseLargo.GetComponent<PinkSlimeFoodTypeTracker>());
			Object.Destroy(BaseLargo.GetComponent<StalkConsumable>());
			Object.Destroy(BaseLargo.GetComponent<GatherIdentifiableItems>());
			Object.Destroy(BaseLargo.GetComponent<AttackPlayer>());
		}

		public static T Get<T>(string name) where T : Object
		{
			foreach (T found in Resources.FindObjectsOfTypeAll<T>())
			{
				bool flag = found.name.Equals(name);
				if (flag)
				{
					return found;
				}
			}
			return default(T);
		}

		public static Object Get(string name, Type type)
		{
			foreach (Object found in Resources.FindObjectsOfTypeAll(type))
			{
				bool flag = found.name.Equals(name);
				if (flag)
				{
					return found;
				}
			}
			return null;
		}

		public static List<T> GetAll<T>() where T : Object
		{
			return new List<T>(Resources.FindObjectsOfTypeAll<T>());
		}

		public static T GetInst<T>(string name) where T : Object
		{
			foreach (T found in Resources.FindObjectsOfTypeAll<T>())
			{
				bool flag = found.name.Equals(name);
				if (flag)
				{
					return Object.Instantiate<T>(found);
				}
			}
			return default(T);
		}

		public static T GetInst<T>(string name, GameObject parent) where T : Object
		{
			foreach (T found in Resources.FindObjectsOfTypeAll<T>())
			{
				bool flag = found.name.Equals(name);
				if (flag)
				{
					return Object.Instantiate<T>(found, parent.transform);
				}
			}
			return default(T);
		}

		public static Object GetInst(string name, Type type)
		{
			foreach (Object found in Resources.FindObjectsOfTypeAll(type))
			{
				bool flag = found.name.Equals(name);
				if (flag)
				{
					return Object.Instantiate(found);
				}
			}
			return null;
		}

		public static T GetWorld<T>(string name) where T : Object
		{
			foreach (T found in Object.FindObjectsOfType<T>())
			{
				bool flag = found.name.Equals(name);
				if (flag)
				{
					return found;
				}
			}
			return default(T);
		}

		public static Object GetWorld(string name, Type type)
		{
			foreach (Object found in Object.FindObjectsOfType(type))
			{
				bool flag = found.name.Equals(name);
				if (flag)
				{
					return found;
				}
			}
			return null;
		}

		public static List<T> GetAllWorld<T>() where T : Object
		{
			return new List<T>(Object.FindObjectsOfType<T>());
		}
	}
}
