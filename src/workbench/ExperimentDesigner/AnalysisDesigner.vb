Imports Microsoft.VisualBasic.Data.csv

''' <summary>
''' 这个对象描述了如何设计一个比对计算实验分析
''' </summary>
<Template("ExperimentDesigner")> Public Class AnalysisDesigner

    Public Property Controls As String
    Public Property Experimental As String

    Public Overrides Function ToString() As String
        Return $"{Controls}/{Experimental}"
    End Function

    Public Function Swap() As AnalysisDesigner
        Return New AnalysisDesigner With {
            .Controls = Experimental,
            .Experimental = Controls
        }
    End Function
End Class