using UnityEngine;
using UnityEngine.Events;
using ViveSR.anipal.Eye;

public class DwellTimeButton : MonoBehaviour
{
    [Header("Eye Tracking")]
    [SerializeField] protected InteractionEyeTracker eyeTracker;

    [Header("Timing Settings")]
    [SerializeField] private float dwellTime = 0.8f;
    [SerializeField] private float cooldownTime = 1f;

    [Header("Events")]
    public UnityEvent onDwellComplete;
    public UnityEvent onGazeEnter;
    public UnityEvent onGazeExit;

    // Internal state
    private bool isGazeHitting = false;
    private bool isOnCooldown = false;
    private float dwellTimer = 0f;
    private float cooldownTimer = 0f;

    // Visual feedback
    private Renderer rend;
    private Material material;
    private Color originalColor;
    private Collider interactionCollider;

    protected virtual void Start()
    {
        // Find eye tracker if not assigned
        if (eyeTracker == null)
        {
            eyeTracker = FindObjectOfType<InteractionEyeTracker>();
            if (eyeTracker == null)
            {
                Debug.LogError("No InteractionEyeTracker found in the scene!", this);
                enabled = false;
                return;
            }
        }

        // Get required components
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            material = rend.material;
            originalColor = material.color;
        }
        interactionCollider = GetComponent<Collider>();
    }

    protected virtual void Update()
    {
        // Update gaze detection
        bool wasHitting = isGazeHitting;
        isGazeHitting = CheckGazeHit();

        // Handle gaze enter/exit events
        if (isGazeHitting && !wasHitting)
            onGazeEnter.Invoke();
        else if (!isGazeHitting && wasHitting)
            onGazeExit.Invoke();

        // Handle cooldown
        if (isOnCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= cooldownTime)
            {
                isOnCooldown = false;
                cooldownTimer = 0f;
            }
            return;
        }

        // Handle dwell timer and feedback
        if (isGazeHitting)
        {
            dwellTimer += Time.deltaTime;
            float progress = dwellTimer / dwellTime;
            UpdateVisualFeedback(progress);

            if (dwellTimer >= dwellTime)
            {
                OnDwellComplete();
                ResetDwell();
                StartCooldown();
            }
        }
        else
        {
            dwellTimer = 0f;
            UpdateVisualFeedback(0f);
        }
    }

    private bool CheckGazeHit()
    {
        try
        {
            Vector3 userPosition = eyeTracker.worldPosition;
            Vector3 fixationPoint = eyeTracker.gazeLocation;
            Vector3 direction = (fixationPoint - userPosition);

            if (direction != Vector3.zero)
            {
                direction.Normalize();
                Ray ray = new Ray(userPosition, direction);
                return interactionCollider.Raycast(ray, out _, Mathf.Infinity);
            }
        }
        catch { }
        return false;
    }

    protected virtual void UpdateVisualFeedback(float progress)
    {
        if (material != null)
        {
            Color newColor = originalColor;
            newColor.a = 1f - progress;
            material.color = newColor;
        }
    }

    protected virtual void OnDwellComplete()
    {
        onDwellComplete.Invoke();
    }

    private void ResetDwell()
    {
        dwellTimer = 0f;
        UpdateVisualFeedback(0f);
    }

    private void StartCooldown()
    {
        isOnCooldown = true;
        cooldownTimer = 0f;
    }

    // Public methods for external control
    public virtual void Enable()
    {
        enabled = true;
    }

    public virtual void Disable()
    {
        enabled = false;
        ResetDwell();
    }

    // Getter methods for state
    public bool IsGazeHitting => isGazeHitting;
    public bool IsOnCooldown => isOnCooldown;
    public float DwellProgress => dwellTimer / dwellTime;
}