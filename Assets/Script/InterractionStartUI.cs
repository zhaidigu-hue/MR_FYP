using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class InterractionStartUI : MonoBehaviour
{
    [SerializeField]
    GameObject Thus;
    [SerializeField]
    GameObject ThusInByrner;
    [SerializeField]
    TransitionManager transitionManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "StartButton")
        {
            Thus.SetActive(false);
            ThusInByrner.SetActive(true);

            transitionManager.StartTransition();
        }
    }
}
