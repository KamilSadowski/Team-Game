using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EntityManager : MonoBehaviour
{
    const int ENTITY_LIMIT = 20;

    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject[] NPCList;
    [SerializeField] GameObject[] WeaponList;
    [SerializeField] GameObject[] PropList;

    List<Entity> entities = new List<Entity>();
    Stack<int> entitySlots = new Stack<int>();
    Entity tempEntity;
    GameObject tempEntityGameObject;
    MovementComponent canThrow;
    private int ID;

    public int TryCreatePlayer(Vector3 Position)
    {
        return TryCreateEntity(playerPrefab, Position);
    }


    public int TryCreateListedWeapon(int index, Vector3 Position)
    {
        if(index >= 0 && index < WeaponList.Length)
        return TryCreateEntity(WeaponList[index], Position);

        //-1 is commonly used as "Invalid"
        return -1;
    }

    public int TryCreateListedProjectile(int index, Vector3 Position, Vector3 NormalDirection, float force)
    {
        if (index >= 0 && index < WeaponList.Length)
            return TryCreateMovingEntity(WeaponList[index], Position, NormalDirection, force);

        //-1 is commonly used as "Invalid"
        return -1;
    }

    public int TryCreateListedNPC(int index, Vector3 Position)
    {
        if (index >= 0 && index < NPCList.Length)
            return TryCreateEntity(NPCList[index], Position);

        //-1 is commonly used as "Invalid"
        return -1;
    }

    public int TryCreateListedProp(int index, Vector3 Position)
    {
        if (index >= 0 && index < PropList.Length)
            return TryCreateEntity(PropList[index], Position);

        //-1 is commonly used as "Invalid"
        return -1;
    }


    // Returns false if failed
    public int TryCreateEntity(GameObject entity, Vector3 position)
    {
        //-1 is commonly used as "Invalid"
        if (entitySlots.Count == 0)
        {
            return -1;
        }

        tempEntityGameObject = Instantiate<GameObject>(entity);
        tempEntityGameObject.TryGetComponent<Entity>(out tempEntity);
        tempEntityGameObject.transform.position = position;

        if (tempEntity != null)
        {
            ID = entitySlots.Peek();
            tempEntity.Create(entitySlots.Peek());
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

        if (entity != null)
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

    public int TryCreateMovingEntity(GameObject entity, Vector3 position, Vector3 Direction, float force)
    {
        //-1 is commonly used as "Invalid"
        if (entitySlots.Count == 0)
        {
            return -1;
        }

        tempEntityGameObject = Instantiate<GameObject>(entity);
        tempEntityGameObject.TryGetComponent<Entity>(out tempEntity);
        tempEntityGameObject.transform.position = position;

        canThrow = tempEntityGameObject.GetComponent<MobileComponent>();

        if (tempEntity != null)
        {

            ID = entitySlots.Peek();
            if (canThrow != null)
            {
                canThrow.Move(Direction * force);
            }

            tempEntity.Create(entitySlots.Peek());
            entities[ID] = tempEntity;
            entitySlots.Pop();

            return ID;
        }

        Destroy(tempEntityGameObject);
        tempEntity = null;
        tempEntityGameObject = null;
        return -1;
    }



    // Setting the bull will skip destroying if its to be done manually
    public void DeleteEntity(int id, bool destroy = true)
    {
        // Destroy the entity if not destroyed
        if (entities[id] != null && destroy)
        {
            //GameObject temp = entities[id].gameObject;
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

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < ENTITY_LIMIT; i++)
        {
            entities.Add(null);
            entitySlots.Push(i);
        }

        //If any of the following are invalid then this IF statement is TRUE;
        if (-1 != TryCreateListedWeapon(0, Vector3.forward) 
            && 
            -1 != TryCreatePlayer(Vector3.zero)
            && 
            -1 != TryCreateListedNPC(0, Vector3.left))
        { }
    }

}
