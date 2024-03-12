using UnityEngine;

public class TransformData
{
    public Vector3 worldPosition;
    public Quaternion worldRotation;
    public Vector3 localPosition;
    public Quaternion localRotation;
    public Vector3 localScale;
    public Transform parent;

    public TransformData(Transform trans)
    {
        worldPosition = trans.position;
        worldRotation = trans.rotation;
        localPosition = trans.localPosition;
        localRotation = trans.localRotation;
        localScale = trans.localScale;
        parent = trans.parent;
    }

    public void InsertWorldData(Transform trans)
    {
        trans.position = worldPosition;
        trans.rotation = worldRotation;
        trans.localScale = localScale;
    }

    public void InsertLocalData(Transform trans)
    {
        trans.localPosition = localPosition;
        trans.localRotation = localRotation;
        trans.localScale = localScale;
    }

    public void SetParent(Transform trans, bool worldPositionStay = false)
    {
        trans.SetParent(parent, worldPositionStay);
    }
}
