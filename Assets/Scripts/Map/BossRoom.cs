using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : Room
{
    [SerializeField] Portal portalPrefab;
    Portal returnPortal;

    // Start is called before the first frame update
    void Start()
    {
        returnPortal = Instantiate<Portal>(portalPrefab);
        returnPortal.transform.position = GetRandomGroundPosition();
        returnPortal.Enable(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void EntityRemoved()
    {
        base.EntityRemoved();
        if (wasCleared)
        {
            Globals.DifficultyModifier += 0.1f;
            PlayerPrefs.SetFloat(Globals.PLAYER_DIFFICULTY_SAVE, Globals.DifficultyModifier);
            returnPortal.Enable(true);
        }
    }

    protected override void CreateEntity()
    {
        currentEntityID = entityManager.TryCreateEnemy(map.GetRandomBoss(), GetRandomGroundPosition());
        if (currentEntityID != -1)
        {
            currentNPC = entityManager.GetEntity(currentEntityID) as NotPlayer;
            currentNPC.AddRoom(this);
            ++enemiesAlive;
        }
    }
}
