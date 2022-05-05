
using UnityEngine;

public class Player : Character
{
    //The player will have modifiers for weapons but the WeaponController will spawn and manage the weapons. User stats can be added at a later date. m
    [SerializeField] protected WeaponController equipmentManager;


    // const int WEAPON_COUNT = 2;
    // [SerializeField] protected int weaponCount = 2;
    // protected int weaponsUsed = 0;
    // [SerializeField] protected float weaponMaxCooldown;
    // protected float WeaponCharge = 0;

    protected EntityManager entitySpawner;

    [SerializeField] protected float CHARGE_STRENGTH_MOD = 10.0f;
    [SerializeField] protected float MAX_STRENGTH_MOD = 2.0f; //120% currently.
    [SerializeField] protected float BASE_STRENGTH = 5.0f;
    protected float curStrength = .0f;

    protected Vector3 SpawnPosition;

    Crosshair crosshair;
    GameManager gameManager;

    //Direct reference to the progress bars held within the UI
    UI_ChargingBar weaponCharge_UI;
    private Animator animator;

    public bool isDead { get; private set; } = false;
    const float deathDuration = 3.0f;
    Timer killTimer = new Timer(deathDuration);

    FollowingCamera camera;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        crosshair = PriorityChar_Manager.instance.getCrosshair();
        entitySpawner = GameObject.FindWithTag("GameController").GetComponent<EntityManager>();
        curStrength = BASE_STRENGTH;


