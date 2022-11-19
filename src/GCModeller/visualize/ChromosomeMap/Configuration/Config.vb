#Region "Microsoft.VisualBasic::de5ae8e7ada6428a84991c7d05508c1c, GCModeller\visualize\ChromosomeMap\Configuration\Config.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 244
    '    Code Lines: 194
    ' Comment Lines: 16
    '   Blank Lines: 34
    '     File Size: 12.22 KB


    '     Class Config
    ' 
    '         Properties: AddLegend, AspectRatio, DefaultRNAColor, DeletionMutation, FLAG_HEIGHT
    '                     FlagLength, FunctionAlignment, FunctionAnnotationFont, GeneObjectHeight, IntegrationMutant
    '                     LegendFont, LineHeight, LineLength, LocusTagFont, Margin
    '                     NoneCogColor, Resolution, ribosomalRNAColor, SavedFormat, SecondaryRuleFont
    '                     tRNAColor
    ' 
    '         Function: [DefaultValue], CssFontParser, GetDrawingColor, GetDrawingSize, GetSavedImageFormat
    '                   GetTextAlignment, (+2 Overloads) Save, ToConfigurationModel, ToString, TypeOfAlignment
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Text
Imports Oracle.Java.IO.Properties
Imports SMRUCC.genomics.Visualize.ChromosomeMap.DrawingModels

