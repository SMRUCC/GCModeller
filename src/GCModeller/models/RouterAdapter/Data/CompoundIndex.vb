' ============================================================================
' CompoundIndex.vb — 与数据源无关的化合物结构索引构建
' ----------------------------------------------------------------------------
' 把"化合物种子（id / SMILES / 名称）"批量净化并解析为 CompoundStructure，
' 同时产出可用于装配日志的诊断统计（总数、可用数、无 SMILES 数、拒绝原因与样例）。
' 两个适配器（BioCyc 与内部代谢模型）共用此件，装配日志格式因此完全一致。
' ============================================================================

Imports SMRUCC.genomics.Analysis.RetroPath.Chem

''' <summary>
''' 结构索引的输入种子：一个待索引的化合物。
''' </summary>
Public Class CompoundSeed

    ''' <summary>化合物唯一标识。</summary>
    Public Property Id As String

    ''' <summary>原始 SMILES（未净化；为空即表示数据源未提供结构）。</summary>
    Public Property Smiles As String

    ''' <summary>显示名称（可选，供名称白名单与结果展示使用）。</summary>
    Public Property Name As String

    ''' <summary>同义名（可选）。</summary>
    Public Property Synonyms As String()

    ''' <summary>便捷构造。</summary>
    Public Shared Function Create(id As String, smiles As String,
                                  Optional name As String = Nothing,
                                  Optional synonyms As String() = Nothing) As CompoundSeed
        Return New CompoundSeed With {
            .Id = id, .Smiles = smiles, .Name = name, .Synonyms = synonyms
        }
    End Function

End Class

''' <summary>
''' 结构索引的构建结果。
''' </summary>
Public Class CompoundIndexResult

    ''' <summary>id → 已净化结构（可直接供规则挖掘与汇构建使用）。</summary>
    Public ReadOnly Property Structures As New Dictionary(Of String, CompoundStructure)()

    ''' <summary>数据源未提供 SMILES 的化合物 id。</summary>
    Public ReadOnly Property MissingSmiles As New List(Of String)()

    ''' <summary>SMILES 存在但不可用（解析失败 / 含不支持元素 / 超重原子上限）的原因计数。</summary>
    Public ReadOnly Property RejectReasons As New Dictionary(Of String, Integer)()

    ''' <summary>每种拒绝原因的第一个样例（"id = SMILES"），便于定位问题条目。</summary>
    Public ReadOnly Property RejectSamples As New Dictionary(Of String, String)()

    ''' <summary>输入的化合物总数。</summary>
    Public Property Total As Integer

    ''' <summary>成功建立结构的化合物数。</summary>
    Public ReadOnly Property Count As Integer
        Get
            Return Structures.Count
        End Get
    End Property

    ''' <summary>取某个化合物的结构；不存在时返回 Nothing。</summary>
    Public Function TryGet(id As String) As CompoundStructure
        Dim st As CompoundStructure = Nothing
        Structures.TryGetValue(id, st)
        Return st
    End Function

End Class

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