        // Get colours from stored
        string c = PlayerPrefs.GetString(Globals.PLAYER_COLOUR_SAVE);
        if (c is { Length: > 0 })
        {
            PlayerColourSave s = JsonUtility.FromJson<PlayerColourSave>(c);
            if (s != null)
            {
                colours = s.Colours;
                ChangeColours(colours[0], colours[1], colours[2]);
            }
        }
    }

    private void Update()
    {
        if (Debug.isDebugBuild)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                TakeDamage(10.0f, transform.position, Vector3.zero);
            }
        }

        if (isDead)
        {
            if (killTimer.Update(Time.deltaTime))
            {
                // If player is killed, he is taken to the hubworld
                if (!gameManager)
                {
                    gameManager = FindObjectOfType<GameManager>();
                }

                if (gameManager)
                {
                    camera.SetAdditionalVignette(0.0f);
                    camera.ResetSaturation();
                    camera.HideDeathScreen();
                    isDead = false;
                    killTimer.Reset(deathDuration);
                    gameManager.EnterScene(Globals.Scenes.HubWorld);
                }
            }
        }
        else
        {
            UpdateEntity();

            if (!crosshair)
                crosshair = PriorityChar_Manager.instance.getCrosshair();
            //Weapon controller please.
            if (!animator)
            {
                animator = GetComponent<Animator>();
            }
            if (equipmentManager)
            {
                equipmentManager.PlayerUpdate();
            }
        }
    }
    public int GetSavedWeaponTemplateID()
    {
        string weaponClassy = PlayerPrefs.GetString(Globals.PLAYER_WEAPON_SAVE);
        PlayerWeaponSaves s = JsonUtility.FromJson<PlayerWeaponSaves>(weaponClassy);
        if (s == null || s.WeaponTemplateID < 0)
            return 0;
        return s.WeaponTemplateID;
    }

    public GameObject WeaponLoad(GameObject obj)
    {
        var weapon = obj.GetComponent<Weapon>();
        var weapCont = obj.GetComponent<WeaponController>();
        var mobileComp = obj.GetComponent<MobileComponent>();


        if (weapon && weapCont && mobileComp)
        {
            string weaponClassy = PlayerPrefs.GetString(Globals.PLAYER_WEAPON_SAVE);
            PlayerWeaponSaves s = JsonUtility.FromJson<PlayerWeaponSaves>(weaponClassy);
            if (s == null)
                return obj;

            obj.transform.localScale = s.Scale;
            //Weapon class
            weapon.setBounceStr(s.bounceStr);
            weapon.setSharpness(s.sharpness);
            weapon.setDamage(s.damage);

            //Weapon controller
            weapCont.setWeaponCooldown(s.WeaponCD);
            weapCont.SetWeaponCost(s.WeaponCost);
            weapCont.SetWeaponReward(s.WeaponPickupReward);

            //Mobile component
            mobileComp.setMovementSpeed(s.MovementSpeed);
            mobileComp.setDrag(s.Drag);

            //Colours
            //Can assume all 3 will be null or not null. (Primary/Secondary/Tetiary)
            if (s.PrimaryColoursRGB != null)
            {
                Color[] output = new Color[3];

                output[0] = s.PrimaryColoursRGB;
                output[1] = s.SecondaryColoursRGB;
                output[2] = s.TetiaryColoursRGB;
 
                weapon.UpdateColour(output);

            }
        }
        return obj;
    }

    private void WeaponSave(GameObject obj)
    {
        var weapon = obj.GetComponent<Weapon>();
        var weapCont = obj.GetComponent<WeaponController>();
        var mobileComp = obj.GetComponent<MobileComponent>();


        if (weapon && weapCont && mobileComp)
        {
            PlayerWeaponSaves weaponClass = new PlayerWeaponSaves();

            weaponClass.WeaponTemplateID = weapon.templateID;
            weaponClass.Scale = weapon.gameObject.transform.localScale;
            //Weapon class
            weaponClass.bounceStr = weapon.getBounceStr();
            weaponClass.sharpness = weapon.getSharpness();
            weaponClass.damage = weapon.getDamage();

            //Weapon controller
            weaponClass.WeaponCD = weapCont.getWeaponCooldown();
            weaponClass.WeaponCost = weapCont.getWeaponCost();
            weaponClass.WeaponPickupReward = weapCont.getWeaponReward();

            //Mobile component
            weaponClass.MovementSpeed = mobileComp.getMovementSpeed();
            weaponClass.Drag = mobileComp.getDrag();

            //Colours
            //Can assume all 3 will be null or not null. (Primary/Secondary/Tetiary)
            if (weaponClass.PrimaryColoursRGB != null)
            {
                 Color[] colors = weapon.GetColours();

                weaponClass.PrimaryColoursRGB   = colors[0];
                weaponClass.SecondaryColoursRGB = colors[1];
                weaponClass.TetiaryColoursRGB   = colors[2];

                //Alpha is automatically set to 0 without interference.
                colors[0].a = 1.0f;
                colors[1].a = 1.0f;
                colors[2].a = 1.0f;

                weapon.UpdateColour(colors);
            }
   

            // Save it
            string json = JsonUtility.ToJson(weaponClass);
            PlayerPrefs.SetString(Globals.PLAYER_WEAPON_SAVE, json);
        }
    }


    //Weapon controller please.
    public void ReleaseWeapon()
    {
        //Animations are organized here. Easier to force a level of charge to be present.
        ChargeWeapon();

        if (equipmentManager != null && crosshair != null)
        {

            equipmentManager.ThrowWeapon(curStrength, crosshair.GetPosition());
            curStrength = BASE_STRENGTH;

            if (animator)
            {
                animator.SetTrigger("Throw");
                animator.SetBool("Attack", false);
            }

        }
    }

    public void ChargeWeapon()
    {
        if (animator)
        {
            // Avoid multiple calculations of the same thing
            curStrength += Time.deltaTime * CHARGE_STRENGTH_MOD;

            animator.SetFloat("AttackAnimationSpeed", Time.deltaTime * CHARGE_STRENGTH_MOD);

            var isPlaying = animator.GetBool("Attack");

            // Play the animation attacking if it is not playing - Not sure if it really works
            if (!isPlaying) animator.SetBool("Attack", true);

            // If either of the weapons are fully charged 
            if (curStrength > BASE_STRENGTH * MAX_STRENGTH_MOD)
            {
                curStrength = BASE_STRENGTH * MAX_STRENGTH_MOD;
                // Stop the throwing animation
                animator.SetFloat("AttackAnimationSpeed", 0f);
            }

        }
    }
    /*
    public void ChargeWeapon()
    {
        if (IsWeaponAvailable())
        {
            // Avoid multiple calculations of the same thing
            var maxCharge = strength * MAX_FORCE_MOD;

            //If the current value is less than the max value (See. If Statement) then increase the stored "Charge" by your strength
            //Delta time is applied to avoid dividing anything by 50 (Fixed update should be done 50 times a second but it can be adjusted, hence the time calculation)
            if (WeaponCharge < maxCharge) WeaponCharge += strength * Time.deltaTime;

            //Directly update the progress bar to avoid the usage of "Update" - Only updating it when a change is made.
            if (weaponCharge_UI != null)
                weaponCharge_UI.UpdateProgBar(WeaponCharge / (maxCharge));
        }
    }
    */

    public bool PickupNewWeapon(WeaponController weapon)
    {
        //Mostly safety checks to see if anything has not been set and if there is no weapon held, at the moment in time. 
        if (weapon != null)
        {
            if (equipmentManager)
                equipmentManager.GetComponent<Entity>().DestroyEntity();
            equipmentManager = weapon;

            //Disable weapon to remove lifespan and use as reference gameobject for future weapons.
            weapon.DisableWeapon();

            //Save weapon in file.
            WeaponSave(weapon.gameObject);
            return true;
        }

        return false;
    }

    public Weapon getEquippedWeapon()
    {
        if (equipmentManager && equipmentManager.GetBoundWeapon())
        {
            return equipmentManager.GetBoundWeapon().GetComponent<Weapon>();
        }
        return null;
    }


    public int getEquippedWeaponID()
    {
        return entityID;
    }

    public void SpawnWeaponPickup()
    {
        entitySpawner.TryCreateRandomListedWeapon(crosshair.GetPosition());
    }

    public void SpawnWeaponPickupAt(Vector3 position)
    {
        entitySpawner.TryCreateRandomListedWeapon(position);
    }

    public void SpawnEnemyTarget()
    {
        entitySpawner.TryCreateRandomListedNPC(crosshair.GetPosition());
    }

    public void SpawnRandomPickup()
    {
        entitySpawner.TryCreateRandomListedPickup(crosshair.GetPosition());
    }

    public override void TakeDamage(float damage, Vector3 sourcePosition, Vector3 sourceVelocity)
    {
        base.TakeDamage(damage, sourcePosition, sourceVelocity);
    }

    public void ForceKill()
    {
        if (!healthComponent)
        {
            healthComponent = GetComponent<BaseHealthComponent>();
        }
        else
        {
            MortalHealthComponent tmpHealthComponent = healthComponent as MortalHealthComponent;
            tmpHealthComponent.SetInvincible(false);
            TakeDamage(tmpHealthComponent.GetHealth() + 1.0f, transform.position, Vector3.zero);
        }
    }

    public override bool DestroyEntity()
    {
        isDead = true;
        CheckCamera();
        camera.SetAdditionalVignette(0.7f);
        camera.SetSaturation(-100.0f);
        camera.ShowDeathScreen();
        return true;
    }

    void CheckCamera()
    {
        if (!camera)
        {
            camera = FindObjectOfType<FollowingCamera>();
        }
    }
}