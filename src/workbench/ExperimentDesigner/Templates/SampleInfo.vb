Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language

''' <summary>
''' 一般而言，对于实验数据的分析而言，在进行数据存储的时候使用的是<see cref="ID"/>属性，而在进行数据可视化或者数据报告输出的时候，则是使用的<see cref="sample_name"/>属性作为显示的label
''' </summary>
<Template(ExperimentDesigner)>
Public Class SampleInfo : Inherits SampleGroup

    ''' <summary>
    ''' 符合VisualBasic标识符语法的目标样品标识符
    ''' </summary>
    ''' <returns></returns>
    Public Property ID As String

    ''' <summary>
    ''' index编号
    ''' </summary>
    ''' <returns></returns>
    Public Property Order As Integer
    ''' <summary>
    ''' 绘图可视化的时候的颜色
    ''' </summary>
    ''' <returns></returns>
    Public Property color As String
    ''' <summary>
    ''' legend的形状
    ''' </summary>
    ''' <returns></returns>
    Public Property shapetype As String

End Class

''' <summary>
''' 样品的分组信息
''' </summary>
<Template(ExperimentDesigner)> Public Class SampleGroup
    Implements INamedValue
    Implements Value(Of String).IValueOf

    ''' <summary>
    ''' 显示名称，可能会含有一些奇怪的符号
    ''' </summary>
    ''' <returns></returns>
    Public Property sample_name As String Implements IKeyedEntity(Of String).Key
    ''' <summary>
    ''' 样品的实验设计分组信息
    ''' </summary>
    ''' <returns></returns>
    Public Property sample_group As String Implements Value(Of String).IValueOf.Value

    Public Overrides Function ToString() As String
        Return $"[{sample_group}] {sample_name}"
    End Function
End Class