using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dvalin
{
    public class DvalinPieceTest : MonoBehaviour
    {
        void Start()
        {
            Logger.LogInfo("Hello this is DvalinPieceTest speaking!");

            StartCoroutine(TickCoroutine());
        }

        void Tick()
        {
            Logger.LogInfo("DvalinPieceTest.Tick()");
        }

        IEnumerator TickCoroutine()
        {
            Tick();

            yield return new WaitForSeconds(5);

            StartCoroutine(TickCoroutine());
        }
    }
}