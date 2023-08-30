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

