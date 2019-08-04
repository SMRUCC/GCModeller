Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.Distributions

Namespace BarPlot.Data

    Public Module Constructor

        ''' <summary>
        ''' 这个应该是生成直方图的数据
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="base!"></param>
        ''' <param name="color$"></param>
        ''' <returns></returns>
        Public Function FromDistributes(data As IEnumerable(Of Double), Optional base! = 10.0F, Optional color$ = "darkblue") As BarDataGroup
            Dim source = data.Distributes(base!)
            Dim bg As Color = color.ToColor(onFailure:=Drawing.Color.DarkBlue)
            Dim values As New List(Of Double)
            Dim serials = LinqAPI.Exec(Of NamedValue(Of Color)) _
 _
                () <= From lv As Integer
                      In source.Keys
                      Let tag As String = lv.ToString
                      Select New NamedValue(Of Color) With {
                          .Name = tag,
                          .Value = bg
                      }

            For Each x As NamedValue(Of Color) In serials
                values += source(CInt(x.Name)).Value
            Next

            Return New BarDataGroup With {
                .Serials = serials,
                .Samples = {
                    New BarDataSample With {
                        .Tag = "Distribution",
                        .data = values
                    }
                }
            }
        End Function
    End Module
End Namespace