#Region "Microsoft.VisualBasic::b9822d4279c7ff5f54ce73b8147df0d4, meme_suite\MEME\Workflows\PromoterParser\GenePromoterRegions.vb"

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



    ' /********************************************************************************/

    ' Summaries:

    '     Module GenePromoterRegions
    ' 
    '         Function: AlwaysHighLevel, AlwaysLowLevel, AlwaysNormalLevel, AlwaysUltraLowLevel, CreateObject
    '                   DiffExpressionPromoters, GenerateExpressionLevelMappings, LoadDESeqResult
    ' 
    '         Sub: ParsingKEGGModules, ParsingKEGGPathways, (+2 Overloads) ParsingList
    '         Class GeneIDList
    ' 
    '             Properties: Count, GeneID, Title
    ' 
    '             Constructor: (+2 Overloads) Sub New
    '             Function: ToString
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Linq.JoinExtensions
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ContextModel.Promoter
Imports SMRUCC.genomics.ContextModel.Promoter.PromoterRegionParser
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.SequenceModel

Namespace Workflows.PromoterParser

    Public Module GenePromoterRegions

        <ExportAPI("Promoter.New", Info:="Create a new promoter sequence parser.")>
        Public Function CreateObject(Fasta As FASTA.FastaSeq, PTT As PTT) As PromoterRegionParser
            Return New PromoterRegionParser(Fasta, PTT)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="locus">需要进行解析的基因的编号的列表</param>
        ''' 
        <ExportAPI("Locus.Promoters")>
        Public Sub ParsingList(Parser As PromoterRegionParser, DOOR As String, locus As IEnumerable(Of String), EXPORT As String)
            Call ParsingList(Parser, DOOR_API.Load(DOOR), locus, EXPORT)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="locus">需要进行解析的基因的编号的列表</param>
        ''' 
        <ExportAPI("Locus.Promoters")>
        Public Sub ParsingList(Parser As PromoterRegionParser,
                                      DOOR As DOOR,
                                      locus As IEnumerable(Of String),
                                      EXPORT As String,
                                      Optional tag As String = "",
                                      Optional method As GetLocusTags = GetLocusTags.UniDOOR)
            Dim GetDOORUni As IGetLocusTag = ParserLocus.CreateMethod(DOOR, method)
            Dim Genes As String() = (From gene As String
                                     In locus
                                     Select GetDOORUni(gene)).IteratesALL.Distinct.ToArray
            Dim SaveFasta = Sub(src As Dictionary(Of String, FASTA.FastaSeq), len As Integer)
                                Dim Path As String
                                If String.IsNullOrEmpty(tag) Then
                                    Path = $"{EXPORT}/{len}bp.fasta"
                                Else
                                    Path = $"{EXPORT}/{len}/{tag}.fasta"
                                End If

                                Call New FASTA.FastaFile(Genes.Select(Function(id) src(id))) _
                                    .Save(-1, Path, Encodings.ASCII)
                            End Sub

            For Each l% In PromoterRegionParser.PrefixLengths
                Call SaveFasta(Parser.GetRegionCollectionByLength(l), l)
            Next
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="modsDIR">包含有KEGG Modules的文件夹</param>
        ''' 
        <ExportAPI("KEGG_Modules.Promoters")>
        Public Sub ParsingKEGGModules(Parser As PromoterRegionParser,
                                             DOOR As String,
                                             modsDIR As String,
                                             EXPORT As String,
                                             Optional method As GetLocusTags = GetLocusTags.UniDOOR)

            Dim Modules As bGetObject.Module() =
                FileIO.FileSystem.GetFiles(modsDIR, FileIO.SearchOption.SearchAllSubDirectories, "*.xml") _
                    .Select(Function(xml) xml.LoadXml(Of bGetObject.Module))
            Dim DoorOperon = DOOR_API.Load(DOOR)
            Dim GetDOORUni As IGetLocusTag = ParserLocus.CreateMethod(DoorOperon, method)

            For Each [mod] As bGetObject.Module In Modules
                Dim Genes As String() = (From gene As String In [mod].GetPathwayGenes Select GetDOORUni(gene)).IteratesALL.Distinct.ToArray
                Dim SaveFasta = Sub(src As Dictionary(Of String, FASTA.FastaSeq), len As Integer)
                                    Dim Path As String = $"{EXPORT}/{len}/{[mod].EntryId}.fasta"
                                    Call New FASTA.FastaFile(Genes.Select(Function(id) src(id))) _
                                        .Save(-1, Path, Encoding.ASCII)
                                End Sub

                For Each l% In PromoterRegionParser.PrefixLengths
                    Call SaveFasta(Parser.GetRegionCollectionByLength(l), l)
                Next

                Call Console.Write(".")
            Next
        End Sub

        ''' <summary>
        ''' 可能包含有RNA基因，故而会很容易导致出错
        ''' </summary>
        ''' <param name="Parser"></param>
        ''' <param name="DOOR"></param>
        ''' <param name="PathwaysDIR"></param>
        ''' <param name="EXPORT"></param>
        <ExportAPI("KEGG_Pathways.Promoters")>
        <Extension>
        Public Sub ParsingKEGGPathways(Parser As PromoterRegionParser,
                                       DOOR$,
                                       PathwaysDIR$,
                                       EXPORT$,
                                       Optional method As GetLocusTags = GetLocusTags.UniDOOR)

            Dim populateID As IGetLocusTag
            Dim prefix As String = BaseName(EXPORT)

            If DOOR.FileExists Then
                populateID = ParserLocus.CreateMethod(DOOR_API.Load(DOOR), method)
            Else
                populateID = ParserLocus.CreateMethod(Nothing, GetLocusTags.locus)
            End If

            Dim modules As bGetObject.Pathway() = OrganismModel _
                .EnumerateModules(handle:=PathwaysDIR) _
                .ToArray
            Dim list = Function([mod] As bGetObject.Pathway)
                           Return From gene As String
                                  In [mod].GetPathwayGenes
                                  Select populateID(gene)
                       End Function

            For Each [mod] As bGetObject.Pathway In modules
                Dim genes$() = list([mod]).IteratesALL.Distinct.ToArray
                Dim saveFasta =
                    Sub(src As Dictionary(Of String, FASTA.FastaSeq), len%)
                        Dim path As String = $"{EXPORT}/{prefix}-{len}/{[mod].EntryId}.fasta"
                        ' 由于会存在有RNA基因，所以这里需要额外注意一下
                        Dim seqs = From id As String
                                   In genes
                                   Where src.ContainsKey(id)
                                   Select src(id)

                        Call New FASTA.FastaFile(seqs).Save(-1, path, Encoding.ASCII)
                    End Sub

                For Each l% In PromoterRegionParser.PrefixLengths
                    Call saveFasta(Parser.GetRegionCollectionByLength(l), l)
                Next

                Call [mod].name.__DEBUG_ECHO
            Next
        End Sub

        <ExportAPI("Read.Csv.DESeq")>
        Public Function LoadDESeqResult(path As String) As DESeq2.ResultData()
            Return path.LoadCsv(Of DESeq2.ResultData)(False).ToArray
        End Function

        ''' <summary>
        ''' 根据DESeq的结果得到启动子区的序列进行MEME分析
        ''' </summary>
        ''' <param name="Promoter"></param>
        ''' <param name="DESeq"></param>
        ''' <param name="EXPORT"></param>
        ''' <returns></returns>
        <ExportAPI("DESeq.Promoters")>
        Public Function DiffExpressionPromoters(Promoter As PromoterRegionParser, DESeq As IEnumerable(Of DESeq2.ResultData),
                                                       <Parameter("Dir.Export")> Optional EXPORT As String = "./") As Boolean
            Dim Diff = (From GeneObject In DESeq.AsParallel Where Math.Abs(GeneObject.log2FoldChange) >= 2 Select GeneObject).ToArray

            Dim DiffEntiredIDList As String() = (From Gene In Diff Select Gene.locus_tag).ToArray
            Dim DiffUpIDList As String() = (From Gene In Diff Where Gene.log2FoldChange > 0 Select Gene.locus_tag).ToArray
            Dim DiffDownIDList As String() = (From Gene In Diff Where Gene.log2FoldChange < 0 Select Gene.locus_tag).ToArray
            Dim DiffUpWithFoldChangeLevels = New Dictionary(Of Integer, String())
            Dim DiffDownWithFoldChnageLevels = New Dictionary(Of Integer, String())

            For Levels As Integer = 2 To 100
                Dim Level As Integer = Levels
                Dim UpBound As Integer = Levels + 1
                Dim ChunkBuffer = (From Gene In Diff Where Gene.log2FoldChange >= Level AndAlso Gene.log2FoldChange < UpBound Select Gene.locus_tag).ToArray

                If Not ChunkBuffer.IsNullOrEmpty Then
                    If ChunkBuffer.Count < 6 Then
                        '假若小于MEME程序所要求的至少6条序列，则将当前的序列合并到上一个level之中
                        Dim LastElement = DiffUpWithFoldChangeLevels.Last
                        ChunkBuffer = LastElement.Value.Join(ChunkBuffer).ToArray
                        DiffUpWithFoldChangeLevels(LastElement.Key) = ChunkBuffer
                    Else
                        Call DiffUpWithFoldChangeLevels.Add(Levels, ChunkBuffer)
                    End If
                End If

                Level *= -1 '请注意，这里是下调
                Dim LowerBound As Integer = Level - 1
                ChunkBuffer = (From Gene In Diff Where Gene.log2FoldChange <= Level AndAlso Gene.log2FoldChange > LowerBound Select Gene.locus_tag).ToArray
                If Not ChunkBuffer.IsNullOrEmpty Then
                    If ChunkBuffer.Count < 6 Then
                        Dim LastElement = DiffDownWithFoldChnageLevels.Last
                        ChunkBuffer = LastElement.Value.Join(ChunkBuffer).ToArray
                        DiffDownWithFoldChnageLevels(LastElement.Key) = ChunkBuffer
                    Else
                        Call DiffDownWithFoldChnageLevels.Add(Level, ChunkBuffer)
                    End If
                End If

            Next


            '先对整个样本进行表达的等级映射
            DESeq = GenerateExpressionLevelMappings(DESeq)

            Dim Identical = (From GeneObject In DESeq.AsParallel Where Math.Abs(GeneObject.log2FoldChange) < 2 Select GeneObject).ToArray
            Dim IdenticalEntiredIDList As String() = (From Gene In Identical Select Gene.locus_tag).ToArray

            '始终高表达的
            Dim IdenticalHighLevel As String() = (From Gene In Identical.AsParallel Where AlwaysHighLevel(Gene) Select Gene.locus_tag).ToArray
            Dim IdenticalLowLevel As String() = (From Gene In Identical.AsParallel Where AlwaysLowLevel(Gene) Select Gene.locus_tag).ToArray
            Dim IdenticalUltraLowlevel As String() = (From Gene In Identical.AsParallel Where AlwaysUltraLowLevel(Gene) Select Gene.locus_tag).ToArray
            Dim IdenticalNormalLevel As String() = (From Gene In Identical.AsParallel Where AlwaysNormalLevel(Gene) Select Gene.locus_tag).ToArray

            If IdenticalHighLevel.Count < 6 Then
                '大多数情况之下恒定高表达的基因数目很少，所以这个时候很可能会小于6，则将其合并到普通表达水平的基因之中
                Call Console.WriteLine($"[DEBUG {Now.ToString}] The genes {String.Join(", ", IdenticalHighLevel)} of high identical value its count '{IdenticalHighLevel.Count }'  was smaller than 6, merge into the normal level gene. ==> {IdenticalNormalLevel.Count }")

                IdenticalNormalLevel = IdenticalNormalLevel.Join(IdenticalHighLevel).ToArray
                IdenticalHighLevel = IdenticalNormalLevel
            End If


            For Each Length As Integer In PromoterRegionParser.PrefixLengths

                Call Console.WriteLine($" >> {Length} bp ......")

                '首先解析出总的
                Call GetSequenceById(Promoter, geneIDs:=DiffEntiredIDList, length:=Length).Save($"{EXPORT}/Diff/Entired_{Length}.fasta")
                Call GetSequenceById(Promoter, geneIDs:=DiffUpIDList, length:=Length).Save($"{EXPORT}/Diff/Up_{Length}.fasta")
                Call GetSequenceById(Promoter, geneIDs:=DiffDownIDList, length:=Length).Save($"{EXPORT}/Diff/Down_{Length}.fasta")

                For Each Level In DiffUpWithFoldChangeLevels
                    Call GetSequenceById(Promoter, geneIDs:=Level.Value, length:=Length).Save($"{EXPORT}/Diff/DiffUpWithFoldChangeLevels_{Level.Key}_{Length}.fasta")
                Next

                For Each Level In DiffDownWithFoldChnageLevels
                    Call GetSequenceById(Promoter, geneIDs:=Level.Value, length:=Length).Save($"{EXPORT}/Diff/DiffDownWithFoldChnageLevels_{Level.Key}_{Length}.fasta")
                Next


                '没有差异性的
                Call GetSequenceById(Promoter, geneIDs:=IdenticalEntiredIDList, length:=Length).Save($"{EXPORT}/Identical/Entired_{Length}.fasta")

                Call GetSequenceById(Promoter, geneIDs:=IdenticalHighLevel, length:=Length).Save($"{EXPORT}/Identical/IdenticalHighLevel_{Length}.fasta")
                Call GetSequenceById(Promoter, geneIDs:=IdenticalLowLevel, length:=Length).Save($"{EXPORT}/Identical/IdenticalLowLevel_{Length}.fasta")
                Call GetSequenceById(Promoter, geneIDs:=IdenticalUltraLowlevel, length:=Length).Save($"{EXPORT}/Identical/IdenticalUltraLowlevel_{Length}.fasta")
                Call GetSequenceById(Promoter, geneIDs:=IdenticalNormalLevel, length:=Length).Save($"{EXPORT}/Identical/IdenticalNormalLevel_{Length}.fasta")
            Next

            Dim Statics = LinqAPI.MakeList(Of GeneIDList) <= {
                New GeneIDList("DiffEntiredIDList", DiffEntiredIDList),
                New GeneIDList("DiffUpIDList", DiffUpIDList),
                New GeneIDList("DiffDownIDList", DiffDownIDList),
                New GeneIDList("IdenticalEntiredIDList", IdenticalEntiredIDList),
                New GeneIDList("IdenticalHighLevel", IdenticalHighLevel),
                New GeneIDList("IdenticalLowLevel", IdenticalLowLevel),
                New GeneIDList("IdenticalUltraLowlevel", IdenticalUltraLowlevel),
                New GeneIDList("IdenticalNormalLevel", IdenticalNormalLevel)
            }

            For Each Level In DiffUpWithFoldChangeLevels
                Call Statics.Add(New GeneIDList($"DiffUpWithFoldChangeLevels, Level={Level.Key}", Level.Value))
            Next
            For Each Level In DiffDownWithFoldChnageLevels
                Call Statics.Add(New GeneIDList($"DiffDownWithFoldChnageLevels, Level={Level.Key}", Level.Value))
            Next

            Call Statics.SaveTo($"{EXPORT}/Statics.csv", False)

            Call Console.WriteLine()
            Call Console.WriteLine("[Job Done!]")

            Return True
        End Function

        Public Class GeneIDList

            Public Property Title As String
            Public Property Count As Integer
            Public Property GeneID As String()

            Sub New()
            End Sub

            Sub New(Title As String, GeneID As String())
                Me.Title = Title
                Me.Count = GeneID.Count
                Me.GeneID = GeneID
            End Sub

            Public Overrides Function ToString() As String
                Return $"{Count} genes in '{Title}'"
            End Function
        End Class

        Private Function AlwaysHighLevel(Gene As DESeq2.ResultData) As Boolean
            Dim LQuery = (From value In Gene.dataExpr0.Values Where value >= 6 Select 1).ToArray
            Return LQuery.Count = Gene.dataExpr0.Count
        End Function

        Private Function AlwaysLowLevel(Gene As DESeq2.ResultData) As Boolean
            Dim LQuery = (From value In Gene.dataExpr0.Values Where value = 2 Select 1).ToArray
            Return LQuery.Count = Gene.dataExpr0.Count
        End Function

        Private Function AlwaysUltraLowLevel(Gene As DESeq2.ResultData) As Boolean
            Dim LQuery = (From value In Gene.dataExpr0.Values Where value = 1 Select 1).ToArray
            Return LQuery.Count = Gene.dataExpr0.Count
        End Function

        Private Function AlwaysNormalLevel(Gene As DESeq2.ResultData) As Boolean
            Dim LQuery = (From value In Gene.dataExpr0.Values Where value < 6 AndAlso value > 2 Select 1).ToArray
            Return LQuery.Count = Gene.dataExpr0.Count
        End Function

        ''' <summary>
        ''' 等级映射只能够在相同的实验条件下得到的样本之中进行操作
        ''' </summary>
        ''' <param name="DESeq"></param>
        ''' <returns></returns>
        Private Function GenerateExpressionLevelMappings(DESeq As IEnumerable(Of DESeq2.ResultData)) As DESeq2.ResultData()
            Dim Experiments = DESeq.First.dataExpr0.Keys '从元数据属性之中得到实验条件的列表

            '对每一个实验条件得到映射值，请注意为了保持对象之间的一一对应关系，这里不可以再使用并行化拓展
            Dim LvExpressions = (From Experiment As String In Experiments.AsParallel Select Experiment, Levels = (From GeneObject In DESeq Select GeneObject.dataExpr0(Experiment)).ToArray).ToArray
            Dim LvMappings = (From Experiment In LvExpressions.AsParallel Select Experiment.Experiment, Levels = Experiment.Levels.GenerateMapping()).ToArray

            '将数据回写进入元数据之中
            For Each Experiment In LvMappings
                For i As Integer = 0 To DESeq.Count
                    Dim Gene = DESeq(i)
                    If Not (Gene Is Nothing OrElse Gene.dataExpr0.IsNullOrEmpty) Then
                        Gene.dataExpr0(Experiment.Experiment) = Experiment.Levels(i)
                    Else
                        Try
                            Call $"Null eror??? {Gene.locus_tag} {String.Join(",", (From token In Gene.dataExpr0 Select str = token.ToString).ToArray)}".__DEBUG_ECHO
                        Catch ex As Exception

                        End Try
                    End If
                Next
            Next

            Return DESeq.ToArray
        End Function
    End Module
End Namespace
