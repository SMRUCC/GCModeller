Imports System.Runtime.CompilerServices

Public Class Matrix

    ''' <summary>
    ''' 线性规划之中的变量名称，即``Xi``
    ''' </summary>
    ''' <returns></returns>
    Public Property Compounds As String()
    ''' <summary>
    ''' 目标函数之中的计算目标，为Compounds之中的一部分
    ''' </summary>
    ''' <returns></returns>
    Public Property Targets As String()

    ''' <summary>
    ''' 矩阵的结构为：
    ''' 
    ''' + 行应该为Compound
    ''' + 列应该为代谢过程
    ''' </summary>
    ''' <returns></returns>
    Public Property Matrix As Double()()

    Public ReadOnly Property NumOfFlux As Integer
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Matrix.Length
        End Get
    End Property

    Public Function GetTargetCoefficients() As Double()
        With Targets.Indexing
            Return Compounds _
                .Select(Function(name)
                            If .IndexOf(name) > -1 Then
                                Return 1.0
                            Else
                                Return 0.0
                            End If
                        End Function) _
                .ToArray
        End With
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetMatrix() As Double(,)
        Return Matrix.ToMatrix
    End Function

End Class
