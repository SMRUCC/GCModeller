Namespace Core

    ''' <summary>
    ''' 用于估算细胞内的某些状态的只读的状态视图，例如使用总蛋白质量来估算细胞的生长状态，（ATP，NADH/NADPH）的总量来估算细胞内的能量与代谢活跃度
    ''' </summary>
    Public Class StatusMapFactor : Inherits Factor

        Public Overrides ReadOnly Property Value As Double
            Get
                Return eval()
            End Get
        End Property

        ''' <summary>
        ''' 定义这个视图的质量组成
        ''' </summary>
        ''' <returns></returns>
        Public Property mass As String()

        Dim env As MassTable

        Sub New(id$, mass As IEnumerable(Of String), compart_id As String, env As MassTable)
            Call MyBase.New(id, MassRoles.status, compart_id)

            Me.mass = mass.ToArray
            Me.env = env
            Me.template_id = id
            Me.name = id
            Me.ID = id & "@" & compart_id
        End Sub

        Sub New(id$, mass As String, compart_id As String, env As MassTable)
            Call Me.New(id, {mass}, compart_id, env)
        End Sub

        Public Function eval() As Double
            Return Aggregate id As String
                   In mass
                   Let val As Double = env(id).Value
                   Into Sum(val)
        End Function

    End Class
End Namespace