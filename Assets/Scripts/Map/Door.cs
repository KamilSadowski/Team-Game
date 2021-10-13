using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] protected Globals.Direction direction;
    public Globals.Grid2D roomLinked;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Globals.Direction GetDirection()
    {
        return direction;
    }
}
