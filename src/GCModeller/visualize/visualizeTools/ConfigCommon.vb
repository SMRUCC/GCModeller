Imports Oracle.Java.IO.Properties
Imports System.Text.RegularExpressions
Imports System.Runtime.CompilerServices
Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Serialization

Public MustInherit Class ConfigCommon
    Implements ISaveHandle

    ''' <summary>
    ''' 
    ''' Due to the GDI+ limitations in the .NET Framework, the image size is limited by your computer memory size, if you want to
    ''' drawing a very large size image, please running this script on a 64bit platform operating system, or you will get a 
    ''' exception about the GDI+ error: parameter is not valid and then you should try a smaller resolution of the drawing output image.
    ''' Value format: &lt;Width(Integer)>[,&lt;Height(Integer)>]
    ''' 
    ''' Example:
    ''' Both specific the size property: 12000,8000
    ''' Which means the drawing script will generate a image file in resolution of width is value 12000 pixels and image height is 8000 pixels.
    ''' </summary>
    ''' <returns></returns>
    <Comment(
        " - -------------------------------------------------------------------------------------------------" & vbCrLf &
        "                    This section will configure the drawing size options" & vbCrLf &
        "--------------------------------------------------------------------------------------------------", -1)>
    <Comment("Due To the GDI+ limitations In the .NET Framework, the image size Is limited by your computer memory size, If you want to " & vbCrLf &
        "drawing a very large size image, please running this script On a 64bit platform operating system, Or you will get a " & vbCrLf &
        "exception about the GDI+ error parameter Is Not valid And then you should try a smaller resolution of the drawing output image.", 0)>
    <Comment("Value format: <Width(Integer)>[,<Height(Integer)>]" & vbCrLf &
        "Example:" & vbCrLf &
        "Both specific the size property: 12000,8000" & vbCrLf &
        "Which means the drawing script will generate a image file in resolution of width is value " & vbCrLf & "12000 pixels and image height is 8000 pixels.", 1)>
    Public Property Resolution As String = "10000,4000"
    <Comment("This property is associated with the Resolution property: if you are not specific the " & vbCrLf &
        "Height property in the resolution property, then configuration will trying to calculate the " & vbCrLf &
        "Height property value automatically from this property value.", 0)>
    <Comment("Value format: <Width(Integer)>:<Height(Integer)>" & vbCrLf &
        "Here is a property value example: 16:9" & vbCrLf &
        "The example means Width/Height=16/9, so that when the Resolution property is specific as 19200 " & vbCrLf &
        "and the Height value is leave empty, then configuration will calculate the empty height " & vbCrLf &
        "value as 19200*9/16 = 10800; So that the Resolution value can be set to 19200,10800", 1)>
    <MappingsIgnored> Public Property AspectRatio As String = "16:9"

    Public MustOverride Function Save(Optional Path As String = "", Optional encoding As System.Text.Encoding = Nothing) As Boolean Implements ISaveHandle.Save

    Public Overrides Function ToString() As String
        Return Me.ToConfigDoc
    End Function

    Public Function Save(Optional Path As String = "", Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
        Return Save(Path, encoding.GetEncodings)
    End Function
End Class
