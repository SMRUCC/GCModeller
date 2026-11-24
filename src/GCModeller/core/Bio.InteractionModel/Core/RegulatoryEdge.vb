Imports System.Runtime.CompilerServices

Namespace Core

    ''' <summary>
    ''' 转录调控关系
    ''' </summary>
    ''' <remarks>
    ''' 方向性(激活/抑制)在这里被刻意丢弃：本轮只把先验用于「哪些 TF 调控这个基因」
    ''' 以及「这条边的惩罚要不要放松」，不使用先验的符号去约束回归系数。
    ''' </remarks>
    Public Class RegulatoryEdge

        ''' <summary>transcript factor protein/rna id.</summary>
        ''' <remarks>
        ''' 转录因子名称
        ''' </remarks>
        Public Property TF As String = ""

        ''' <summary>靶基因名称</summary>
        Public Property TargetGene As String = ""

        ''' <summary>调控类型（激活/抑制）</summary>
        Public Property RegulationType As Effector = Effector.Activator

        ''' <summary>置信度分数（0-1）</summary>
        ''' <remarks>
        ''' 先验的置信度，取值 0~1(High = 0.95，Medium = 0.7，Low = 0.3)
        ''' </remarks>
        Public Property Confidence As Double = 1.0

        ''' <summary>证据来源</summary>
        Public Property Evidence As String = ""

        ''' <summary>
        ''' 先验的因果符号：<c>+1</c> 激活、<c>-1</c> 抑制、<c>0</c> 未知。
        ''' 
        ''' 这个符号会被用作回归系数的<b>可行域约束</b>(signed lasso)：
        ''' 激活边只允许 β ≥ 0、抑制边只允许 β ≤ 0。
        ''' 
        ''' 之所以必须这样做，是因为共表达数据本身无法区分激活与抑制
        ''' (抽样验证过：Inhibitor 边的共表达相关系数中位数同样是正的)，
        ''' 而任何偶次惩罚(拉普拉斯、L1、L2)对 β → -β 都是对称的，
        ''' 只有把可行域限制到半空间才能固定绝对符号。
        ''' </summary>
        Public ReadOnly Property Sign As Integer
            Get
                If RegulationType = Effector.Activator Then
                    Return 1
                ElseIf RegulationType = Effector.Inhibitor Then
                    Return -1
                Else
                    Return 0
                End If
            End Get
        End Property

        Sub New()
        End Sub

        Sub New(copy As RegulatoryEdge)
            TF = copy.TF
            TargetGene = copy.TargetGene
            RegulationType = copy.RegulationType
            Confidence = copy.Confidence
            Evidence = copy.Evidence
        End Sub

        ''' <summary>
        ''' 构造一条先验调控关系
        ''' </summary>
        ''' <param name="tf">调控者(转录因子)的基因编号</param>
        ''' <param name="target">被调控的目标基因编号</param>
        ''' <param name="confidence">先验的置信度，取值 0~1</param>
        ''' <param name="sign">因果符号：+1 激活、-1 抑制、0 未知</param>
        Public Sub New(tf As String, target As String,
                       Optional confidence As Double = 1.0R,
                       Optional sign As Integer = 0)

            Me.TF = tf
            Me.TargetGene = target
            Me.Confidence = confidence
            Me.RegulationType = If(sign > 0, Effector.Activator, If(sign < 0, Effector.Inhibitor, Effector.Unknown))
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0} → {1} ({2}, conf={3:F2})", TF, TargetGene, RegulationType.Description, Confidence)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ParseEffector(str As String) As Effector
            Return Extensions.ParseRegulationType(str)
        End Function

        Public Shared Function ParseConfidenceEnum(type As String) As Double
            If type.StringEmpty(, True) Then
                ' unknown confidence label
                Return 0
            End If

            Select Case type
                Case "High" : Return 0.95
                Case "Medium" : Return 0.7
                Case "Low" : Return 0.5
                Case Else
                    Return 0.1
            End Select
        End Function

    End Class

End Namespace