using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : Controller
{
    [SerializeField] public KeyCode ActivationKey = KeyCode.E;
    [SerializeField] public GameObject InteractionEntity;
    protected EntityManager entityManager;

    [SerializeField] public float interactionRadius = .25f;
    GameObject playerRef;

    protected bool interactionSpawned = false;
    protected int interactionID = -1;

    const float interactionBaseScale = 0.1875f;
    protected Vector3 newScale;
    protected GameObject objectRef;
    // Start is called before the first frame update

    

    protected virtual void ActivateInteraction()
    {

    }
    protected virtual void EndInteraction()
    {

    }

    protected bool IsPlayerWithinRadius()
    {

        if (!InteractionEntity) return false;
        float collRad = interactionRadius * ((transform.localScale.x + transform.localScale.y));
       


        //Doesn  float collRad = interactionRadius * ((transform.localScale.x + transform.localScale.y));'t have to be accurate. Just a general sphere that scales with the object.
        //If distance is less than the stored radius.
        if (collRad >= Vector3.Distance(transform.position, playerRef.transform.position))
        {
           if (entityManager && !interactionSpawned)
            {
                if (objectRef != null)
                {
                    objectRef.SetActive(true);
                }
                else
                {
                    //Set NewScale as custom scale so it can be used in Vector multiplication
                    newScale.Set(interactionBaseScale, interactionBaseScale, interactionBaseScale);


                    interactionID = entityManager.TryCreateEntity(InteractionEntity, transform.position + (Vector3.up * interactionBaseScale * 1.5f));
                    objectRef = entityManager.GetEntity(interactionID).gameObject;//
                    objectRef.transform.localScale = newScale * transform.localScale.x;
                    objectRef.transform.SetParent(gameObject.transform);
                }
                interactionSpawned = true;
            }
            return true;
        }
        else if(interactionSpawned)
        {
            objectRef.SetActive(false);// = ;
            //entityManager.DeleteEntity(interactionID);
            interactionSpawned = false;

            if (Input.GetKeyDown(ActivationKey))
            {
                EndInteraction();
            }
            return true;

        }
        return false;

    }
    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionRadius * ((transform.localScale.x + transform.localScale.y)));
    }
    protected virtual void InheritedUpdate()
    {
    }
    // Update is called once per frame
    void Update()
    {
        InheritedUpdate();

        if (entityManager == null)
        {
            entityManager = FindObjectOfType<EntityManager>();
        }

        if (!isValidReferences())
        {
            BindVariables();
        }

        if (!playerRef)
        {
            playerRef = PriorityChar_Manager.instance.getPlayer();
        }
        else if(IsPlayerWithinRadius())
        {
            if (Input.GetKeyDown(ActivationKey))
            {
                ActivateInteraction();
            }
     
        }
    }
}
