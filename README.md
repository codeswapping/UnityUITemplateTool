# UnityUITemplateTool
A simple UI template tool to create and use UI template to create UI in unity faster.

# Use
To use this tool, you will need a json for UI template. You can create a new one in Ui Template Manager window.
Open UI Template Manager window through Menu->Window->UI Template Manager

Once window is opened, you can select "Load template" button to open a text file which contains the Ui template json file.

To create a new template, click on the "Create New Template" button. It will create a sample template json and it will be visible in the UI template manager window. This is a sample json.
[{
  "templateName":"New Template",
  "templateJson":
  {
  	"elementName":"New Element",
  	"templatePosition":[0.0,0.0,0.0],
  	"templateScale":[1.0,1.0,1.0],
  	"templateRotation":[0.0,0.0,0.0],
  	"allComponents":
  	[
  		{
  			"componentType":0,
  			"componentValues":
  			{
  				"Color":"[0.3,0.4,0.3,1]"
  			}
  		}
  	],
  	"childElements":
  	[
  		{
  			"elementName":"Child Element 1"
  			,"templatePosition":[0.0,0.0,0.0],
  			"templateScale":[1.0,1.0,1.0],
  			"templateRotation":[0.0,0.0,0.0],
  			"allComponents":[],
  			"childElements":[]
  		}
  	]
  }
}]
Here is brief 

-> templateName - Name for your template, you can set any name here and it will appear in the list of template at left hand side of the window.

-> templateJson - A JSON string which contains all data for your UI objects. For ex. object position, rotation, scale and if UI object has some other UI components then it will be also in this json only.

#Brief on templateJson format.

templateJson will contain one element only which will be the parent for all other UI object under it.

-> elementName - Name of you parent UI game object.

-> templatePosition - A array of float of size 3 representing position of UI game object.

-> templateScale - A array of float of size 3 representing scale of UI game object.

-> templateRotation - A array of float of size 3 representing rotation of UI game object.

-> allComponents - List of All the components attached to this UI game object.

-> childElements - List of All the child game object hierarchy in this UI game object.

#allComponents

allComponents contains the list of components attached with this UI game object.

-> componentType - Type of the component. can be any of these; Image - 0, Button - 1, Text - 2, InputField - 3.
-> componentValues - Properies and it's values for this particular component, for ex. if component is Image, then componentValues could contain Color, RaycastTarget, RaycastPadding, ImageType etc. All properties that can be specified in componentValues are given below.

#Image Component

For image component you can specify following properties

-> Color - Set float array of size 4 in order of RGBA to set default color for the image.
-> RaycastTarget - Set bool value true or false to set Raycast Target property.
-> RaycastPadding - Set a float array of size 4 in this order Left, Bottom, Right, Top to set raycast padding for an image.
-> ImageType - Set int value for the type of image, Simple = 0, Sliced = 1, Tiled = 2, Filled = 3
-> UseSpriteMesh - Set bool value to set Image Use Sprite Mesh to true or false.
-> PreserveAspect - Set bool value to set Image preserve Aspect to true of false.
-> SetNativeSize - Set bool value to set Image native size to true or false.
-> PixelPerUnitMultiplyer - Set float value to change image pixel per unit multiplyer value.
-> FillMethod - Set int value for fill method of the image. Horizontal - 0, Vertical - 1, Radial90 - 2, Radial180 - 3, Radial360 - 4.
-> FillOrigin - Set int value for fill origin of the image. 
    For Horizontal Fill Method set Fill origin 0 (Left) or 1 (Right)
    For Vertical Fill Method set fill origin 0 (bottom) or 1 (top)
    For Radial90 Fill method set fill origin 0 (BottomLeft), 1 (TopLeft), 2 (TopRight) or 3 (BottomRight).
    For Radial180 and Radial360 fill method set fill origin 0 (Bottom), 1 (Left), 2 (top) or 3 (right).

-> FillAmount - set float value within 0 and 1 for image fill Amount.
-> Clockwise - set bool value to set image fill in clockwise (true) or antiClockwise (false).

