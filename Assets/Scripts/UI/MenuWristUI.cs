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
    [HideInInspector] public List<Image> images;
    HashSet<Transform> visitedObjects = new HashSet<Transform>();

    void Start()
    {
        CollectImagesRecursive(transform);
    }

    void Update()
    {
        Vector3 DirectionToCamera = VectorUtils.Direction(Forward.position, CentreCamera.position);
        Debug.LogError("Angle: " + Vector3.Angle(Forward.forward, DirectionToCamera));
        if (Vector3.Angle(Forward.forward, DirectionToCamera) < Threshhold)
        {
            Debug.LogError("In threshold! Angle: " + Vector3.Angle(Forward.forward, DirectionToCamera));
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }

            foreach (var image in images)
            {
                var tempColor = image.color;
                tempColor.a = Mathf.Lerp(0, 1, 1 - (Mathf.Abs(Vector3.Angle(Forward.forward, DirectionToCamera)) / Threshhold));
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

    void CollectImagesRecursive(Transform parent)
    {
        visitedObjects.Add(parent);
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (!visitedObjects.Contains(child))
            {
                Image imageComponent = child.GetComponent<Image>();
                if (imageComponent != null)
                {
                    images.Add(imageComponent);
                }
                CollectImagesRecursive(child);
            }
        }
    }
}
