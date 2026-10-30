Namespace Core

    ''' <summary>
    ''' 转录调控关系
    ''' </summary>
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
        Public Property Confidence As Double = 1.0

        ''' <summary>证据来源</summary>
        Public Property Evidence As String = ""

        Sub New()
        End Sub

        Sub New(copy As RegulatoryEdge)
            TF = copy.TF
            TargetGene = copy.TargetGene
            RegulationType = copy.RegulationType
            Confidence = copy.Confidence
            Evidence = copy.Evidence
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0} → {1} ({2}, conf={3:F2})", TF, TargetGene, RegulationType.Description, Confidence)
        End Function

        Public Shared Function ParseEffector(type As String) As Effector
            If type.StringEmpty(, True) Then
                Return Effector.Activator
            End If

            Select Case type
                Case NameOf(Effector.Activator), "activation", "+", "↑" : Return Effector.Activator
                Case NameOf(Effector.Inhibitor), "repression", "inhibit", "suppress", "-", "↓" : Return Effector.Inhibitor
                Case "1" : Return Effector.Activator
                Case "2" : Return Effector.Inhibitor
                Case Else
                    Return Effector.Unknown
            End Select
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