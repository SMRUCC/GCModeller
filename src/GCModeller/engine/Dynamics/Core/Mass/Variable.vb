Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language

Namespace Core

    ''' <summary>
    ''' 针对描述某一个生物学功能的参数变量
    ''' </summary>
    Public Class Variable

        ''' <summary>
        ''' 对反应容器之中的某一种物质的引用
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Mass As Factor
        ''' <summary>
        ''' 在反应过程之中的变异系数，每完成一个单位的反应过程，当前的<see cref="Mass"/>
        ''' 将会丢失或者增加这个系数相对应的数量的含量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Coefficient As Double

        ''' <summary>
        ''' 当前的这种物质因子在目标反应通道之中是否为模板物质？对于模板物质而言，其容量是不会被消耗掉的
        ''' 例如，转录过程或者翻译过程，基因对象或者mRNA对象为模板物质，其不会像小分子反应一样作为底物被消耗掉
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsTemplate As Boolean

        Sub New(mass As Factor, factor As Double, Optional isTemplate As Boolean = False)
            Me.Mass = mass
            Me.Coefficient = factor
            Me.IsTemplate = isTemplate
        End Sub

        Public Overrides Function ToString() As String
            If Not IsTemplate Then
                Return Mass.ToString
            Else
                Return $"[{Mass}]"
            End If
        End Function

    End Class
End Namespace