Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Public Module PolarScalePlot

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <param name="sampleinfo">
    ''' 每一个实验分组就是一个极坐标
    ''' </param>
    ''' <param name="size$"></param>
    ''' <param name="padding$"></param>
    ''' <param name="bg$"></param>
    ''' <returns></returns>
    Public Function Plot(matrix As Matrix, sampleinfo As SampleInfo(),
                         Optional size$ = "3000,2700",
                         Optional padding$ = g.DefaultPadding,
                         Optional bg$ = "white") As GraphicsData

        Dim polarAxis = sampleinfo.GroupBy(Function(sample) sample.sample_info).ToArray

    End Function

End Module
