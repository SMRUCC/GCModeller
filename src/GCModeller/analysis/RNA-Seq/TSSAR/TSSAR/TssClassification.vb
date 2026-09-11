Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Analysis.RNA_Seq.TSSAR.Models
Imports std = System.Math

''' <summary>
''' 基于基因注释（PTT）对 TSS 执行基因组上下文分类，并推断 5'UTR 长度。
''' </summary>
''' <remarks>
''' 对应 TSSAR 论文后处理阶段的分类：
''' 
''' - <b>Primary</b>：位于某个基因起始密码子上游 <c>primaryUpstream</c>（默认 250 nt）内的启动子区；
''' - <b>Internal</b>：位于某个基因的开放阅读框内部；
''' - <b>Antisense (Ai / Ad)</b>：位于反义链基因的内部（Ai）或其下游 <c>antisenseDownstream</c>
'''   （默认 30 nt）范围内（Ad）；
''' - <b>Orphan</b>：附近没有任何关联基因。
''' </remarks>
Public Module TssClassification

    ''' <summary>
    ''' 对一批 TSS 位置执行上下文分类。
    ''' </summary>
    Public Sub Classify(sites As IEnumerable(Of TssSite),
                        ptt As PTT,
                        Optional primaryUpstream As Integer = 250,
                        Optional antisenseDownstream As Integer = 30)

        If sites Is Nothing OrElse ptt Is Nothing Then
            Return
        End If

        Dim genes As GeneBrief() = ptt.GeneObjects

        If genes Is Nothing Then
            Return
        End If

        For Each site As TssSite In sites
            Classify(site, genes, primaryUpstream, antisenseDownstream)
        Next
    End Sub

    ''' <summary>
    ''' 对单个 TSS 位置执行上下文分类。
    ''' </summary>
    Public Sub Classify(site As TssSite,
                        ptt As PTT,
                        Optional primaryUpstream As Integer = 250,
                        Optional antisenseDownstream As Integer = 30)

        If ptt Is Nothing Then
            Return
        End If

        Classify(site, ptt.GeneObjects, primaryUpstream, antisenseDownstream)
    End Sub

    Private Sub Classify(site As TssSite,
                         genes As GeneBrief(),
                         primaryUpstream As Integer,
                         antisenseDownstream As Integer)

        site.Type = TssTypes.Orphan
        site.UtrLength = 0
        site.Gene = Nothing
        site.GeneLocation = Nothing

        Dim antisenseGene As GeneBrief = Nothing
        Dim antisenseType As TssTypes = TssTypes.Orphan
        Dim antisenseDistance As Integer = Integer.MaxValue

        For Each gene As GeneBrief In genes
            If gene Is Nothing OrElse gene.Location Is Nothing Then
                Continue For
            End If

            Dim left As Integer = gene.Location.left
            Dim right As Integer = gene.Location.right
            Dim inside As Boolean = site.Position >= left AndAlso site.Position <= right

            If gene.Location.Strand = site.Strand Then
                ' 同链基因
                If inside Then
                    SetAssociation(site, gene, TssTypes.Internal, 0)
                    Return
                End If

                Dim upstreamDistance As Integer

                If site.Strand = Strands.Forward Then
                    upstreamDistance = If(site.Position < left, left - site.Position, Integer.MaxValue)
                Else
                    upstreamDistance = If(site.Position > right, site.Position - right, Integer.MaxValue)
                End If

                If upstreamDistance <= primaryUpstream AndAlso
                   (site.Type <> TssTypes.Primary OrElse upstreamDistance < site.UtrLength) Then
                    SetAssociation(site, gene, TssTypes.Primary, upstreamDistance)
                End If
            Else
                ' 反义链基因：寻找内部（Ai）或下游（Ad）关联
                Dim candidateType As TssTypes
                Dim candidateDistance As Integer

                If inside Then
                    candidateType = TssTypes.AntisenseInternal
                    candidateDistance = 0
                Else
                    Dim downstreamDistance As Integer

                    If gene.Location.Strand = Strands.Forward Then
                        downstreamDistance = If(site.Position > right, site.Position - right, Integer.MaxValue)
                    Else
                        downstreamDistance = If(site.Position < left, left - site.Position, Integer.MaxValue)
                    End If

                    If downstreamDistance > antisenseDownstream Then
                        Continue For
                    End If

                    candidateType = TssTypes.AntisenseDownstream
                    candidateDistance = downstreamDistance
                End If

                If candidateType = TssTypes.AntisenseInternal OrElse
                   antisenseType <> TssTypes.AntisenseInternal Then

                    If candidateDistance < antisenseDistance OrElse antisenseGene Is Nothing Then
                        antisenseGene = gene
                        antisenseType = candidateType
                        antisenseDistance = candidateDistance
                    End If
                End If
            End If
        Next

        ' 只有在没有同链 Internal / Primary 的情况下才采用反义分类
        If site.Type = TssTypes.Orphan AndAlso antisenseGene IsNot Nothing Then
            SetAssociation(site, antisenseGene, antisenseType, 0)
        End If
    End Sub

    Private Sub SetAssociation(site As TssSite, gene As GeneBrief, type As TssTypes, utrLength As Integer)
        site.Type = type
        site.Gene = gene.Synonym

        If gene.Location IsNot Nothing Then
            site.GeneLocation = gene.Location.ToString()
        End If

        site.UtrLength = If(type = TssTypes.Primary, utrLength, 0)
    End Sub
End Module
