using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject bullet;

    [Header("Ammo")]
    public int totalThrow;
    public float throwCD;

    [Header("Shooting")]
    public KeyCode fireKey;
}
