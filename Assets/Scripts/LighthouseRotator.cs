using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LighthouseRotator : MonoBehaviour {

    [SerializeField] private float speed;

    private float targetYaw;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        targetYaw += Time.deltaTime * speed;
        transform.localRotation = Quaternion.Euler(0, targetYaw, 0);
    }
    
}
