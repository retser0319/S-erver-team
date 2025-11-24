using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Range : MonoBehaviour
{
    public List<GameObject> targets = new List<GameObject>();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            targets.Add(collision.gameObject);
            SortByDistanceRemaining();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        targets.Remove(collision.gameObject);
        SortByDistanceRemaining();
    }

    private void SortByDistanceRemaining()
    {
        /*
        targets.Sort((a, b) =>
        {
            float distA = a.GetComponent<Nav_AI>().distance;
            float distB = b.GetComponent<Nav_AI>().distance;

            return distA.CompareTo(distB);
        });
        */
    }
}
