using NuiN.ScriptableHarmony.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialSound : MonoBehaviour
{
    [SerializeField] SoundSO sound;

    public void PlayAtPosition()
    {
        sound.PlaySpatial(transform.position);
    }
}