Namespace Configuration

    ''' <summary>
    ''' 配置数据的文本文件模型，保存或者读取配置数据，请使用本对象类型来执行
    ''' </summary>
    ''' <remarks></remarks>
    <Comment(
        "--------------------------------------------------------------------------------------------------" & vbCrLf &
        "                 Configuration data for ploting the genome chromsome." & vbCrLf &
        "--------------------------------------------------------------------------------------------------", 0)>
    <Comment(
        "If you have any question about the drawing script and this configuration file, " & vbCrLf &
        "please contact the author via: " & vbCrLf &
        "     Gmail:    xie.guigang@gcmodeller.org" & vbCrLf &
        "     Work:     genomics@smrucc.org" & vbCrLf &
        "     Twitter:  @xieguigang(https://twitter.com/xieguigang)", 1)>
    Public Class Config : Implements ISaveHandle

        ''' <summary>
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
            "--------------------------------------------------------------------------------------------------" & vbCrLf &
            "                    This section will configure the drawing size options" & vbCrLf &
            "--------------------------------------------------------------------------------------------------", -1)>
        <Comment(
        "Due To the GDI+ limitations In the .NET Framework, the image size Is limited by your computer memory size, If you want to " & vbCrLf &
        "drawing a very large size image, please running this script On a 64bit platform operating system, Or you will get a " & vbCrLf &
        "exception about the GDI+ error parameter Is Not valid And then you should try a smaller resolution of the drawing output image.", 0)>
        <Comment(
        "Value format: <Width(Integer)>[,<Height(Integer)>]" & vbCrLf &
        "Example:" & vbCrLf &
        "Specific the size property width and height: 12000,8000" & vbCrLf &
        "Which means the drawing script will generate a image file in resolution of width is value " & vbCrLf &
        "12000 pixels and image height is 8000 pixels.", 1)>
        Public Property Resolution As String = "12000,4500"

        <Comment(
            "This property is associated with the Resolution property: if you are not specific the " & vbCrLf &
            "Height property in the resolution property, then configuration will trying to calculate the " & vbCrLf &
            "Height property value automatically from this property value.", 0)>
        <Comment(
        "Value format: <Width(Integer)>:<Height(Integer)>" & vbCrLf &
        "Here is a property value example: 16:9" & vbCrLf &
        "The example means Width/Height=16/9, so that when the Resolution property is specific as 19200 " & vbCrLf &
        "and the Height value is leave empty, then configuration will calculate the empty height " & vbCrLf &
        "value as 19200*9/16 = 10800; So that the Resolution value can be set to 19200,10800", 1)>
        Public Property AspectRatio As String = "16:9"

        <Comment(
            "This property specific the height value of the arrow shape which is stand for a gene object on the chromesome.", 0)>
        Public Property GeneObjectHeight As Integer

        <Comment(
            "This property specific the drawing segment length of a line, value unit is Mbp; defualt value is 0.1MBp, " & vbCrLf &
            "which means the script will draw the line stands 100KBp", 0)>
        Public Property LineLength As Double

        <Comment(
            "This property specific the drawing height of a line, it can be in the format of a integer value or a expression " & vbCrLf &
            "associated with GeneObjectHeight property.", 0)>
        <Comment(
        "Value example:   600, the property value is specific as an integer which means the drawing height will be 600 pixels;" & vbCrLf &
        "6x , this property value is assign by a expression which means the drawing height value " & vbCrLf &
        "is the 6 times of the GeneObjectHeight value.", 1)>
        Public Property LineHeight As String

        Public Property Margin As Integer

        Public Property FlagLength As Integer

        <Comment(
            "This property value can be both Integer value or an expression associated with FlagLength property", 0)>
        <MappingsIgnored> Public Property FLAG_HEIGHT As String

        <Comment(
            "This property shows how to aligned the function description string in the drawing image.", 0)>
        <Comment(
        "Available value is: left, middle, right", 1)>
        Public Property FunctionAlignment As String = "middle"

        <Comment(
            "---------------------------------------------------------------------------------------------------" & vbCrLf &
            "                  This section will configure the drawing fonts style opetions" & vbCrLf &
            "---------------------------------------------------------------------------------------------------", -1)>
        <Comment("Text font value in html css style format:", 0)>
        <Comment("Example as: ""font-style: normal; font-size: 20; font-family: Ubuntu;""", 1)>
        Public Property LocusTagFont As String = CSSFont.UbuntuLarge
        Public Property FunctionAnnotationFont As String = CSSFont.UbuntuNormal
        Public Property SecondaryRuleFont As String = CSSFont.UbuntuNormal
        Public Property LegendFont As String = CSSFont.UbuntuNormal

#Region "Color Configurations"

        <Comment(
            "--------------------------------------------------------------------------------------------" & vbCrLf &
            "    The sections below will configure the colors profile of the script drawing objects." & vbCrLf &
            "--------------------------------------------------------------------------------------------", 0)>
        <Comment(
        "Value format: " & vbCrLf &
        "<Alpha(Integer)>,<Red(Integer)>,<Green(Integer)>,<Blue(Integer)> " & vbCrLf &
        "<Red(Integer)>,<Green(Integer)>,<Blue(Integer)> " & vbCrLf &
        "<ARGB(Integer)> " & vbCrLf &
        "<ColorName(String)>", 1)>
        <Comment(
        "Please notice that the A, R, G, B value for the color is limit at 0-255", 2)>
        Public Property DeletionMutation As String
        Public Property IntegrationMutant As String

        <Comment("This property specific the color value for drawing the tRNA gene", 0)>
        Public Property tRNAColor As String
        Public Property DefaultRNAColor As String
        Public Property ribosomalRNAColor As String

        <Comment("The default color for the gene unable to assigned cog number is brown.", 0)>
        Public Property NoneCogColor As String = "Brown"
#End Region

        <Comment(
            "--------------------------------------------------------------------------------------------------" & vbCrLf &
            "                The section below will configure the drawing data save opetions" & vbCrLf &
            "--------------------------------------------------------------------------------------------------", 0)>
        <Comment("Valid value is jpg, bmp, png, tiff", 1)>
        Public Property SavedFormat As String

        Public Property AddLegend As String

        Public Shared Function [DefaultValue]() As [Default](Of Config)
            Return New Config With {
                .Resolution = "18000,10000",
                .AspectRatio = "16:9",
                .DeletionMutation = "DarkBlue",
                .IntegrationMutant = "Red",
                .GeneObjectHeight = 85,
                .SavedFormat = "png",
                .tRNAColor = "Yellow",
                .LineLength = 0.1,
                .Margin = 400,
                .LineHeight = "5x",
                .FlagLength = 60,
                .FLAG_HEIGHT = "3x",
                .DefaultRNAColor = 155,
                .ribosomalRNAColor = "DarkGreen",
                .LocusTagFont = CSSFont.UbuntuLarge,
                .LegendFont = CSSFont.UbuntuNormal,
                .AddLegend = "TRUE"
            }
        End Function

#Region "Map Parser"

        ' 下面的这些函数都是进行反射操作之中自定义的数据类型加载的所必须要用到的函数

        Public Shared Function CssFontParser(css$, dpi As Integer) As Font
            Return CSSFont.TryParse(css).GDIObject(dpi)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function TypeOfAlignment(p As String) As TextPadding
            Return LabelPaddingExtensions _
                .TextAlignments _
                .TryGetValue(
                    index:=Strings.Trim(p).ToLower,
                    [default]:=LabelPaddingExtensions.defaultPadding
                )
        End Function

        Public Function GetDrawingSize(config As String) As Size
            Dim tokens$() = config.Trim.Split(CChar(","))

            If tokens.Length = 1 Then
                Dim arTokens = AspectRatio.Split(CChar(":"))
                Dim Width As Integer = Val(tokens.First)
                Dim Height As Integer = Width * Val(arTokens.Last) / Val(arTokens.First)
                Return New Size(Width, Height)
            Else
                Return New Size(Val(tokens.First), Val(tokens.Last))
            End If
        End Function

        Public Shared Function GetDrawingColor(strValue As String) As Color
            Return strValue.TranslateColor
        End Function

        Public Shared Function GetSavedImageFormat(config As String) As ImageFormat
            Return GetSaveImageFormat(config)
        End Function

        Public Function GetTextAlignment(s As String) As DataReader.TextAlignment
            Select Case s.ToLower.Trim
                Case "left" : Return DataReader.TextAlignment.Left
                Case "right" : Return DataReader.TextAlignment.Right
                Case Else
                    Return DataReader.TextAlignment.Middle
            End Select
        End Function
#End Region

        Public Function ToConfigurationModel() As DataReader
            Dim data As DataReader = ConfigurationMappings.LoadMapping(Of DataReader, Config)(Me)

            With data
                .LineHeight = GetLineHeight
                .FLAG_HEIGHT = GetFlagHeight

                Return data
            End With
        End Function

        Public Function Save(Path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
            Dim Text As String = Me.ToConfigDoc
            Return Text.SaveTo(Path, encoding)
        End Function

        Public Overrides Function ToString() As String
            Return Me.ToConfigDoc
        End Function

        Public Function Save(Path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(Path, encoding.CodePage)
        End Function
    End Class
End Namespace
