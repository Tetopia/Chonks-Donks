using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscListener : MonoBehaviour
{
	public static EscListener i;

	public List<VoidDelegateVoid> escapeListeners = new();

	void Awake()
	{
		if (i != null && i != this)
		{
			Destroy(this);
			Debug.LogWarning("EscListener can only exist once! Destroying this instance");
			return;
		}
		i = this;
	}

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
			if (escapeListeners.Count > 0)
			{
				VoidDelegateVoid cancelationAction = escapeListeners[escapeListeners.Count - 1];
				cancelationAction();
			}
        }
    }
}
