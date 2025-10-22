using UnityEngine;

public class Chest : MonoBehaviour
{
    private bool isOpened = false;

    // Puedes añadir referencias a un Sprite de cofre abierto, partículas, etc.
    // [SerializeField] private Sprite openChestSprite;

    private void OnMouseDown()
    {
        if (isOpened)
        {
            Debug.Log("Este cofre ya ha sido abierto.");
            return;
        }

        OpenChest();
    }

    private void OpenChest()
    {
        isOpened = true;
        Debug.Log("¡Cofre abierto! Elige tu recompensa.");

        // Aquí está la magia: le decimos al GameManager que inicie la selección de cartas.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameManager.GameState.CardSelection);
        }

        // Lógica visual adicional (opcional)
        // Por ejemplo, cambiar el sprite al de un cofre abierto.
        // GetComponent<SpriteRenderer>().sprite = openChestSprite;

        // O desactivar el objeto para que no se pueda volver a clickear.
        // gameObject.SetActive(false);
    }
}