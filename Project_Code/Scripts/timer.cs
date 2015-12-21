using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class timer : MonoBehaviour {

     public TextMesh timerLabel;
 
     private float time;
 
     void LateUpdate() {

         time += Time.deltaTime;

         var hours = time/3600;
         var minutes = (time / 60) % 60;
         var seconds = time % 60;

         timerLabel.text = string.Format ("{0:00} : {1:00} : {2:00}", hours, minutes, seconds);
     }
 }