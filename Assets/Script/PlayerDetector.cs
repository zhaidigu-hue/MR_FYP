using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.HandGrab;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    public float detectRadius = 2.63f;
    private bool revealed = false;
    [SerializeField]
    private GameObject GrableBlock;

    private void Start()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
        if (col is SphereCollider sphere)
            sphere.radius = detectRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (revealed) return;
   
        if (other.tag==("Hands"))
        {
            Reveal();
        }
    }

    private void Reveal()
    {
        revealed = true;
   
        foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
            renderer.enabled = true;

        GrableBlock.SetActive(true);

    }
}
