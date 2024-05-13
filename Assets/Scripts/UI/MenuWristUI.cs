using NuiN.NExtensions;
using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuWristUI : MonoBehaviour
{
    [SerializeField] float Threshhold = 60f;
    [SerializeField] Transform CentreCamera;
    [SerializeField] Transform Forward;
    public List<Image> images;

    void Start()
    {
        CollectImagesRecursive(transform);
    }

    void Update()
    {
        Vector3 DirectionToCamera = VectorUtils.Direction(Forward.position, CentreCamera.position);
        UpdateColledImagesRecursive();
        if (Vector3.Angle(Forward.forward, DirectionToCamera) < Threshhold)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }

            foreach (var image in images)
            {
                var tempColor = image.color;
                tempColor.a = Mathf.Lerp(0, 1, 2 - (Mathf.Abs(Vector3.Angle(Forward.forward, DirectionToCamera)) * 2f / (Threshhold)));
                image.color = tempColor;
            }
        }
        else
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }

            foreach (var image in images)
            {
                var tempColor = image.color;
                tempColor.a = 0;
                image.color = tempColor;
            }
        }
    }

    public void UpdateColledImagesRecursive(List<GameObject> addImage)
    {
        images.Clear();
        foreach (var image in addImage)
        {
            images.Add(image.GetComponent<Image>());
        }
        CollectImagesRecursive(transform);
    }

    public void UpdateColledImagesRecursive()
    {
        images.Clear();
        CollectImagesRecursive(transform);
    }

    void CollectImagesRecursive(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            Image imageComponent = child.GetComponent<Image>();
            if (imageComponent != null)
            {
                images.Add(imageComponent);
            }
            CollectImagesRecursive(child);
        }
    }
}
