using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class Footsteps : MonoBehaviour
{
    public AudioSource footstepsSound, sprintSound, pantingSound;
    [Header("Stamina Main")]
    public float stamina = 100.0f;
    [SerializeField] private float maxStamina = 100.0f;
    [HideInInspector] public bool sprinting = false;

    [Header("Stamina Regen")]
    [Range(0, 50)][SerializeField] private float staminaDrain = 10f;
    [Range(0, 50)][SerializeField] private float staminaRegen = 5f;
    [HideInInspector] public bool isDrained = false;

    [Header("Stamina Speed")]
    [SerializeField] private int normalWalk = 4;
    [SerializeField] private int slowedRun = 3;

    [Header("Stamina UI")]
    [SerializeField] private Image staminaProgress = null;
    [SerializeField] private CanvasGroup sliderCanvasGroup = null;

    private FirstPersonController playerController;
    private StarterAssetsInputs _input;

    private void Awake()
    {
        footstepsSound.volume = 0.5f;
        sprintSound.volume = 0.5f;
        playerController = GetComponent<FirstPersonController>();
        _input = GetComponent<StarterAssetsInputs>();
    }

    void Update()
    {
        // Determinar si el jugador está intentando correr
        sprinting = Input.GetKey(KeyCode.LeftShift) && !isDrained && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D));

        // Habilitar sonido de pasos cuando el jugador se mueve, pero no está corriendo
        footstepsSound.enabled = !sprinting && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) && playerController.Grounded;
        sprintSound.enabled = sprinting && playerController.Grounded && !isDrained;

        if (sprinting && !isDrained)
        {
            StaminaDrain();
        }
        else if (!sprinting)
        {
            StaminaRegain();
        }
    }

    private void StaminaDrain()
    {
        stamina -= staminaDrain * Time.deltaTime;
        stamina = Mathf.Clamp(stamina, 0, maxStamina);
        if (stamina == 0)
        {
            isDrained = true;
            pantingSound.enabled = true;
            playerController.SetMoveSpeed(slowedRun);
        }
        UpdateStaminaUI();
    }

    private void StaminaRegain()
    {
        stamina += staminaRegen * Time.deltaTime;
        stamina = Mathf.Clamp(stamina, 0, maxStamina);
        if (stamina == maxStamina)
        {
            isDrained = false;
            pantingSound.enabled = false;
            playerController.SetMoveSpeed(normalWalk);
        }
        UpdateStaminaUI();
    }

    void UpdateStaminaUI()
    {
        staminaProgress.fillAmount = stamina / maxStamina;
        sliderCanvasGroup.alpha = stamina < maxStamina ? 1 : 0; // Solo muestra la UI de stamina cuando no está llena
    }
}
