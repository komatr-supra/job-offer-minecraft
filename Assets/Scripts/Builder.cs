using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Builder : MonoBehaviour
{
    [SerializeField] private Transform lookTransform;

    public void OnMainAction(InputValue inputValue)
    {
        Debug.Log("main" + inputValue.isPressed);
    }
    public void OnSecondaryAction(InputValue inputValue)
    {
        Debug.Log("sec" + inputValue.isPressed);
    }
}
