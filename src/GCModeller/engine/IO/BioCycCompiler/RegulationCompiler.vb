Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2

''' <summary>
''' 由 ``regulation.dat``/``dnabindsites.dat``/``regulons.dat`` 构建虚拟细胞模型之中的
''' 转录调控网络。
''' 
''' 旧版本实现之中仅仅写入了调控因子以及调控模式，而 ``operonId`` 一直为空，导致下游的
''' 虚拟细胞构建过程会丢弃全部的调控关系；本实现会通过结合位点、启动子以及基因三个层级
''' 建立从被调控对象到转录单元的映射
''' </summary>
Public Class RegulationCompiler

    ReadOnly biocyc As Workspace

    ''' <summary>
    ''' 转录单元编号到转录单元对象的索引
    ''' </summary>
    ReadOnly operonIndex As New Dictionary(Of String, TranscriptUnit)
    ''' <summary>
    ''' 基因编号、启动子编号或者结合位点编号到转录单元编号的索引
    ''' </summary>
    ReadOnly entityIndex As New Dictionary(Of String, String)
    ''' <summary>
    ''' 结合位点编号到结合位点对象的索引
    ''' </summary>
    ReadOnly sites As New Dictionary(Of String, BindingSite)
    ''' <summary>
    ''' 调控因子编号到调控组编号的索引
    ''' </summary>
    ReadOnly regulonIndex As New Dictionary(Of String, String)

    Sub New(biocyc As Workspace, operons As IEnumerable(Of TranscriptUnit), bindingSites As BindingSite(), regulons As Regulon())
        Me.biocyc = biocyc

        For Each operon As TranscriptUnit In operons.SafeQuery
            If operon.id.StringEmpty(, True) Then
                Continue For
            End If

            operonIndex(operon.id) = operon
            entityIndex(operon.id) = operon.id

            For Each gene As gene In operon.genes.SafeQuery
                If Not gene.locus_tag.StringEmpty(, True) Then
                    entityIndex(gene.locus_tag) = operon.id
                End If
            Next

            If Not operon.promoter Is Nothing Then
                entityIndex(operon.promoter.id) = operon.id
            End If
        Next

        For Each site As BindingSite In bindingSites.SafeQuery
            If site.id.StringEmpty(, True) Then
                Continue For
            End If

            sites(site.id) = site

            If Not site.operonId.StringEmpty(, True) Then
                entityIndex(site.id) = site.operonId
            End If
        Next

        For Each regulon As Regulon In regulons.SafeQuery
            If Not regulon.regulator.StringEmpty(, True) Then
                regulonIndex(regulon.regulator) = regulon.id
            End If
        Next
    End Sub

    ''' <summary>
    ''' 创建转录调控关系
    ''' </summary>
    ''' <param name="operons">保留该参数以兼容旧版本的调用方式，实际的索引在构造函数之中已经建立</param>
    ''' <returns></returns>
    Public Iterator Function CreateRegulations(Optional operons As IEnumerable(Of TranscriptUnit) = Nothing) As IEnumerable(Of transcription)
        For Each reg As regulation In biocyc.regulation.features
            If Not reg.types.SafeQuery.Any(Function(t) t = "Transcription-Factor-Binding") Then
                Continue For
            End If

            Dim entity As String = reg.regulatedEntity
            Dim operonId As String = resolveOperon(entity)
            Dim site As BindingSite = sites.TryGetValue(entity)
            Dim motif As Motif = createMotif(site)

            If motif Is Nothing AndAlso Not operonId.StringEmpty(, True) Then
                ' 没有直接匹配到结合位点的时候，使用被调控转录单元的启动子坐标作为替代
                Dim operon As TranscriptUnit = operonIndex.TryGetValue(operonId)

                If Not operon Is Nothing AndAlso Not operon.promoter Is Nothing Then
                    motif = New Motif With {
                        .family = "promoter",
                        .left = operon.promoter.left,
                        .right = operon.promoter.right,
                        .strand = If(operon.promoter.strand.StringEmpty(, True), "+"c, operon.promoter.strand.First),
                        .distance = 0
                    }
                End If
            End If

            Yield New transcription With {
                .regulator = reg.regulator,
                .mode = reg.mode,
                .mechanism = reg.mechanism,
                .ki = reg.Ki,
                .accessoryProteins = reg.accessoryProteins,
                .regulonId = regulonIndex.TryGetValue(reg.regulator),
                .operonId = operonId,
                .centralDogma = If(operonId.StringEmpty(, True), New String() {}, {operonId}),
                .biological_process = If(Not site Is Nothing, site.siteType, "Transcription-Factor-Binding"),
                .motif = motif,
                .bindingSite = site,
                .evidence = compilerEvidence(reg),
                .note = reg.comment
            }
        Next
    End Function

    Private Function compilerEvidence(reg As regulation) As String()
        Return reg.citations _
            .SafeQuery _
            .Select(Function(citation)
                        Dim token As String = citation.Split(":"c).First.Trim

                        If token.StringEmpty(, True) Then
                            Return Nothing
                        Else
                            Return "PUB-" & token
                        End If
                    End Function) _
            .Where(Function(id) Not id.StringEmpty(, True)) _
            .Distinct _
            .ToArray
    End Function

    Private Function resolveOperon(entity As String) As String
        If entity.StringEmpty(, True) Then
            Return Nothing
        End If

        Return entityIndex.TryGetValue(entity)
    End Function

    Private Shared Function createMotif(site As BindingSite) As Motif
        If site Is Nothing Then
            Return Nothing
        End If

        Return New Motif With {
            .family = site.siteType,
            .left = site.left,
            .right = site.right,
            .strand = If(site.strand.StringEmpty(, True), "+"c, site.strand.First),
            .distance = 0
        }
    End Function

End Class
