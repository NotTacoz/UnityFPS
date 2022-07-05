// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class BulletTrail : MonoBehaviour
// {
//     // define startposition as vector 3 at the coords (-0.034, 0, 0.0053)
//     private Vector3 startPosition = new Vector3(-0.034f, 0, 0.0053f);


//     [Header("Camera")]
//     [SerializeField] private Transform _camera = null;

//     // Update is called once per frame
//     void Update()
//     {
//         // if fire 1, create bullet trail to raycast
//         if (Input.GetButton("Fire1"))
//         {
//             // create bullet trail
//             CreateBulletTrail();
//         }
//     }

//     void CreateBulletTrail() {
//         // create a bullet trail from startposition to where the raycast hit
//         RaycastHit hit;
//         if (Physics.Raycast(_camera.position + startPosition, _camera.forward, out hit, 100))
//         {
//             // create a clone of itself and lerp from startposition to where the raycast hit
//             GameObject clone = Instantiate(gameObject, transform.position, transform.rotation);
//             clone.transform.position = Vector3.Lerp(transform.position, hit.point, 10f * Time.deltaTime);
//             // destroy the clone after 1 second
//             Destroy(clone, 1f);
//         }
//     }
// }
