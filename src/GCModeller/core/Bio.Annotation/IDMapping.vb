Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' GCModeller id mapping services based on the <see cref="Ptf.ProteinAnnotation"/>
''' </summary>
Public Module IDMapping

    <Extension>
    Public Iterator Function Mapping(Of T As {INamedValue, Class})(proteins As Ptf.PtfFile, data As IEnumerable(Of T)) As IEnumerable(Of T)
        Dim unifyIdIndex As Dictionary(Of String, String) = proteins _
            .AsEnumerable _
            .Select(Function(pro)
                        Return pro.attributes _
                            .Select(Iterator Function(a) As IEnumerable(Of (id$, geneId$))
                                        For Each id As String In a.Value
                                            If HeaderFormats.HasVersionNumber(id) Then
                                                Yield (HeaderFormats.TrimAccessionVersion(id), pro.geneId)
                                            End If

                                            Yield (id, pro.geneId)
                                        Next
                                    End Function)
                    End Function) _
            .IteratesALL _
            .IteratesALL _
            .GroupBy(Function(a) a.id) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a.First.geneId
                          End Function)

        For Each protein In data
            If unifyIdIndex.ContainsKey(protein.Key) Then
                protein.Key = unifyIdIndex(protein.Key)
            End If

            Yield protein
        Next
    End Function

End Module
