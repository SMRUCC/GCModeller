# Designer
_namespace: [Microsoft.VisualBasic.Imaging.Drawing2D.Colors](./index.md)_





### Methods

#### Colors
```csharp
Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Designer.Colors(System.Drawing.Color[],System.Int32,System.Int32)
```
Some useful color tables for images and tools to handle them.
 Several color scales useful for image plots: a pleasing rainbow style color table patterned after 
 that used in Matlab by Tim Hoar and also some simple color interpolation schemes between two or 
 more colors. There is also a function that converts between colors and a real valued vector.

|Parameter Name|Remarks|
|--------------|-------|
|col|A list of colors (names or hex values) to interpolate|
|n%|Number of color levels. The setting n=64 is the orignal definition.|
|alpha%|The transparency of the color – 255 is opaque and 0 is transparent. This is useful for overlays of color and still being able to view the graphics that is covered.|


_returns: A vector giving the colors in a hexadecimal format, two extra hex digits are added for the alpha channel._


### Properties

#### AvailableInterpolates
{ 
 "Color [PapayaWhip]": [
 {
 "knownColor": 93,
 "name": null,
 "state": 1,
 "value": 0
 },
 {
 "knownColor": 119,
 "name": null,
 "state": 1,
 "value": 0
 },
 {
 "knownColor": 30,
 "name": null,
 "state": 1,
 "value": 0
 },
 {
 "knownColor": 165,
 "name": null,
 "state": 1,
 "value": 0
 },
 {
 "knownColor": 81,
 "name": null,
 "state": 1,
 "value": [rest of string was truncated]";.
