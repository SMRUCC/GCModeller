Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports RDotNet.Extensions.VisualBasic
Imports RDotNet.Extensions.VisualBasic.API

Public Class graph

    Public g As NetworkGraph

    Public Function write() As String

    End Function
End Class

Public Class graphList

    Public graphs As NamedValue(Of graph)()

    Public Function write() As String
        Dim list As New var(base.list)

        For Each map In graphs
            list(map.Name) = map.Value.write
        Next

        Return list
    End Function
End Class