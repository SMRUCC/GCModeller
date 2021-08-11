
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.Microarray.PhenoGraph

''' <summary>
''' PhenoGraph algorithm
''' 
''' Jacob H. Levine and et.al. Data-Driven Phenotypic Dissection of AML Reveals Progenitor-like Cells that Correlate with Prognosis. Cell, 2015.
''' </summary>
<Package("phenograph")>
Module phenograph

    ''' <summary>
    ''' PhenoGraph algorithm
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <param name="k"></param>
    ''' <param name="cutoff"></param>
    ''' <returns></returns>
    <ExportAPI("phenograph")>
    Public Function phenograph(matrix As Matrix, Optional k As Integer = 30, Optional cutoff As Double = 0) As NetworkGraph
        Dim sampleId = matrix.sampleID.SeqIterator.ToArray
        Dim dataset As DataSet() = matrix.expression _
            .Select(Function(gene)
                        Return New DataSet With {
                            .ID = gene.geneID,
                            .Properties = sampleId _
                                .ToDictionary(Function(id) id.value,
                                              Function(i)
                                                  Return gene.experiments(i)
                                              End Function)
                        }
                    End Function) _
            .Transpose
        Dim graph As NetworkGraph = CommunityGraph.CreatePhenoGraph(
            data:=dataset,
            k:=k,
            cutoff:=cutoff
        )

        Return graph
    End Function

End Module
