using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DvalinTest : MonoBehaviour
{
    void Start()
    {
        print("Hello this is DvalinTest speaking!");

        StartCoroutine(TickCoroutine());
    }

    void Tick()
    {
        print("DvalinTest.Tick()");
    }

    IEnumerator TickCoroutine()
    {
        Tick();

        yield return new WaitForSeconds(5);

        StartCoroutine(TickCoroutine());
    }
}
