Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Metagenomics

Namespace Assembly.NCBI.Taxonomy

    Public Module TaxonomyExtensions

        ''' <summary>
        ''' Create taxonomy lineage string with <see cref="BIOMPrefix"/>
        ''' </summary>
        ''' <param name="nodes"></param>
        ''' <returns></returns>
        ''' 
        <Extension> Public Function BuildBIOM(nodes As IEnumerable(Of TaxonomyNode)) As String
            Dim data As Dictionary(Of String, String) = TaxonomyNode.RankTable(nodes)
            Dim list As New List(Of String)

            For Each r$ In NcbiTaxonomyTree.stdranks.Reverse
                If data.ContainsKey(r) Then
                    list.Add(data(r$))
                Else
                    list.Add("")
                End If
            Next

            SyncLock BIOMPrefix
                Return list _
                    .SeqIterator _
                    .Select(Function(x) BIOMPrefix(x.i) & x.value) _
                    .JoinBy(";")
            End SyncLock
        End Function
    End Module
End Namespace