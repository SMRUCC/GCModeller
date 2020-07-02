Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports SMRUCC.genomics.foundation.BIOM.v10

Public Module BIOMExtensions

    <Extension>
    Public Function Union(tables As IEnumerable(Of BIOMDataSet(Of Double))) As IEnumerable(Of DataSet)
        Dim matrix As New Dictionary(Of String, DataSet)

        For Each table As BIOMDataSet(Of Double) In tables
            For Each otu In table.PopulateRows
                If Not matrix.ContainsKey(otu.Name) Then
                    matrix(otu.Name) = New DataSet With {
                        .ID = otu.Name
                    }
                End If

                Call matrix(otu.Name).Append(otu, AddressOf Math.Max)
            Next
        Next

        Return matrix.Values
    End Function
End Module
