Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language

Public Class Variable

    ''' <summary>
    ''' 对反应容器之中的某一种物质的引用
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Mass As Factor
    ''' <summary>
    ''' 在反应过程之中的变异系数，每完成一个单位的反应过程，当前的<see cref="Mass"/>将会丢失或者增加这个系数相对应的数量的含量
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Coefficient As Double

    Sub New(mass As Factor, factor As Double)
        Me.Mass = mass
        Me.Coefficient = factor
    End Sub

    Public Overrides Function ToString() As String
        Return Mass.ToString
    End Function

End Class

''' <summary>
''' 一个变量因子，这个对象主要是用于存储值
''' </summary>
Public Class Factor : Inherits Value(Of Double)
    Implements INamedValue

    Public Property ID As String Implements IKeyedEntity(Of String).Key

    Public Overrides Function ToString() As String
        Return $"{ID} ({Value} unit)"
    End Function
End Class
