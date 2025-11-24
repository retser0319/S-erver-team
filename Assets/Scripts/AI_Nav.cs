using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Nav_AI : MonoBehaviour
{

}
public class Node
{
    public Vector2 pos;
    public Node parent;

    public Node(Vector2 p, Node par = null)
    {
        pos = p;
        parent = par;
    }
}