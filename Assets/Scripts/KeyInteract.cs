using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInteract : MonoBehaviour, IInteractable
{
    public AudioSource src;
    [SerializeField] GameObject keyObject;
    [SerializeField] GameObject minimapObject;
    public void Interact()
    {
        if (keyObject.GetComponent<Renderer>().enabled == true)
        {
            src.time = 0.1f;
            src.Play();
            keyObject.GetComponent<Renderer>().enabled = false;
            keyObject.GetComponent<BoxCollider>().enabled = false;
            minimapObject.SetActive(false);
            // Emitir el evento de que la llave ha sido recogida.
            EventManager.KeyCollected();
        } 
    }
}
