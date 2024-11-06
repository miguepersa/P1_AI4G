using UnityEngine;

public class Button : MonoBehaviour
{
    public string targetTag = "NPC";  // Tag of the character to detect (e.g., "Player" or "Enemy")

    [SerializeField] private NPC prisoner;
    [SerializeField] private NPC gunner;
    private void OnTriggerEnter(Collider other)
    {

        prisoner.findPath = true;
        gunner.findPath = true;
        gunner.seek = true;
    }

}
