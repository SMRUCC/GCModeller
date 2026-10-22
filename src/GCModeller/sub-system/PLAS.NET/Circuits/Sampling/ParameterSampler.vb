Imports Microsoft.VisualBasic.Math.Distributions.LHS

Namespace Sampling

    ''' <summary>
    ''' The parameter sampler of the topology search: a Latin hypercube design
    ''' inside of the logarithm space, following the sampling strategy of the
    ''' paper.
    ''' (拓扑搜索的参数采样器：对数空间内的拉丁超立方设计)
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Every sampled parameter is a catalytic constant pair ``(k_cat, K_M)``.
    ''' The paper samples uniformly inside of the logarithm space so that every
    ''' order of magnitude owns the same sampling density:
    ''' </para>
    ''' <list type="bullet">
    ''' <item> ``k_cat`` inside of ``[0.1, 10]``, i.e. log10 inside of ``[-1, 1]``; </item>
    ''' <item> ``K_M`` inside of ``[0.001, 100]``, i.e. log10 inside of ``[-3, 2]``. </item>
    ''' </list>
    ''' <para>
    ''' The dimension of the design is the parameter count of the topology, see
    ''' <see cref="Model.CircuitModel.ParameterCount"/>: one pair for every
    ''' active network link, one pair for the input link and one pair for every
    ''' background enzyme. The background enzyme concentrations are not sampled,
    ''' they are fixed at 0.5 by the paper.
    ''' </para>
    ''' <para>
    ''' Every topology owns a deterministic sampler seed derived from the base
    ''' seed and the topology identifier, so that a search run is fully
    ''' reproducible and a checkpointed resume does not change any result.
    ''' </para>
    ''' </remarks>
    Public Module ParameterSampler

        ''' <summary>The lower bound of the sampled catalytic constants k_cat.</summary>
        Public Const KCatLower As Double = 0.1
        ''' <summary>The upper bound of the sampled catalytic constants k_cat.</summary>
        Public Const KCatUpper As Double = 10.0
        ''' <summary>The lower bound of the sampled Michaelis constants K_M.</summary>
        Public Const KmLower As Double = 0.001
        ''' <summary>The upper bound of the sampled Michaelis constants K_M.</summary>
        Public Const KmUpper As Double = 100.0

        ''' <summary>
        ''' Sample a batch of parameter vectors for the given topology.
        ''' (为一个拓扑采样一批参数向量)
        ''' </summary>
        ''' <param name="topology">the network topology</param>
        ''' <param name="nSamples">the number of parameter sets, the paper uses 10,000</param>
        ''' <param name="seed">the sampler seed, use <see cref="SeedOf"/> for the deterministic per topology seed</param>
        ''' <returns>a ``nSamples x nParams`` matrix, the row is one parameter set</returns>
        Public Function Sample(topology As Model.NetworkTopology, nSamples As Integer, seed As Integer) As Double()()
            If topology Is Nothing Then
                Throw New ArgumentNullException(NameOf(topology))
            End If
            If nSamples < 1 Then
                Throw New ArgumentOutOfRangeException(NameOf(nSamples), nSamples, "the number of samples must be a positive integer.")
            End If

            Dim nParams As Integer = Model.CircuitModel.ParameterCount(topology)
            Dim sampler As New LatinHypercubeSampler(nDims:=nParams, seed:=seed)
            Dim unit As Double()() = sampler.Random(nSamples)
            Dim bounds = Log10Bounds(nParams)

            Return LhsScale.ScaleLog10(unit, bounds.lower, bounds.upper)
        End Function

        ''' <summary>
        ''' The deterministic sampler seed of a topology, derived from the base
        ''' seed of the search run and the topology identifier with a Knuth
        ''' multiplicative hash.
        ''' (由基准种子与拓扑编号确定性地派生每个拓扑的采样种子)
        ''' </summary>
        ''' <param name="topologyId">the topology identifier, 0 .. 19682</param>
        ''' <param name="baseSeed">the base seed of the search run</param>
        Public Function SeedOf(topologyId As Integer, baseSeed As Integer) As Integer
            Dim h As Long = (CLng(topologyId) + 1L) * 2654435761L + CLng(baseSeed) * 40503L

            ' keep the hash inside of the positive 31 bit range of the sampler seed
            h = h And &H7FFFFFFFL

            Return CInt(h)
        End Function

        ''' <summary>
        ''' The log10 bounds of the parameter dimensions: the catalytic constant
        ''' bounds alternate with the Michaelis constant bounds, which matches
        ''' the (k, K) pair layout of the parameter vector.
        ''' (参数维度的 log10 上下界，与 (k, K) 交替布局一一对应)
        ''' </summary>
        ''' <param name="nParams">the number of parameter dimensions</param>
        Public Function Log10Bounds(nParams As Integer) As (lower As Double(), upper As Double())
            If nParams < 2 OrElse nParams Mod 2 <> 0 Then
                Throw New ArgumentException($"the parameter count must be a positive even number, but got {nParams}!", NameOf(nParams))
            End If

            Dim lower As Double() = LhsScale.TileBounds(New Double() {-1.0, -3.0}, nParams)
            Dim upper As Double() = LhsScale.TileBounds(New Double() {1.0, 2.0}, nParams)

            Return (lower, upper)
        End Function

        ''' <summary>
        ''' The real value bounds of the parameter dimensions, in the same
        ''' alternating (k, K) layout.
        ''' (参数维度的实数上下界)
        ''' </summary>
        ''' <param name="nParams">the number of parameter dimensions</param>
        Public Function RealBounds(nParams As Integer) As (lower As Double(), upper As Double())
            If nParams < 2 OrElse nParams Mod 2 <> 0 Then
                Throw New ArgumentException($"the parameter count must be a positive even number, but got {nParams}!", NameOf(nParams))
            End If

            Dim lower As Double() = LhsScale.TileBounds(New Double() {KCatLower, KmLower}, nParams)
            Dim upper As Double() = LhsScale.TileBounds(New Double() {KCatUpper, KmUpper}, nParams)

            Return (lower, upper)
        End Function

    End Module

End Namespace
