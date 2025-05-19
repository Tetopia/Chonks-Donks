

using System.Collections.Generic;
using UnityEngine;

public delegate void VoidDelegateVoid();
public delegate void VoidDelegateFloat(float f);
public delegate void VoidDelegateFloatFloat(float f, float g);

public static class Utils
{
	public static List<T> GetComponentsInChildrenRecursively<T>(this Transform _transform, List<T> _componentList)
	{
		foreach (Transform t in _transform)
		{
			T[] components = t.GetComponents<T>();

			foreach (T component in components)
			{
				if (component != null) _componentList.Add(component);
			}
			GetComponentsInChildrenRecursively<T>(t, _componentList);
		}
		return _componentList;
	}

	public static T GetComponentInChildrenRecursively<T>(this Transform _transform)
	{
		foreach (Transform t in _transform)
		{
			T component = t.GetComponent<T>();
			if (component != null) return component;

			return GetComponentInChildrenRecursively<T>(t);
		}
		return default(T);
	}
}
