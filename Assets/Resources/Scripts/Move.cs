using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

    GameObject s1;
    GameObject s2;
    float d;

    StarOne so;
    StarTwo st;

    float ao;
    float at;

    Vector3 v;

    public GameObject nouveau;

    public float G = 0.02f;

    // Use this for initialization
    void Start () {

        s1 =GameObject.Find("S1");
        s2=GameObject.Find("S2");

        Debug.Log(s1.transform.position);
        Debug.Log(s2.transform.position);

        //Test star1 = s1.GetComponent<Test>();

        so = s1.GetComponent<StarOne>();
        st = s2.GetComponent<StarTwo>();

        so.speed = Vector3.zero;
        st.speed = Vector3.back * 0.12f;

        //nouveau = Instantiate(Resources.Load(path: "Prefabs/star"), new Vector3(5, 5, 5), Quaternion.identity) as GameObject;

        GameObject go;

        //go = Instantiate(Resources.Load(path: "Prefabs/star"), Random.insideUnitSphere * 10.0f, Quaternion.identity) as GameObject;

        Vector3 minV = new Vector3(-10.0f, -10.0f, -10.0f);

        Vector3 maxV = new Vector3(10.0f, 10.0f, 10.0f);

        go = Instantiate(Resources.Load(path: "Prefabs/star"), new Vector3(0,0,0), Quaternion.identity) as GameObject;
        Star s = new Star(minV, maxV, 0.0f);
        Tree<Star> arbre = new Tree<Star>(s);
        
        Debug.Log("cadran : " + s.cadran);

        go = Instantiate(Resources.Load(path: "Prefabs/star"), new Vector3(8, 8, 5), Quaternion.identity) as GameObject;
        s = new Star(go, minV, maxV, 23.0f);
        Debug.Log("cadran : " + s.cadran);
        arbre.addNodeToTree(2, s.cadran, 1, s);

        go = Instantiate(Resources.Load(path: "Prefabs/star"), new Vector3(4, 4, 5), Quaternion.identity) as GameObject;
        s = new Star(go, minV, maxV, 17.0f);
        Debug.Log("cadran : " + s.cadran);
        arbre.addNodeToTree(2, s.cadran, 1, s);

        /*arbre.addNodeToTree(1, 2, 1, s);
        arbre.addNodeToTree(1, 3, 1, s);
        arbre.addNodeToTree(1, 4, 1, s);*/




        // Create a Tree of 'Strings' 
        /*Tree<string> myTreeString = new Tree<string>("Stepa");  /* A Sample Tree with 'String' Nodes  */
        /*myTreeString.addNodeToTree(2, 1, 1, "Tatiana");
        myTreeString.addNodeToTree(2, 2, 1, "Diana");
        myTreeString.addNodeToTree(2, 3, 1, "Ardalion");*/
    }

	// Update is called once per frame
	/*void Update () {

        // s1.transform.Translate(Vector3.down * Time.deltaTime);
        d = Vector3.Distance(s1.transform.position, s2.transform.position);

        ao = G * st.m / (d * d);

        v = (s2.transform.position - s1.transform.position); 

        v.Normalize();
        
        v*=ao;

        so.speed = so.speed + v;

        //Debug.Log("la vitesse : " + so.speed);

        s1.transform.Translate(so.speed);

        //Debug.Log("la distance est de : " + d);

        at = G * so.m / (d * d);

        v = (s1.transform.position - s2.transform.position);

        v.Normalize();

        v*= at;

        st.speed = st.speed + v;

        //Debug.Log("la vitesse : " + st.speed);

        s2.transform.Translate(st.speed);

        //Debug.Log("la distance est de : " + d);

    }*/
}
