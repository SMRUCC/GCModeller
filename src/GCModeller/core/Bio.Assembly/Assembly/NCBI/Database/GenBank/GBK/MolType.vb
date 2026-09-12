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
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

Namespace Assembly.NCBI.GenBank.GBFF

    ''' <summary>
    ''' The molecule type of the target genbank record.(目标genbank记录所描述的分子的类型)
    ''' </summary>
    ''' <remarks>
    ''' 判定策略参考 moltype.md 文档：source 限定符(结构化) &gt; mol_type 语义 &gt; 
    ''' DEFINITION/KEYWORDS 关键词(自由文本兜底) &gt; Unknown。
    ''' 
    ''' 注意 /mol_type 只区分 DNA/RNA，不区分核基因组/质粒/细胞器：INSDC 明确规定 organelle 
    ''' 与 plasmid DNA 也使用 genomic DNA，所以不能够依赖这个限定符做分子定位的判定。
    ''' </remarks>
    Public Enum GenomeMolType

        ''' <summary>
        ''' 无法判定目标记录的分子类型，需要人工核查
        ''' </summary>
        Unknown = 0
        ''' <summary>
        ''' 核基因组：染色体基因组或者WGS组装的基因组序列
        ''' </summary>
        Nuclear = 1
        ''' <summary>
        ''' 质粒基因组
        ''' </summary>
        Plasmid = 2
        ''' <summary>
        ''' 线粒体基因组(含kinetoplast)
        ''' </summary>
        Mitochondrion = 3
        ''' <summary>
        ''' 质体基因组：叶绿体chloroplast / apicoplast / chromatophore / nitroplast等
        ''' </summary>
        Chloroplast = 4
        ''' <summary>
        ''' 其他的细胞器基因组，例如hydrogenosome / nucleomorph等
        ''' </summary>
        OtherOrganelle = 5
        ''' <summary>
        ''' 病毒(含噬菌体)基因组
        ''' </summary>
        ViralGenome = 6
        ''' <summary>
        ''' RNA基因组
        ''' </summary>
        RnaGenomic = 7
        ''' <summary>
        ''' 转录本或者其他非基因组RNA：mRNA/tRNA/rRNA/ncRNA等
        ''' </summary>
        Transcript = 8
    End Enum

    ''' <summary>
    ''' 分子类型判定结果的证据描述，用于日志输出与人工核查
    ''' </summary>
    Public Structure MolTypeEvidence

        ''' <summary>
        ''' 判定所得到的分子类型
        ''' </summary>
        ''' <returns></returns>
        Public Property Type As GenomeMolType
        ''' <summary>
        ''' 证据来源：organelle / plasmid / chromosome / mol_type / keyword / none
        ''' </summary>
        ''' <returns></returns>
        Public Property Source As String
        ''' <summary>
        ''' 所命中的限定符名称或者关键词
        ''' </summary>
        ''' <returns></returns>
        Public Property Hit As String
        ''' <summary>
        ''' 所命中的限定符或者关键词所对应的原始值
        ''' </summary>
        ''' <returns></returns>
        Public Property Value As String

        Public Overrides Function ToString() As String
            If Type = GenomeMolType.Unknown Then
                Return "Unknown"
            Else
                Return $"{Type} <{Source}: {Hit}>"
            End If
        End Function
    End Structure

    ''' <summary>
    ''' A helper module for determines the molecule type of the target genbank record.
    ''' </summary>
    ''' <remarks>
    ''' (依据NCBI genbank数据的source feature的限定符以及头部的DEFINITION/KEYWORDS字段，
    ''' 判断目标基因组数据是属于核基因组，质粒，细胞器基因组还是病毒基因组)
    ''' </remarks>
    Public Module MolTypeClassifier

        ''' <summary>
        ''' 线粒体相关的受控词表与关键词
        ''' </summary>
        Private ReadOnly mitochondria As String() = {"mitochondrion", "mitochondrial", "kinetoplast"}
        ''' <summary>
        ''' 质体相关的受控词表与关键词
        ''' </summary>
        Private ReadOnly plastid As String() = {"chloroplast", "apicoplast", "chromatophore", "nitroplast", "plastid"}
        ''' <summary>
        ''' 质粒关键词
        ''' </summary>
        Private ReadOnly plasmidKeys As String() = {"plasmid"}
        ''' <summary>
        ''' 病毒相关的关键词
        ''' </summary>
        Private ReadOnly virus As String() = {"virus", "viruses", "viral", "phage", "virion", "viroid"}
        ''' <summary>
        ''' 染色体关键词
        ''' </summary>
        Private ReadOnly chromosomeKeys As String() = {"chromosome"}
        ''' <summary>
        ''' 基因组关键词(兜底)
        ''' </summary>
        Private ReadOnly genomeKeys As String() = {"genome"}

        ''' <summary>
        ''' 判断目标genbank对象所描述的分子的类型
        ''' </summary>
        ''' <param name="gbk">目标genbank数据库数据对象</param>
        ''' <returns>返回一个<see cref="GenomeMolType"/>枚举值，用于指示目标genbank数据的分子类型</returns>
        <Extension>
        Public Function GetMolType(gbk As File) As GenomeMolType
            Return GetMolTypeEvidence(gbk).Type
        End Function

        ''' <summary>
        ''' 判断目标genbank对象所描述的分子的类型，同时返回本次判定所依据的证据
        ''' </summary>
        ''' <param name="gbk">目标genbank数据库数据对象</param>
        ''' <returns>
        ''' 除了分子类型之外，还会返回本次判定所命中的限定符或者关键词，便于排查误判
        ''' </returns>
        <Extension>
        Public Function GetMolTypeEvidence(gbk As File) As MolTypeEvidence
            If gbk Is Nothing Then
                Return Evidence(GenomeMolType.Unknown, "none", "nothing", "")
            End If

            Dim source As Feature = GetSourceFeature(gbk)
            Dim organelle As String = Nothing
            Dim plasmid As String = Nothing
            Dim chromosome As String = Nothing
            Dim molType As String = Nothing
            Dim organism As String = Nothing

            If Not source Is Nothing Then
                organelle = source.Query("organelle")
                plasmid = source.Query("plasmid")
                chromosome = source.Query("chromosome")
                molType = source.Query(FeatureQualifiers.mol_type)
                organism = source.Query(FeatureQualifiers.organism)
            End If

            ' 优先级1：/organelle 限定符(官方受控词表，最权威)
            ' 假若同时存在 /organelle 与 /plasmid(例如线粒体质粒)，则以 /organelle 为准
            If Not String.IsNullOrEmpty(organelle) Then
                Dim hit As String = MatchAny(organelle, mitochondria)

                If Not hit Is Nothing Then
                    Return Evidence(GenomeMolType.Mitochondrion, "organelle", hit, organelle)
                End If

                hit = MatchAny(organelle, plastid)

                If Not hit Is Nothing Then
                    Return Evidence(GenomeMolType.Chloroplast, "organelle", hit, organelle)
                End If

                Return Evidence(GenomeMolType.OtherOrganelle, "organelle", "organelle", organelle)
            End If

            ' 优先级2：/plasmid 限定符
            If Not String.IsNullOrEmpty(plasmid) Then
                Return Evidence(GenomeMolType.Plasmid, "plasmid", "plasmid", plasmid)
            End If

            ' 优先级3：/chromosome 限定符
            If Not String.IsNullOrEmpty(chromosome) Then
                Return Evidence(GenomeMolType.Nuclear, "chromosome", "chromosome", chromosome)
            End If

            ' 优先级4：/mol_type 仅能够区分病毒与RNA，无法区分核基因组/质粒/细胞器
            If Not String.IsNullOrEmpty(molType) Then
                Dim hit As String = MatchAny(molType, virus)

                If Not hit Is Nothing Then
                    Return Evidence(GenomeMolType.ViralGenome, "mol_type", hit, molType)
                End If
                If MatchAny(molType, {"genomic rna"}) IsNot Nothing Then
                    Return Evidence(GenomeMolType.RnaGenomic, "mol_type", "genomic rna", molType)
                End If
                If MatchAny(molType, {"rna"}) IsNot Nothing Then
                    Return Evidence(GenomeMolType.Transcript, "mol_type", "rna", molType)
                End If
            End If

            ' 优先级5：DEFINITION/KEYWORDS 自由文本关键词兜底
            ' 例如WGS记录一般都并没有携带/chromosome限定符，只能够依靠DEFINITION行中的描述文本判定
            Dim header As String = GetHeaderText(gbk, organism)
            Dim key As String = MatchAny(header, mitochondria)

            If Not key Is Nothing Then
                Return Evidence(GenomeMolType.Mitochondrion, "keyword", key, header)
            End If

            key = MatchAny(header, plastid)

            If Not key Is Nothing Then
                Return Evidence(GenomeMolType.Chloroplast, "keyword", key, header)
            End If

            key = MatchAny(header, plasmidKeys)

            If Not key Is Nothing Then
                Return Evidence(GenomeMolType.Plasmid, "keyword", key, header)
            End If

            key = MatchAny(header, virus)

            If Not key Is Nothing Then
                Return Evidence(GenomeMolType.ViralGenome, "keyword", key, header)
            End If

            key = MatchAny(header, chromosomeKeys)

            If Not key Is Nothing Then
                Return Evidence(GenomeMolType.Nuclear, "keyword", key, header)
            End If

            key = MatchAny(header, genomeKeys)

            If Not key Is Nothing Then
                Return Evidence(GenomeMolType.Nuclear, "keyword", key, header)
            End If

            Return Evidence(GenomeMolType.Unknown, "none", "none", header)
        End Function

        ''' <summary>
        ''' 目标genbank数据是否为质粒基因组
        ''' </summary>
        ''' <param name="gbk">目标genbank数据库数据对象</param>
        ''' <returns></returns>
        <Extension>
        Public Function IsPlasmidSource(gbk As File) As Boolean
            Return GetMolType(gbk) = GenomeMolType.Plasmid
        End Function

        ''' <summary>
        ''' 目标genbank数据是否为细胞器基因组(线粒体/质体/其他细胞器)
        ''' </summary>
        ''' <param name="gbk">目标genbank数据库数据对象</param>
        ''' <returns></returns>
        <Extension>
        Public Function IsOrganelleSource(gbk As File) As Boolean
            Dim type As GenomeMolType = GetMolType(gbk)

            Return type = GenomeMolType.Mitochondrion OrElse
                   type = GenomeMolType.Chloroplast OrElse
                   type = GenomeMolType.OtherOrganelle
        End Function

        ''' <summary>
        ''' 目标genbank数据是否为核基因组
        ''' </summary>
        ''' <param name="gbk">目标genbank数据库数据对象</param>
        ''' <returns></returns>
        <Extension>
        Public Function IsNuclearSource(gbk As File) As Boolean
            Return GetMolType(gbk) = GenomeMolType.Nuclear
        End Function

        ''' <summary>
        ''' 获取目标genbank数据之中的source feature对象
        ''' </summary>
        ''' <param name="gbk">目标genbank数据库数据对象</param>
        ''' <returns>假若目标genbank数据之中并不存在有source feature，则会返回空值</returns>
        ''' <remarks>
        ''' 这里不能够通过 <see cref="Keywords.FEATURES.FEATURES.source"/> 属性来获取，因为该属性
        ''' 直接取的是内部列表之中的第一个元素，当列表为空的时候会抛出下标越界的错误。
        ''' </remarks>
        Public Function GetSourceFeature(gbk As File) As Feature
            If gbk Is Nothing OrElse gbk.Features Is Nothing Then
                Return Nothing
            End If

            For Each feature As Feature In gbk.Features
                If feature Is Nothing Then
                    Continue For
                End If
                If String.Equals(feature.KeyName, "source", StringComparison.OrdinalIgnoreCase) Then
                    Return feature
                End If
            Next

            Return Nothing
        End Function

        ''' <summary>
        ''' 拼接用于关键词兜底匹配的头部文本：DEFINITION + KEYWORDS + organism/SOURCE
        ''' </summary>
        ''' <param name="gbk"></param>
        ''' <param name="organism">source feature之中的/organism限定符的值</param>
        ''' <returns></returns>
        Private Function GetHeaderText(gbk As File, organism As String) As String
            Dim text As New StringBuilder(512)

            If Not gbk.Definition Is Nothing Then
                Call text.Append(gbk.Definition.Value).Append(" "c)
            End If
            If Not gbk.Keywords Is Nothing AndAlso Not gbk.Keywords.KeyWordList Is Nothing Then
                For Each key As String In gbk.Keywords.KeyWordList
                    Call text.Append(key).Append(" "c)
                Next
            End If
            If Not String.IsNullOrEmpty(organism) Then
                Call text.Append(organism).Append(" "c)
            End If
            If Not gbk.Source Is Nothing AndAlso Not String.IsNullOrEmpty(gbk.Source.SpeciesName) Then
                Call text.Append(gbk.Source.SpeciesName).Append(" "c)
            End If

            Return text.ToString()
        End Function

        ''' <summary>
        ''' 在目标文本之中不区分大小写的匹配给定的关键词列表之中的任意一个关键词
        ''' </summary>
        ''' <param name="text"></param>
        ''' <param name="keywords"></param>
        ''' <returns>返回第一个命中的关键词，假若全部都没有命中，则会返回空值</returns>
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

        Private Function Evidence(type As GenomeMolType, source As String, hit As String, value As String) As MolTypeEvidence
            Return New MolTypeEvidence With {
                .Type = type,
                .Source = source,
                .Hit = hit,
                .Value = value
            }
        End Function
    End Module
End Namespace
