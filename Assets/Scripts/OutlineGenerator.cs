using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System;

public class OutlineGenerator : MonoBehaviour
{
    public GameObject referenceImage;
    public GameObject outlineImagePrefab;
    private List<GameObject> borderPieces = new List<GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateBorder(Color color)
    {
        RectTransform refRT = referenceImage.GetComponent<RectTransform>();

        LayoutRebuilder.ForceRebuildLayoutImmediate(refRT.parent as RectTransform);

        float width = refRT.rect.width;
        float height = refRT.rect.height;

        Debug.Log("width: " + width);
        Debug.Log("height: " + height);

        float thickness = 0.05f * Mathf.Min(width, height);

        CreateBorderPiece("Top",
            new Vector2(0, height / 2 + thickness / 2),
            new Vector2(width + 2 * thickness, thickness),
            color);

        CreateBorderPiece("Bottom",
            new Vector2(0, -height / 2 - thickness / 2),
            new Vector2(width + 2 * thickness, thickness),
            color);

        CreateBorderPiece("Left",
            new Vector2(-width / 2 - thickness / 2, 0),
            new Vector2(thickness, height),
            color);

        CreateBorderPiece("Right",
            new Vector2(width / 2 + thickness / 2, 0),
            new Vector2(thickness, height),
            color);
    }

    private void CreateBorderPiece(string name, Vector2 position, Vector2 size, Color color)
    {
        GameObject obj = Instantiate(outlineImagePrefab, referenceImage.transform);
        obj.name = name;

        obj.GetComponent<Image>().color = color;

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        rt.anchoredPosition = position;
        rt.sizeDelta = size;

        borderPieces.Add(obj);
    }

    public void DisableBorder()
    {
        foreach (GameObject piece in borderPieces)
        {
            if (piece != null)
                piece.SetActive(false);
        }
    }

    public void EnableBorder()
    {
        foreach (GameObject piece in borderPieces)
        {
            if (piece != null)
                piece.SetActive(true);
        }
    }

    

}
