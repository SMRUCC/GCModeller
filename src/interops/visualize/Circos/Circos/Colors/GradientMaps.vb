#Region "Microsoft.VisualBasic::2b097cb8e8d018ea54dc4a570b834c8a, ..\interops\visualize\Circos\Circos\Colors\GradientMaps.vb"

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

Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Mathematical
Imports System.Runtime.CompilerServices

Namespace Colors

    ''' <summary>
    ''' Crcos程序的所支持的颜色
    ''' </summary>
    ''' <remarks></remarks>
    <PackageNamespace("Circos.Color.GradientMaps",
                      Category:=APICategories.UtilityTools,
                      Publisher:="amethyst.asuka@gcmodeller.org",
                      Description:="Provides of the mappings between the scale vector and the circos color string, 
                  and also provides the mappings option between the .NET color value and the circos color name string.")>
    Public Module GradientMaps

        <ExportAPI("Map.Annotations")>
        Public Function MapAnnotations(<Parameter("map.Name")> mapName As String,
                                       <Parameter("map.Levels")> mapLevel As Integer,
                                       <Parameter("Title")> title As String,
                                       <Parameter("Is.Percentage?")> Optional isPercentage As Boolean = True,
                                       Optional min As Double = 0,
                                       Optional max As Double = 1) As Image

            Dim Gr As GDIPlusDeviceHandle = New Size(1024, 300).CreateGDIDevice
            Dim titleFont As New Font(FontFace.MicrosoftYaHei, 64, FontStyle.Regular)
            Dim sz As SizeF = Gr.Graphics.MeasureString(title, titleFont)

            Call Gr.Graphics.DrawString(title, titleFont, Brushes.Black, New Point)

            Dim Y As Integer = CInt(5 + sz.Height) - 20
            Dim X As Double = 5
            Dim maps As New ColorMap(mapLevel * 2)
            Dim clSequence As Color() = ColorSequence(maps, mapName).Reverse.ToArray
            Dim offset = CInt(clSequence.Length * offsetPercentage)
            Dim drWidth As Integer = Gr.Width - 5 * 2
            Dim dx As Double = drWidth / (clSequence.Length - offset)
            Dim drHeight As Integer = CInt((Gr.Height - sz.Height) * 0.85)

            Dim sMin, sMax As String
            Dim ruleFont As New Font(FontFace.MicrosoftYaHei, 24, FontStyle.Bold)

            If isPercentage Then
                sMin = Mid(CStr(min * 100), 1, 6) & "%"
                sMax = Mid(CStr(max * 100), 1, 6) & "%"
            Else
                sMin = Mid(CStr(min), 1, 6)
                sMax = Mid(CStr(max), 1, 6)
            End If

            Call Gr.DrawString(sMin, ruleFont, Brushes.Black, New Point(CInt(X), Y + drHeight))

            For idx As Integer = offset To clSequence.Length - 1
                Dim color As Color = clSequence(idx)
                Dim currX As Integer = CInt(X)

                X += dx

                Dim currXNext As Integer = CInt(X)
                Dim rect As New Rectangle(New Point(currX, Y), New Size(CInt(dx), drHeight))

                Call Gr.FillRectangle(New SolidBrush(color), rect)
            Next

            sz = Gr.MeasureString(sMax, ruleFont)
            Call Gr.DrawString(sMax, ruleFont, Brushes.Black, New Point(CInt(Gr.Width - sz.Width), Y + drHeight))

            Return Gr.ImageResource
        End Function

        Dim offsetPercentage As Double = 0.06

        ''' <summary>
        ''' Creates a scale gradient color mappings between a vector and the circos RGB color.
        ''' </summary>
        ''' <param name="values"></param>
        ''' <param name="mapName"><see cref="ColorMap"/></param>
        ''' <param name="mapLevel"></param>
        ''' <param name="offset"></param>
        ''' <param name="replaceBase"></param>
        ''' <returns></returns>
        <ExportAPI("Gradient.Mappings",
                   Info:="Creates a scale gradient color mappings between a vector and the circos RGB color.")>
        <Extension>
        Public Function GradientMappings(values As IEnumerable(Of Double),
                                         Optional mapName As String = "Jet",
                                         Optional mapLevel As Integer = 512,
                                         Optional offset As Double = Double.NaN,
                                         Optional replaceBase As Boolean = False) As Mappings()
            If Not Double.IsNaN(offset) Then
                values = values.Join(offset)
            End If

            Dim mapLvs = values.GenerateMapping(mapLevel)
            Dim maps As New __maps(values, mapName, mapLevel, replaceBase)

            Call mapLvs.Max.__DEBUG_ECHO
            Call mapLvs.Min.__DEBUG_ECHO

            Dim mappings As Mappings() =
                mapLvs.ToArray(AddressOf maps.CreateMaps)
            Return mappings
        End Function

        Private Structure __maps

            Public values As Double()
            Public clSequence As Color()
            Public replaceBase As Boolean

            Dim highest As Color
            Dim offset As Integer

            Sub New(values As IEnumerable(Of Double), name As String, mapLevels As Integer, replaceBase As Boolean)
                Dim maps As New ColorMap(mapLevels * 2)
                Me.clSequence = ColorSequence(maps, name).Reverse.ToArray
                Me.values = values.ToArray
                Me.replaceBase = replaceBase
                Me.highest = clSequence.Last
                Me.offset = CInt(clSequence.Length * offsetPercentage)
            End Sub

            Public Function CreateMaps(lv As Integer, index As Integer) As Mappings
                Dim value As Double = values(index)
                Dim Color As Color

                If lv <= 1 AndAlso replaceBase Then
                    Color = Color.WhiteSmoke
                Else
                    Dim idx As Integer = lv + offset

                    If idx < clSequence.Length Then
                        Color = clSequence(idx)
                    Else
                        Color = highest
                    End If
                End If

                Return New Mappings With {
                    .value = value,
                    .Level = lv,
                    .Color = Color
                }
            End Function
        End Structure
    End Module
End Namespace
