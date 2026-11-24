Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Alignment

Namespace Bayesian.Model

    ''' <summary>
    ''' 论文第 3 节的 orphan 推广模型。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' cognate 模型假设「每个蛋白在同一个基因组之内恰有一个互作伙伴」。
    ''' orphan 推广模型放宽为「至多一个伙伴」，因此每个基因组之内可以分配的互作对数目为
    ''' <c>min(#orphan kinases, #orphan regulators)</c>，剩下的蛋白被标记为「未分配」。
    ''' </para>
    ''' <para>
    ''' 记 <c>J</c> 为互作对的联合比对、<c>J_k</c> 与 <c>J_r</c> 分别为其中的激酶与响应调节蛋白部分，
    ''' <c>K</c> 为未分配激酶的比对、<c>R</c> 为未分配响应调节蛋白的比对，则论文给出分解假设
    ''' </para>
    ''' <code>
    ''' P(D | a) = P(J) · P(K | J_k) · P(R | J_r)
    ''' </code>
    ''' <para>
    ''' 其中 <c>P(K | J_k) = P(K, J_k) / P(J_k)</c>，都可以用同一套 Dirichlet-多项似然在
    ''' 相应的列子集上计算出来。本类就是这一分解的直接实现：把一个大的联合比对切分成
    ''' 「cognate 部分」与「orphan 部分」两组列下标，然后分别调用 <see cref="PpiBayesianModel.LogEvidenceSubset"/>。
    ''' </para>
    ''' <para>
    ''' 为了在采样之中保持数值稳定，三棵树都使用最大生成树近似(论文在并行多类时也采用了同样的处理)。
    ''' </para>
    ''' </remarks>
    Public Class OrphanModel

        Private ReadOnly _model As PpiBayesianModel
        Private ReadOnly _cognateColumns As Integer()
        Private ReadOnly _kinaseColumns As Integer()
        Private ReadOnly _cognateKinaseColumns As Integer()
        Private ReadOnly _receiverColumns As Integer()
        Private ReadOnly _cognateReceiverColumns As Integer()

        ''' <summary>
        ''' 构造 orphan 模型
        ''' </summary>
        ''' <param name="model">覆盖全部列的贝叶斯模型</param>
        ''' <param name="cognateColumns">cognate 互作对所对应的列(联合比对的 J 部分)</param>
        ''' <param name="kinaseColumns">互作对与未分配激酶合起来所覆盖的全部列(J_k ∪ K)</param>
        ''' <param name="cognateKinaseColumns">cognate 互作对之中的激酶列(J_k)</param>
        ''' <param name="receiverColumns">互作对与未分配响应调节蛋白合起来所覆盖的全部列(J_r ∪ R)</param>
        ''' <param name="cognateReceiverColumns">cognate 互作对之中的响应调节蛋白列(J_r)</param>
        Public Sub New(model As PpiBayesianModel,
                       cognateColumns As Integer(),
                       kinaseColumns As Integer(),
                       cognateKinaseColumns As Integer(),
                       receiverColumns As Integer(),
                       cognateReceiverColumns As Integer())

            If model Is Nothing Then
                Throw New ArgumentNullException(NameOf(model))
            End If

            _model = model
            _cognateColumns = Sorted(cognateColumns)
            _kinaseColumns = Sorted(kinaseColumns)
            _cognateKinaseColumns = Sorted(cognateKinaseColumns)
            _receiverColumns = Sorted(receiverColumns)
            _cognateReceiverColumns = Sorted(cognateReceiverColumns)
        End Sub

        ''' <summary>
        ''' 底层的贝叶斯模型
        ''' </summary>
        Public ReadOnly Property Model As PpiBayesianModel
            Get
                Return _model
            End Get
        End Property

        ''' <summary>
        ''' <c>ln P(J)</c>：cognate 互作对部分的边缘似然
        ''' </summary>
        Public Function LogJointEvidence() As Double
            Return _model.LogEvidenceSubset(_cognateColumns)
        End Function

        ''' <summary>
        ''' <c>ln P(K | J_k) = ln P(K, J_k) - ln P(J_k)</c>
        ''' </summary>
        Public Function LogOrphanKinaseGivenCognate() As Double
            If _cognateKinaseColumns.Length = 0 Then
                Return 0.0
            End If

            Return _model.LogEvidenceSubset(_kinaseColumns) - _model.LogEvidenceSubset(_cognateKinaseColumns)
        End Function

        ''' <summary>
        ''' <c>ln P(R | J_r) = ln P(R, J_r) - ln P(J_r)</c>
        ''' </summary>
        Public Function LogOrphanReceiverGivenCognate() As Double
            If _cognateReceiverColumns.Length = 0 Then
                Return 0.0
            End If

            Return _model.LogEvidenceSubset(_receiverColumns) - _model.LogEvidenceSubset(_cognateReceiverColumns)
        End Function

        ''' <summary>
        ''' 分解之后的整体对数似然 <c>ln [ P(J) P(K|J_k) P(R|J_r) ]</c>
        ''' </summary>
        Public Function LogEvidence() As Double
            Return LogJointEvidence() + LogOrphanKinaseGivenCognate() + LogOrphanReceiverGivenCognate()
        End Function

        ''' <summary>
        ''' 执行一致性检查：列集合必须是顶层列集合的子集
        ''' </summary>
        Public Function Validate() As Boolean
            Dim total As Integer = _model.Counts.Length
            Dim ok As Boolean = True

            ok = DebugAudit.Test(_cognateColumns.All(Function(c) c >= 0 AndAlso c < total),
                                 "the cognate column set contains an out-of-range index") AndAlso ok
            ok = DebugAudit.Test(_cognateKinaseColumns.All(Function(c) _kinaseColumns.Contains(c)),
                                 "the cognate kinase columns must be a subset of the kinase column set") AndAlso ok
            ok = DebugAudit.Test(_cognateReceiverColumns.All(Function(c) _receiverColumns.Contains(c)),
                                 "the cognate receiver columns must be a subset of the receiver column set") AndAlso ok

            Return ok
        End Function

        Private Shared Function Sorted(columns As Integer()) As Integer()
            If columns Is Nothing Then
                Return New Integer() {}
            End If

            Return columns.Distinct().OrderBy(Function(c) c).ToArray()
        End Function
    End Class
End Namespace
