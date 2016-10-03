#Region "Microsoft.VisualBasic::a96b8cd94b1d6324b3670782fd3712ef, ..\GCModeller\visualize\visualizeTools\ChromosomeMap\Configurations.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports System.Text.RegularExpressions
Imports System.Runtime.CompilerServices
Imports System.Drawing
Imports SMRUCC.genomics.Visualize.ValueParser
Imports Microsoft.VisualBasic.Serialization
Imports Oracle.Java.IO.Properties
Imports Microsoft.VisualBasic.Imaging

Namespace ChromosomeMap

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
        "     Gmail:    xie.guigang@gmail.com" & vbCrLf &
        "     Twitter:  @xieguigang(https://twitter.com/xieguigang)", 1)>
    Public Class Configurations : Inherits ConfigCommon

        <Comment("This property specific the height value of the arrow shape which is stand for a gene object on the chromesome.", 0)>
        Public Property GeneObjectHeight As Integer

        <Comment("This property specific the drawing segment length of a line, value unit is Mbp; defualt value is 0.1MBp, " & vbCrLf &
            "which means the script will draw the line stands 100KBp", 0)>
        Public Property LineLength As Double

        <Comment("This property specific the drawing height of a line, it can be in the format of a integer value or a expression " & vbCrLf &
            "associated with GeneObjectHeight property.", 0)>
        <Comment("Value example:   600, the property value is specific as an integer which means the drawing height will be 600 pixels;" & vbCrLf &
            "6x , this property value is assign by a expression which means the drawing height value " & vbCrLf &
            "is the 6 times of the GeneObjectHeight value.", 1)>
        Public Property LineHeight As String

        Public Property Margin As Integer

        Public Property FlagLength As Integer
        <Comment("This property value can be both Integer value or an expression associated with FlagLength property", 0)>
        <MappingsIgnored> Public Property FLAG_HEIGHT As String

        <Comment("This property shows how to aligned the function description string in the drawing image.", 0)>
        <Comment("Available value is: left, middle, right", 1)>
        Public Property FunctionAlignment As String = "middle"

        <Comment(
            "---------------------------------------------------------------------------------------------------" & vbCrLf &
            "                  This section will configure the drawing fonts style opetions" & vbCrLf &
            "---------------------------------------------------------------------------------------------------", -1)>
        <Comment("""FontName"",size[,bold/italic/regular/strikeout/underline]", 0)>
        <Comment("""FontName"",size[,StyleCombination]" & vbCrLf &
        "Drawing.FontStyle.Bold = 1" & vbCrLf &
                   "Drawing.FontStyle.Italic = 2" & vbCrLf &
                   "Drawing.FontStyle.Regular = 0" & vbCrLf &
                 "Drawing.FontStyle.Strikeout = 8" & vbCrLf &
                   "Drawing.FontStyle.Underline = 4", 1)>
        Public Property LocusTagFont As String = """Ubuntu"",20"
        Public Property FunctionAnnotationFont As String = """Ubuntu"",12"
        Public Property SecondaryRuleFont As String = """Ubuntu"",12"
        Public Property LegendFont As String = """Ubuntu"",12"

