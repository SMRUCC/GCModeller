Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Linq
Imports System.Runtime.CompilerServices

Module DataSource

    Public Function LoadAs(path As String) As FlareImports()
        Return LoadFromMaps(path)
    End Function

    Public Const fromNode As String = NameOf(fromNode)
    Public Const toNode As String = NameOf(toNode)
    Public Const Confidence As String = NameOf(Confidence)

    Public Function LoadFromMaps(path As String,
                                 Optional fromMap As String = fromNode,
                                 Optional toMap As String = toNode,
                                 Optional sizeMap As String = Confidence) As FlareImports()

        Dim maps As New Dictionary(Of String, String) From {
            {fromNode, fromMap},
            {toNode, toMap},
            {Confidence, sizeMap}
        }
        Dim interacts As IEnumerable(Of Interacts) =
            path.LoadCsv(Of Interacts)(maps:=maps)
        Dim GroupNodes = From x As Interacts
                         In interacts
                         Select x
                         Group x By x.From Into Group
        Dim __imports As FlareImports() =
            LinqAPI.Exec(Of FlareImports) <=
                From g In GroupNodes
                Let datas As Interacts() = g.Group.ToArray
                Select New FlareImports With {
                    .name = g.From.AllTrims,
                    .size = datas.Sum(Function(x) If(x.size = 0, 1, x.size)),
                    .imports = datas.ToArray(Function(x) x.To.AllTrims)
                }

        Return __imports
    End Function

    <Extension>
    Private Function AllTrims(s As String) As String
        Return "flare.vis.operator.label." &
            s.Replace(".", "_") _
             .Replace("[", "_") _
             .Replace("]", "_")
    End Function
End Module

Public Class Interacts : Implements sIdEnumerable

    <Column(fromNode)>
    Public Property From As String Implements sIdEnumerable.Identifier
    <Column(toNode)>
    Public Property [To] As String
    <Column(Confidence)>
    Public Property size As Integer

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class