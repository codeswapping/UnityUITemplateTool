using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using Newtonsoft.Json;
using UnityEngine.UI;

public class UITemplateManager : EditorWindow
{
    private List<TemplateInfo> loadedTemplate;
    private ListView leftPane;
    private TextField templateInput;

    private UnityEngine.UIElements.Button createUITemplateButton, saveTemplateButton;

    private int selectedTemplate = -1;

    [MenuItem("Window/UI Template Manager")]
    public static void OpenTemplateManager()
    {
        var window = GetWindow<UITemplateManager>("UI Template Manager");
        window.minSize = new Vector2(480, 320);
        window.maxSize = new Vector2(1920, 720);
    }

    private void CreateGUI()
    {
        //Main top level split view - Vertical
        var verticalSplitView = new TwoPaneSplitView(0, 20, TwoPaneSplitViewOrientation.Vertical);

        //Template list and json preview level split view
        var splitView = new TwoPaneSplitView(0, 200, TwoPaneSplitViewOrientation.Horizontal);

        //Load button
        var loadButton = GetLoadButton();
        loadButton.style.width = 100;
        loadButton.style.height = 20;

        //Create Button
        var createButton = new UnityEngine.UIElements.Button(() =>
        {
            var template = new TemplateInfo
            {
                templateName = "New Template",
                templateJson = "{\n\t" +
                "\"elementName\":\"New Element\",\n\t" +
                "\"templatePosition\":[0.0,0.0,0.0],\n\t" +
                "\"templateScale\":[1.0,1.0,1.0],\n\t" +
                "\"templateRotation\":[0.0,0.0,0.0],\n\t" +
                "\"allComponents\":\n\t" +
                "[\n\t\t" +
                "{\n\t\t\t" +
                "\"componentType\":0,\n\t\t\t" +
                "\"componentValues\":\n\t\t\t" +
                "{\n\t\t\t\t" +
                "\"Color\":\"[0.3,0.4,0.3,1]\"\n\t\t\t" +
                "}\n\t\t" +
                "}\n\t" +
                "],\n\t" +
                "\"childElements\":\n\t" +
                "[\n\t\t" +
                "{\n\t\t\t" +
                "\"elementName\":\"Child Element 1\"\n\t\t\t" +
                ",\"templatePosition\":[0.0,0.0,0.0],\n\t\t\t" +
                "\"templateScale\":[1.0,1.0,1.0],\n\t\t\t" +
                "\"templateRotation\":[0.0,0.0,0.0],\n\t\t\t" +
                "\"allComponents\":[],\n\t\t\t" +
                "\"childElements\":[]\n\t\t" +
                "}\n\t" +
                "]\n" +
                "}"
            };
            loadedTemplate ??= new();
            loadedTemplate.Add(template);
            selectedTemplate = loadedTemplate.Count - 1;
            LoadTemplate();
        })
        {
            text = "Create New Template",
        };
        createButton.style.width = 200;
        createButton.style.height = 20;
        //Button list to add in top level split view section 1;
        var buttonList = new VisualElement();
        buttonList.style.flexDirection = FlexDirection.Row;
        buttonList.style.alignItems = Align.FlexStart;
        buttonList.style.width = 600;
        buttonList.hierarchy.Add(loadButton);
        buttonList.hierarchy.Add(createButton);

        verticalSplitView.Add(buttonList);

        leftPane = new ListView
        {
            makeItem = () => new Label(),
            fixedItemHeight = 20
        };
        leftPane.selectionChanged += LeftPane_onSelectionChange;

        templateInput = new TextField
        {
            multiline = true,
        };
        templateInput.style.whiteSpace = WhiteSpace.Normal;
        
        splitView.Add(leftPane);

        var jsonSplitView = new TwoPaneSplitView(0, 20, TwoPaneSplitViewOrientation.Vertical);
        createUITemplateButton = new UnityEngine.UIElements.Button(() =>
        {
            CreateUIFromSelectedTemplate();
        })
        {
            text = "Load this template"
        };
        saveTemplateButton = GetSaveButton();
        createUITemplateButton.SetEnabled(selectedTemplate > -1);
        saveTemplateButton.SetEnabled(selectedTemplate > -1);
        templateInput.SetEnabled(selectedTemplate > -1);

        var saveloadTemplate = new VisualElement();
        saveloadTemplate.style.flexDirection = FlexDirection.Row;
        saveloadTemplate.style.alignItems = Align.FlexStart;
        saveloadTemplate.hierarchy.Add(createUITemplateButton);
        saveloadTemplate.hierarchy.Add(saveTemplateButton);

        jsonSplitView.Add(saveloadTemplate);
        jsonSplitView.Add(templateInput);
        splitView.Add(jsonSplitView);
        verticalSplitView.Add(splitView);
        rootVisualElement.Add(verticalSplitView);
    }

