using UnityEngine;

public class InelasticCollisionPassThrough : MonoBehaviour
{
    public GameObject cubeRight;

    void OnCollisionEnter(Collision collision)
    {
        // Prüfen, ob das kollidierte Objekt eine Rigidbody-Komponente hat
        if (collision.rigidbody != null)
        {
            cubeRight.GetComponent<CubeRightController>().phase = CubeRightController.Phase.inelasticCollision;

            // Ein neues Fixed Joint hinzufügen
            FixedJoint fixedJoint = gameObject.AddComponent<FixedJoint>();

            // Das Joint mit dem kollidierten Objekt verbinden
            fixedJoint.connectedBody = collision.rigidbody;

            // Optionale Einstellungen für das Joint
            fixedJoint.breakForce = Mathf.Infinity; // Unzerstörbares Joint
            fixedJoint.breakTorque = Mathf.Infinity; // Unzerstörbares Joint

            // Die Objekte verbinden
            fixedJoint.connectedMassScale = 1;
            fixedJoint.massScale = 1;
        }
    }
}