#Region "Color Configurations"

        <Comment("--------------------------------------------------------------------------------------------" & vbCrLf &
                 "    The sections below will configure the colors profile of the script drawing objects." & vbCrLf &
                 "--------------------------------------------------------------------------------------------", 0
            )>
        <Comment(
            "Value format: " & vbCrLf &
            "<Alpha(Integer)>,<Red(Integer)>,<Green(Integer)>,<Blue(Integer)> " & vbCrLf &
            "<Red(Integer)>,<Green(Integer)>,<Blue(Integer)> " & vbCrLf &
            "<ARGB(Integer)> " & vbCrLf &
            "<ColorName(String)>", 1)>
        <Comment("Please notice that the A, R, G, B value for the color is limit at 0-255", 2)>
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
        <Comment("Valid value is jpg, bmp, png, gif, wmf, emf", 1)>
        Public Property SavedFormat As String

        Public Property AddLegend As String

        Public Shared Function [DefaultValue]() As Configurations
            Return New Configurations With {
                .Resolution = "10000,4000",
                .AspectRatio = "16:9",
                .DeletionMutation = "DarkBlue",
                .IntegrationMutant = "Red",
                .GeneObjectHeight = 85,
                .SavedFormat = "wmf",
                .tRNAColor = "Yellow",
                .LineLength = 0.1,
                .Margin = 400,
                .LineHeight = "5x",
                .FlagLength = 60,
                .FLAG_HEIGHT = "3x",
                .DefaultRNAColor = 155,
                .ribosomalRNAColor = "DarkGreen",
                .LocusTagFont = """Ubuntu"",20",
                .AddLegend = "TRUE"
            }
        End Function

        Public Shared Function TypeOfAlignment(p As String) As DrawingModels.SegmentObject.__TextAlignment
            p = p.Trim.ToLower

            If Not DrawingModels.SegmentObject.TextAlignments.ContainsKey(p) Then
                Return DrawingModels.SegmentObject.TextAlignments("middle")
            Else
                Return DrawingModels.SegmentObject.TextAlignments(p.ToLower)
            End If
        End Function

        ''' <summary>
        ''' 从文件之中读取配置数据
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MappingsIgnored>
        Public Shared Widening Operator CType(Path As String) As Configurations
            Return Oracle.Java.IO.Properties.Properties.Load(Path).FillObject(Of Configurations)()
        End Operator

        Public Function GetDrawingSize(config As String) As System.Drawing.Size
            Dim Tokens As String() = config.Trim.Split(CChar(","))

            If Tokens.Count = 1 Then
                Dim arTokens = AspectRatio.Split(CChar(":"))
                Dim Width As Integer = Val(Tokens.First)
                Dim Height As Integer = Width * Val(arTokens.Last) / Val(arTokens.First)
                Return New System.Drawing.Size(Width, Height)
            Else
                Return New System.Drawing.Size(Val(Tokens.First), Val(Tokens.Last))
            End If
        End Function

        Public Shared Function GetDrawingColor(strValue As String) As System.Drawing.Color
            If String.IsNullOrEmpty(strValue) Then
                Return System.Drawing.Color.Black
            End If

            Dim Tokens As String() = strValue.Split(CChar(","))

            If Tokens.IsNullOrEmpty Then '值是空的，返回默认的黑色
                Return System.Drawing.Color.Black
            ElseIf Tokens.Count = 4 Then
                Dim A = Val(Tokens(0)), R = Val(Tokens(1)), G = Val(Tokens(2)), B = Val(Tokens(3))
                Return System.Drawing.Color.FromArgb(A, R, G, B)
            ElseIf Tokens.Count = 3 Then
                Dim A = Val(Tokens(0)), R = Val(Tokens(1)), G = Val(Tokens(2)), B = Val(Tokens(3))
                Return System.Drawing.Color.FromArgb(R, G, B)
            ElseIf Tokens.Count = 1 Then
                Dim argb_value As Integer = Val(Tokens.First)
                If argb_value = 0 Then '可能是一个颜色的名称
                    Return System.Drawing.Color.FromName(Tokens.First.Trim)
                Else
                    Return System.Drawing.Color.FromArgb(argb_value)
                End If
            End If
        End Function

        Public Function GetSavedImageFormat(config As String) As System.Drawing.Imaging.ImageFormat
            Return GetSaveImageFormat(config)
        End Function

        Public Function ToConfigurationModel() As Conf
            Dim data As Conf = ConfigurationMappings.LoadMapping(Of Conf, Configurations)(Me)
            data.LineHeight = Me.GetLineHeight
            data.FLAG_HEIGHT = Me.GetFlagHeight

            Return data
        End Function

        Public Function GetTextAlignment(s As String) As Conf.TextAlignment
            Select Case s.ToLower.Trim
                Case "left" : Return Conf.TextAlignment.Left
                Case "right" : Return Conf.TextAlignment.Right
                Case Else
                    Return Conf.TextAlignment.Middle
            End Select
        End Function

        Public Overrides Function Save(Optional Path As String = "", Optional encoding As System.Text.Encoding = Nothing) As Boolean
            Dim Text As String = Me.ToConfigDoc
            Return Text.SaveTo(Path, encoding)
        End Function
    End Class

    ''' <summary>
    ''' 配置数据的数据模型，请使用<see cref="Configurations.ToConfigurationModel"></see>方法来初始化本数据模型对象，真正所被使用到的内存之中的配置文件的对象模型
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Conf

        Public Enum TextAlignment
            Left
            Right
            Middle
        End Enum

        ''' <summary> 
        ''' This section will configure the drawing size options
        ''' ----------------------------------------------------
        ''' Due to the GDI+ limitations in the .NET Framework, the image size is limited by your computer memory size, if you want to 
        ''' drawing a very large size image, please running this script on a 64bit platform operating system, or you will get a 
        ''' exception about the GDI+ error: parameter is not valid and then you should try a smaller resolution of the drawing output image.
        ''' Value format: &lt;Width(Integer)&gt;[,&lt;Height(Integer)&gt;]
        ''' Example:
        ''' Both specific the size property: 12000,8000
        ''' Which means the drawing script will generate a image file in resolution of width is value 12000 pixels and image height 
        ''' is 8000 pixels.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <UseCustomMapping>
        Public Property Resolution As Size
        Public Property GeneObjectHeight As Integer
        ''' <summary>
        ''' The chromosome segment length of each line on the chromosome map, this property can effects the drawing model scale factor.
        ''' (在ChromosomeMap之中的每一行基因组片段的长度值，通过这个属性可以影响图形的缩放大小)
        ''' </summary>
        ''' <returns></returns>
        Public Property LineLength As Double
        <MappingsIgnored> Public Property LineHeight As Integer
        Public Property Margin As Integer
        Public Property FlagLength As Integer
        <MappingsIgnored> Public Property FLAG_HEIGHT As String
        Public Property FunctionAlignment As TextAlignment
        Public Property LocusTagFont As Font
        Public Property FunctionAnnotationFont As Font
        Public Property SecondaryRuleFont As Font

        Public Property AddLegend As Boolean
        Public Property LegendFont As Font

#Region "Color Configurations"

        Public Property DeletionMutation As Color
        Public Property IntegrationMutant As Color
        Public Property tRNAColor As Color
        Public Property DefaultRNAColor As Color
        Public Property ribosomalRNAColor As Color
        Public Property NoneCogColor As Color
#End Region

        Public Property SavedFormat As System.Drawing.Imaging.ImageFormat
    End Class
End Namespace
