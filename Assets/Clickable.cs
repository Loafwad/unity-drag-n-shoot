using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    [SerializeField] private Reticle reticleManager;
    private void OnMouseDown()
    {
        reticleManager.Selected(this.gameObject);
    }

    private void OnMouseUp()
    {
        reticleManager.Deselect();
    }
}
