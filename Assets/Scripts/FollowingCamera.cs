using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class FollowingCamera : MonoBehaviour
{
    // Camera variables
    [SerializeField] float cameraHeight = 8.0f;
    [SerializeField] float cameraSpeed = 100.0f;
    [SerializeField] Vector3 cameraEntityOffset;
    [SerializeField] float cameraForwardOffset = 1.0f;
    [SerializeField] float cameraForwardAimingOffset = 3.0f;
    [SerializeField] GameObject deathScreen;
    [SerializeField] TMPro.TextMeshPro currency;

    Crosshair crosshair;
    //[SerializeField] Material effectMaterial;
    PostProcessVolume postProcessing;
    ColorGrading colorGrading;
    Vignette vignette;
    LensDistortion lansDistortion;
    float defaultLansDistortion;
    float defaultVignette;
    float defaultSaturation;
    Camera camera;

    // Camera states
    bool shaking = false;
    bool aiming = false;
    bool explosion = false;
    bool explosionReversed = false;

    // Camera data
    public Entity targetToFollow { get; protected set; }
    Vector3 moveTo;
    Vector3 teleportTo;
    Transform originPoint; // Point you attach the camera through (separate to the camera object to allow shaking)
    Vector3 mousePos;
    Vector3 shakeOffset;

    // Explosion variables
    Vector3 explosivePosition;
    Vector3 explosionOffset;
    float explosionDuration = 0.5f;
    float explosionTimer = 0.0f;
    float explosionStrength = 0.25f;

    // Start is called before the first frame update

    #region FollowingCamera_Singleton

    //There is only ever one instance of this class and it can be used to reference information so making it a singleton should be justified.
    public static FollowingCamera instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion


    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        Setup();
    }
    void Setup()
    {
        originPoint = gameObject.GetComponentInParent<Transform>();
        camera = GetComponent<Camera>();
        crosshair = FindObjectOfType<Crosshair>();
        postProcessing = GetComponent<PostProcessVolume>();

        // Get colour grading
        postProcessing.profile.TryGetSettings(out colorGrading);
        colorGrading.saturation.overrideState = true;
        defaultSaturation = colorGrading.saturation;
        colorGrading.contrast.overrideState = true;

        // Get vignette
        postProcessing.profile.TryGetSettings(out vignette);
        defaultVignette = vignette.intensity.value;

        // Get lens distortion
        postProcessing.profile.TryGetSettings(out lansDistortion);
        defaultLansDistortion = lansDistortion.intensity.value;
    }

    // Update is called once per frame
    void Update()
    {
        if (crosshair == null)
        {
            Setup();
            return;
        }
        // Update the crosshair
        // Get the mouse position
        mousePos = Input.mousePosition;

        // Subtract the screen size from the mouse position
        mousePos.x = Mathf.Abs(mousePos.x);
        mousePos.y = Mathf.Abs(mousePos.y);

        // Set the height of the mouse to the camera height
        mousePos.z = cameraHeight;

        // Follow the player
        if (targetToFollow != null)
        {
            moveTo = targetToFollow.transform.position + cameraEntityOffset;

            Vector3 distance = (crosshair.transform.position - targetToFollow.transform.position);

            distance.x = Mathf.Clamp(distance.x, -Screen.width * 1.1f, Screen.width * 1.1f);
            distance.y = Mathf.Clamp(distance.y, -Screen.height * 1.1f, Screen.height * 1.1f);

            if (aiming)
            {
                moveTo += distance * cameraForwardAimingOffset;
            }
            else
            {
                moveTo += distance * cameraForwardOffset;
            }

            moveTo.z = cameraHeight;

        }



        if (targetToFollow != null)
        {
            originPoint.position = Vector3.MoveTowards(originPoint.position, moveTo, cameraSpeed * Time.deltaTime);
        }

    }

    public void CameraUpdate()
    {
        // Convert the position from screen space to world space
        if (camera != null && crosshair != null)
            crosshair.SetPosition((camera.ScreenToWorldPoint(mousePos)));
    }


    void FixedUpdate()
    {
        // Apply the explosion effect to the camera itself and not the origin
        if (explosion)
        {
            if (explosionReversed)
            {
                transform.position -= explosionOffset;
            }
            else
            {
                transform.position += explosionOffset;
            }
            explosionReversed = !explosionReversed;

            if (explosionTimer < explosionDuration)
            {
                explosionTimer += Time.fixedDeltaTime;
            }
            else
            {
                explosionTimer = 0.0f;
                explosion = false;
            }

        }

        // Apply shaking to the camera itself and not the origin
        if (shaking)
        {
            transform.position += shakeOffset;
            shaking = false;
        }
    }

    public void SetTarget(Entity target)
    {
        if (target != null)
        {
            targetToFollow = target;
        }
    }

    public void Shake(float intensity, Vector3 direction)
    {
        shakeOffset = direction * intensity;
        shaking = true;
    }

    public Vector3 GetCrosshairPosition()
    {
        return crosshair.transform.position;
    }

    public Transform GetCrosshairTransform()
    {
        return crosshair.transform;
    }

    public void Teleport(Vector2 position)
    {
        teleportTo = position;
        teleportTo.z = transform.position.z;
        originPoint.position = teleportTo;
        transform.position = teleportTo;
        moveTo = teleportTo;
    }

    //public void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    Graphics.Blit(source, destination, effectMaterial);
    //}

    public void SetSaturation(float saturation)
    {
        colorGrading.saturation.value = saturation;
    }

    public void ResetSaturation()
    {
        colorGrading.saturation.value = defaultSaturation;
    }

    public void SetContrast(float contrast)
    {
        colorGrading.contrast.value = contrast;
    }

    public void SetAdditionalVignette(float amount)
    {
        vignette.intensity.value = defaultVignette + amount;
    }

    public void SetLensDistortion(float amount)
    {
        lansDistortion.intensity.value = amount;
    }

    public void Aim(bool aim)
    {
        aiming = aim;
    }

    public void ShowDeathScreen()
    {
        deathScreen.active = true;
    }

    public void HideDeathScreen()
    {
        deathScreen.active = false;
    }

    public void UpdateCurrency(string newCurrency)
    {
        currency.text = newCurrency;
    }

    //public void Explosion(Vector3 explosionPosition, float intensity)
    //{
    //    // Only shake the camera if the explosion happened on screen
    //    explosivePosition = camera.WorldToViewportPoint(explosionPosition);
    //    if (explosivePosition.z > 0 && explosivePosition.x > 0 && explosivePosition.x < 1 && explosivePosition.y > 0 && explosivePosition.y < 1)
    //    {
    //        explosionOffset = Vector3.up * intensity * explosionStrength;
    //        explosion = true;
    //    }
    //}
}
