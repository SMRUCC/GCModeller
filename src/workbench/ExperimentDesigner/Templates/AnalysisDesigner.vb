Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.csv

''' <summary>
''' ```
''' <see cref="Controls"/> / <see cref="Experimental"/>
''' ```
''' 这个对象描述了如何设计一个比对计算实验分析
''' </summary>
<Template(ExperimentDesigner)>
Public Class AnalysisDesigner

    <XmlAttribute("control")>
    Public Property Controls As String
    <XmlAttribute("experimental")>
    Public Property Experimental As String
    <XmlAttribute("reversed?")>
    Public Property Reversed As Boolean

    ''' <summary>
    ''' 对于iTraq实验数据而言，这里是具体的样品的编号的比对
    ''' 对于LabelFree实验数据而言，由于需要手工计算FC值，所以在这里比对的是样品的组别名称
    ''' </summary>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        If Reversed Then
            Return $"{Controls}/{Experimental}"
        Else
            Return $"{Experimental}/{Controls}"
        End If
    End Function

    Public Function Swap() As AnalysisDesigner
        Return New AnalysisDesigner With {
            .Controls = Experimental,
            .Experimental = Controls,
            .Reversed = Reversed
        }
    End Function
End Class