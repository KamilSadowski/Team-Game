using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntityManager : MonoBehaviour
{
    const int ENTITY_LIMIT = 2;

    [SerializeField] GameObject playerPrefab;

    List<Entity> entities = new List<Entity>();
    Stack<int> entitySlots = new Stack<int>();
    Entity tempEntity;
    GameObject tempEntityGameObject;


    public bool TryCreatePlayer()
    {
        return TryCreateEntity(playerPrefab, Vector3.zero);
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

        TryCreatePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
