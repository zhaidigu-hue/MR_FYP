using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ScenceManager : MonoBehaviour
{
   public void ChangeToStartScence()
    {
        SceneManager.LoadScene("TowerTest");
    }
    
}
