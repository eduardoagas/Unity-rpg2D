using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsLayer, grassLayer, interactableLayer, playerLayer, fovLayer;

    public static GameLayers i {get; set; }
    private void Awake() {
        i = this;
    }
    public LayerMask SolidObjectsLayer { get => solidObjectsLayer; set => solidObjectsLayer = value; }
    public LayerMask GrassLayer { get => grassLayer; set => grassLayer = value; }
    public LayerMask InteractableLayer { get => interactableLayer; set => interactableLayer = value; }
    public LayerMask PlayerLayer { get => playerLayer; set => playerLayer = value; }
    public LayerMask FovLayer { get => fovLayer; set => fovLayer = value; }
}
