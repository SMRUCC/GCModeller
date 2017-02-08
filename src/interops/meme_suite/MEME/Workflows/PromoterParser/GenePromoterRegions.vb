Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Mathematical
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.ContextModel.PromoterRegionParser
Imports SMRUCC.genomics.SequenceModel

Namespace Workflows.PromoterParser

    Public Module GenePromoterRegions

        <ExportAPI("Promoter.New", Info:="Create a new promoter sequence parser.")>
        Public Function CreateObject(Fasta As FASTA.FastaToken, PTT As PTT) As PromoterRegionParser
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
            Dim SaveFasta = Sub(src As Dictionary(Of String, FASTA.FastaToken), len As Integer)
                                Dim Path As String
                                If String.IsNullOrEmpty(tag) Then
                                    Path = $"{EXPORT}/{len}bp.fasta"
                                Else
                                    Path = $"{EXPORT}/{len}/{tag}.fasta"
                                End If

                                Call New FASTA.FastaFile(Genes.Select(Function(id) src(id))) _
                                    .Save(-1, Path, Encodings.ASCII)
                            End Sub

            For Each l% In PromoterRegionParser.PrefixLength
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
                    .ToArray(Function(xml) xml.LoadXml(Of bGetObject.Module))
            Dim DoorOperon = DOOR_API.Load(DOOR)
            Dim GetDOORUni As IGetLocusTag = ParserLocus.CreateMethod(DoorOperon, method)

            For Each [mod] As bGetObject.Module In Modules
                Dim Genes As String() = (From gene As String In [mod].GetPathwayGenes Select GetDOORUni(gene)).IteratesALL.Distinct.ToArray
                Dim SaveFasta = Sub(src As Dictionary(Of String, FASTA.FastaToken), len As Integer)
                                    Dim Path As String = $"{EXPORT}/{len}/{[mod].EntryId}.fasta"
                                    Call New FASTA.FastaFile(Genes.Select(Function(id) src(id))) _
                                        .Save(-1, Path, System.Text.Encoding.ASCII)
                                End Sub

                For Each l% In PromoterRegionParser.PrefixLength
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
        Public Sub ParsingKEGGPathways(Parser As PromoterRegionParser,
                                              DOOR As String,
                                              PathwaysDIR As String,
                                              EXPORT As String,
                                              Optional method As GetLocusTags = GetLocusTags.UniDOOR)

            Dim Modules As bGetObject.Pathway() =
               FileIO.FileSystem.GetFiles(PathwaysDIR, FileIO.SearchOption.SearchAllSubDirectories, "*.xml") _
                   .ToArray(Function(xml) xml.LoadXml(Of bGetObject.Pathway))
            Dim DoorOperon = DOOR_API.Load(DOOR)
            Dim GetDOORUni As IGetLocusTag = ParserLocus.CreateMethod(DoorOperon, method)
            Dim prefix As String = BaseName(EXPORT)

            For Each [mod] As bGetObject.Pathway In Modules
                Dim Genes As String() = (From gene As String In [mod].GetPathwayGenes Select GetDOORUni(gene)).IteratesALL.Distinct.ToArray
                Dim SaveFasta = Sub(src As Dictionary(Of String, FASTA.FastaToken), len As Integer)
                                    Dim Path As String = $"{EXPORT}/{prefix}-{len}/{[mod].EntryId}.fasta"
                                    ' 由于会存在有RNA基因，所以这里需要额外注意一下
                                    Call New FASTA.FastaFile(From id As String In Genes Where src.ContainsKey(id) Select src(id)).Save(-1, Path, Encoding.ASCII)
                                End Sub

                For Each l% In PromoterRegionParser.PrefixLength
                    Call SaveFasta(Parser.GetRegionCollectionByLength(l), l)
                Next

                Call Console.Write(".")
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


            For Each Length As Integer In {150, 200, 250, 300, 350, 400, 450, 500}

                Call Console.WriteLine($" >> {Length} bp ......")

                '首先解析出总的
                Call GetSequenceById(Promoter, idList:=DiffEntiredIDList, Length:=Length).Save($"{EXPORT}/Diff/Entired_{Length}.fasta")
                Call GetSequenceById(Promoter, idList:=DiffUpIDList, Length:=Length).Save($"{EXPORT}/Diff/Up_{Length}.fasta")
                Call GetSequenceById(Promoter, idList:=DiffDownIDList, Length:=Length).Save($"{EXPORT}/Diff/Down_{Length}.fasta")

                For Each Level In DiffUpWithFoldChangeLevels
                    Call GetSequenceById(Promoter, idList:=Level.Value, Length:=Length).Save($"{EXPORT}/Diff/DiffUpWithFoldChangeLevels_{Level.Key}_{Length}.fasta")
                Next

                For Each Level In DiffDownWithFoldChnageLevels
                    Call GetSequenceById(Promoter, idList:=Level.Value, Length:=Length).Save($"{EXPORT}/Diff/DiffDownWithFoldChnageLevels_{Level.Key}_{Length}.fasta")
                Next


                '没有差异性的
                Call GetSequenceById(Promoter, idList:=IdenticalEntiredIDList, Length:=Length).Save($"{EXPORT}/Identical/Entired_{Length}.fasta")

                Call GetSequenceById(Promoter, idList:=IdenticalHighLevel, Length:=Length).Save($"{EXPORT}/Identical/IdenticalHighLevel_{Length}.fasta")
                Call GetSequenceById(Promoter, idList:=IdenticalLowLevel, Length:=Length).Save($"{EXPORT}/Identical/IdenticalLowLevel_{Length}.fasta")
                Call GetSequenceById(Promoter, idList:=IdenticalUltraLowlevel, Length:=Length).Save($"{EXPORT}/Identical/IdenticalUltraLowlevel_{Length}.fasta")
                Call GetSequenceById(Promoter, idList:=IdenticalNormalLevel, Length:=Length).Save($"{EXPORT}/Identical/IdenticalNormalLevel_{Length}.fasta")
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