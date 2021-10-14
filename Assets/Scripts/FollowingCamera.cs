using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    // Camera variables
    [SerializeField] float cameraHeight = 8.0f;
    [SerializeField] float cameraSpeed = 100.0f;
    [SerializeField] Vector3 cameraEntityOffset;
    [SerializeField] float cameraForwardOffset = 1.0f;
    [SerializeField] float cameraForwardAimingOffset = 3.0f;
    //[SerializeField] CCrosshair crosshair;
    //[SerializeField] Material effectMaterial;
    //PostProcessVolume postProcessing;
    //ColorGrading colorGrading;
    //Vignette vignette;
    float defaultVignette;
    Camera camera;

    // Camera states
    bool shaking = false;
    bool aiming = false;
    bool explosion = false;
    bool explosionReversed = false;

    // Camera data
    public Entity targetToFollow { get; protected set; }
    Vector3 moveTo;
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
    void Start()
    {
        originPoint = gameObject.GetComponentInParent<Transform>();
        camera = GetComponent<Camera>();
        //postProcessing = GetComponent<PostProcessVolume>();

        // Get colour grading
        //postProcessing.profile.TryGetSettings(out colorGrading);
        //colorGrading.saturation.overrideState = true;
        //colorGrading.contrast.overrideState = true;

        // Get vignette
        //postProcessing.profile.TryGetSettings(out vignette);
        //defaultVignette = vignette.intensity.value;
    }

    // Update is called once per frame
    void Update()
    {
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
                  
            if (aiming)
            {
                moveTo += targetToFollow.transform.forward * cameraForwardAimingOffset;
            }
            else
            {
                moveTo += targetToFollow.transform.forward * cameraForwardOffset;
            }

            moveTo.z = cameraHeight;
           
        }


        // Convert the position from screen space to world space
        //crosshair.SetPosition((camera.ScreenToWorldPoint(mousePos)));

        if (targetToFollow != null)
        {
            originPoint.position = Vector3.MoveTowards(originPoint.position, moveTo, cameraSpeed * Time.deltaTime);
        }

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

    //public Vector3 GetCrosshairPosition()
    //{
    //    return crosshair.transform.position;
    //}

    //public void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    Graphics.Blit(source, destination, effectMaterial);
    //}

    //public void SetSaturation(float saturation)
    //{
    //    colorGrading.saturation.value = saturation;
    //}
    //
    //public void SetContrast(float contrast)
    //{
    //    colorGrading.contrast.value = contrast;
    //}
    //
    //public void SetAdditionalVignette(float amount)
    //{
    //    vignette.intensity.value = defaultVignette + amount;
    //}

    public void Aim(bool aim)
    {
        aiming = aim;
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
