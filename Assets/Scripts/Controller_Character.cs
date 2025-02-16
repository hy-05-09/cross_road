using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Controller_Character : MonoBehaviour
{
   public GameManager gameManager;

   void OnTriggerEnter(Collider col) { //내장함수
      if(col.tag == "Deadline"){
         gameManager.Result();
      }
   }
}
