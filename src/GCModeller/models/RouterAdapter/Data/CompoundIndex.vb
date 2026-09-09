' ============================================================================
' CompoundIndex.vb — 与数据源无关的化合物结构索引构建
' ----------------------------------------------------------------------------
' 把"化合物种子（id / SMILES / 名称）"批量净化并解析为 CompoundStructure，
' 同时产出可用于装配日志的诊断统计（总数、可用数、无 SMILES 数、拒绝原因与样例）。
' 两个适配器（BioCyc 与内部代谢模型）共用此件，装配日志格式因此完全一致。
' ============================================================================

Imports SMRUCC.genomics.Analysis.RetroPath.Chem

Public Module CompoundIndex

    ''' <summary>
    ''' 批量构建结构索引。单条脏数据只跳过并计数，绝不让装配整体失败。
    ''' </summary>
    ''' <param name="seeds">化合物种子序列（可为任意数据源的产物）。</param>
    ''' <param name="maxAtoms">单个分子的重原子数上限；超出者跳过（控耗时）。</param>
    ''' <returns>结构索引与诊断统计。</returns>
    Public Function Build(seeds As IEnumerable(Of CompoundSeed),
                          Optional maxAtoms As Integer = 80) As CompoundIndexResult

        Dim result As New CompoundIndexResult()

        If seeds Is Nothing Then Return result

        For Each seed As CompoundSeed In seeds
            If seed Is Nothing OrElse String.IsNullOrEmpty(seed.Id) Then Continue For

            result.Total += 1

            If String.IsNullOrEmpty(seed.Smiles) Then
                result.MissingSmiles.Add(seed.Id)
                Continue For
            End If

            Dim smiles As String = Nothing
            Dim mol As Molecule = Nothing
            Dim why As String = Nothing

            If Not SmilesSanitizer.TryParse(seed.Smiles, smiles, mol, maxAtoms, why) Then
                Dim n As Integer = 0
                result.RejectReasons.TryGetValue(why, n)
                result.RejectReasons(why) = n + 1
                If Not result.RejectSamples.ContainsKey(why) Then
                    result.RejectSamples(why) = seed.Id & " = " & seed.Smiles
                End If
                Continue For
            End If

            result.Structures(seed.Id) = New CompoundStructure With {
                .Id = seed.Id,
                .Smiles = smiles,
                .Mol = mol,
                .Name = seed.Name,
                .Synonyms = seed.Synonyms
            }
        Next

        Return result
    End Function

    ''' <summary>
    ''' 反应"参与度"统计：各化合物在反应中作为底物/产物出现的总次数，
    ''' 用于识别枢纽代谢物（Core 模式下汇集合的判定依据之一）。
    ''' </summary>
    ''' <param name="specs">已归正的反应契约集合。</param>
    ''' <returns>compound id → 出现次数。</returns>
    Public Function DegreeOf(specs As IEnumerable(Of ReactionSpec)) As Dictionary(Of String, Integer)
        Dim degrees As New Dictionary(Of String, Integer)()

        If specs Is Nothing Then Return degrees

        For Each spec As ReactionSpec In specs
            If spec Is Nothing Then Continue For
            CountSide(spec.ReactantIds, degrees)
            CountSide(spec.ProductIds, degrees)
        Next

        Return degrees
    End Function

    Private Sub CountSide(ids As IEnumerable(Of String), degrees As Dictionary(Of String, Integer))
        If ids Is Nothing Then Return

        For Each id As String In ids
            If String.IsNullOrEmpty(id) Then Continue For
            Dim n As Integer = 0
            degrees.TryGetValue(id, n)
            degrees(id) = n + 1
        Next
    End Sub

End Module
