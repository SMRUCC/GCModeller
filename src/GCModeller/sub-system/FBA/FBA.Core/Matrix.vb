Imports System.Runtime.CompilerServices

''' <summary>
''' 未知变量xyz为代谢过程，而在约束的目标函数之中则可以通过对代谢过程的约束计算出系数
''' </summary>
Public Class Matrix

    ''' <summary>
    ''' 线性规划之中的行名称，约束值都是零
    ''' </summary>
    ''' <returns></returns>
    Public Property Compounds As String()
    ''' <summary>
    ''' 线性规划之中的列名称，即未知变量列表
    ''' </summary>
    ''' <returns></returns>
    Public Property Flux As String()
    ''' <summary>
    ''' 目标函数之中的计算目标，为Reaction <see cref="Flux"/>编号之中的一部分
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

    ''' <summary>
    ''' 这个矩阵之中有多少列？即有多少个代谢物
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property NumOfCompounds As Integer
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Compounds.Length
        End Get
    End Property

    ''' <summary>
    ''' 生成目标函数对未知变量，即flux的系数向量
    ''' </summary>
    ''' <returns></returns>
    Public Function GetTargetCoefficients() As Double()
        With Targets.Indexing
            Return Flux _
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
