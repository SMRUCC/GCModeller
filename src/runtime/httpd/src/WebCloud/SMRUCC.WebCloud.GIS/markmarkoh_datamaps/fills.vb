#Region "Microsoft.VisualBasic::d53be77dc701e21abae6a0bec4872bd3, WebCloud\SMRUCC.WebCloud.GIS\markmarkoh_datamaps\fills.vb"

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

    '     Class MapFills
    ' 
    '         Properties: data, defaultFill, fills
    ' 
    '         Function: GetJson, ToString
    ' 
    '     Module ColorManager
    ' 
    '         Function: GetColors
    ' 
    '     Structure fill
    ' 
    '         Properties: fillKey
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace d3js.markmarkoh_datamaps

    Public Class MapFills

        Public Property defaultFill As String = "#ABDDA4"
        Public Property fills As New Dictionary(Of NamedValue(Of String))

        ''' <summary>
        ''' 这个属性不会被序列化
        ''' </summary>
        ''' <returns></returns>
        Public Property data As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        ''' <summary>
        ''' Generates the map fills for d3js maps:
        ''' 
        ''' ```javascript
        ''' var map = new Datamap({
        '''     element: document.getElementById('container'),
        '''     fills: {
        '''         defaultFill: 'rgba(23,48,210,0.9)' // Any hex, color name or rgb/rgba value
        '''     }
        ''' });
        ''' ```
        ''' </summary>
        ''' <returns></returns>
        Public Function GetJson() As String
            Dim keys As String() = fills.Values.Select(Function(x) $"""{x.Name}"": ""{x.Value}""").ToArray
            Return "{ defaultFill: """ & defaultFill & """, " & String.Join(", ", keys) & " }"
        End Function
    End Class

    Public Module ColorManager

        ''' <summary>
        ''' Generates css color value for Javascript reports
        ''' </summary>
        ''' <param name="countries"></param>
        ''' <param name="mapName"></param>
        ''' <param name="defaultFill"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GetColors(countries As IEnumerable(Of NamedValue(Of Integer)),
                                  Optional mapName As String = "Jet",
                                  Optional defaultFill As String = "#ABDDA4") As MapFills

            Dim colors = ColorMapsExtensions.ColorSequence
            Dim data = countries.ToArray
            Dim levels%() = data _
                .Select(Function(x) x.Value) _
                .GenerateMapping(colors.Length)
            Dim fills As New MapFills With {
                .defaultFill = defaultFill
            }

            For Each c As SeqValue(Of Color) In colors.SeqIterator
                Dim x As Color = c.value

                fills.fills += New NamedValue(Of String) With {
                    .Name = "c" & c.i.ToString,
                    .Value = $"rgb({x.R},{x.G},{x.B})"
                }
            Next

            Dim dataKeys As New Dictionary(Of String, fill)

            For Each x As SeqValue(Of Integer) In levels.SeqIterator
                Dim c As String = data(x.i).Name
                Dim k As String = "c" & x.value.ToString
                Dim fill As New fill With {
                    .fillKey = k
                }

                Call dataKeys.Add(c, fill)
            Next

            fills.data = dataKeys.GetJson

            Return fills
        End Function
    End Module

    Public Structure fill

        Public Property fillKey As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace
