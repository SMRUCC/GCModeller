' ============================================================================
' Module 1: Genome Annotation & Feature Extraction
' File: GenomeAnnotation/FeatureMatrix.vb
'
' 功能: 将 Pfam 命中结果转化为二值化特征矩阵（系统发育谱 / phyletic pattern）。
'       对应论文中 "将每个样本中各 Pfam 家族的数量转化为存在(1)/缺失(0)的二元矩阵 X"。
'
' 论文发现: 二元表示（presence/absence）能获得较好的分类性能。
' ============================================================================

Namespace Traitar.GenomeAnnotation

    ''' <summary>
    ''' 表示一个基因组的 Pfam 特征向量（二值化系统发育谱）
    ''' </summary>
    Public Class PhyleticProfile
        ''' <summary>基因组/样本ID</summary>
        Public Property SampleId As String
        ''' <summary>存在的 Pfam 家族集合（值为 1）</summary>
        Public Property PresentPfams As New HashSet(Of String)
        ''' <summary>所有已知的 Pfam 家族列表（用于确定特征维度顺序）</summary>
        Public Property AllPfams As List(Of String)

        ''' <summary>
        ''' 转换为二值向量（按 AllPfams 顺序）
        ''' </summary>
        Public Function ToBinaryVector() As Integer()
            If AllPfams Is Nothing Then Return Nothing
            Dim vec(AllPfams.Count - 1) As Integer
            For i = 0 To AllPfams.Count - 1
                If PresentPfams.Contains(AllPfams(i)) Then
                    vec(i) = 1
                Else
                    vec(i) = 0
                End If
            Next
            Return vec
        End Function

        ''' <summary>
        ''' 转换为 Double 向量（兼容 SVM Node 输入）
        ''' </summary>
        Public Function ToDoubleVector() As Double()
            If AllPfams Is Nothing Then Return Nothing
            Dim vec(AllPfams.Count - 1) As Double
            For i = 0 To AllPfams.Count - 1
                If PresentPfams.Contains(AllPfams(i)) Then
                    vec(i) = 1.0
                Else
                    vec(i) = 0.0
                End If
            Next
            Return vec
        End Function

        ''' <summary>检查某 Pfam 是否存在</summary>
        Public Function HasPfam(pfamAcc As String) As Boolean
            Return PresentPfams.Contains(pfamAcc)
        End Function

        ''' <summary>添加 Pfam 家族</summary>
        Public Sub AddPfam(pfamAcc As String)
            PresentPfams.Add(pfamAcc)
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{SampleId}] {PresentPfams.Count} Pfam families present"
        End Function
    End Class

    ''' <summary>
    ''' 特征矩阵构建器
    ''' </summary>
    Public Class FeatureMatrixBuilder

        ''' <summary>
        ''' 从 Pfam 命中列表构建特征矩阵
        ''' </summary>
        ''' <param name="hits">所有 Pfam 命中记录</param>
        ''' <returns>(profiles, allPfams) - 每个样本的特征向量 + 所有 Pfam 家族列表</returns>
        Public Shared Function BuildFromHits(hits As List(Of PfamHit)) _
            As (PresentPfams As List(Of PhyleticProfile), List(Of String))

            ' 按蛋白质ID分组（这里假设每个蛋白质属于一个基因组/样本）
            ' 实际使用时可能需要按基因组分组
            Dim hitsBySample As New Dictionary(Of String, List(Of PfamHit))
            For Each hit In hits
                ' 使用蛋白质ID作为样本ID（简化版）
                ' 实际应用中应按基因组分组
                Dim sampleId = hit.ProteinId
                If Not hitsBySample.ContainsKey(sampleId) Then
                    hitsBySample(sampleId) = New List(Of PfamHit)
                End If
                hitsBySample(sampleId).Add(hit)
            Next

            ' 收集所有出现过的 Pfam 家族，排序后作为特征维度
            Dim allPfamsSet As New HashSet(Of String)
            For Each kv In hitsBySample
                For Each hit In kv.Value
                    allPfamsSet.Add(hit.PfamAcc)
                Next
            Next
            Dim allPfams As New List(Of String)(allPfamsSet)
            allPfams.Sort()

            ' 为每个基因组构建特征向量
            Dim profiles As New List(Of PhyleticProfile)
            For Each kv In hitsBySample
                profiles.Add(BuildProfile(kv.Key, kv.Value, allPfams))
            Next

            Return (profiles, allPfams)
        End Function

        ''' <summary>
        ''' 为单个基因组构建特征向量
        ''' </summary>
        Public Shared Function BuildProfile(sampleId As String,
                                            hits As List(Of PfamHit),
                                            allPfams As List(Of String)) As PhyleticProfile
            Dim profile As New PhyleticProfile With {
                .SampleId = sampleId,
                .AllPfams = allPfams
            }
            For Each hit In hits
                profile.AddPfam(hit.PfamAcc)
            Next
            Return profile
        End Function

        ''' <summary>
        ''' 从单个基因组的 Pfam 命中构建特征向量
        ''' </summary>
        Public Shared Function BuildSingleProfile(sampleId As String,
                                                   hits As List(Of PfamHit)) As PhyleticProfile
            Dim allPfams As New List(Of String)
            Dim seen As New HashSet(Of String)
            For Each hit In hits
                If seen.Add(hit.PfamAcc) Then
                    allPfams.Add(hit.PfamAcc)
                End If
            Next
            allPfams.Sort()

            Dim profile As New PhyleticProfile With {
                .SampleId = sampleId,
                .AllPfams = allPfams
            }
            For Each hit In hits
                profile.AddPfam(hit.PfamAcc)
            Next
            Return profile
        End Function

        ''' <summary>
        ''' 将特征矩阵输出为 TSV 格式（行=样本, 列=Pfam, 值=0/1）
        ''' </summary>
        Public Shared Function ToTsv(profiles As List(Of PhyleticProfile)) As String
            If profiles.Count = 0 Then Return ""
            Dim allPfams = profiles(0).AllPfams
            Dim lines As New List(Of String)

            ' header
            lines.Add("SampleID" & vbTab & String.Join(vbTab, allPfams))

            ' data
            For Each p In profiles
                Dim vec = p.ToBinaryVector()
                Dim cells(allPfams.Count - 1) As String
                For i = 0 To vec.Length - 1
                    cells(i) = vec(i).ToString()
                Next
                lines.Add(p.SampleId & vbTab & String.Join(vbTab, cells))
            Next

            Return String.Join(vbCrLf, lines)
        End Function
    End Class
End Namespace