#Button Component

For button component you can specify following properties

-> Interactable - Set bool value to set button interactable (true) or not (false).
-> NormalColor - Set array of float of size 4 in RGBA order to set normal color of the button for ex [0.3,0.4,0.6,1]
-> HighlightedColor - Set array of float of size 4 in RGBA order to set highlighted color of the button for ex [0.3,0.4,0.6,1]
-> PressedColor - Set array of float of size 4 in RGBA order to set Pressed color of the button for ex [0.3,0.4,0.6,1]
-> SelectedColor - Set array of float of size 4 in RGBA order to set Selected color of the button for ex [0.3,0.4,0.6,1]
-> DisabledColor - Set array of float of size 4 in RGBA order to set Disabled color of the button for ex [0.3,0.4,0.6,1]
-> ColorMultiplyer - Set float value to change button color multiplyer value.
-> FadeDuration - Set float value to change fade duration of the button.
-> NavigationMode - Set int value for navigation mode. -1 - Everything, 0 - None,  1 - Horizontal, 2 - vertical, 3 - Automatic,  4 - explicit, 5 - Horizontal and Explicit, 6 - Vertical and explicit, 

#Text Component

For text component you can specify following properties

-> Text - string value to display on text compoent.
-> FontStyle - int value to set font style of text component. 0 - Normal, 1 - Bold, 2 - Italic, 3 - BoldItalic
-> FontSize - int value to set font size of text component.
-> LineSpecign - float value to set line specing of text componet.
-> RichText - bool value to set rich text on (true) or off (false).
-> Alignment - Text alignment of the text component. 0 - UpperLeft, 1 - UpperCenter, 2 - UpperRight, 3 - MiddleLeft, 4 - MiddleCenter, 5 - MiddleRight, 6 - LowerLeft, 7 - LowerCenter, 8 - LowerRight.

-> AlignByGeometry - Set bool value to true or false to set this property.
-> HorizontalOverflow - Set horizontal wrap mode of the text. 0 - Wrap, 1 - Overflow.
-> VerticalOverflow - Set vertical wrap mode of the text. 0 - Truncate, 1 - Overflow.
-> BestFit - set value to true for automatically resize text to fit in rect otherwise set false.
-> Color - Array of float of size 4 to set color of the text. for ex [0.3,0.4,0.6,1]
-> RaycastTarget - Same as Image Component.
-> RaycastPadding - same as image component.
-> Maskable - Set bool value to true for maskable or false for non-maskable.

#InputField Component

For InputField Component, you can specify following properties.

-> Text - string value to set text to input field.
-> Interactable - bool value to set interactable (true) or not (false).
-> NormalColor - Same as button normal color.
-> HighlightedColor - Same as button highlighted color.
-> PressedColor - Same as button pressed color.
-> SelectedColor - Same as button selected color.
-> DisabledColor - Same as button disabled color.
-> ColorMultiplyer - Same as button color multiplyer.
-> FadeDuration - Same as button fade duration.
-> CharacterLimit - Set int value for limit the character user can input in input field.
-> ContentType - Type of the content that can be entered in input field. It can be any of the following.
    0 - Standard
    1 - Autocorrected.
    2 - IntegerNumber,
    3 - DecimalNumber,
    4 - AlphaNumeric,
    5 - Name,
    6 - EmailAddress,
    7 - Password,
    8 - Pin,
    9 - Custom

-> CaretBlinkRate - Set float value for caret blink rate.
-> CaretWidth - Set int value for caret width within range of 1 and 5.
-> CustomCaretColor - Set bool value to true if custom color is required for caret otherwise set false.
-> CaretColor - Array of float of size 4 to set caret color. for Ex [0.3,0.4,0.6,1].
-> SelectionColor - Array of float of size 4 to set selection color. for ex. [0.3,0.4,0.6,1].
-> HideMobileInput - Set bool value to true to hide mobile input else set false.
-> ReadOnly - Set bool value to true to set read only else set false.
-> ShouldActiveOnSelect - Bool value to represet Should set input field active when selected (true) else (false).
