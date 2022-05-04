using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : Controller
{
    [SerializeField] public KeyCode ActivationKey = KeyCode.E;
    protected EntityManager entityManager;

    [SerializeField] public float interactionRadius = .25f;
    GameObject playerRef;

    // Text to show when interacting
    [SerializeField] string interactText;
    [SerializeField] EditableText interactPrefab;
    EditableText text;

    // Start is called before the first frame update
    private void Start()
    {
        text = Instantiate(interactPrefab);
        text.transform.parent = transform;
        text.transform.localPosition = interactPrefab.transform.position;
        text.SetText(interactText);
    }


    protected virtual void ActivateInteraction()
    {

    }
    protected virtual void EndInteraction()
    {

    }

    protected bool IsPlayerWithinRadius()
    {
        if (text == null)
        {
            text = Instantiate(interactPrefab);
            text.transform.parent = transform;
            text.transform.localPosition = interactPrefab.transform.position;
            text.SetText(interactText);
        }

        float collRad = interactionRadius * ((transform.localScale.x + transform.localScale.y));

        //Doesn  float collRad = interactionRadius * ((transform.localScale.x + transform.localScale.y));'t have to be accurate. Just a general sphere that scales with the object.
        //If distance is less than the stored radius.
        if (collRad >= Vector3.Distance(transform.position, playerRef.transform.position))
        {
           if (entityManager)
            {
                text.TextVisible(true);
            }
            return true;
        }
        else
        {
            text.TextVisible(false);

            if (Input.GetKeyDown(ActivationKey))
            {
                EndInteraction();
            }
            return false;
        }

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
