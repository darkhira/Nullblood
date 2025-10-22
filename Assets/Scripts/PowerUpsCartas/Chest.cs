using UnityEngine;

public class Chest : MonoBehaviour
{
    private bool isOpened = false;

    // Puedes a�adir referencias a un Sprite de cofre abierto, part�culas, etc.
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
        Debug.Log("�Cofre abierto! Elige tu recompensa.");

        // Aqu� est� la magia: le decimos al GameManager que inicie la selecci�n de cartas.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameManager.GameState.CardSelection);
        }

        // L�gica visual adicional (opcional)
        // Por ejemplo, cambiar el sprite al de un cofre abierto.
        // GetComponent<SpriteRenderer>().sprite = openChestSprite;

        // O desactivar el objeto para que no se pueda volver a clickear.
        // gameObject.SetActive(false);
    }
}