using UnityEngine;

public class OverlayCam : MonoBehaviour {

    public Transform mainCam;

    void Update() {
        // copy rotation of mainCam
        transform.rotation = mainCam.rotation;
    }
}
