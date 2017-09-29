Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

''' <summary>
''' avg(A)/avg(B) = FC
''' </summary>
Public Module LabelFreeTtest

    ''' <summary>
    ''' 一次只计算出一组实验设计的结果
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="analysis"></param>
    ''' <param name="level#"></param>
    ''' <param name="pvalue#"></param>
    ''' <param name="fdrThreshold#"></param>
    ''' <returns></returns>
    <Extension>
    Public Function logFCtest(data As IEnumerable(Of DataSet),
                              analysis As AnalysisDesigner,
                              sampleInfo As SampleInfo(),
                              Optional level# = 1.5,
                              Optional pvalue# = 0.05,
                              Optional fdrThreshold# = 0.05) As DEP_iTraq()

        Dim calc = data.Select(Function(protein)

                               End Function)
    End Function
End Module
