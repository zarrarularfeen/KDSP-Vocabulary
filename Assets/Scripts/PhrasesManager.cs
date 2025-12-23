using UnityEngine;
using System.Collections.Generic;
using System;

public enum Phrases1Connectors
{
    eating,
    sleeping,
    drinking,
    sitting,
    washing,
    brushing,
    ball,
    where
}

public class PhrasesManager : MonoBehaviour
{
    public static PhrasesManager Instance { get; private set; }
    public static int bookSelected;
    public static Phrases1Connectors connectorSelected;
    private List<ContentPicturePair> content = new List<ContentPicturePair>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetBook()
    {

    }
}
