Imports System.Runtime.CompilerServices
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports LANS.SystemsBiology.Assembly.DOOR
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic
Imports LANS.SystemsBiology.Toolkits.RNA_Seq

Namespace Analysis.FootprintTraceAPI

    ''' <summary>
    ''' 操作Operon调控相关的信息
    ''' </summary>
    <PackageNamespace("Operon.Footprints")>
    Public Module OperonFootprints

        ''' <summary>
        ''' 为调控关系之中的基因联系上操纵子的信息
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="DOOR"></param>
        ''' <returns></returns>
        <ExportAPI("Assign.DOOR")>
        <Extension>
        Public Function AssignDOOR(source As IEnumerable(Of PredictedRegulationFootprint), DOOR As DOOR) As List(Of PredictedRegulationFootprint)
            Dim LQuery = (From x In source Select x Group x By x.ORF Into Group)
            Dim result As New List(Of PredictedRegulationFootprint)

            For Each block In LQuery
                If String.IsNullOrEmpty(block.ORF) Then
                    result += block.Group
                Else
                    Dim gene = DOOR.GetGene(block.ORF)
                    Dim footprints = block.Group.ToArray

                    If Not gene Is Nothing Then ' RNA 基因没有记录，则会返回空值 
                        Dim first As Boolean = String.Equals(DOOR.DOOROperonView.GetOperon(gene.OperonID).InitialX.Synonym, gene.Synonym, StringComparison.Ordinal)
                        Dim flag As Char = If(first, "1"c, "0"c)

                        For Each x As PredictedRegulationFootprint In footprints
                            x.DoorId = gene.OperonID
                            x.InitX = flag
                        Next
                    End If
                    result += footprints
                End If
            Next

            Return result
        End Function

        ''' <summary>
        ''' 假若被调控的基因是操纵子的第一个基因，则后面的基因假设都会被一同调控
        ''' </summary>
        ''' <param name="footprints"></param>
        ''' <param name="DOOR"></param>
        ''' <returns>新拓展出来的数据以及原来的数据</returns>
        <ExportAPI("Expand.DOOR")>
        <Extension>
        Public Function ExpandDOOR(footprints As IEnumerable(Of PredictedRegulationFootprint),
                                   DOOR As DOOR,
                                   coors As Correlation2,
                                   cut As Double) As List(Of PredictedRegulationFootprint)
            Dim LQuery = (From x As PredictedRegulationFootprint
                          In footprints
                          Where x.InitX.getBoolean
                          Select x,
                              opr = DOOR.GetOperon(x.ORF))
            Dim expands = (From x In LQuery Select x.x.__expands(x.opr, coors)).MatrixAsIterator
            Dim cuts = (From x As PredictedRegulationFootprint
                        In expands.AsParallel
                        Where Math.Abs(x.Pcc) >= cut OrElse
                            Math.Abs(x.sPcc) >= cut
                        Select x).ToList
            Return cuts + footprints
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="operon"></param>
        ''' <returns>原来的数据将不会被添加</returns>
        <Extension>
        Private Function __expands(x As PredictedRegulationFootprint, operon As Operon, corrs As Correlation2) As PredictedRegulationFootprint()
            Dim genes = (From g As GeneBrief In operon.Genes
                         Where Not String.Equals(g.Synonym, x.ORF)
                         Select g) ' 由于操纵的第一个基因的调控数据已经有了，所以在这里筛选掉
            Dim LQuery = (From g As GeneBrief In genes Select x.__copy(g, corrs)).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 这里主要是拓展Trace信息
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="g"></param>
        ''' <param name="corrs">操纵子的数据可能会有预测错误的，所以在这里任然需要转录组数据进行筛选</param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Private Function __copy(x As PredictedRegulationFootprint, g As GeneBrief, corrs As Correlation2) As PredictedRegulationFootprint
            Dim footprint As PredictedRegulationFootprint = x.Clone

            ' 由于操纵子的模式是连带调控的。所以调控位点的信息不会被修改，任然是第一个基因的信息

            footprint.ORF = g.Synonym
            footprint.InitX = "0"c
            footprint.MotifTrace = g.OperonID & "@" & footprint.MotifTrace
            footprint.Pcc = corrs.GetPcc(footprint.Regulator, g.Synonym)
            footprint.sPcc = corrs.GetSPcc(footprint.Regulator, g.Synonym)

            Return footprint
        End Function
    End Module
End Namespace