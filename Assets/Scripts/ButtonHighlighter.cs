using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHighlighter : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Button button;
    private Image image;

    [Header("Colores del botón")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    [Header("Escala al seleccionar")]
    public Vector3 selectedScale = new Vector3(1.2f, 1.2f, 1f);

    private Vector3 originalScale;

    void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        originalScale = transform.localScale;

        // Asegura el color inicial
        if (image != null)
            image.color = normalColor;
    }

    // Se llama cuando el botón es seleccionado (por mando o teclado)
    public void OnSelect(BaseEventData eventData)
    {
        if (image != null)
            image.color = selectedColor;

        transform.localScale = selectedScale;
    }

    // Se llama cuando el botón deja de estar seleccionado
    public void OnDeselect(BaseEventData eventData)
    {
        if (image != null)
            image.color = normalColor;

        transform.localScale = originalScale;
    }
}
