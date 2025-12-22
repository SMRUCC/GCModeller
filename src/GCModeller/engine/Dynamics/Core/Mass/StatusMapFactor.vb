Imports System.Runtime.CompilerServices

Namespace Core

    ''' <summary>
    ''' 用于估算细胞内的某些状态的只读的状态视图，例如使用总蛋白质量来估算细胞的生长状态，（ATP，NADH/NADPH）的总量来估算细胞内的能量与代谢活跃度
    ''' </summary>
    Public Class StatusMapFactor : Inherits Factor

        Public Overrides ReadOnly Property Value As Double
            Get
                Return eval() * coefficient
            End Get
        End Property

        ''' <summary>
        ''' 定义这个视图的质量组成
        ''' </summary>
        ''' <returns></returns>
        Public Property mass As String()
        Public Property coefficient As Double = 1

        Dim massCache As Factor()

        Sub New(id$, mass As IEnumerable(Of String), compart_id As String, env As MassTable)
            Call MyBase.New(id, MassRoles.status, compart_id)

            Me.mass = mass.ToArray
            Me.template_id = id
            Me.name = id
            Me.ID = id & "@" & compart_id
            Me.massCache = (From m As String
                            In Me.mass
                            Let factor As Factor = env(m)
                            Where factor IsNot Nothing
                            Select factor).ToArray
        End Sub

        Sub New(id$, mass As String, compart_id As String, env As MassTable)
            Call Me.New(id, {mass}, compart_id, env)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function eval() As Double
            Return Aggregate mass As Factor
                   In massCache
                   Let val As Double = mass.Value
                   Into Sum(val)
        End Function

    End Class
End Namespace