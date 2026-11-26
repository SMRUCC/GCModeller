Namespace Model

    ''' <summary>
    ''' CellularAI 模型的全部超参数：七个参数对象的聚合根。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 默认值逐项对应参考配置 <c>configs/example.yaml</c> 的 <c>model</c> 段。
    ''' 这些值定义的是"已发布的方法"，正常使用不应修改；测试与调参时用 <see cref="Clone"/> 派生。
    ''' </para>
    ''' <para>
    ''' 本类型只是数据容器，不含算法；算法入口接收它并保持只读语义。
    ''' </para>
    ''' </remarks>
    Public Class CellularAiModel

        ''' <summary>池化参数。</summary>
        Public Property Pooling As PoolingOptions = New PoolingOptions

        ''' <summary>调用参数。</summary>
        Public Property Calls As CallOptions = New CallOptions

        ''' <summary>计划参数。</summary>
        Public Property Plan As PlanOptions = New PlanOptions

        ''' <summary>总量设计参数。</summary>
        Public Property Totals As TotalsOptions = New TotalsOptions

        ''' <summary>放置参数。</summary>
        Public Property Placement As PlacementOptions = New PlacementOptions

        ''' <summary>合成参数。</summary>
        Public Property Synthesis As SynthesisOptions = New SynthesisOptions

        ''' <summary>护栏阈值。</summary>
        Public Property Guards As GuardOptions = New GuardOptions

        ''' <summary>默认模型（每次返回新实例）。</summary>
        Public Shared Function [Default]() As CellularAiModel
            Return New CellularAiModel
        End Function

        ''' <summary>深拷贝。</summary>
        Public Function Clone() As CellularAiModel
            Return New CellularAiModel With {
                .Pooling = New PoolingOptions With {
                    .L2Row = Pooling.L2Row,
                    .L2Col = Pooling.L2Col,
                    .L2Mat = Pooling.L2Mat,
                    .ZRow = Pooling.ZRow,
                    .ZCol = Pooling.ZCol,
                    .ZMat = Pooling.ZMat,
                    .Noise = Pooling.Noise,
                    .Se = Pooling.Se,
                    .SeClip = Pooling.SeClip,
                    .KdRef = Pooling.KdRef,
                    .KdFloor = Pooling.KdFloor,
                    .Sim = Pooling.Sim,
                    .SimHvg = Pooling.SimHvg,
                    .Correction = Pooling.Correction,
                    .Fill = Pooling.Fill,
                    .OneStream = Pooling.OneStream,
                    .Views = DirectCast(Pooling.Views.Clone(), String()),
                    .AuxiliaryWeight = Pooling.AuxiliaryWeight,
                    .PairWeight = Pooling.PairWeight
                },
                .Calls = New CallOptions With {
                    .EligibleCpm = Calls.EligibleCpm,
                    .MinLfc = Calls.MinLfc,
                    .Gamma = Calls.Gamma,
                    .NMin = Calls.NMin,
                    .NMax = Calls.NMax,
                    .WPow = Calls.WPow,
                    .Phi = Calls.Phi,
                    .StrengthZ = Calls.StrengthZ,
                    .StrengthK = Calls.StrengthK,
                    .StrengthC = Calls.StrengthC,
                    .StrengthPhi = Calls.StrengthPhi
                },
                .Plan = New PlanOptions With {
                    .CurveSlope = Plan.CurveSlope,
                    .SimTau = Plan.SimTau,
                    .SimNtop = Plan.SimNtop,
                    .SimCap = Plan.SimCap,
                    .PlanZUniform = Plan.PlanZUniform,
                    .XsimTau = Plan.XsimTau,
                    .XsimStrength = Plan.XsimStrength,
                    .XsimCapFactor = Plan.XsimCapFactor,
                    .XsimMaxTotal = Plan.XsimMaxTotal,
                    .PowerK = Plan.PowerK,
                    .PowerC = Plan.PowerC,
                    .PowerPhi = Plan.PowerPhi,
                    .PowerDepth = Plan.PowerDepth,
                    .PowerZN2 = Plan.PowerZN2,
                    .PowerTPen = Plan.PowerTPen,
                    .PowerSoftZ = Plan.PowerSoftZ,
                    .ShrinkQuantile = Plan.ShrinkQuantile,
                    .ShrinkFactor = Plan.ShrinkFactor
                },
                .Totals = Totals.Clone(),
                .Placement = Placement.Clone(),
                .Synthesis = Synthesis.Clone(),
                .Guards = Guards.Clone()
            }
        End Function

        Public Overrides Function ToString() As String
            Return $"CellularAI model: views={String.Join("/", Pooling.Views)}, rank={Totals.ResidualRank}, ridge={Totals.ResidualRidge:G4}, n_max={Calls.NMax}"
        End Function

    End Class
End Namespace
