Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Public Module DataSimulation

    ''' <summary>
    ''' DO HTS data simulation for test analysis program
    ''' </summary>
    ''' <param name="proteins">基因组注释数据</param>
    ''' <param name="range">表达范围</param>
    ''' <param name="profiles">[KOmap => log2(foldchange)]</param>
    ''' <param name="tagName">
    ''' the label formatter, ``%s`` for <paramref name="replication"/> n.
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function PopulateSimulateData(proteins As PtfFile,
                                                  range As DoubleRange,
                                                  profiles As Dictionary(Of String, Double),
                                                  Optional replication As Integer = 6,
                                                  Optional tagName As String = "sample_%s") As IEnumerable(Of DataSet)

        Dim KOMapIndex As New Dictionary(Of String, Double)

        For Each level_A In htext.ko00001.Hierarchical.categoryItems
            For Each level_B In level_A.categoryItems
                For Each map In level_B.categoryItems
                    ' default is no changes
                    Dim foldchange As Double = profiles.TryGetValue(map.entryID, default:=1)

                    For Each ko In map.categoryItems
                        KOMapIndex(ko.entryID) += foldchange
                    Next
                Next
            Next
        Next

        Dim sampleNames As String() = replication _
            .Sequence _
            .Select(Function(i) sprintf(tagName, i + 1)) _
            .ToArray

        For Each protein As ProteinAnnotation In proteins.AsEnumerable
            Dim ko As String = AnnotationReader.KO(protein)
            Dim scaleRange As DoubleRange = {0.9 * range.Min, 1.2 * range.Max}

            If Not ko.StringEmpty Then
                If KOMapIndex(ko) < 1 Then
                    scaleRange = {KOMapIndex(ko) * range.Min, range.Min}
                Else
                    scaleRange = {range.Max, KOMapIndex(ko) * range.Max}
                End If
            End If

            Dim data As New DataSet With {.ID = protein.geneId}

            For i As Integer = 1 To replication
                data(sampleNames(i - 1)) = randf.GetRandomValue(scaleRange)
            Next

            Yield data
        Next
    End Function
End Module