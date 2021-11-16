#Region "Microsoft.VisualBasic::5d1de4e9dd5099dc50db78a84bb3f09f, visualize\Circos\Circos\Colors\CircosColor.vb"

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

    '     Module CircosColor
    ' 
    '         Properties: AllCircosColors, DefaultCOGColor
    ' 
    '         Function: ColorFromHSV, (+2 Overloads) ColorProfiles, FromColor, FromHsv, FromKnownColorName
    '                   FromRGB, getColorMaps, getColorNames, getColorRgb, loadResource
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports r = System.Text.RegularExpressions.Regex

Namespace Colors

    ''' <summary>
    ''' Crcos程序的所支持的颜色
    ''' </summary>
    ''' <remarks></remarks>
    <Package("Circos.Color",
                  Category:=APICategories.UtilityTools,
                  Publisher:="amethyst.asuka@gcmodeller.org",
                  Description:="Provides of the mappings between the scale vector and the circos color string, 
                  and also provides the mappings option between the .NET color value and the circos color name string.")>
    Public Module CircosColor

        ''' <summary>
        ''' 在这里是需要根据RGB的数值将其映射为文本
        ''' </summary>
        ''' <remarks></remarks>
        Dim ColorNames As KeyValuePair(Of Color, String)()
        Dim RGBColors As Dictionary(Of String, Color) = CircosColor.loadResource

#Region "Initialize"

        ''' <summary>
        ''' 从资源文件之中加载可以被使用的CIRCOS颜色映射数据，这个函数会在模块的构造函数之中自动调用
        ''' </summary>
        ''' <remarks></remarks>
        ''' 
        Private Function loadResource() As Dictionary(Of String, Color)
            Dim clBufs$() = LinqAPI.Exec(Of String) <= {
 _
                Strings.Split(My.Resources.colors, vbLf),
                Strings.Split(My.Resources.colors_brewer, vbLf),
                Strings.Split(My.Resources.colors_brewer_lists, vbLf),
                Strings.Split(My.Resources.colors_ucsc, vbLf),
                Strings.Split(My.Resources.colors_unix, vbLf)
            }

            Dim value As List(Of NamedValue(Of String)) = clBufs.getColorNames
            Dim RGBValue = value.getColorRgb
            Dim RGBList As NamedValue(Of String)() = RGBValue.Select(Function(x) x.color).ToArray
            Dim nameEquals = LinqAPI.Exec(Of NamedValue(Of String)) _
 _
                () <= From item As NamedValue(Of String)
                      In value.AsParallel
                      Where Array.IndexOf(RGBList, item) = -1
                      Select item

            CircosColor.ColorNames = RGBValue.getColorMaps

            Dim colors = From item As KeyValuePair(Of Color, String)
                         In CircosColor.ColorNames.AsParallel
                         Let name As String = item.Value.ToLower.Trim.Split.Last
                         Select clName = name,
                              item.Key
                         Group By clName Into Group

            CircosColor.RGBColors = colors.ToDictionary(Function(x) x.clName, Function(x) x.Group.First.Key)
            CircosColor.RGBColors = (From Color
                                     In CircosColor.RGBColors
                                     Where Color.Value.R <> 0 AndAlso Color.Value.G <> 0 AndAlso Color.Value.B <> 0
                                     Select Color) _
                                        .ToDictionary(Function(x) x.Key,
                                                      Function(x) x.Value)
            CircosColor.RGBColors.Add("black", Color.Black)

            Call $"Circos color profiles init done!".__DEBUG_ECHO

            Return CircosColor.RGBColors
        End Function

        <Extension>
        Private Function getColorMaps(RgbValues As (color As NamedValue(Of String), name$, Rgb As String())()) As KeyValuePair(Of Color, String)()
            Return LinqAPI.Exec(Of KeyValuePair(Of Color, String)) _
 _
                () <= From item
                      In RgbValues.AsParallel
                      Where item.Rgb.Length >= 3
                      Let R As Integer = CInt(Val(item.Rgb(0)))
                      Let G As Integer = CInt(Val(item.Rgb(1)))
                      Let B As Integer = CInt(Val(item.Rgb(2)))
                      Let color = Color.FromArgb(R, G, B)
                      Select New KeyValuePair(Of Color, String)(color, item.name)
        End Function

        <Extension>
        Private Function getColorRgb(colors As IEnumerable(Of NamedValue(Of String))) As (color As NamedValue(Of String), name$, Rgb As String())()
            Return LinqAPI.Exec(Of (NamedValue(Of String), String, String())) _
 _
                () <= From item As NamedValue(Of String)
                      In colors.AsParallel
                      Let rgb = r.Match(item.Value, "\d+,\d+,\d+").Value
                      Where Not String.IsNullOrEmpty(rgb)
                      Let name = item.Name.Trim
                      Let Rgbtokens = rgb.Split(","c)
                      Select (item, name, Rgbtokens)
        End Function

        <Extension>
        Private Function getColorNames(expressions As IEnumerable(Of String)) As List(Of NamedValue(Of String))
            Return LinqAPI.MakeList(Of NamedValue(Of String)) _
 _
                () <= From str As String
                      In expressions.AsParallel
                      Let strM As String = r.Match(str, ".+?=\s*\S+").Value
                      Where Not String.IsNullOrEmpty(strM)
                      Let tokens As String() = Strings.Split(strM, "=")
                      Let name = tokens.First
                      Let color = r.Replace(strM, $"{name}\s*=\s*", "").Trim.Split.First
                      Select New NamedValue(Of String) With {
                          .Name = name,
                          .Value = color
                      }
        End Function
#End Region

        Public ReadOnly Property AllCircosColors As String()
            Get
                Return LinqAPI.Exec(Of String) <=
                    From x As KeyValuePair(Of Color, String)
                    In CircosColor.ColorNames
                    Select x.Value
                    Distinct
            End Get
        End Property

        ''' <summary>
        ''' Gets the .NET color object from the circos color name. If the function failed, then the Color.Black value will be return.
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <returns></returns>
        <ExportAPI("From.Name", Info:="Gets the .NET color object from the circos color name. If the function failed, then the Color.Black value will be return.")>
        Public Function FromKnownColorName(Name As String) As Color
            Dim key As String = Name.ToLower

            If CircosColor.RGBColors.ContainsKey(key) Then
                Return CircosColor.RGBColors(key)
            Else
                Return Color.Black
            End If
        End Function

        Public ReadOnly Property DefaultCOGColor As String = Color.Brown.RGBExpression

        ''' <summary>
        ''' Gets circos color name from the .NET color object R,G,B value.
        ''' </summary>
        ''' <param name="R"></param>
        ''' <param name="G"></param>
        ''' <param name="B"></param>
        ''' <returns></returns>
        <ExportAPI("From.RGB", Info:="Gets circos color name from the .NET color object R,G,B value.")>
        Public Function FromRGB(R As Integer, G As Integer, B As Integer) As String
            Dim LQuery As String =
                LinqAPI.DefaultFirst(Of String) <= From color As KeyValuePair(Of Color, String)
                                                   In CircosColor.ColorNames
                                                   Let cl As Color = color.Key
                                                   Where cl.R = R AndAlso
                                                       cl.G = G AndAlso
                                                       cl.B = B
                                                   Select color.Value
                                                   Order By Len(Value) Ascending
            Return LQuery
        End Function

        ''' <summary>
        ''' 将hsv颜色转换为Circos里面的颜色名称
        ''' </summary>
        ''' <param name="H"></param>
        ''' <param name="S"></param>
        ''' <param name="V"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("From.HSV", Info:="Gets circos color name from HSV color value.")>
        Public Function FromHsv(H As Double, S As Double, V As Double) As String
            Dim Color As Color = ColorFromHSV(H, S, V)
            Return FromRGB(Color.R, Color.G, Color.B)
        End Function

        <ExportAPI("Color.From.HSV", Info:="Gets .NET color object from the hsv color value.")>
        Public Function ColorFromHSV(hue As Double, saturation As Double, value As Double) As Color
            Dim hi As Integer = Convert.ToInt32(Math.Floor(hue / 60)) Mod 6
            Dim f As Double = hue / 60 - Math.Floor(hue / 60)

            value = value * 255

            Dim v As Integer = Convert.ToInt32(value)
            Dim p As Integer = Convert.ToInt32(value * (1 - saturation))
            Dim q As Integer = Convert.ToInt32(value * (1 - f * saturation))
            Dim t As Integer = Convert.ToInt32(value * (1 - (1 - f) * saturation))

            If hi = 0 Then
                Return Color.FromArgb(255, v, t, p)
            ElseIf hi = 1 Then
                Return Color.FromArgb(255, q, v, p)
            ElseIf hi = 2 Then
                Return Color.FromArgb(255, p, v, t)
            ElseIf hi = 3 Then
                Return Color.FromArgb(255, p, q, v)
            ElseIf hi = 4 Then
                Return Color.FromArgb(255, t, p, v)
            Else
                Return Color.FromArgb(255, v, p, q)
            End If
        End Function

        ''' <summary>
        ''' 将VB.NET的颜色映射为Perl之中的颜色
        ''' </summary>
        ''' <param name="Color"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("From.Color", Info:="Gets the Circos color name from the .NET color object RGB value.")>
        <Extension>
        Public Function FromColor(Color As Drawing.Color) As String
            Return FromRGB(Color.R, Color.G, Color.B)
        End Function

        ''' <summary>
        ''' Mappings each item in the categories into the Circos color name to generates a color profiles for drawing the elements in the circos plot.
        ''' (生成颜色谱, {<paramref name="categories"/>, Circos <see cref="System.Drawing.Color"/> Code})
        ''' </summary>
        ''' <param name="categories"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Color.Profiles", Info:="Mappings each item in the categories into the Circos color name to generates a color profiles for drawing the elements in the circos plot.")>
        <Extension> Public Function ColorProfiles(Of T)(categories As T()) As Dictionary(Of T, String)
            Dim Colors As String() = CircosColor.RGBColors.Keys.Shuffles

            If categories.IsNullOrEmpty OrElse (categories.Count = 1 AndAlso categories(Scan0) Is Nothing) Then
                Call $"{NameOf(categories)} is null...".Warning
                categories = New T() {}
            End If

            Return categories _
                .SeqIterator _
                .ToDictionary(Function(cl) cl.value,
                              Function(cl)
                                  Return Colors(cl.i)
                              End Function)
        End Function

        ''' <summary>
        ''' Mappings each item in the categories into the Circos color name to generates a color profiles for drawing the elements in the circos plot.
        ''' </summary>
        ''' <param name="categories"></param>
        ''' <returns></returns>
        <ExportAPI("Color.Profiles",
                   Info:="Mappings each item in the categories into the Circos color name to generates a color profiles for drawing the elements in the circos plot.")>
        <Extension> Public Function ColorProfiles(categories As IEnumerable(Of String)) As Dictionary(Of String, String)
            Return ColorProfiles(Of String)(categories.ToArray)
        End Function
    End Module
End Namespace
