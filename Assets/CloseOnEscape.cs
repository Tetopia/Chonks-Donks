using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseOnEscape : MonoBehaviour
{
    private void OnEnable()
    {
        EscListener.i.escapeListeners.Add(Close);
    }

    private void OnDisable()
    {
        EscListener.i.escapeListeners.Remove(Close);
    }

    void Close()
    {
        gameObject.SetActive(false);
    }
}
