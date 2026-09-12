' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.

Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

Namespace Assembly.NCBI.GenBank.GBFF

    ''' <summary>
    ''' The genome assembly level of the target genbank record.(目标genbank记录所对应的基因组组装水平)
    ''' </summary>
    ''' <remarks>
    ''' 判定策略参考 complete_genome.md 文档，按照可靠性从高到低组合：
    ''' WGS accession 格式 &gt; 草图关键词(contig/scaffold/draft/shotgun) &gt; 
    ''' 完整信号(Genome Representation :: Full / DEFINITION complete / Finishing Goal) &gt; 
    ''' assembly_gap &gt; /chromosome 限定符。
    ''' </remarks>
    Public Enum GenomeAssemblyLevel

        ''' <summary>
        ''' 无法判定目标记录的组装水平，需要人工核查
        ''' </summary>
        Unknown = 0
        ''' <summary>
        ''' 完整组装：COMMENT 之中声明了 Genome Representation :: Full，或者 DEFINITION 之中
        ''' 声明了 complete genome / complete sequence
        ''' </summary>
        CompleteGenome = 1
        ''' <summary>
        ''' 染色体级组装：并没有明确的 complete 声明，但是存在有 /chromosome 限定符，
        ''' 例如ENA风格的 "genome assembly, chromosome: I" 记录
        ''' </summary>
        ChromosomeLevel = 2
        ''' <summary>
        ''' scaffold 级别的草图组装，包含WGS记录与声明了 scaffold/draft/shotgun 的记录
        ''' </summary>
        Scaffold = 3
        ''' <summary>
        ''' contig 级别的草图组装
        ''' </summary>
        Contig = 4
    End Enum

    ''' <summary>
    ''' 组装水平判定结果的证据描述，用于日志输出与人工核查
    ''' </summary>
    Public Structure AssemblyLevelEvidence

        ''' <summary>
        ''' 判定所得到的组装水平
        ''' </summary>
        ''' <returns></returns>
        Public Property Level As GenomeAssemblyLevel
        ''' <summary>
        ''' 证据来源：contig / scaffold / wgs_accession / wgs_keyword / completeness / 
        ''' genome_representation / definition / finishing_goal / assembly_gap / chromosome / none
        ''' </summary>
        ''' <returns></returns>
        Public Property Source As String
        ''' <summary>
        ''' 所命中的关键词或者限定符名称
        ''' </summary>
        ''' <returns></returns>
        Public Property Hit As String
        ''' <summary>
        ''' 所命中的关键词或者限定符所对应的原始值
        ''' </summary>
        ''' <returns></returns>
        Public Property Value As String

        Public Overrides Function ToString() As String
            If Level = GenomeAssemblyLevel.Unknown Then
                Return "Unknown"
            Else
                Return $"{Level} <{Source}: {Hit}>"
            End If
        End Function
    End Structure

    ''' <summary>
    ''' Helper module for determines that the target genbank record is a complete 
    ''' assembled chromosome genome data or not.
    ''' </summary>
    ''' <remarks>
    ''' (判断目标genbank数据对象是否为一个完整组装的染色体基因组数据，用于泛基因组分析的取样过滤)
    ''' </remarks>
    Public Module AssemblyLevelClassifier

        ''' <summary>
        ''' WGS master accession: 4/6个字母前缀之后跟随一个0以及若干位数字，例如 ``AKBV00000000``
        ''' </summary>
        Private ReadOnly wgsMaster As Regex() = {
            New Regex("^[A-Z]{4}0\d{7}$"),
            New Regex("^[A-Z]{6}0\d{9}$")
        }
        ''' <summary>
        ''' WGS contig/scaffold accession: 4个字母前缀+8位数字，或者6个字母前缀+至少10位数字，
        ''' 例如 ``AKBV01000001``
        ''' </summary>
        Private ReadOnly wgsContig As Regex() = {
            New Regex("^[A-Z]{4}\d{8}$"),
            New Regex("^[A-Z]{6}\d{10,}$")
        }

        Private ReadOnly contigKeys As String() = {"contig"}
        Private ReadOnly scaffoldKeys As String() = {"scaffold", "draft", "shotgun", "whole genome shotgun"}
        Private ReadOnly wgsKeys As String() = {"WGS"}
        Private ReadOnly completeKeys As String() = {"complete genome", "complete sequence"}

        ''' <summary>
        ''' 判断目标genbank对象是否为一个完整组装的染色体基因组数据
        ''' </summary>
        ''' <param name="gbk">目标genbank数据库数据对象</param>
        ''' <returns>
        ''' 只有当目标记录的分子类型为核基因组(<see cref="GenomeMolType.Nuclear"/>)，
        ''' 并且其组装水平为完整组装或者染色体级别的时候，才会返回True。
        ''' 质粒与细胞器基因组(线粒体/叶绿体等)一律返回False。
        ''' </returns>
        ''' <remarks>
        ''' (泛基因组分析一般只关注核基因组的主染色体，所以需要在这里排除掉质粒与细胞器基因组)
        ''' </remarks>
        <Extension>
        Public Function IsCompleteChromosome(gbk As File) As Boolean
            If gbk Is Nothing Then
                Return False
            End If
            If MolTypeClassifier.GetMolType(gbk) <> GenomeMolType.Nuclear Then
                Return False
            End If

            Dim level As GenomeAssemblyLevel = GetAssemblyLevel(gbk)

            Return level = GenomeAssemblyLevel.CompleteGenome OrElse
                   level = GenomeAssemblyLevel.ChromosomeLevel
        End Function

        ''' <summary>
        ''' 判断目标genbank对象所对应的基因组的组装水平
        ''' </summary>
        ''' <param name="gbk">目标genbank数据库数据对象</param>
        ''' <returns></returns>
        <Extension>
        Public Function GetAssemblyLevel(gbk As File) As GenomeAssemblyLevel
            Return GetAssemblyLevelEvidence(gbk).Level
        End Function

        ''' <summary>
        ''' 判断目标genbank对象所对应的基因组的组装水平，同时返回本次判定所依据的证据
        ''' </summary>
        ''' <param name="gbk">目标genbank数据库数据对象</param>
        ''' <returns></returns>
        <Extension>
        Public Function GetAssemblyLevelEvidence(gbk As File) As AssemblyLevelEvidence
            If gbk Is Nothing Then
                Return Evidence(GenomeAssemblyLevel.Unknown, "none", "nothing", "")
            End If

            Dim source As Feature = MolTypeClassifier.GetSourceFeature(gbk)
            Dim header As String = GetHeaderText(gbk, source)
            Dim comment As String = GetComment(gbk)
            Dim accession As String = GetAccession(gbk)
            Dim hit As String

            ' 优先级1：contig 关键词
            hit = MatchAny(header, contigKeys)

            If Not hit Is Nothing Then
                Return Evidence(GenomeAssemblyLevel.Contig, "contig", hit, header)
            End If

            ' 优先级2：scaffold / draft / shotgun 关键词
            hit = MatchAny(header, scaffoldKeys)

            If Not hit Is Nothing Then
                Return Evidence(GenomeAssemblyLevel.Scaffold, "scaffold", hit, header)
            End If

            ' 优先级3：WGS accession 格式(最直接区分WGS与非WGS的证据)
            If IsMatch(accession, wgsMaster) Then
                Return Evidence(GenomeAssemblyLevel.Scaffold, "wgs_accession", "master", accession)
            End If
            If IsMatch(accession, wgsContig) Then
                Return Evidence(GenomeAssemblyLevel.Contig, "wgs_accession", "contig", accession)
            End If

            hit = MatchAny(header, wgsKeys)

            If Not hit Is Nothing Then
                Return Evidence(GenomeAssemblyLevel.Scaffold, "wgs_keyword", hit, header)
            End If

            ' 优先级4：/completeness 限定符声明为 partial 的序列为片段序列
            If Not source Is Nothing Then
                Dim completeness As String = source.Query("completeness")

                If Not String.IsNullOrEmpty(completeness) AndAlso
                    completeness.IndexOf("partial", StringComparison.OrdinalIgnoreCase) >= 0 Then

                    Return Evidence(GenomeAssemblyLevel.Contig, "completeness", "partial", completeness)
                End If
            End If

            ' 优先级5：明确的完整组装信号
            Dim representation As String = StructuredValue(comment, "Genome Representation")

            If Not String.IsNullOrEmpty(representation) AndAlso
                representation.StartsWith("Full", StringComparison.OrdinalIgnoreCase) Then

                Return Evidence(GenomeAssemblyLevel.CompleteGenome, "genome_representation", "Full", representation)
            End If

            hit = MatchAny(header, completeKeys)

            If Not hit Is Nothing Then
                Return Evidence(GenomeAssemblyLevel.CompleteGenome, "definition", hit, header)
            End If

            Dim finishing As String = StructuredValue(comment, "Finishing Goal")

            If Not String.IsNullOrEmpty(finishing) Then
                If finishing.StartsWith("complete", StringComparison.OrdinalIgnoreCase) Then
                    Return Evidence(GenomeAssemblyLevel.CompleteGenome, "finishing_goal", "complete", finishing)
                End If
                If finishing.IndexOf("draft", StringComparison.OrdinalIgnoreCase) >= 0 Then
                    Return Evidence(GenomeAssemblyLevel.Scaffold, "finishing_goal", "draft", finishing)
                End If
            End If

            ' 优先级6：存在有 assembly_gap 说明序列之中还有缺口，并非无gap的完整组装
            Dim gaps As Integer = CountAssemblyGaps(gbk)

            If gaps > 0 Then
                Return Evidence(GenomeAssemblyLevel.Scaffold, "assembly_gap", $"gaps={gaps}", gaps.ToString())
            End If

            ' 优先级7：宽松的染色体级判定，例如ENA风格的 "genome assembly, chromosome: I" 记录
            If Not source Is Nothing Then
                Dim chromosome As String = source.Query("chromosome")

                If Not String.IsNullOrEmpty(chromosome) Then
                    Return Evidence(GenomeAssemblyLevel.ChromosomeLevel, "chromosome", "chromosome", chromosome)
                End If
            End If

            Return Evidence(GenomeAssemblyLevel.Unknown, "none", "none", header)
        End Function

        ''' <summary>
        ''' 统计目标genbank数据之中的 assembly_gap 特征的数量
        ''' </summary>
        ''' <param name="gbk"></param>
        ''' <returns></returns>
        Public Function CountAssemblyGaps(gbk As File) As Integer
            If gbk Is Nothing OrElse gbk.Features Is Nothing Then
                Return 0
            End If

            Dim gaps As Integer = 0

            For Each feature As Feature In gbk.Features
                If feature Is Nothing Then
                    Continue For
                End If
                If String.Equals(feature.KeyName, "assembly_gap", StringComparison.OrdinalIgnoreCase) Then
                    gaps += 1
                End If
            Next

            Return gaps
        End Function

        ''' <summary>
        ''' 获取目标genbank记录之中的结构化注释字段的值
        ''' </summary>
        ''' <param name="comment">COMMENT字段的文本内容</param>
        ''' <param name="key">结构化注释的字段名，例如 ``Genome Representation``</param>
        ''' <returns>假若不存在目标字段，则会返回空值</returns>
        Public Function StructuredValue(comment As String, key As String) As String
            If String.IsNullOrEmpty(comment) Then
                Return Nothing
            End If

            Dim i As Integer = comment.IndexOf(key, StringComparison.OrdinalIgnoreCase)

            If i < 0 Then
                Return Nothing
            End If

            Dim p As Integer = comment.IndexOf("::", i, StringComparison.Ordinal)

            If p < 0 Then
                Return Nothing
            End If

            Dim value As String = comment.Substring(p + 2)
            Dim nextKey As Integer = value.IndexOf("::", StringComparison.Ordinal)
            Dim endBlock As Integer = value.IndexOf("##", StringComparison.Ordinal)
            Dim ends As Integer = -1

            If nextKey >= 0 AndAlso endBlock >= 0 Then
                ends = Math.Min(nextKey, endBlock)
            ElseIf nextKey >= 0 Then
                ends = nextKey
            ElseIf endBlock >= 0 Then
                ends = endBlock
            End If

            If ends >= 0 Then
                value = value.Substring(0, ends)
            End If

            Return value.Trim()
        End Function

        ''' <summary>
        ''' 获取目标genbank记录的COMMENT字段的文本内容
        ''' </summary>
        ''' <param name="gbk"></param>
        ''' <returns>不存在COMMENT字段的时候会返回空字符串</returns>
        Private Function GetComment(gbk As File) As String
            If gbk.Comment Is Nothing Then
                Return ""
            End If

            Return If(gbk.Comment.Comment, "")
        End Function

        ''' <summary>
        ''' 获取目标genbank记录的accession编号，用于WGS格式的正则匹配
        ''' </summary>
        ''' <param name="gbk"></param>
        ''' <returns></returns>
        Private Function GetAccession(gbk As File) As String
            If Not gbk.Locus Is Nothing AndAlso Not String.IsNullOrEmpty(gbk.Locus.AccessionID) Then
                Return gbk.Locus.AccessionID
            End If
            If Not gbk.Accession Is Nothing Then
                Return If(gbk.Accession.AccessionId, "")
            End If

            Return ""
        End Function

        ''' <summary>
        ''' 拼接用于关键词匹配的头部文本：DEFINITION + KEYWORDS + /submitter_seqid + SOURCE
        ''' </summary>
        ''' <param name="gbk"></param>
        ''' <param name="source">目标genbank记录的source feature对象</param>
        ''' <returns></returns>
        Private Function GetHeaderText(gbk As File, source As Feature) As String
            Dim text As New StringBuilder(512)

            If Not gbk.Definition Is Nothing Then
                Call text.Append(gbk.Definition.Value).Append(" "c)
            End If
            If Not gbk.Keywords Is Nothing AndAlso Not gbk.Keywords.KeyWordList Is Nothing Then
                For Each key As String In gbk.Keywords.KeyWordList
                    Call text.Append(key).Append(" "c)
                Next
            End If
            If Not source Is Nothing Then
                Call text.Append(If(source.Query("submitter_seqid"), "")).Append(" "c)
            End If
            If Not gbk.Source Is Nothing AndAlso Not String.IsNullOrEmpty(gbk.Source.SpeciesName) Then
                Call text.Append(gbk.Source.SpeciesName).Append(" "c)
            End If

            Return text.ToString()
        End Function

        ''' <summary>
        ''' 判断目标accession编号是否命中给定的WGS编号格式的正则列表
        ''' </summary>
        Private Function IsMatch(accession As String, patterns As Regex()) As Boolean
            If String.IsNullOrEmpty(accession) Then
                Return False
            End If

            For Each pattern As Regex In patterns
                If pattern.IsMatch(accession) Then
                    Return True
                End If
            Next

            Return False
        End Function

        Private Function MatchAny(text As String, keywords As String()) As String
            If String.IsNullOrEmpty(text) Then
                Return Nothing
            End If

            For Each key As String In keywords
                If text.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0 Then
                    Return key
                End If
            Next

            Return Nothing
        End Function

        Private Function Evidence(level As GenomeAssemblyLevel, source As String, hit As String, value As String) As AssemblyLevelEvidence
            Return New AssemblyLevelEvidence With {
                .Level = level,
                .Source = source,
                .Hit = hit,
                .Value = value
            }
        End Function
    End Module
End Namespace
