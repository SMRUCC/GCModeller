Namespace Model

    ''' <summary>
    ''' 数值护栏的阈值：对应参考配置 <c>model.guards</c> 段。
    ''' </summary>
    ''' <remarks>
    ''' 这些阈值本身不改变算法，只决定"哪些数值退化会被记录/拦截"。
    ''' 默认值全部取自已发布配置，正常情况下不要修改。
    ''' </remarks>
    Public Class GuardOptions

        ''' <summary>合并后 log2FC 的绝对值上限，默认 6.25。</summary>
        Public Property L2AbsMax As Double = 6.25

        ''' <summary>合并后 z 的绝对值上限，默认 187.5。</summary>
        Public Property ZAbsMax As Double = 187.5

        ''' <summary>逆方差权重里"相对方差地板"的系数（乘以该基因的方差中位数），默认 0.0024。</summary>
        Public Property VFloorRel As Double = 0.0024

        ''' <summary>相似度 z 标准化时允许的最小标准差，默认 0.04。</summary>
        Public Property SimSdMin As Double = 0.04

        ''' <summary>相似度 z 的裁剪幅度，默认 8.125。</summary>
        Public Property SimZClip As Double = 8.125

        ''' <summary>噪声比因子的裁剪上限，默认 320。</summary>
        Public Property NrClip As Double = 320.0

        ''' <summary>设计层 log2 比例的绝对上限，默认 12.5。</summary>
        Public Property Log2Max As Double = 12.5

        ''' <summary>放置目标 log2 偏移的裁剪幅度，默认 7.5。</summary>
        Public Property MtLsClip As Double = 7.5

        ''' <summary>排序用相关系数标准差的下限，默认 0.028。</summary>
        Public Property OrderSdMin As Double = 0.028

        ''' <summary>各阶段允许的最大单项权重占比；键为阶段名（plan / sim / xsim / pooling）。</summary>
        Public Property ShareMax As Dictionary(Of String, Double) = New Dictionary(Of String, Double)(StringComparer.Ordinal) From {
            {"plan", 0.24},
            {"sim", 0.45},
            {"xsim", 0.45},
            {"pooling", 0.12}
        }

        ''' <summary>各阶段要求的最小有效权重数；键为阶段名。</summary>
        Public Property NeffMin As Dictionary(Of String, Double) = New Dictionary(Of String, Double)(StringComparer.Ordinal) From {
            {"plan", 16.0},
            {"sim", 5.9},
            {"xsim", 3.4},
            {"pooling", 30.0}
        }

        ''' <summary>允许的"完全未测靶"数量，默认 0。</summary>
        Public Property MaxUnmeasured As Integer = 0

        ''' <summary>允许的缺失 readout 比例上限，默认 0.005。</summary>
        Public Property MaxMissingReadoutFrac As Double = 0.005

        ''' <summary>信用比的上折点，默认 1.001。</summary>
        Public Property CreditKink As Double = 1.001

        ''' <summary>信用比的下界，默认 0.975。</summary>
        Public Property CreditBandLo As Double = 0.975

        ''' <summary>逐类护栏的命中率上限；键为护栏名。</summary>
        Public Property MaxHitRate As Dictionary(Of String, Double) = New Dictionary(Of String, Double)(StringComparer.Ordinal) From {
            {"nr_clip", 0.00002},
            {"log2_max", 0.0075}
        }

        ''' <summary>读取某阶段的最大权重占比；未配置时返回 0（表示不检查）。</summary>
        Public Function ShareMaxFor(stage As String) As Double
            Return Lookup(ShareMax, stage)
        End Function

        ''' <summary>读取某阶段的最小有效权重数；未配置时返回 0（表示不检查）。</summary>
        Public Function NeffMinFor(stage As String) As Double
            Return Lookup(NeffMin, stage)
        End Function

        ''' <summary>读取某类护栏的命中率上限；未配置时返回 0（表示不检查）。</summary>
        Public Function MaxHitRateFor(name As String) As Double
            Return Lookup(MaxHitRate, name)
        End Function

        Private Shared Function Lookup(table As Dictionary(Of String, Double), key As String) As Double
            If table Is Nothing OrElse String.IsNullOrEmpty(key) Then
                Return 0.0
            End If

            Dim value As Double = Nothing

            If table.TryGetValue(key, value) Then
                Return value
            Else
                Return 0.0
            End If
        End Function

        ''' <summary>深拷贝（用于在测试或调参时派生新配置）。</summary>
        Public Function Clone() As GuardOptions
            Return New GuardOptions With {
                .L2AbsMax = L2AbsMax,
                .ZAbsMax = ZAbsMax,
                .VFloorRel = VFloorRel,
                .SimSdMin = SimSdMin,
                .SimZClip = SimZClip,
                .NrClip = NrClip,
                .Log2Max = Log2Max,
                .MtLsClip = MtLsClip,
                .OrderSdMin = OrderSdMin,
                .ShareMax = New Dictionary(Of String, Double)(ShareMax, StringComparer.Ordinal),
                .NeffMin = New Dictionary(Of String, Double)(NeffMin, StringComparer.Ordinal),
                .MaxUnmeasured = MaxUnmeasured,
                .MaxMissingReadoutFrac = MaxMissingReadoutFrac,
                .CreditKink = CreditKink,
                .CreditBandLo = CreditBandLo,
                .MaxHitRate = New Dictionary(Of String, Double)(MaxHitRate, StringComparer.Ordinal)
            }
        End Function

    End Class
End Namespace
