Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Mathematical.Calculus
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' Methods for raw data processing
''' </summary>
Public Module Normalization

    Public Structure TimeValue

        Public Time#, value#

        Public Overrides Function ToString() As String
            Return $"[{Time}]  {value}"
        End Function
    End Structure

    <Extension>
    Public Function Normalize(data As IEnumerable(Of NamedValue(Of TimeValue())), Optional expected% = 5000) As ODEsOut

        Return data.Build
    End Function

    <Extension>
    Public Function Build(data As IEnumerable(Of NamedValue(Of TimeValue()))) As ODEsOut
        Dim array As NamedValue(Of TimeValue())() =
            data.ToArray
        Return New ODEsOut With {
            .x = array(Scan0).x _
                .ToArray(Function(x) x.Time),
            .y = array _
                .Select(Function(x) New NamedValue(Of Double()) With {
                    .Name = x.Name,
                    .x = x.x _
                        .ToArray(Function(o) o.value)
                }).ToDictionary
        }
    End Function
End Module
