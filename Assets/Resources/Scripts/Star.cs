using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : IObject
{

    public GameObject g;
    private Vector3 minV;
    private Vector3 maxV;
    public Vector3 center;
    public Vector3 position;

    public int cadran;
    public float m;
    public bool group;

    public int getCadran()
    {
        return this.cadran;
    }

    public Vector3 getMinV()
    {
        return minV;
    }

    public Vector3 getMaxV()
    {
        return maxV;
    }

    public float getM()
    {
        return m;
    }

    public void addM(float m)
    {
        this.m+=m;
    }

    public void setCadran(Vector3 minV,Vector3 maxV)
    {
        this.minV = minV;
        this.maxV = maxV;
        center = (minV + maxV) / 2;

        if (this.position.x < center.x) this.cadran = 1;
        else cadran = 2;

        if (this.position.y > center.y) this.cadran += 2;

        if (this.position.z > center.z) this.cadran += 4;
    }

    public void newMinMaxCadran()
    {
        center = (this.minV + this.maxV) / 2;

        this.position = new Vector3(g.transform.position.x, g.transform.position.y, g.transform.position.z);

        if (this.position.x < center.x) this.maxV.x=center.x;
        else this.minV.x = center.x;

        if (this.position.y < center.y) this.maxV.y = center.y;
        else this.minV.y = center.y;

        if (this.position.z < center.z) this.maxV.z = center.z;
        else this.minV.z = center.z;

        setCadran(minV, maxV);
    }

    public Star(GameObject g, Vector3 minV, Vector3 maxV, float m)
    {
        this.g = g;
        this.position = new Vector3(g.transform.position.x, g.transform.position.y, g.transform.position.z);
        this.minV = minV;
        this.maxV = maxV;
        this.m = m;
        this.group = false;

        setCadran(minV, maxV);

        /*Debug.Log("x : " + g.transform.position.x + ", y : " + g.transform.position.y + ", z : " + g.transform.position.z);

        Debug.Log("centre : " + center);

        Debug.Log("cadran : " + cadran);*/
    }

    public bool isGroup()
    {
        return this.group;
    }

    public Star newStar(Vector3 minV, Vector3 maxV, float m)
    {
        return new Star(minV, maxV, m);
    }

    public Star(Vector3 minV, Vector3 maxV, float m)
    {
        this.minV = minV;
        this.maxV = maxV;
        this.m = m;
        this.group = true;
        setCadran(minV,maxV);
}
}