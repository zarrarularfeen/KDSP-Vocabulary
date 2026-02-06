using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System;

public class BlockerManager : MonoBehaviour
{
    public static BlockerManager Instance { get; private set; }
    [SerializeField] private GameObject blocker;
    private float waitTime;

    void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blocker.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActivateBlocker(float wait)
    {
        waitTime = wait;
        StartCoroutine(Blocker());
    }

    IEnumerator Blocker()
    {
        blocker.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        blocker.SetActive(false);
    }

}