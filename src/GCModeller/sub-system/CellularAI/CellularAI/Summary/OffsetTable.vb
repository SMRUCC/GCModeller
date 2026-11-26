Namespace Summary

    ''' <summary>
    ''' 一条训练摘要的"逐基因中心化表"与"响应方差表"。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 上游把"每条记录的逐基因中心"（<c>effect_center</c>、<c>signed_score_center</c>）与
    ''' "该记录的噪声水平"（<c>response_variance</c>）单独存成一份 <c>.npz</c>，
    ''' 池化时先减中心再合并。中心值缺失（NaN）表示该基因没有估计值，此时<b>不做事</b>
    ''' （直接透传原值），而不是把原值清零。
    ''' </para>
    ''' <para>
    ''' <see cref="EffectCount"/> 是该基因上做过估计的扰动数量，用于诊断"这个中心是否可信"。
    ''' </para>
    ''' </remarks>
    Public Class OffsetTable

        ''' <summary>细胞系（训练行）名称。</summary>
        Public Property CellLine As String

        ''' <summary>视图名称（与摘要行的 view 一致）。</summary>
        Public Property View As String

        ''' <summary>本表的基因轴。</summary>
        Public Property Genes As String()

        ''' <summary>log2FC 的逐基因中心。</summary>
        Public Property EffectCenter As Double()

        ''' <summary>中心估计所用的扰动数量（逐基因）。</summary>
        Public Property EffectCount As Double()

        ''' <summary>带符号正态分数（Liptak 分数）的逐基因中心。</summary>
        Public Property SignedScoreCenter As Double()

        ''' <summary>响应方差（逐基因），用作逆方差权重。</summary>
        Public Property ResponseVariance As Double()

        ''' <summary>基因数。</summary>
        Public ReadOnly Property Count As Integer
            Get
                Return If(Genes Is Nothing, 0, Genes.Length)
            End Get
        End Property

        ''' <summary>
        ''' 把本表映射到给定基因轴：返回 <c>G</c> 长的索引（-1 表示该基因不在本表里）。
        ''' </summary>
        Public Function IndexOf(genes As String()) As Integer()
            Dim positions As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For i As Integer = 0 To Count - 1
                positions(Genes(i)) = i
            Next

            Dim index(If(genes Is Nothing, -1, genes.Length - 1)) As Integer

            For i As Integer = 0 To index.Length - 1
                Dim at As Integer = -1

                Call positions.TryGetValue(genes(i), at)
                index(i) = at
            Next

            Return index
        End Function

        ''' <summary>校验各数组长度一致且与基因轴等长。</summary>
        Public Sub Validate()
            If Genes Is Nothing Then
                Throw New ArgumentException("the offset table has no gene axis", NameOf(Genes))
            End If
            If EffectCenter Is Nothing OrElse EffectCenter.Length <> Genes.Length Then
                Throw New ArgumentException("effect_center does not match the gene axis", NameOf(EffectCenter))
            End If
            If SignedScoreCenter Is Nothing OrElse SignedScoreCenter.Length <> Genes.Length Then
                Throw New ArgumentException("signed_score_center does not match the gene axis", NameOf(SignedScoreCenter))
            End If
            If ResponseVariance Is Nothing OrElse ResponseVariance.Length <> Genes.Length Then
                Throw New ArgumentException("response_variance does not match the gene axis", NameOf(ResponseVariance))
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return $"{View}/{CellLine}: {Count} gene(s)"
        End Function

    End Class
End Namespace
