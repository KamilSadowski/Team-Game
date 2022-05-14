using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EntityManager : MonoBehaviour
{
    const bool IS_DEBUGGING = true;
    const int ENTITY_LIMIT = 250;

    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject[] NPCList;
    [SerializeField] public GameObject[] WeaponList;
    [SerializeField] public GameObject[] PropList;
    [SerializeField] public GameObject[] PickupList;

    List<Entity> entities = new List<Entity>();
    Stack<int> entitySlots = new Stack<int>();
    Entity tempEntity;
    GameObject tempEntityGameObject;
    MovementComponent canThrow;
    private int ID;

    int playerID;
    Player player;

    #region Singleton_EntityManager
    public static EntityManager instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    // Creates a player with his weapons
    public int TryCreatePlayer(Vector3 position)
    {
        if (!player)
        {
            playerID = TryCreateEntity(playerPrefab, position);
            if (playerID != -1)
            {
                player = entities[playerID] as Player;
                player.Create(playerID);

            }
            return playerID;
        }
        return -1;
    }

    public int FindWeaponInList(SpriteRenderer image)
    {
        SpriteRenderer output;
        int i = 0;
        foreach (GameObject obj in WeaponList)
        {
            //Sprite renderer should always be stored ina  child to allow for rotations.
            output = obj.GetComponentInChildren<SpriteRenderer>();
            if (output)
            {
                if (output.sprite == image.sprite)
                    return i;

                output = null;
            }
            ++i;
        }

        return -1;
    }

    public int TryCreateListedWeapon(int index, Vector3 Position)
    {
        if(index >= 0 && index < WeaponList.Length)
            return TryCreateEntity(WeaponList[index], Position, index);

        //-1 is commonly used as "Invalid"
        return -1;
    }

    public int TryCreateRandomListedWeapon(Vector3 Position)
    {
        ID = Random.Range(0, WeaponList.Length);
            return TryCreateEntity(WeaponList[ID], Position, ID);

        //-1 is commonly used as "Invalid"
        return -1;
    }

    public int TryCreateListedProjectile(int index, Vector3 Position, Vector3 NormalDirection, float force)
    {
        if (index >= 0 && index < WeaponList.Length) {
            return TryCreateMovingEntity(WeaponList[index], Position, NormalDirection, force, index); 
        }
        //-1 is commonly used as "Invalid"
        return -1;
    }

    public int TryCreateListedNPC(int index, Vector3 Position)
    {
        if (index >= 0 && index < NPCList.Length)
            return TryCreateEntity(NPCList[index], Position, index);

        //-1 is commonly used as "Invalid"
        return -1;
    }

    public int TryCreateRandomListedNPC(Vector3 Position)
    {
        int ID = Random.Range(0, NPCList.Length);
        if (ID >= 0 && ID < NPCList.Length)
            return TryCreateEntity(NPCList[ID], Position , ID);

        //-1 is commonly used as "Invalid"
        return -1;
    }

    public int TryCreateEnemy(GameObject enemy, Vector3 Position)
    {
        return TryCreateEntity(enemy, Position);
    }

    public int TryCreateListedProp(int index, Vector3 Position)
    {
        if (index >= 0 && index < PropList.Length)
            return TryCreateEntity(PropList[index], Position, index);

        //-1 is commonly used as "Invalid"
        return -1;
    }

    // Returns false if failed
    public int TryCreateEntity(GameObject entity, Vector3 position, int TemplateID = -1)
    {
        //-1 is commonly used as "Invalid"
        Debug.Log(entity.name + " " + entitySlots.Count);
        if (entitySlots.Count == 0)
        {
           
            return -1;
        }

        tempEntityGameObject = Instantiate<GameObject>(entity);
        tempEntityGameObject.TryGetComponent<Entity>(out tempEntity);
        tempEntityGameObject.transform.position = position;

        if(IS_DEBUGGING)
            tempEntityGameObject.name += ID;

        if (tempEntity)
        {
            ID = entitySlots.Peek();
            tempEntity.Create(entitySlots.Peek(), TemplateID);
            entities[ID] = tempEntity;
            entitySlots.Pop();

            tempEntity = null;
            tempEntityGameObject = null;
            return ID;
        }

        Destroy(tempEntityGameObject);
        tempEntity = null;
        tempEntityGameObject = null;
        return -1;

    }

    // Adds an already existing entity to the manager, returns -1 if it cannot be added and destroys the object
    public int TryCreateEntity(Entity entity)
    {
        //-1 is commonly used as "Invalid"
        if (entitySlots.Count == 0)
        {
            return -1;
        }

        if (entity)
        {
            ID = entitySlots.Peek();
            entity.Create(entitySlots.Peek());
            entities[ID] = entity;
            entitySlots.Pop();

            return ID;
        }

        Destroy(entity);
        return -1;
    }
    public int TryCreateRandomListedPickup(Vector3 Position)
    {
        int number = Random.Range(0, PickupList.Length);
        return TryCreateEntity(PickupList[number], Position);
    }

    public int TryCreateMovingEntity(GameObject entity, Vector3 position, Vector3 Direction, float force, int index = -1)
    {
        ID = TryCreateEntity(entity, position, index);
        tempEntity = GetEntity(ID);

        if (tempEntity != null)
        {
            canThrow = tempEntity.GetComponent<MobileComponent>();

            if (canThrow != null)
            {
                canThrow.Move(Direction * force);
            }

        }
        
        tempEntity = null;
        tempEntityGameObject = null;
        return ID;
    }



    // Setting the bool will skip destroying if its to be done manually
    public void DeleteEntity(int id, bool destroy = true)
    {
        // Destroy the entity if not destroyed
        if (entities[id] != null && destroy)
        {
            //GameObject temp = entities[id].gameObject;
            entities[id].OnRemove();
            Destroy(entities[id].gameObject);
            //Destroy(temp);
        }

        entitySlots.Push(id);
    }

    public Entity GetEntity(int id)
    {
        if (id >= 0 && id < entities.Count)
            return entities[id];

        return null;
    }

    void OnEnable()
    {
        for (int i = 0; i < ENTITY_LIMIT; i++)
        {
            entities.Add(null);
            entitySlots.Push(i);
        }

        //If any of the following are invalid then this IF statement is TRUE;
        if (
            -1 != TryCreatePlayer(Vector3.zero))
            //&& 
            //-1 != TryCreateListedNPC(0, Vector3.left))
        { }
    }

}
