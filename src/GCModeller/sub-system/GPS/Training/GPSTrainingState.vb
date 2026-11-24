Imports System.IO
Imports System.Text
Imports System.Text.Json

Namespace Training

    ''' <summary>
    ''' 可持久化的训练状态：从 checkpoint 恢复续训所需的全部 trainer 侧状态。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 该类型只描述"训练进行到哪了"，不包含模型参数与优化器动量
    ''' （前者在 <c>parameters.tsv</c>、后者在 <c>optimizer.tsv</c>）。
    ''' 三者合起来就是一次训练中断时的完整现场。
    ''' </para>
    ''' <para>
    ''' 数值序列化约定与 <c>GPSFile</c> 一致：NaN/±∞ 以 JSON <c>null</c> 承载，
    ''' 读取端还原为 <see cref="Double.NaN"/>/<see cref="Double.PositiveInfinity"/>。
    ''' </para>
    ''' </remarks>
    Public Class GPSTrainingState

        ''' <summary>阶段标识：掩码重建预训练</summary>
        Public Const PhasePretrain As Integer = 0

        ''' <summary>阶段标识：下一时刻预测微调</summary>
        Public Const PhaseFinetune As Integer = 1

        ''' <summary>中断时所处阶段（<see cref="PhasePretrain"/> 或 <see cref="PhaseFinetune"/>）</summary>
        Public Property Phase As Integer = PhasePretrain

        ''' <summary>预训练已完成的 epoch 数</summary>
        Public Property PretrainEpochsDone As Integer

        ''' <summary>微调已完成的 epoch 数</summary>
        Public Property FinetuneEpochsDone As Integer

        ''' <summary>全局参数更新步数（Adam 偏置校正依据，从 1 开始）</summary>
        Public Property StepIndex As Integer

        ''' <summary>早停耐心计数（验证集连续未改善的 epoch 数）</summary>
        Public Property Patience As Integer

        ''' <summary>最优验证损失对应的 epoch（0 基；无验证集时为 -1 表示未发生快照）</summary>
        Public Property BestEpoch As Integer = -1

        ''' <summary>最优验证损失（无验证集时为 +∞）</summary>
        Public Property BestValidationLoss As Double = Double.PositiveInfinity

        ''' <summary>最优 epoch 的参数快照（键 = 参数名；无验证集时为空）</summary>
        Public Property BestParameters As New Dictionary(Of String, Double())(StringComparer.Ordinal)

        ''' <summary>历史损失曲线前缀：预训练各 epoch 平均损失</summary>
        Public Property PretrainLoss As Double()

        ''' <summary>历史损失曲线前缀：微调各 epoch 训练损失</summary>
        Public Property TrainLoss As Double()

        ''' <summary>历史损失曲线前缀：微调各 epoch 验证损失</summary>
        Public Property ValidationLoss As Double()

        ''' <summary>序列化为 JSON 文本。</summary>
        ''' <param name="includeBestParameters">
        ''' 是否把最优参数快照内嵌到 JSON。全基因组模型的最优快照有上千万个数值，
        ''' 内嵌会生成数 GB 的字符串；此时调用方应传 <c>False</c> 并把快照单独流式落盘
        ''' （见 <c>GPSFile</c> 的 <c>state.best.tsv</c>）。
        ''' </param>
        ''' <returns>JSON 文本</returns>
        Public Function ToJson(Optional includeBestParameters As Boolean = True) As String
            Using stream As New MemoryStream()
                Using writer As New Utf8JsonWriter(stream, New JsonWriterOptions With {.Indented = True})
                    Call Write(writer, includeBestParameters)
                End Using

                Return Encoding.UTF8.GetString(stream.ToArray())
            End Using
        End Function

        Private Sub Write(writer As Utf8JsonWriter, includeBestParameters As Boolean)
            writer.WriteStartObject()
            writer.WriteNumber(NameOf(GPSTrainingState.Phase), Phase)
            writer.WriteNumber(NameOf(GPSTrainingState.PretrainEpochsDone), PretrainEpochsDone)
            writer.WriteNumber(NameOf(GPSTrainingState.FinetuneEpochsDone), FinetuneEpochsDone)
            writer.WriteNumber(NameOf(GPSTrainingState.StepIndex), StepIndex)
            writer.WriteNumber(NameOf(GPSTrainingState.Patience), Patience)
            writer.WriteNumber(NameOf(GPSTrainingState.BestEpoch), BestEpoch)

            If Double.IsNaN(BestValidationLoss) OrElse Double.IsInfinity(BestValidationLoss) Then
                writer.WriteNull(NameOf(GPSTrainingState.BestValidationLoss))
            Else
                writer.WriteNumber(NameOf(GPSTrainingState.BestValidationLoss), BestValidationLoss)
            End If

            writer.WriteStartArray(NameOf(GPSTrainingState.PretrainLoss))

            For Each value As Double In If(PretrainLoss, Array.Empty(Of Double)())
                Call WriteNumber(writer, value)
            Next

            writer.WriteEndArray()
            writer.WriteStartArray(NameOf(GPSTrainingState.TrainLoss))

            For Each value As Double In If(TrainLoss, Array.Empty(Of Double)())
                Call WriteNumber(writer, value)
            Next

            writer.WriteEndArray()
            writer.WriteStartArray(NameOf(GPSTrainingState.ValidationLoss))

            For Each value As Double In If(ValidationLoss, Array.Empty(Of Double)())
                Call WriteNumber(writer, value)
            Next

            writer.WriteEndArray()

            If includeBestParameters Then
                writer.WriteStartObject(NameOf(GPSTrainingState.BestParameters))

                For Each kv As KeyValuePair(Of String, Double()) In BestParameters
                    writer.WriteStartArray(kv.Key)

                    For Each value As Double In kv.Value
                        Call WriteNumber(writer, value)
                    Next

                    writer.WriteEndArray()
                Next

                writer.WriteEndObject()
            End If

            writer.WriteEndObject()
        End Sub

        ''' <summary>从 JSON 文本还原训练状态；缺失字段保留默认值（v1 包降级路径）。</summary>
        ''' <param name="json">JSON 文本</param>
        ''' <returns>训练状态对象</returns>
        Public Shared Function FromJson(json As String) As GPSTrainingState
            Dim state As New GPSTrainingState()

            If String.IsNullOrWhiteSpace(json) Then Return state

            Using document As JsonDocument = JsonDocument.Parse(json)
                Dim root As JsonElement = document.RootElement
                Dim value As JsonElement

                If root.TryGetProperty(NameOf(GPSTrainingState.Phase), value) Then state.Phase = value.GetInt32()
                If root.TryGetProperty(NameOf(GPSTrainingState.PretrainEpochsDone), value) Then state.PretrainEpochsDone = value.GetInt32()
                If root.TryGetProperty(NameOf(GPSTrainingState.FinetuneEpochsDone), value) Then state.FinetuneEpochsDone = value.GetInt32()
                If root.TryGetProperty(NameOf(GPSTrainingState.StepIndex), value) Then state.StepIndex = value.GetInt32()
                If root.TryGetProperty(NameOf(GPSTrainingState.Patience), value) Then state.Patience = value.GetInt32()
                If root.TryGetProperty(NameOf(GPSTrainingState.BestEpoch), value) Then state.BestEpoch = value.GetInt32()

                If root.TryGetProperty(NameOf(GPSTrainingState.BestValidationLoss), value) Then
                    state.BestValidationLoss = If(value.ValueKind = JsonValueKind.Null,
                                                  Double.PositiveInfinity,
                                                  value.GetDouble())
                End If

                If root.TryGetProperty(NameOf(GPSTrainingState.PretrainLoss), value) Then state.PretrainLoss = ReadDoubleArray(value)
                If root.TryGetProperty(NameOf(GPSTrainingState.TrainLoss), value) Then state.TrainLoss = ReadDoubleArray(value)
                If root.TryGetProperty(NameOf(GPSTrainingState.ValidationLoss), value) Then state.ValidationLoss = ReadDoubleArray(value)

                If root.TryGetProperty(NameOf(GPSTrainingState.BestParameters), value) AndAlso
                   value.ValueKind = JsonValueKind.Object Then

                    For Each item As JsonProperty In value.EnumerateObject()
                        state.BestParameters(item.Name) = ReadDoubleArray(item.Value)
                    Next
                End If
            End Using

            Return state
        End Function

        Private Shared Sub WriteNumber(writer As Utf8JsonWriter, value As Double)
            If Double.IsNaN(value) OrElse Double.IsInfinity(value) Then
                writer.WriteNullValue()
            Else
                writer.WriteNumberValue(value)
            End If
        End Sub

        Private Shared Function ReadDoubleArray(element As JsonElement) As Double()
            Dim values As New List(Of Double)()

            For Each item As JsonElement In element.EnumerateArray()
                Call values.Add(If(item.ValueKind = JsonValueKind.Null, Double.NaN, item.GetDouble()))
            Next

            Return values.ToArray()
        End Function

    End Class

End Namespace
