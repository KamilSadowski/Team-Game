using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntityManager : MonoBehaviour
{
    const int ENTITY_LIMIT = 64;

    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject[] NPCList;
    [SerializeField] GameObject[] WeaponList;
    [SerializeField] GameObject[] PropList;

    List<Entity> entities = new List<Entity>();
    Stack<int> entitySlots = new Stack<int>();
    Entity tempEntity;
    GameObject tempEntityGameObject;


    public bool TryCreatePlayer(Vector3 Position)
    {
        return TryCreateEntity(playerPrefab, Position);
    }


    public bool TryCreateListedWeapon(int index, Vector3 Position)
    {
        if(index >= 0 && index < WeaponList.Length)
        return TryCreateEntity(WeaponList[index], Position);

        return false;
    }

    public bool TryCreateListedNPC(int index, Vector3 Position)
    {
        if (index >= 0 && index < NPCList.Length)
            return TryCreateEntity(NPCList[index], Position);

        return false;
    }

    public bool TryCreateListedProp(int index, Vector3 Position)
    {
        if (index >= 0 && index < PropList.Length)
            return TryCreateEntity(PropList[index], Position);

        return false;
    }


    // Returns false if failed
    public bool TryCreateEntity(GameObject entity, Vector3 position)
    {
        if (entitySlots.Count == 0)
        {
            return false;
        }

        tempEntityGameObject = Instantiate<GameObject>(entity);
        tempEntityGameObject.TryGetComponent<Entity>(out tempEntity);
        tempEntityGameObject.transform.position = position;

        if (tempEntity != null)
        {
            tempEntity.Create(entitySlots.Peek());
            entities.Add(tempEntity);
            entitySlots.Pop();
        }
        else
        {
            Destroy(tempEntityGameObject);
            return false;
        }

        tempEntity = null;
        tempEntityGameObject = null;
        return true;
    }

    // Setting the bull will skip destroying if its to be done manually
    public void DeleteEntity(int id, bool destroy = true)
    {
        // Destroy the entity if not destroyed
        if (entities[id] != null && destroy)
        {
            Destroy(entities[id]);
        }

        entitySlots.Push(id);
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < ENTITY_LIMIT; i++)
        {
            entitySlots.Push(i);
        }

        bool loadupCheck = true;

        //If any of the following are false then this IF statement is TRUE;
        if (!(
            TryCreateListedWeapon(0, Vector3.forward) 
            &&
            TryCreatePlayer(Vector3.zero) 
            && TryCreateListedNPC(0, Vector3.left)
            ))
        { }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
