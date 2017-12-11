Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Analysis.Metagenome.gast
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus

Namespace greengenes

    Public Module Extensions

        <Extension>
        Public Iterator Function OTUgreengenesTaxonomy(blastn As IEnumerable(Of Query), OTUs As Dictionary(Of String, NamedValue(Of Integer)), taxonomy As Dictionary(Of String, otu_taxonomy)) As IEnumerable(Of gastOUT)
            For Each query As Query In blastn
                If Not OTUs.ContainsKey(query.QueryName) Then
                    Continue For
                End If

                ' Create an array Of taxonomy objects For all the associated refssu_ids.
                Dim hits = query _
                    .SubjectHits _
                    .Select(Function(h) taxonomy(h.Name)) _
                    .Select(Function(h)
                                Return New NamedValue(Of gast.Taxonomy) With {
                                    .Name = h.ID,
                                    .Value = New gast.Taxonomy(h.ToString)
                                }
                            End Function) _
                    .ToArray
            Next
        End Function
    End Module
End Namespace