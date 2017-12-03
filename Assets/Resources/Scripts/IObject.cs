using UnityEngine;

public interface IObject
{
    void newMinMaxCadran();
    int getCadran();
    Vector3 getMinV();
    Vector3 getMaxV();
    float getM();
    void addM(float m);
    bool isGroup();
    Star newStar(Vector3 minV, Vector3 maxV, float m);
}