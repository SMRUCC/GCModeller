#Region "Microsoft.VisualBasic::8664ea7ba12da6040a41aa7cbf95d613, ..\interops\meme_suite\MEME\Workflows\PromoterParser\GenePromoterParser.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Workflows.PromoterParser

    ''' <summary>
    ''' 直接从基因的启动子区选取序列数据以及外加操纵子的第一个基因的启动子序列
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <[PackageNamespace]("Parser.Gene.Promoter", Publisher:="xie.guigang@gmail.com")>
    Public Class GenePromoterParser : Inherits Workflows.PromoterParser.PromoterParser
        Implements System.IDisposable

        ''' <summary>
        ''' 基因组的Fasta核酸序列
        ''' </summary>
        ''' <param name="Fasta"></param>
        ''' <remarks></remarks>
        Sub New(Fasta As FASTA.FastaToken, PTT As PTT)
            Dim GenomeSeq As SegmentReader = New SegmentReader(Fasta, LinearMolecule:=False)

            Promoter_100 = CreateObject(100, PTT, GenomeSeq)
            Promoter_150 = CreateObject(150, PTT, GenomeSeq)
            Promoter_200 = CreateObject(200, PTT, GenomeSeq)
            Promoter_250 = CreateObject(250, PTT, GenomeSeq)
            Promoter_300 = CreateObject(300, PTT, GenomeSeq)
            Promoter_350 = CreateObject(350, PTT, GenomeSeq)
            Promoter_400 = CreateObject(400, PTT, GenomeSeq)
            Promoter_450 = CreateObject(450, PTT, GenomeSeq)
            Promoter_500 = CreateObject(500, PTT, GenomeSeq)
        End Sub

        Sub New(genome As PTTDbLoader)
            Call Me.New(genome.GenomeFasta, genome.ORF_PTT)
        End Sub

        ''' <summary>
        ''' 解析出所有基因前面的序列片段
        ''' </summary>
        ''' <param name="Length"></param>
        ''' <param name="PTT"></param>
        ''' <param name="GenomeSeq"></param>
        ''' <returns></returns>
        Private Shared Function CreateObject(Length As Integer, PTT As PTT, GenomeSeq As SegmentReader) As Dictionary(Of String, FASTA.FastaToken)
            Dim LQuery = (From gene As ComponentModels.GeneBrief
                          In PTT.GeneObjects.AsParallel
                          Select gene.Synonym,
                              Promoter = GetFASTA(gene, GenomeSeq, Length)).ToArray
            Dim DictData As Dictionary(Of String, FASTA.FastaToken) =
                LQuery.ToDictionary(Function(obj) obj.Synonym,
                                    Function(obj) obj.Promoter)
            Return DictData
        End Function

        Private Shared Function GetFASTA(Gene As ComponentModels.GeneBrief, GenomeSeq As SegmentReader, SegmentLength As Integer) As FASTA.FastaToken
            Dim Location As NucleotideLocation = Gene.Location

            Call Location.Normalization()

            If Location.Strand = Strands.Forward Then
                Location = New NucleotideLocation(Location.Left - SegmentLength, Location.Left)  ' 正向序列是上游，无需额外处理
            Else
                Location = New NucleotideLocation(Location.Right, Location.Right + SegmentLength, ComplementStrand:=True)  '反向序列是下游，需要额外小心
            End If

            Dim PromoterFsa As FASTA.FastaToken = New FASTA.FastaToken With {
                .Attributes = New String() {Gene.Synonym},
                .SequenceData = GenomeSeq.TryParse(Location).SequenceData
            }

            Return PromoterFsa
        End Function

        <ExportAPI("Promoter.New", Info:="Create a new promoter sequence parser.")>
        Public Shared Function CreateObject(Fasta As FASTA.FastaToken, PTT As PTT) As GenePromoterParser
            Return New GenePromoterParser(Fasta, PTT)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="locus">需要进行解析的基因的编号的列表</param>
        ''' 
        <ExportAPI("Locus.Promoters")>
        Public Shared Sub ParsingList(Parser As GenePromoterParser, DOOR As String, locus As IEnumerable(Of String), EXPORT As String)
            Call ParsingList(Parser, DOOR_API.Load(DOOR), locus, EXPORT)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="locus">需要进行解析的基因的编号的列表</param>
        ''' 
        <ExportAPI("Locus.Promoters")>
        Public Shared Sub ParsingList(Parser As GenePromoterParser,
                                      DOOR As DOOR,
                                      locus As IEnumerable(Of String),
                                      EXPORT As String,
                                      Optional tag As String = "",
                                      Optional method As GetLocusTags = GetLocusTags.UniDOOR)
            Dim GetDOORUni As IGetLocusTag = ParserLocus.CreateMethod(DOOR, method)
            Dim Genes As String() = (From gene As String
                                     In locus
                                     Select GetDOORUni(gene)).MatrixAsIterator.Distinct.ToArray
            Dim SaveFasta = Sub(src As Dictionary(Of String, FASTA.FastaToken), len As Integer)
                                Dim Path As String
                                If String.IsNullOrEmpty(tag) Then
                                    Path = $"{EXPORT}/{len}bp.fasta"
                                Else
                                    Path = $"{EXPORT}/{len}/{tag}.fasta"
                                End If

                                Dim Seqs = Genes.ToArray(Function(id) src(id)).ToList
                                Call New FASTA.FastaFile(Seqs).Save(-1, Path, Encodings.ASCII)
                            End Sub

            Call SaveFasta(Parser.Promoter_100, 100)
            Call SaveFasta(Parser.Promoter_150, 150)
            Call SaveFasta(Parser.Promoter_200, 200)
            Call SaveFasta(Parser.Promoter_250, 250)
            Call SaveFasta(Parser.Promoter_300, 300)
            Call SaveFasta(Parser.Promoter_350, 350)
            Call SaveFasta(Parser.Promoter_400, 400)
            Call SaveFasta(Parser.Promoter_450, 450)
            Call SaveFasta(Parser.Promoter_500, 500)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="modsDIR">包含有KEGG Modules的文件夹</param>
        ''' 
        <ExportAPI("KEGG_Modules.Promoters")>
        Public Shared Sub ParsingKEGGModules(Parser As GenePromoterParser,
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
                Dim Genes As String() = (From gene As String In [mod].GetPathwayGenes Select GetDOORUni(gene)).MatrixAsIterator.Distinct.ToArray
                Dim SaveFasta = Sub(src As Dictionary(Of String, FASTA.FastaToken), len As Integer)
                                    Dim Path As String = $"{EXPORT}/{len}/{[mod].EntryId}.fasta"
                                    Dim Seqs = Genes.ToArray(Function(id) src(id)).ToList
                                    Call New FASTA.FastaFile(Seqs).Save(-1, Path, System.Text.Encoding.ASCII)
                                End Sub

                Call SaveFasta(Parser.Promoter_100, 100)
                Call SaveFasta(Parser.Promoter_150, 150)
                Call SaveFasta(Parser.Promoter_200, 200)
                Call SaveFasta(Parser.Promoter_250, 250)
                Call SaveFasta(Parser.Promoter_300, 300)
                Call SaveFasta(Parser.Promoter_350, 350)
                Call SaveFasta(Parser.Promoter_400, 400)
                Call SaveFasta(Parser.Promoter_450, 450)
                Call SaveFasta(Parser.Promoter_500, 500)

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
        Public Shared Sub ParsingKEGGPathways(Parser As GenePromoterParser,
                                              DOOR As String,
                                              PathwaysDIR As String,
                                              EXPORT As String,
                                              Optional method As GetLocusTags = GetLocusTags.UniDOOR)

            Dim Modules As bGetObject.Pathway() =
               FileIO.FileSystem.GetFiles(PathwaysDIR, FileIO.SearchOption.SearchAllSubDirectories, "*.xml") _
                   .ToArray(Function(xml) xml.LoadXml(Of bGetObject.Pathway))
            Dim DoorOperon = DOOR_API.Load(DOOR)
            Dim GetDOORUni As IGetLocusTag = ParserLocus.CreateMethod(DoorOperon, method)
            Dim prefix As String = IO.Path.GetFileNameWithoutExtension(EXPORT)

            For Each [mod] As bGetObject.Pathway In Modules
                Dim Genes As String() = (From gene As String In [mod].GetPathwayGenes Select GetDOORUni(gene)).MatrixAsIterator.Distinct.ToArray
                Dim SaveFasta = Sub(src As Dictionary(Of String, FASTA.FastaToken), len As Integer)
                                    Dim Path As String = $"{EXPORT}/{prefix}-{len}/{[mod].EntryId}.fasta"
                                    Dim Seqs = (From id As String In Genes Where src.ContainsKey(id) Select src(id)).ToList  ' 由于会存在有RNA基因，所以这里需要额外注意一下
                                    Call New FASTA.FastaFile(Seqs).Save(-1, Path, System.Text.Encoding.ASCII)
                                End Sub

                Call SaveFasta(Parser.Promoter_100, 100)
                Call SaveFasta(Parser.Promoter_150, 150)
                Call SaveFasta(Parser.Promoter_200, 200)
                Call SaveFasta(Parser.Promoter_250, 250)
                Call SaveFasta(Parser.Promoter_300, 300)
                Call SaveFasta(Parser.Promoter_350, 350)
                Call SaveFasta(Parser.Promoter_400, 400)
                Call SaveFasta(Parser.Promoter_450, 450)
                Call SaveFasta(Parser.Promoter_500, 500)

                Call Console.Write(".")
            Next
        End Sub

        <ExportAPI("Read.Csv.DESeq")>
        Public Shared Function LoadDESeqResult(path As String) As DESeq2.ResultData()
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
        Public Shared Function DiffExpressionPromoters(Promoter As GenePromoterParser, DESeq As IEnumerable(Of DESeq2.ResultData),
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

            Dim Statics = {New GeneIDList("DiffEntiredIDList", DiffEntiredIDList), New GeneIDList("DiffUpIDList", DiffUpIDList), New GeneIDList("DiffDownIDList", DiffDownIDList),
                New GeneIDList("IdenticalEntiredIDList", IdenticalEntiredIDList),
                New GeneIDList("IdenticalHighLevel", IdenticalHighLevel),
                New GeneIDList("IdenticalLowLevel", IdenticalLowLevel),
                New GeneIDList("IdenticalUltraLowlevel", IdenticalUltraLowlevel),
                New GeneIDList("IdenticalNormalLevel", IdenticalNormalLevel)}.ToList

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

        Private Shared Function AlwaysHighLevel(Gene As DESeq2.ResultData) As Boolean
            Dim LQuery = (From value In Gene.dataExpr0.Values Where value >= 6 Select 1).ToArray
            Return LQuery.Count = Gene.dataExpr0.Count
        End Function

        Private Shared Function AlwaysLowLevel(Gene As DESeq2.ResultData) As Boolean
            Dim LQuery = (From value In Gene.dataExpr0.Values Where value = 2 Select 1).ToArray
            Return LQuery.Count = Gene.dataExpr0.Count
        End Function

        Private Shared Function AlwaysUltraLowLevel(Gene As DESeq2.ResultData) As Boolean
            Dim LQuery = (From value In Gene.dataExpr0.Values Where value = 1 Select 1).ToArray
            Return LQuery.Count = Gene.dataExpr0.Count
        End Function

        Private Shared Function AlwaysNormalLevel(Gene As DESeq2.ResultData) As Boolean
            Dim LQuery = (From value In Gene.dataExpr0.Values Where value < 6 AndAlso value > 2 Select 1).ToArray
            Return LQuery.Count = Gene.dataExpr0.Count
        End Function

        ''' <summary>
        ''' 等级映射只能够在相同的实验条件下得到的样本之中进行操作
        ''' </summary>
        ''' <param name="DESeq"></param>
        ''' <returns></returns>
        Private Shared Function GenerateExpressionLevelMappings(DESeq As IEnumerable(Of DESeq2.ResultData)) As DESeq2.ResultData()
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

        Public Function GetSequenceById(lstId As IEnumerable(Of String), <Parameter("Len")> Length As Integer) As FASTA.FastaFile
            Return GetSequenceById(Me, lstId, Length)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Promoter"></param>
        ''' <param name="idList"></param>
        ''' <param name="Length"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Get.Sequence.By.Id")>
        Public Shared Function GetSequenceById(Promoter As GenePromoterParser,
                                               <Parameter("id.list", "The gene id list.")> idList As IEnumerable(Of String),
                                               <Parameter("Len")> Optional Length As Integer = 150) As FASTA.FastaFile
            If Not ContainsLength(Length) Then
                Call $"The promoter region length {Length} is not valid, using default value is 150bp.".__DEBUG_ECHO
                Length = 150
            End If

            Dim ListData = idList.ToList

            Select Case Length
                Case 100
                    Return CType((From Fasta In Promoter.Promoter_150.AsParallel Where ListData.IndexOf(Fasta.Key) > -1 Select Fasta.Value).ToArray, SMRUCC.genomics.SequenceModel.FASTA.FastaFile)
                Case 150
                    Return CType((From Fasta In Promoter.Promoter_150.AsParallel Where ListData.IndexOf(Fasta.Key) > -1 Select Fasta.Value).ToArray, SMRUCC.genomics.SequenceModel.FASTA.FastaFile)
                Case 200
                    Return CType((From Fasta In Promoter.Promoter_200.AsParallel Where ListData.IndexOf(Fasta.Key) > -1 Select Fasta.Value).ToArray, SMRUCC.genomics.SequenceModel.FASTA.FastaFile)
                Case 250
                    Return CType((From Fasta In Promoter.Promoter_250.AsParallel Where ListData.IndexOf(Fasta.Key) > -1 Select Fasta.Value).ToArray, SMRUCC.genomics.SequenceModel.FASTA.FastaFile)
                Case 300
                    Return CType((From Fasta In Promoter.Promoter_300.AsParallel Where ListData.IndexOf(Fasta.Key) > -1 Select Fasta.Value).ToArray, SMRUCC.genomics.SequenceModel.FASTA.FastaFile)
                Case 350
                    Return CType((From Fasta In Promoter.Promoter_350.AsParallel Where ListData.IndexOf(Fasta.Key) > -1 Select Fasta.Value).ToArray, SMRUCC.genomics.SequenceModel.FASTA.FastaFile)
                Case 400
                    Return CType((From Fasta In Promoter.Promoter_400.AsParallel Where ListData.IndexOf(Fasta.Key) > -1 Select Fasta.Value).ToArray, SMRUCC.genomics.SequenceModel.FASTA.FastaFile)
                Case 450
                    Return CType((From Fasta In Promoter.Promoter_450.AsParallel Where ListData.IndexOf(Fasta.Key) > -1 Select Fasta.Value).ToArray, SMRUCC.genomics.SequenceModel.FASTA.FastaFile)
                Case 500
                    Return CType((From Fasta In Promoter.Promoter_500.AsParallel Where ListData.IndexOf(Fasta.Key) > -1 Select Fasta.Value).ToArray, SMRUCC.genomics.SequenceModel.FASTA.FastaFile)
                Case Else
                    Throw New Exception
            End Select
        End Function

        Private Shared Function ContainsLength(l As Integer) As Boolean
            Return Array.IndexOf(PrefixLength, l) > -1
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overrides Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub
#End Region
    End Class
End Namespace


