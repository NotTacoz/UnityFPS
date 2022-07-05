using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // change rotation based on time
        transform.rotation = Quaternion.Euler(0, Time.time * 1f, Time.time * 1f);
    }
}
