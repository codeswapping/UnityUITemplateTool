using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateHelper : MonoBehaviour
{
    public UnityEngine.UI.Button testButton;

    public UiElementInfo1 info;

    [ContextMenu("Print Json")]
    public void PrintJson()
    {
        Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(info));
        var c = new Color(0.3f, 0.4f, 0.5f, 1f);
        //Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(c));
        Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(testButton.navigation));
    }
}
[System.Serializable]
public class UiElementInfo1
{
    public string elementName;
    public float[] templatePosition;
    public float[] templateScale;
    public float[] templateRotation;

    public List<ComponentInfo1> allComponents;
    public List<UiElementInfo1> childElements;
}
[System.Serializable]
public class ComponentInfo1
{
    public ComponentType1 componentType;
    public Dictionary<string, string> componentValues;
}

public enum ComponentType1
{
    Image,
    Button,
    Text,
    InputField
}