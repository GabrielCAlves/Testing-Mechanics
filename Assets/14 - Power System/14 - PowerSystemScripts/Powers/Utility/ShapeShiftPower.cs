// ShapeShiftPower.cs
using FreeflowCombatSpace;
using SceneScript;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShapeShiftPower", menuName = "Powers/Utility/Shape Shift Power")]
public class ShapeShiftPower : Power
{
    [Header("Configurações de Mudança de Forma")]
    public GameObject[] alternateForms;
    public float transformDuration = 0.5f;
    public GameObject transformEffect;
    public AudioClip transformSound;
    public bool inheritStats = true;

    private int currentFormIndex = -1;
    private GameObject currentFormObject;
    private GameObject userObject;
    private Vector3 originalScale;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        NextForm(user);
    }

    void NextForm(GameObject user)
    {
        if (alternateForms.Length == 0) return;

        userObject = user;
        originalScale = user.transform.localScale;

        // Remove forma atual
        if (currentFormObject != null)
        {
            Destroy(currentFormObject);
        }

        // Seleciona próxima forma
        currentFormIndex = (currentFormIndex + 1) % alternateForms.Length;
        GameObject newForm = alternateForms[currentFormIndex];

        // Instancia nova forma
        currentFormObject = Instantiate(newForm, user.transform.position, user.transform.rotation);
        currentFormObject.transform.localScale = originalScale;

        // Esconde o jogador original
        user.SetActive(false);

        // Transfere componentes importantes
        if (inheritStats)
        {
            TransferComponents(user, currentFormObject);
        }

        // Efeitos
        if (transformEffect != null)
        {
            Instantiate(transformEffect, user.transform.position, Quaternion.identity);
        }

        if (transformSound != null)
        {
            AudioSource.PlayClipAtPoint(transformSound, user.transform.position);
        }
    }

    void TransferComponents(GameObject from, GameObject to)
    {
        // Transfere Health
        var fromHealth = from.GetComponent<Health>();
        var toHealth = to.GetComponent<Health>();
        if (fromHealth != null && toHealth != null)
        {
            toHealth.currentHealth = fromHealth.currentHealth;
            toHealth.maxHealth = fromHealth.maxHealth;
        }

        // Transfere movimento
        var fromMovement = from.GetComponent<PlayerMovement>();
        var toMovement = to.GetComponent<PlayerMovement>();
        if (fromMovement != null && toMovement != null)
        {
            toMovement.moveSpeed = fromMovement.moveSpeed;
            toMovement.jumpForce = fromMovement.jumpForce;
        }
    }

    public void UpdateShapeShift(GameObject user)
    {
        if (currentFormObject != null)
        {
            // Sincroniza posição com o usuário original (que está invisível)
            currentFormObject.transform.position = user.transform.position;
            currentFormObject.transform.rotation = user.transform.rotation;
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);

        if (currentFormObject != null)
        {
            Destroy(currentFormObject);
            currentFormObject = null;
        }

        user.SetActive(true);
        currentFormIndex = -1;
    }
}