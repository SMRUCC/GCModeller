Imports Microsoft.VisualBasic.Data.csv.IO
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

''' <summary>
''' 直接进行FoldChange比较的误差会非常大,在这里可以将原始数据进行处理,使用iTraq方法进行数据分析
''' </summary>
Public Module FoldChangeMatrix

    Public Iterator Function iTraqMatrix(rawMatrix As IEnumerable(Of DataSet), sampleInfo As SampleGroup(), analysis As AnalysisDesigner()) As IEnumerable(Of DataSet)
        Dim sampleGroups = sampleInfo.GroupBy(Function(s) s.sample_group).ToArray
    End Function
End Module
