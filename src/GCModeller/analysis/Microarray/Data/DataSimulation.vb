Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry

Public Module DataSimulation

    ''' <summary>
    ''' DO HTS data simulation for test analysis program
    ''' </summary>
    ''' <param name="proteins">基因组注释数据</param>
    ''' <param name="range">表达范围</param>
    ''' <param name="profiles">[KOmap => foldchange]</param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function PopulateSimulateData(proteins As PtfFile,
                                                  range As DoubleRange,
                                                  profiles As Dictionary(Of String, Double),
                                                  Optional replication As Integer = 6,
                                                  Optional tagName As String = "sample") As IEnumerable(Of DataSet)

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

        For Each protein As ProteinAnnotation In proteins.AsEnumerable
            If protein.
        Next
    End Function
End Module