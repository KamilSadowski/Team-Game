using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : Controller
{
    protected EntityManager entityManager;

    // Start is called before the first frame update
    void Start()
    {
        entityManager = GameObject.FindWithTag("GameController").GetComponent<EntityManager>();
        BindVariables();

        entityMoveComp = GetComponent<MovementComponent>();
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        BindVariables();
    }

    //The above two functions will be the same within every level of inheritance. 








}
   