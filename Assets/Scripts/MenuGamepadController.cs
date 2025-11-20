using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuGamepadController : MonoBehaviour
{
    public GameObject firstSelectedButton; // Primer botón seleccionado al abrir el menú
    private InputAction navigateAction;
    private InputAction submitAction;
    private PlayerInput playerInput;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        // Selecciona automáticamente el primer botón
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    void Update()
    {
        // Si no hay nada seleccionado (por ejemplo, tras usar ratón), vuelve a seleccionar
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }
}
