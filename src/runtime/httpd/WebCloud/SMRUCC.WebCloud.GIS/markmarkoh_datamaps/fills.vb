Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Mathematical
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Language

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
            Dim keys As String() = fills.Values.ToArray(Function(x) $"""{x.Name}"": ""{x.value}""")
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
                Dim x As Color = c.obj

                fills.fills += New NamedValue(Of String) With {
                    .Name = "c" & c.i.ToString,
                    .value = $"rgb({x.R},{x.G},{x.B})"
                }
            Next

            Dim dataKeys As New List(Of String)

            For Each x In levels.SeqIterator
                Dim c As String = data(x.i).Name
                Dim k As String = "c" & x.obj.ToString

                dataKeys += New NamedValue(Of fill) With {
                    .Name = c,
                    .Value = New fill With {
                        .fillKey = k
                    }
                }.NamedProperty
            Next

            fills.data = "{ " & String.Join(", ", dataKeys.ToArray) & " }"

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