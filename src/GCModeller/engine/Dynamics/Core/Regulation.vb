
''' <summary>
''' 对反应过程的某一个方向的控制效应
''' </summary>
Public Class Regulation

    Public Property Activation As Variable() = {}
    ''' <summary>
    ''' 如果抑制的总量大于激活的总量，那么这个调控的反应过程将不会进行
    ''' </summary>
    ''' <returns></returns>
    Public Property Inhibition As Variable() = {}
    ''' <summary>
    ''' 没有任何调控的时候的基准反应单位，因为有些过程是不需要调控以及催化的
    ''' </summary>
    ''' <returns></returns>
    Public Property baseline As Double = 0.5

    ''' <summary>
    ''' 计算出当前的调控效应单位
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Coefficient As Double
        Get
            If Activation Is Nothing AndAlso Inhibition Is Nothing Then
                Return baseline
            End If

            Dim i = Inhibition.Sum(Function(v) v.Coefficient * v.Mass.Value)
            Dim a = Activation.Sum(Function(v) v.Coefficient * v.Mass.Value)

            If i >= a Then
                ' 抑制的总量已经大于等于激活的总量的，则反应过程不会发生
                Return 0
            Else
                Return a - i
            End If
        End Get
    End Property

    Public Shared Operator >(a As Regulation, b As Regulation) As Boolean
        If a Is Nothing Then
            Return False
        ElseIf b Is Nothing Then
            Return True
        Else
            Return a.Coefficient > b.Coefficient
        End If
    End Operator

    Public Shared Operator <(a As Regulation, b As Regulation) As Boolean
        Return Not a.Coefficient > b.Coefficient
    End Operator

    Public Shared Operator =(a As Regulation, b As Regulation) As Boolean
        If a Is Nothing AndAlso b Is Nothing Then
            Return True
        ElseIf a Is Nothing OrElse b Is Nothing Then
            Return False
        Else
            Return a.Coefficient = b.Coefficient
        End If
    End Operator

    Public Shared Operator <>(a As Regulation, b As Regulation) As Boolean
        Return Not a.Coefficient = b.Coefficient
    End Operator

End Class
