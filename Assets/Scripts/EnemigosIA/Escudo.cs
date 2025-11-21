using UnityEngine;

public class Escudo : Enemigo
{
    void Awake()
    {
        esEnemigoEscudo = true;
    }
    void Start()
    {
        GameManager.Instance.ActivarEscudoGlobal();
    }
    public override void TomarDaño(float daño)
    {
        base.TomarDaño(daño);
    }
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
    {
        GameManager.Instance.DesactivarEscudoGlobal();
    }
    }
}
