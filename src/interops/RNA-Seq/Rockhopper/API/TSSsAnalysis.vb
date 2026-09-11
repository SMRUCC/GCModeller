' /********************************************************************************/
'
'  Rockhopper —— 分析结果读写与序列解析（API 层）
'
'  由原始 `API/TSSsAnalysis.vb` 迁移而来：
'    * 结果读取不再依赖已失效的 `Microsoft.VisualBasic.DocumentFormat.Csv`，
'      改为按 Rockhopper 的原始列约定解析（Transcription Start / ... / Expression N / qValue ...）；
'    * 序列解析不再依赖已删除的 `SegmentReader`，改为对 `FastaSeq` 直接切片
'      （<see cref="Transcripts.GetSegment"/>）；
'    * KEGG 富集迁移到 `Regulation.KEGGEnrichment`；
'    * DOOR 转换沿用 `SMRUCC.genomics.Assembly.DOOR` 模型。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace AnalysisAPI

    ''' <summary>
    ''' 从分析结果之中解析出 TSS→ATG 的序列（5'UTR）、-35 区→TSS 的序列（启动子）、
    ''' TGA→TTS 的序列（3'UTR）等，用于下游 motif 分析。
    ''' 所生成的 FASTA 序列文件中每条序列标题的第一个元素都是基因号。
    ''' </summary>
    Public Module TSSsAnalysis

        ''' <summary>
        ''' 读取 *_transcripts.txt（保持 Rockhopper 原始列约定）。
        ''' </summary>
        Public Function LoadResult(Path As String) As Transcripts()
            Dim lines As String() = File.ReadAllLines(Path)
            If lines.Length = 0 Then Return New Transcripts() {}

            Dim headers As String() = lines(0).Split(ControlChars.Tab)
            Dim iTSS As Integer = indexOf(headers, "Transcription Start")
            Dim iATG As Integer = indexOf(headers, "Translation Start")
            Dim iTGA As Integer = indexOf(headers, "Translation Stop")
            Dim iTTS As Integer = indexOf(headers, "Transcription Stop")
            Dim iStrand As Integer = indexOf(headers, "Strand")
            Dim iName As Integer = indexOf(headers, "Name")
            Dim iSynonym As Integer = indexOf(headers, "Synonym")
            Dim iProduct As Integer = indexOf(headers, "Product")

            ' 第一个 "Expression" 列作为表达量；所有 "qValue" 列取最小值
            Dim iExpression As Integer = firstIndexOf(headers, "Expression")
            Dim qColumns As Integer() = allIndexesOf(headers, "qValue")

            Dim result As New List(Of Transcripts)()
            For i As Integer = 1 To lines.Length - 1
                If String.IsNullOrWhiteSpace(lines(i)) Then Continue For
                Dim tokens As String() = lines(i).Split(ControlChars.Tab)

                Dim item As New Transcripts With {
                    .TSSs = CLng(Val(tokenAt(tokens, iTSS))),
                    .ATG = CLng(Val(tokenAt(tokens, iATG))),
                    .TGA = CLng(Val(tokenAt(tokens, iTGA))),
                    .TTSs = CLng(Val(tokenAt(tokens, iTTS))),
                    .Strand = tokenAt(tokens, iStrand),
                    .Name = tokenAt(tokens, iName),
                    .Synonym = tokenAt(tokens, iSynonym),
                    .Product = tokenAt(tokens, iProduct),
                    .Expression = CLng(Val(tokenAt(tokens, iExpression)))
                }

                Dim minQ As Double = 1.0
                For Each qc As Integer In qColumns
                    minQ = System.Math.Min(minQ, Val(tokenAt(tokens, qc)))
                Next
                item.QValue = minQ

                result.Add(item)
            Next

            Return result.ToArray
        End Function

        ''' <summary>
        ''' 读取 *_operons.txt（格式：<c>[Strand]Start,Stop;    gene1, gene2, ...</c>）。
        ''' </summary>
        Public Function LoadOperonResult(Path As String) As Operon()
            Dim result As New List(Of Operon)()
            For Each line As String In File.ReadLines(Path)
                Dim operon As Operon = parseOperonLine(line)
                If operon IsNot Nothing Then result.Add(operon)
            Next
            Return result.ToArray
        End Function

        Private Function parseOperonLine(line As String) As Operon
            If String.IsNullOrWhiteSpace(line) Then Return Nothing

            Dim strand As String = ""
            Dim rest As String = line.Trim
            If rest.StartsWith("[") Then
                Dim close As Integer = rest.IndexOf("]"c)
                If close > 0 Then
                    strand = rest.Substring(1, close - 1)
                    rest = rest.Substring(close + 1)
                End If
            End If

            Dim parts As String() = rest.Split(";"c)
            If parts.Length = 0 Then Return Nothing

            Dim coords As String() = parts(0).Split(","c)
            If coords.Length < 2 Then Return Nothing

            Dim genes As String() = If(parts.Length > 1,
                                       parts(1).Split(","c).Select(Function(s) s.Trim()).Where(Function(s) s.Length > 0).ToArray(),
                                       New String() {})

            Return New Operon With {
                .Strand = strand,
                .Start = CLng(Val(coords(0))),
                .[Stop] = CLng(Val(coords(1))),
                .Genes = genes
            }
        End Function

        ''' <summary>
        ''' 将结果文件中的基因名替换为基因号（locus_tag）。
        ''' </summary>
        Public Function SubstituteID(Operons As Operon(), PTT As PTT) As Operon()
            If PTT Is Nothing Then Return Operons
            For Each operon As Operon In Operons
                If operon.Genes Is Nothing Then Continue For
                For i As Integer = 0 To operon.Genes.Length - 1
                    Dim map As SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief =
                        PTT.GeneObjects.FirstOrDefault(Function(g) String.Equals(g.Gene, operon.Genes(i)))
                    If map IsNot Nothing Then operon.Genes(i) = map.Synonym
                Next
            Next
            Return Operons
        End Function

        ''' <summary>
        ''' 将 Rockhopper 的操纵子转换为 DOOR 数据库的操纵子格式（单基因也会补为一个操纵子）。
        ''' </summary>
        Public Function GenerateDoorOperon(Operons As Operon(), PTT As PTT) As DOOR
            Dim doorGenes As New List(Of OperonGene)()
            Dim idx As Integer = 1

            Dim covered As New HashSet(Of String)()
            For Each operon As Operon In Operons
                For Each gene As String In If(operon.Genes, New String() {})
                    Dim brief = If(PTT Is Nothing, Nothing, PTT.GeneObject(gene))
                    Dim doorGene As New OperonGene(brief)
                    doorGene.OperonID = $"OP{idx:0000}"
                    doorGenes.Add(doorGene)
                    covered.Add(gene)
                Next
                idx += 1
            Next

            ' 未出现在任何操纵子中的基因，各自作为一个单基因操纵子
            If PTT IsNot Nothing Then
                For Each brief In PTT.GeneObjects
                    If covered.Contains(brief.Synonym) Then Continue For
                    Dim doorGene As New OperonGene(brief)
                    doorGene.OperonID = $"OP{idx:0000}"
                    doorGenes.Add(doorGene)
                    idx += 1
                Next
            End If

            Return New DOOR With {.Genes = doorGenes.ToArray()}
        End Function

        ''' <summary>
        ''' 解析 5'UTR（TSS→ATG）序列。
        ''' </summary>
        Public Function Parsing5UTR(data As Transcripts(), Genome As FastaSeq) As FastaFile
            Dim fasta As FastaSeq() =
                (From site As Transcripts In data
                 Where Not (site.IsRNA OrElse site.Leaderless OrElse site.TSSs = 0 OrElse site.ATG = 0)
                 Let seq = site.Get5UTRLeader(Genome)
                 Where seq IsNot Nothing AndAlso seq.Length >= 6
                 Select seq).ToArray()
            Return internalTrimHead(fasta)
        End Function

        ''' <summary>解析 TSS 位点附近 ±5 bp 的片段序列。</summary>
        Public Function ParsingTSSs(data As Transcripts(), Genome As FastaSeq) As FastaFile
            Dim fasta As FastaSeq() =
                (From site As Transcripts In data
                 Where site.TSSs <> 0
                 Let seq = site.GetTSSLoci(Genome)
                 Where seq IsNot Nothing AndAlso seq.Length >= 6
                 Select seq).ToArray()
            Return internalTrimHead(fasta)
        End Function

        ''' <summary>解析 3'UTR（TGA→TTS）序列。</summary>
        Public Function ParsingTTSs(data As Transcripts(), Genome As FastaSeq) As FastaFile
            Dim fasta As FastaSeq() =
                (From site As Transcripts In data
                 Where Not (site.IsRNA OrElse site.TGA = 0 OrElse site.TTSs = 0 OrElse site.TGA = site.TTSs)
                 Let seq = site.GetTTSsLoci(Genome)
                 Where seq IsNot Nothing AndAlso seq.Length >= 6
                 Select seq).ToArray()
            Return internalTrimHead(fasta)
        End Function

        ''' <summary>解析 -35 区→TSS 的启动子序列。</summary>
        Public Function ParsingPromoterBox(data As Transcripts(), Genome As FastaSeq) As FastaFile
            Dim fasta As FastaSeq() =
                (From site As Transcripts In data
                 Where site.TSSs > 0
                 Let seq = site.GetPromoterBoxLoci(Genome)
                 Where seq IsNot Nothing AndAlso seq.Length >= 6 ' MEME 要求序列长度 > 6 bp
                 Select seq).ToArray()
            Return internalTrimHead(fasta)
        End Function

        ''' <summary>
        ''' 对重复的基因号追加序号，并把标题中的空格替换为下划线。
        ''' </summary>
        Private Function internalTrimHead(Fasta As FastaSeq()) As FastaFile
            If Fasta Is Nothing OrElse Fasta.Length = 0 Then Return New FastaFile()

            Dim groups = Fasta.Where(Function(f) f.Headers IsNot Nothing AndAlso f.Headers.Length > 0) _
                              .GroupBy(Function(f) f.Headers(0)).ToArray()
            For Each group In groups
                Dim items As FastaSeq() = group.ToArray()
                If items.Length > 1 Then
                    For i As Integer = 0 To items.Length - 1
                        items(i).Headers(0) = items(i).Headers(0) & "-" & (i + 1)
                    Next
                End If
            Next

            For Each item As FastaSeq In Fasta
                If item.Headers IsNot Nothing AndAlso item.Headers.Length > 0 Then
                    item.Headers(0) = item.Headers(0).Replace(" ", "_")
                End If
            Next

            Return New FastaFile(Fasta)
        End Function

        ''' <summary>
        ''' 条件间 TSS/TTS 差异计算（委托给 <see cref="Regulation.TSSsDifferentAnalysis"/>）。
        ''' </summary>
        Public Function DifferentTSSs(condition1 As Transcripts(), condition2 As Transcripts()) As Regulation.TSSsDifferent()
            Dim loci1 = condition1.Where(Function(t) Not String.IsNullOrEmpty(t.Synonym)) _
                                  .Select(Function(t) New Regulation.TranscriptLoci(t.Synonym, t.TSSs, t.TTSs))
            Dim loci2 = condition2.Where(Function(t) Not String.IsNullOrEmpty(t.Synonym)) _
                                  .Select(Function(t) New Regulation.TranscriptLoci(t.Synonym, t.TSSs, t.TTSs))
            Return Regulation.TSSsDifferentAnalysis.DifferentTSSs(loci1, loci2, "condition1", "condition2")
        End Function

#Region "Header helpers"

        Private Function indexOf(headers As String(), name As String) As Integer
            For i As Integer = 0 To headers.Length - 1
                If String.Equals(headers(i).Trim(), name, System.StringComparison.OrdinalIgnoreCase) Then Return i
            Next
            Return -1
        End Function

        Private Function firstIndexOf(headers As String(), prefix As String) As Integer
            For i As Integer = 0 To headers.Length - 1
                If headers(i).Trim().StartsWith(prefix, System.StringComparison.OrdinalIgnoreCase) Then Return i
            Next
            Return -1
        End Function

        Private Function allIndexesOf(headers As String(), prefix As String) As Integer()
            Dim list As New List(Of Integer)()
            For i As Integer = 0 To headers.Length - 1
                If headers(i).Trim().StartsWith(prefix, System.StringComparison.OrdinalIgnoreCase) Then list.Add(i)
            Next
            Return list.ToArray()
        End Function

        Private Function tokenAt(tokens As String(), index As Integer) As String
            If index >= 0 AndAlso index < tokens.Length Then Return tokens(index).Trim()
            Return ""
        End Function

#End Region

    End Module

End Namespace
