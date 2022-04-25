using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class BaseParticleComponent : MonoBehaviour
{

    const float RGB_MAX_VAL = 1.0f / 255.0f;
    [SerializeField] int _numberOfColumns = 180;
    [SerializeField] float _damage = 10.0f;
    [SerializeField] float _speed = .75f;
    [SerializeField] float IncrementingSpeed = 0.0f;

    [SerializeField] Sprite _texture;

    //Iterates through colors when creating particles. Abusing this I can make a colour palette without adding extra calculations in real time.
    [SerializeField]
    Color[] _color = {
    new Color(1.0f, 0, .0f, 0.3f), new Color(.9f, RGB_MAX_VAL*145, .0f, 0.3f),  new Color(1.0f, RGB_MAX_VAL*160, .0f, 0.3f),  new Color(1.0f, RGB_MAX_VAL*150, .0f, 0.3f),
    new Color(1.0f,0, .0f, 0.3f), new Color(.9f, 0, .0f, 0.3f),  new Color(.75f, 0, 0.3f),  new Color(1.0f, RGB_MAX_VAL*130, .0f, 0.3f),
    new Color(1.0f, 0, .0f, 0.3f), new Color(.9f, RGB_MAX_VAL*10, .0f, 0.3f),  new Color(1.0f, RGB_MAX_VAL*160, .0f, 0.3f),  new Color(1.0f, RGB_MAX_VAL*150, .0f, 0.3f)};


    //Constant is done to allow for _emissionDuration to be multiplied by the _firerate to say "I want this to make 6 waves" - As an example. Not needed but useful as a base example. 
    const float FIRE_RATE_BASE = .04f;
    [SerializeField] float _fireRate = FIRE_RATE_BASE;
    [SerializeField] float _fireRateRandomization = FIRE_RATE_BASE * .75f;
    [SerializeField] float _emissionDuration = FIRE_RATE_BASE * 25.0f;

    [SerializeField] float _size = 0.15f * .4f;
    [SerializeField] float _sizeOverLifetime = 0.0f;
    [SerializeField] float _sizeRandomizationPercentage = 0.1f;
    float _angle;
    [SerializeField] float _attackRadius = 360;
    [SerializeField] Material _material;
    protected float time;
    [SerializeField] float rotationSpeed;

    private const float BASE_LIFETIME = .15f;
    [SerializeField] float _lifetime = BASE_LIFETIME;
    [SerializeField] float _lifetimeRandomization = BASE_LIFETIME * 0.95f;


    [SerializeField] LayerMask collisionLayers;
    [SerializeField] string sortingLayerName;

    GameObject ParticleManager;
    private Transform ParticleParent = null;
    private ParticleSystem system;

    System.Func<Vector3> liveTargetPos;

    private ParticleCollisionsComponent ParticleCollisionsClass;

    const int MAX_PARTICLE_COUNT = 1;
    List<Transform>[] organizedChildren;
    List<Transform> childrenInput;
    float scalingMod = 1.0f;


    #region CHILDREN_ID_MANAGER
    List<int> childrenIDs;

    int GetID()
    {
        if (childrenIDs.Count > 0)
        {
            int output = childrenIDs[0];
            childrenIDs.RemoveAt(0);
            return output;
        }
        return -1;
    }
    void AddID(int ID)
    {
        childrenIDs.Add(ID);
    }
    #endregion
    private void Awake()
    {
        organizedChildren = new List<Transform>[MAX_PARTICLE_COUNT];
        childrenIDs = new List<int>();
        for (int i = 0; i < MAX_PARTICLE_COUNT; ++i)
        {
            childrenIDs.Add(i);
        }
    }

    private void FixedUpdate()
    {
        if (rotationSpeed > 0)
        {
            time += Time.deltaTime;
            if (time >= 180.0f) time = 0;
            transform.rotation = Quaternion.Euler(0, 0, time * rotationSpeed);
        }
    }



    public void StartParticleEffect(Transform inputPos /*Parent*/, Transform startPos = null/*StartPosition*/, System.Func<Vector3> TargetPos = null)
    {
        liveTargetPos = TargetPos;

        scalingMod = startPos.transform.localScale.x;

        _damage *= Globals.DifficultyModifier;
        _emissionDuration *= Globals.DifficultyModifier;



        //If this IF statement is not passed then it means that the particle system is either running or does not have enough data.
        if ((startPos != null && inputPos != null))
            StartCoroutine(CreateParticles(inputPos, startPos));
    }
    protected IEnumerator CreateParticles(Transform inputPos, Transform startPos) // If the attack is a ray then it needs to know where to target
    {
      
        int temporaryID = GetID();

        //If ID is invalid then there's too many particles.
        if (temporaryID >= 0)
        {
            childrenInput = new List<Transform>();

            if (organizedChildren[temporaryID] == null || organizedChildren[temporaryID].Count != _numberOfColumns)
            {
                if (organizedChildren[temporaryID] != null)
                {
                    foreach(Transform particles in organizedChildren[temporaryID])
                    Destroy(particles);
                }
                    ParticleParent = inputPos;
                if (_material == null)
                {
                    _material = new Material(Shader.Find("Particles/Standard Unlit"));
                }


                _angle = _attackRadius / _numberOfColumns;

                for (int i = 0; i < _numberOfColumns; ++i)
                {


                    Material particleMaterial = _material;

                    var go = new GameObject("Particle System");
                    childrenInput.Add(go.transform);

                    if (liveTargetPos != null)
                        go.transform.Rotate( (i-_numberOfColumns/2) * _angle,0,0);
                    else
                        go.transform.Rotate(i * _angle, 90, 0);

                    system = go.AddComponent<ParticleSystem>();
                    go.GetComponent<ParticleSystemRenderer>().material = particleMaterial;
                    go.GetComponent<ParticleSystemRenderer>().sortingLayerName = sortingLayerName;

                    go.transform.parent = inputPos;
                    go.transform.position = inputPos.position;
                    go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, -0.1f);


                    //IsRay is asking if the attack 
                    
                    


                    var mainModule = system.main;
                    mainModule.startColor = Color.white;
                    mainModule.startSize = .25f * .25f;
                    mainModule.startSpeed = _speed  * scalingMod;
                  
                    mainModule.scalingMode = ParticleSystemScalingMode.Hierarchy;
                    //If too extreme then it'll need to be simulated. If slow you can just guestimate
                    if (_speed > 0.25 && (_emissionDuration + _lifetime + _lifetimeRandomization + (_fireRateRandomization * (_emissionDuration / _fireRate))) > 1.0)
                        mainModule.cullingMode = ParticleSystemCullingMode.AlwaysSimulate;
                    else
                        mainModule.cullingMode = ParticleSystemCullingMode.Pause;


                    mainModule.maxParticles = (int)(_emissionDuration / _fireRate + _fireRateRandomization) + 1;
                    var emission = system.emission;
                    emission.enabled = false;

                    //This allows for lines to be outputted. 
                    var forma = system.shape;
                    forma.enabled = true;
                    forma.shapeType = ParticleSystemShapeType.Sprite;

                    if (rotationSpeed < 0.0000001f)
                        mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
                    else
                        mainModule.simulationSpace = ParticleSystemSimulationSpace.Local;
                    //Optional include for texture.
                    if (_texture != null)
                    {
                        var tex = system.textureSheetAnimation;
                        tex.enabled = true;
                        tex.mode = ParticleSystemAnimationMode.Sprites;
                        tex.AddSprite(_texture);
                    }

                    if (0 < Mathf.Abs(_sizeOverLifetime))
                    {
                        var sizeOverLife = system.sizeOverLifetime;

                        sizeOverLife.enabled = true;
                        sizeOverLife.sizeMultiplier = _sizeOverLifetime;
                    }

                    if(0 < Mathf.Abs(IncrementingSpeed))
                    {
                        var velOverLifetime = system.velocityOverLifetime;

                        velOverLifetime.enabled = true;
                        velOverLifetime.speedModifier = IncrementingSpeed;

                    }

                    //Collisions
                    if (collisionLayers.value > 0)
                    {

                        ParticleCollisionsClass = go.AddComponent<ParticleCollisionsComponent>();

                        var collision = system.collision;

                        var trigger = system.trigger;
                            trigger.enabled = true;

                        collision.enabled = true;
                        collision.type = ParticleSystemCollisionType.World;
                        collision.mode = ParticleSystemCollisionMode.Collision2D;
                        collision.maxKillSpeed = 10000;
                        collision.radiusScale = 1.0f;
                        collision.quality = ParticleSystemCollisionQuality.High;
                        collision.collidesWith = collisionLayers;
                        collision.maxCollisionShapes = 256;
                        collision.enableDynamicColliders = true;
                        collision.multiplyColliderForceByCollisionAngle = true;
                        collision.sendCollisionMessages = true;
                    }

                }



                organizedChildren[temporaryID] = childrenInput;
            }
            //If there is no lifespan then assume the duration is indefinite. Removes need for a boolean toggle as a duration of 0 is likely to be never used.
            //Random.Range likely makes this a bit more expensive than it would otherwise be but it makes more visually appealing effects when introducing randomization,
            {
                for (int i = 0; _fireRate * i < _emissionDuration; ++i)
                    StartCoroutine(ParticleEmittion((((_fireRate* scalingMod) + Random.Range(-_fireRateRandomization, _fireRateRandomization)) * i), temporaryID));
                //Invoke("ParticleEmittion", organizedChildren.Count-1, (_fireRate + Random.Range(-_fireRateRandomization, _fireRateRandomization)) * i);

                //In theory, assuming a particle spawns at the last instant with the max possible time, the time between its death and startup time is the duration plus its lifespan, with max potential modifiers.

                StartCoroutine(ParticleCleanup((_emissionDuration + _lifetime + _lifetimeRandomization + (_fireRateRandomization * (_emissionDuration / _fireRate * scalingMod))), temporaryID));
            }
        }
        ParticleCollisionsClass.SetDamage(_damage);
        yield return null;
    }

    IEnumerator ParticleCleanup(float delay, int index)
    {
        yield return new WaitForSeconds(delay);
        AddID(index);

        // ParticleParent = null;
    }

   
    IEnumerator ParticleEmittion(float delay, int index)
    {
        yield return new WaitForSeconds(delay);

          if (liveTargetPos != null)
              transform.LookAt(liveTargetPos());

        int gradiantMult = Random.Range(0, _color.Length);
        foreach (Transform child in organizedChildren[index])
        {

            system = child.GetComponent<ParticleSystem>();

            if (system != null)
            {
              //  if (liveTargetPos != null)
              //      child.transform.LookAt(liveTargetPos());
                


                var emitParams = new ParticleSystem.EmitParams();
                emitParams.startSize = (_size* scalingMod) * (1.0f + Random.Range(-_sizeRandomizationPercentage, _sizeRandomizationPercentage)) ;

                if (_lifetimeRandomization > 0.01f)
                    emitParams.startLifetime = _lifetime + Random.Range(-_lifetimeRandomization, _lifetimeRandomization);
                else
                    emitParams.startLifetime = _lifetime;


                emitParams.startColor = _color[gradiantMult];
               
                ++gradiantMult;
                if (gradiantMult >= _color.Length) gradiantMult = 0;

            
                system.Emit(emitParams, 1);

            }


        }
    }
}
