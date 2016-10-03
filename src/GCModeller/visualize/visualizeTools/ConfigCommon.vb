#Region "Microsoft.VisualBasic::56f7e2d4476aa807c4f7028c88de0d27, ..\GCModeller\visualize\visualizeTools\ConfigCommon.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Text
Imports Oracle.Java.IO.Properties

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

