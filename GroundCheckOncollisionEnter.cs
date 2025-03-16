using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public bool grounded = false;
    public Transform groundCheck; // Asegúrate de que sea del tipo Transform

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && grounded) // jump code
        {
            GetComponent<Rigidbody>().AddForce(transform.up * 3, ForceMode.Impulse);

        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        grounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        grounded = false;
    }
}
