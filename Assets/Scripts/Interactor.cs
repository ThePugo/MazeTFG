using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

interface IInteractable
{
    public void Interact();
}

public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;
    public GameObject keyText;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                keyText.GetComponent<TextMeshProUGUI>().enabled = true;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    keyText.GetComponent<TextMeshProUGUI>().enabled = false;
                    interactObj.Interact();
                }
            }
            else
            {
                keyText.GetComponent<TextMeshProUGUI>().enabled = false;
            }
        }
    }
}