    private void LeftPane_onSelectionChange(IEnumerable<object> obj)
    {
        selectedTemplate = leftPane.selectedIndex;
        if (selectedTemplate < 0)
            return;
        saveTemplateButton.SetEnabled(true);
        createUITemplateButton.SetEnabled(true);
        templateInput.SetEnabled(true);
        templateInput.value = loadedTemplate[selectedTemplate].templateJson;
    }

    private void CreateUIFromSelectedTemplate()
    {
        if (selectedTemplate < 0)
            return;
        var template = loadedTemplate[selectedTemplate];
        var canvas = FindObjectOfType<Canvas>(true);
        if (canvas == null)
        {
            var canvasGo = new GameObject($"{template.templateName}");
            canvas = canvasGo.AddComponent<Canvas>();
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        var elementInfo = JsonConvert.DeserializeObject<UiElementInfo>(template.templateJson);
        ArrageUIHierarchy(elementInfo, canvas.transform);
    }

    private void ArrageUIHierarchy(UiElementInfo elementInfo, Transform parent)
    {
        var p1 = new GameObject(elementInfo.elementName);
        p1.transform.SetParent(parent);
        p1.transform.position = new Vector3(elementInfo.templatePosition[0],
            elementInfo.templatePosition[1], elementInfo.templatePosition[2]);
        p1.transform.localScale = new Vector3(elementInfo.templateScale[0], elementInfo.templateScale[1],
        elementInfo.templateScale[2]);
        p1.transform.rotation = Quaternion.Euler(elementInfo.templateRotation[0], elementInfo.templateRotation[1],
        elementInfo.templateRotation[2]);

        foreach (var c in elementInfo.allComponents)
        {
            switch (c.componentType)
            {
                case ComponentType.Button:
                    var b = p1.AddComponent<UnityEngine.UI.Button>();
                    CheckButtonProperties(b, c.componentValues);
                    break;
                case ComponentType.Image:
                    var img = p1.AddComponent<UnityEngine.UI.Image>();
                    CheckImageProperties(img, c.componentValues);
                    break;
                case ComponentType.Text:
                    var txt = p1.AddComponent<Text>();
                    CheckTextProperties(txt, c.componentValues);
                    break;
                case ComponentType.InputField:
                    var inpf = p1.AddComponent<InputField>();
                    CheckInputFieldProperties(inpf, c.componentValues);
                    break;
            }
        }

        for (int i = 0; i < elementInfo.childElements.Count; i++)
        {
            ArrageUIHierarchy(elementInfo.childElements[i], p1.transform);
        }
    }

    private void LoadTemplate()
    {
        leftPane.Clear();
        leftPane.itemsSource = loadedTemplate;
        leftPane.bindItem = (e, i) => (e as Label).text = loadedTemplate[i].templateName;
        leftPane.RefreshItems();
        leftPane.selectedIndex = loadedTemplate.Count - 1;

        //for (int i = 0; i < loadedTemplate.Count; i++)
        //{
        //    int j = i;
        //    TemplateInfo t = loadedTemplate[i];
        //    var button = new Button(() =>
        //    {
        //        selectedTemplate = j;
        //        templateInput.Clear();
        //        templateInput.value = t.templateJson;
        //    });
        //}
    }
    private UnityEngine.UIElements.Button GetLoadButton()
    {
        return new UnityEngine.UIElements.Button(() =>
        {
            //Load button is clicked, open file browser and let user select json file.
            string path = EditorUtility.OpenFilePanel("Load Json template", "", ".json");
            if (path.Length != 0)
            {
                try
                {
                    var jsonString = System.IO.File.ReadAllText(path);
                    loadedTemplate = JsonConvert.DeserializeObject<List<TemplateInfo>>(jsonString);
                    LoadTemplate();
                }
                catch (System.Exception e)
                {
                    Debug.Log($"Error occured while reading file\n{e.Message} ");
                }
            }
        })
        {
            text = "Load Template"
        };
    }
    private UnityEngine.UIElements.Button GetSaveButton()
    {
        return new UnityEngine.UIElements.Button(() =>
        {
            //Save current selected template
            loadedTemplate[selectedTemplate].templateJson = templateInput.text;
            //Load button is clicked, open file browser and let user select json file.
            string path = EditorUtility.SaveFilePanel("Save Json template", "", "MyTemplate", ".json");
            if (path.Length != 0)
            {
                try
                {
                    var jsonString = $"{JsonConvert.SerializeObject(loadedTemplate)}";
                    System.IO.File.WriteAllBytes(path, System.Text.Encoding.ASCII.GetBytes(jsonString));
                    //LoadTemplate();
                }
                catch (System.Exception e)
                {
                    Debug.Log($"Error occured while saving file\n{e.Message} ");
                }
            }
        })
        {
            text = "Save this Template"
        };
    }

    private void CheckImageProperties(UnityEngine.UI.Image img, Dictionary<string,string> componentValues)
    {
        foreach (var p in componentValues)
        {
            switch (p.Key)
            {
                case "Color":
                    try
                    {
                        var colorFloat = JsonConvert.DeserializeObject<float[]>(p.Value);
                        var color = new Color(colorFloat[0], colorFloat[1], colorFloat[2], colorFloat[3]);
                        img.color = color;
                    }
                    catch(System.Exception e)
                    {
                        Debug.Log($"Error setting color propery to {img.name} : {e.Message}");
                    }
                    break;
                case "RaycastTarget":
                    try
                    {
                        img.raycastTarget = JsonConvert.DeserializeObject<bool>(p.Value);
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting raycast target {img.name} : {e.Message}");
                    }
                    break;
                case "RaycastPadding":
                    try
                    {
                        var raycastFloat = JsonConvert.DeserializeObject<float[]>(p.Value);
                        var v4 = new Vector4(raycastFloat[0], raycastFloat[1], raycastFloat[2], raycastFloat[3]);
                        img.raycastPadding = v4;
                    }
                    catch(System.Exception e)
                    {
                        Debug.Log($"Error setting raycast Padding {img.name} : {e.Message}");
                    }
                    break;
                case "ImageType":
                    try
                    {
                        img.type = JsonConvert.DeserializeObject<UnityEngine.UI.Image.Type>(p.Value);
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting image type {img.name} : {e.Message}");
                    }
                    break;
                case "UseSpriteMesh":
                    try
                    {
                        img.useSpriteMesh = JsonConvert.DeserializeObject<bool>(p.Value);
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting image Use sprite mesh {img.name} : {e.Message}");
                    }
                    break;
                case "PreserveAspect":
                    try
                    {
                        img.preserveAspect = JsonConvert.DeserializeObject<bool>(p.Value);
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting image preserve aspect {img.name} : {e.Message}");
                    }
                    break;
                case "SetNativeSize":
                    try
                    {
                        bool shouldSetNative = JsonConvert.DeserializeObject<bool>(p.Value);
                        if (shouldSetNative)
                            img.SetNativeSize();
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting image set native size - {img.name} : {e.Message}");
                    }
                    break;
                case "PixelPerUnitMultiplyer":
                    try
                    {
                        var multiplyer = JsonConvert.DeserializeObject<float>(p.Value);
                        img.pixelsPerUnitMultiplier = multiplyer;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting image pixel per unity multiplyer - {img.name} : {e.Message}");
                    }
                    break;
                case "FillMethod":
                    try
                    {
                        var fillMethod = JsonConvert.DeserializeObject<UnityEngine.UI.Image.FillMethod>(p.Value);
                        img.fillMethod = fillMethod;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting image fill method - {img.name} : {e.Message}");
                    }
                    break;
                case "FillOrigin":
                    try
                    {
                        var fillOrigin = JsonConvert.DeserializeObject<int>(p.Value);
                        img.fillOrigin = (int)fillOrigin;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting image fill origin - {img.name} : {e.Message}");
                    }
                    break;
                case "FillAmount":
                    try
                    {
                        var fillAmount = JsonConvert.DeserializeObject<float>(p.Value);
                        fillAmount = Mathf.Clamp01(fillAmount);
                        img.fillAmount = fillAmount;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting image fill amount - {img.name} : {e.Message}");
                    }
                    break;
                case "Clockwise":
                    try
                    {
                        var clockwise = JsonConvert.DeserializeObject<bool>(p.Value);
                        img.fillClockwise = clockwise;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting image fill clockwise - {img.name} : {e.Message}");
                    }
                    break;
            }
        }
    }
    private void CheckButtonProperties(UnityEngine.UI.Button btn, Dictionary<string, string> componentValues)
    {
        foreach (var p in componentValues)
        {
            switch (p.Key)
            {
                case "Interactable":
                    try
                    {
                        btn.interactable = JsonConvert.DeserializeObject<bool>(p.Value);
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting button interactable - {btn.name} : {e.Message}");
                    }
                    break;
                case "NormalColor":
                    try
                    {
                        var colorFloat = JsonConvert.DeserializeObject<float[]>(p.Value);
                        var color = new Color(colorFloat[0], colorFloat[1], colorFloat[2], colorFloat[3]);
                        var colors = btn.colors;
                        colors.normalColor = color;
                        btn.colors = colors;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting normal color propery to {btn.name} : {e.Message}");
                    }
                    break;

                case "HighlightedColor":
                    try
                    {
                        var colorFloat = JsonConvert.DeserializeObject<float[]>(p.Value);
                        var color = new Color(colorFloat[0], colorFloat[1], colorFloat[2], colorFloat[3]);
                        var colors = btn.colors;
                        colors.highlightedColor = color;
                        btn.colors = colors;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting highlighted color propery to {btn.name} : {e.Message}");
                    }
                    break;
                case "PressedColor":
                    try
                    {
                        var colorFloat = JsonConvert.DeserializeObject<float[]>(p.Value);
                        var color = new Color(colorFloat[0], colorFloat[1], colorFloat[2], colorFloat[3]);
                        var colors = btn.colors;
                        colors.pressedColor = color;
                        btn.colors = colors;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting pressed color propery to {btn.name} : {e.Message}");
                    }
                    break;
                case "SelectedColor":
                    try
                    {
                        var colorFloat = JsonConvert.DeserializeObject<float[]>(p.Value);
                        var color = new Color(colorFloat[0], colorFloat[1], colorFloat[2], colorFloat[3]);
                        var colors = btn.colors;
                        colors.selectedColor = color;
                        btn.colors = colors;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting highlighted color propery to {btn.name} : {e.Message}");
                    }
                    break;
                case "DisabledColor":
                    try
                    {
                        var colorFloat = JsonConvert.DeserializeObject<float[]>(p.Value);
                        var color = new Color(colorFloat[0], colorFloat[1], colorFloat[2], colorFloat[3]);
                        var colors = btn.colors;
                        colors.disabledColor = color;
                        btn.colors = colors;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting disabled color propery to {btn.name} : {e.Message}");
                    }
                    break;
                case "ColorMultiplyer":
                    try
                    {
                        var multiplyer = JsonConvert.DeserializeObject<float>(p.Value);
                        multiplyer = Mathf.Clamp(multiplyer, 1f, 5f);
                        var colors = btn.colors;
                        colors.colorMultiplier = multiplyer;
                        btn.colors = colors;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting button color multiplyer - {btn.name} : {e.Message}");
                    }
                    break;
                case "FadeDuration":
                    try
                    {
                        var fadeDuration = JsonConvert.DeserializeObject<float>(p.Value);
                        var colors = btn.colors;
                        colors.fadeDuration = fadeDuration;
                        btn.colors = colors;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting button color multiplyer - {btn.name} : {e.Message}");
                    }
                    break;

                case "NavigationMode":
                    try
                    {
                        var navigation = JsonConvert.DeserializeObject<Navigation.Mode>(p.Value);
                        var nav = btn.navigation;
                        nav.mode = navigation;
                        btn.navigation = nav;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting button navigation {btn.name} : {e.Message}");
                    }
                    break;
            }
        }
    }
    private void CheckTextProperties(Text txt, Dictionary<string, string> componentValues)
    {
        foreach (var p in componentValues)
        {
            switch (p.Key)
            {
                case "Text":
                    try
                    {
                        txt.text = p.Value;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting Tex - {txt.name} : {e.Message}");
                    }
                    break;
                case "FontStyle":
                    try
                    {
                        var fontStyle = JsonConvert.DeserializeObject<FontStyle>(p.Value);
                        txt.fontStyle = fontStyle;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting font Style to {txt.name} : {e.Message}");
                    }
                    break;

                case "FontSize":
                    try
                    {
                        var fontSize = JsonConvert.DeserializeObject<int>(p.Value);
                        txt.fontSize = fontSize;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting highlighted color propery to {txt.name} : {e.Message}");
                    }
                    break;
                case "LineSpacing":
                    try
                    {
                        var lineSpacing = JsonConvert.DeserializeObject<float>(p.Value);
                        txt.lineSpacing = lineSpacing;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting line specing {txt.name} : {e.Message}");
                    }
                    break;
                case "RichText":
                    try
                    {
                        var richText = JsonConvert.DeserializeObject<bool>(p.Value);
                        txt.supportRichText = richText;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting rich text to {txt.name} : {e.Message}");
                    }
                    break;
                case "Alignment":
                    try
                    {
                        var alignment = JsonConvert.DeserializeObject<TextAnchor>(p.Value);
                        txt.alignment = alignment;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting disabled color propery to {txt.name} : {e.Message}");
                    }
                    break;
                case "AlignByGeometry":
                    try
                    {
                        var alignByGeo = JsonConvert.DeserializeObject<bool>(p.Value);
                        txt.alignByGeometry = alignByGeo;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting align by geometry - {txt.name} : {e.Message}");
                    }
                    break;
                case "HorizontalOverflow":
                    try
                    {
                        var overflow = JsonConvert.DeserializeObject<HorizontalWrapMode>(p.Value);
                        txt.horizontalOverflow = overflow;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting horizontal overflow - {txt.name} : {e.Message}");
                    }
                    break;

                case "VerticalOverflow":
                    try
                    {
                        var verticalOverflow = JsonConvert.DeserializeObject<VerticalWrapMode>(p.Value);
                        txt.verticalOverflow = verticalOverflow;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting vertical overflow {txt.name} : {e.Message}");
                    }
                    break;
                case "BestFit":
                    try
                    {
                        var bestFit = JsonConvert.DeserializeObject<bool>(p.Value);
                        txt.resizeTextForBestFit = bestFit;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting best fit - {txt.name} : {e.Message}");
                    }
                    break;
                case "Color":
                    try
                    {
                        var colorFloat = JsonConvert.DeserializeObject<float[]>(p.Value);
                        var color = new Color(colorFloat[0], colorFloat[1], colorFloat[2], colorFloat[3]);
                        txt.color = color;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting color propery to {txt.name} : {e.Message}");
                    }
                    break;
                case "RaycastTarget":
                    try
                    {
                        txt.raycastTarget = JsonConvert.DeserializeObject<bool>(p.Value);
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting raycast target {txt.name} : {e.Message}");
                    }
                    break;
                case "RaycastPadding":
                    try
                    {
                        txt.raycastPadding = JsonConvert.DeserializeObject<Vector4>(p.Value);
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting raycast Padding {txt.name} : {e.Message}");
                    }
                    break;
                case "Maskable":
                    try
                    {
                        txt.maskable = JsonConvert.DeserializeObject<bool>(p.Value);
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting maskable {txt.name} : {e.Message}");
                    }
                    break;
            }
        }
    }
    private void CheckInputFieldProperties(InputField inputField, Dictionary<string, string> componentValues)
    {
        foreach (var p in componentValues)
        {
            switch (p.Key)
            {
                case "Text":
                    try
                    {
                        inputField.text = p.Value;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting Tex - {inputField.name} : {e.Message}");
                    }
                    break;
                case "Interactable":
                    try
                    {
                        inputField.interactable = JsonConvert.DeserializeObject<bool>(p.Value);
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting inputfield interactable - {inputField.name} : {e.Message}");
                    }
                    break;
                case "NormalColor":
                    try
                    {
                        var colorFloat = JsonConvert.DeserializeObject<float[]>(p.Value);
                        var color = new Color(colorFloat[0], colorFloat[1], colorFloat[2], colorFloat[3]);
                        var colors = inputField.colors;
                        colors.normalColor = color;
                        inputField.colors = colors;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting normal color propery to {inputField.name} : {e.Message}");
                    }
                    break;

                case "HighlightedColor":
                    try
                    {
                        var colorFloat = JsonConvert.DeserializeObject<float[]>(p.Value);
                        var color = new Color(colorFloat[0], colorFloat[1], colorFloat[2], colorFloat[3]);
                        var colors = inputField.colors;
                        colors.highlightedColor = color;
                        inputField.colors = colors;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting highlighted color propery to {inputField.name} : {e.Message}");
                    }
                    break;
                case "PressedColor":
                    try
                    {
                        var colorFloat = JsonConvert.DeserializeObject<float[]>(p.Value);
                        var color = new Color(colorFloat[0], colorFloat[1], colorFloat[2], colorFloat[3]);
                        var colors = inputField.colors;
                        colors.pressedColor = color;
                        inputField.colors = colors;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting pressed color propery to {inputField.name} : {e.Message}");
                    }
                    break;
                case "SelectedColor":
                    try
                    {
                        var colorFloat = JsonConvert.DeserializeObject<float[]>(p.Value);
                        var color = new Color(colorFloat[0], colorFloat[1], colorFloat[2], colorFloat[3]);
                        var colors = inputField.colors;
                        colors.selectedColor = color;
                        inputField.colors = colors;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting highlighted color propery to {inputField.name} : {e.Message}");
                    }
                    break;
                case "DisabledColor":
                    try
                    {
                        var colorFloat = JsonConvert.DeserializeObject<float[]>(p.Value);
                        var color = new Color(colorFloat[0], colorFloat[1], colorFloat[2], colorFloat[3]);
                        var colors = inputField.colors;
                        colors.disabledColor = color;
                        inputField.colors = colors;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting disabled color propery to {inputField.name} : {e.Message}");
                    }
                    break;
                case "ColorMultiplyer":
                    try
                    {
                        var multiplyer = JsonConvert.DeserializeObject<float>(p.Value);
                        multiplyer = Mathf.Clamp(multiplyer, 1f, 5f);
                        var colors = inputField.colors;
                        colors.colorMultiplier = multiplyer;
                        inputField.colors = colors;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting inputfield color multiplyer - {inputField.name} : {e.Message}");
                    }
                    break;
                case "FadeDuration":
                    try
                    {
                        var fadeDuration = JsonConvert.DeserializeObject<float>(p.Value);
                        var colors = inputField.colors;
                        colors.fadeDuration = fadeDuration;
                        inputField.colors = colors;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting inputfield color multiplyer - {inputField.name} : {e.Message}");
                    }
                    break;

                case "CharacterLimit":
                    try
                    {
                        var limit = JsonConvert.DeserializeObject<int>(p.Value);
                        inputField.characterLimit = limit;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting character limit to {inputField.name} : {e.Message}");
                    }
                    break;

                case "ContentType":
                    try
                    {
                        var contentType = JsonConvert.DeserializeObject<InputField.ContentType>(p.Value);
                        inputField.contentType = contentType;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting highlighted color propery to {inputField.name} : {e.Message}");
                    }
                    break;
                case "CaretBlinkRate":
                    try
                    {
                        var blinkRate = JsonConvert.DeserializeObject<float>(p.Value);
                        blinkRate = Mathf.Clamp(blinkRate, 0, 4);
                        inputField.caretBlinkRate = blinkRate;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting caret blink rate {inputField.name} : {e.Message}");
                    }
                    break;
                case "CaretWidth":
                    try
                    {
                        var caretWidth = JsonConvert.DeserializeObject<int>(p.Value);
                        caretWidth = Mathf.Clamp(caretWidth, 1, 5);
                        inputField.caretWidth = caretWidth;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting caret width to {inputField.name} : {e.Message}");
                    }
                    break;
                case "CustomCaretColor":
                    try
                    {
                        var color = JsonConvert.DeserializeObject<bool>(p.Value);
                        inputField.customCaretColor = color;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting custom caret to {inputField.name} : {e.Message}");
                    }
                    break;
                case "CaretColor":
                    try
                    {
                        var colorFloat = JsonConvert.DeserializeObject<float[]>(p.Value);
                        var color = new Color(colorFloat[0], colorFloat[1], colorFloat[2], colorFloat[3]);
                        inputField.caretColor = color;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting caret color - {inputField.name} : {e.Message}");
                    }
                    break;
                case "SelectionColor":
                    try
                    {
                        var colorFloat = JsonConvert.DeserializeObject<float[]>(p.Value);
                        var color = new Color(colorFloat[0], colorFloat[1], colorFloat[2], colorFloat[3]);
                        inputField.selectionColor = color;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting selection color - {inputField.name} : {e.Message}");
                    }
                    break;

                case "HideMobileInput":
                    try
                    {
                        var hideInput = JsonConvert.DeserializeObject<bool>(p.Value);
                        inputField.shouldHideMobileInput = hideInput;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting hide mobile input {inputField.name} : {e.Message}");
                    }
                    break;
                case "ReadOnly":
                    try
                    {
                        var readOnly = JsonConvert.DeserializeObject<bool>(p.Value);
                        inputField.readOnly = readOnly;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting Read only - {inputField.name} : {e.Message}");
                    }
                    break;
                case "ShouldActiveOnSelect":
                    try
                    {
                        var shouldActive = JsonConvert.DeserializeObject<bool>(p.Value);
                        inputField.shouldActivateOnSelect = shouldActive;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Error setting should active on select to {inputField.name} : {e.Message}");
                    }
                    break;
            }
        }
    }
}

public class TemplateInfo
{
    public string templateName;
    public string templateJson;
}
[System.Serializable]
public class UiElementInfo
{
    public string elementName;
    public float[] templatePosition;
    public float[] templateScale;
    public float[] templateRotation;

    public List<ComponentInfo> allComponents;
    public List<UiElementInfo> childElements;
}
[System.Serializable]
public class ComponentInfo
{
    public ComponentType componentType;
    public Dictionary<string, string> componentValues;
}

public enum ComponentType
{
    Image,
    Button,
    Text,
    InputField
}
