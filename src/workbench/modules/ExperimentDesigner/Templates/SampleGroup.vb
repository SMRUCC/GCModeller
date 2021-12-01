Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language

''' <summary>
''' 样品的分组信息
''' </summary>
<Template(ExperimentDesigner)> Public Class SampleGroup
    Implements INamedValue
    Implements Value(Of String).IValueOf

    ''' <summary>
    ''' 在报告之中的显示名称，可能会含有一些奇怪的符号
    ''' </summary>
    ''' <returns></returns>
    Public Property sample_name As String Implements IKeyedEntity(Of String).Key

    ''' <summary>
    ''' the sample info.
    ''' 
    ''' (样品的实验设计分组信息)
    ''' </summary>
    ''' <returns></returns>
    Public Property sample_info As String Implements Value(Of String).IValueOf.Value

    Public Overrides Function ToString() As String
        Return $"[{sample_info}] {sample_name}"
    End Function
End Class
