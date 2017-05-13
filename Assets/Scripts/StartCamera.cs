using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCamera : MonoBehaviour {
    public GameObject ListaPasos, Camera;

    [HideInInspector]
    public bool activated = true;
    private Transform[] coordenadas;
    private Vector3 step;
    private int iteracion = 0;
    private float elapsedTime = 2f;
	// Use this for initialization
	void Start () {
        coordenadas = ListaPasos.GetComponentsInChildren<Transform>();
        step = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
        if (activated) {
            elapsedTime -= Time.deltaTime;
            if (elapsedTime < 0) {
                iteracion++;
                if (iteracion < coordenadas.Length)
                {
                    elapsedTime = 1.7f;
                    //step = (coordenadas[iteracion].position - coordenadas[iteracion - 1].position) / elapsedTime;
                    step = (coordenadas[iteracion].position - Camera.transform.position) / elapsedTime;
                }
                else
                {
                    //GetComponentInParent<Drive>().hasBroom = true;
                    //GameObject.FindGameObjectWithTag("Command").GetComponent<Commands>().startRecCommand("up");
                    activated = false;
                }
            }
            else {
                Camera.transform.position += step * Time.deltaTime;
            }
        }
	}
}
