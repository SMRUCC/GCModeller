#Region "Microsoft.VisualBasic::795781fc87cffacc4f832ae1456475e6, RNA-Seq\Rockhopper\API\TSSsAnalysis.vb"

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

    '     Module TSSsAnalysis
    ' 
    '         Function: DifferentTSSs, (+2 Overloads) GenerateDoorGene, (+3 Overloads) GenerateDoorOperon, InternalTrimHead, KEGGDifferent
    '                   LoadOperonResult, LoadResult, Parsing5UTR, ParsingPromoterBox, ParsingTSSs
    '                   ParsingTTSs, (+2 Overloads) SubstituteID
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat
Imports LANS.SystemsBiology.Assembly
Imports Microsoft.VisualBasic.Linq
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.ComponentModels

Namespace AnalysisAPI

    ''' <summary>
    ''' 从分析结果之中解析出TSS到ATG的序列，-35区到TSSs的序列进行分析，所生成的Fasta序列文件之中的每一条序列的标题之中的第一个元素都是基因号
    ''' </summary>
    Public Module TSSsAnalysis

        Public Function LoadResult(Path As String) As Transcripts()
            Dim Csv = Microsoft.VisualBasic.DocumentFormat.Csv.Imports(Path, vbTab)
            Dim Head = Csv(0)
            Dim idx = Head.LocateKeyWord("Expression ", False)
            Csv(0)(idx) = "Expression"

            Dim data As Transcripts() = Csv.AsDataSource(Of Transcripts)(False)
            Return data
        End Function

        Public Function LoadOperonResult(Path As String) As Operon()
            Dim Csv = Microsoft.VisualBasic.DocumentFormat.Csv.Imports(Path, vbTab)
            Call Console.WriteLine($"Load datafram from handle {Path.ToFileURL}")
            Dim data As Operon() = Csv.AsDataSource(Of Operon)(False)
            Call Console.WriteLine("Job Done!!")

            Return data
        End Function

        ''' <summary>
        ''' 将Rokhopper的操纵子转换为Door数据库之中的操纵子数据的格式，同时这个函数还会将单个的基因填充进来作为一个操纵子
        ''' </summary>
        ''' <param name="Operons"></param>
        ''' <param name="PTT"></param>
        ''' <returns></returns>
        Public Function GenerateDoorOperon(Operons As Operon(), PTT As LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.PTT) As LANS.SystemsBiology.Assembly.DOOR.DOOR
            Dim LQuery = (From idx As Integer In Operons.Sequence Let Operon = Operons(idx) Select GenerateDoorOperon(idx, Operon, PTT)).ToArray
            Dim DoorOperons = LANS.SystemsBiology.Assembly.DOOR.Reload(LQuery)
            Dim SingleGenes = (From Gene In PTT.GeneObjects.AsParallel Where Not DoorOperons.ContainsGene(Gene.Synonym) Select Gene).ToArray
            Dim SngDoorGeneOperonsLQuery = (From i As Integer In SingleGenes.Sequence Select GenerateDoorOperon(i + DoorOperons.DOOROperonView.Count, SingleGenes(i))).ToArray
            Dim Merged = SngDoorGeneOperonsLQuery.Join(DoorOperons.DOOROperonView.Operons)
            DoorOperons = LANS.SystemsBiology.Assembly.DOOR.Reload(Merged.ToArray)

            Return DoorOperons
        End Function

        Public Function GenerateDoorOperon(Idx As String, Gene As LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief) As LANS.SystemsBiology.Assembly.DOOR.Operon
            Dim Genes = {GenerateDoorGene(Gene)}
            Dim DoorOperon As LANS.SystemsBiology.Assembly.DOOR.Operon =
              New Assembly.DOOR.Operon(Idx, Genes)
            Return DoorOperon
        End Function

        Public Function GenerateDoorOperon(idx As String, Operon As Operon, PTT As PTT) As DOOR.Operon
            Dim setValue = New SetValue(Of DOOR.GeneBrief) <= NameOf(DOOR.GeneBrief.OperonID)
            Dim Genes = (From ID As String
                         In Operon.Genes
                         Select setValue(GenerateDoorGene(ID, PTT), value:=idx)).ToArray
            Dim DoorOperon As DOOR.Operon = New DOOR.Operon(idx, Genes)
            Return DoorOperon
        End Function

        Private Function GenerateDoorGene(gene As GeneBrief) As DOOR.GeneBrief
            Dim DoorGene As New DOOR.GeneBrief With {
                .Synonym = gene.Synonym
            }

            DoorGene.COG_number = gene.COG
            DoorGene.GI = gene.Code
            DoorGene.Length = gene.Length
            DoorGene.Location = gene.Location
            DoorGene.Product = gene.Product

EXIT_:      Return DoorGene
        End Function

        Private Function GenerateDoorGene(ID As String, PTT As PTT) As DOOR.GeneBrief
            Dim Gene = (From GeneObject In PTT.GeneObjects Where String.Equals(GeneObject.Synonym, ID) Select GeneObject).FirstOrDefault

            If Gene Is Nothing Then
                Return New LANS.SystemsBiology.Assembly.DOOR.GeneBrief With {.Synonym = ID}
            Else
                Return GenerateDoorGene(Gene)
            End If
        End Function

        ''' <summary>
        ''' 将文件里面的基因名称替换为基因号
        ''' </summary>
        ''' <returns></returns>
        Public Function SubstituteID(Operons As Operon(), PTT As LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.PTT) As Operon()
            Dim LQuery = (From Operon In Operons.AsParallel Select SubstituteID(Operon, PTT)).ToArray
            Return LQuery
        End Function

        Public Function SubstituteID(Operon As Operon, PTT As LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.PTT) As Operon
            For i As Integer = 0 To Operon.Genes.Count - 1
                Dim ID As String = Operon.Genes(i)
                Dim LQuery = (From Gene In PTT.GeneObjects Where String.Equals(Gene.Gene, ID) Select Gene.Synonym).ToArray
                If Not LQuery.IsNullOrEmpty Then
                    Operon.Genes(i) = LQuery.First
                End If
            Next

            Return Operon
        End Function

        ''' <summary>
        ''' 解析出TSS到ATG之间的序列片段  5'UTR
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="Genome"></param>
        ''' <returns></returns>
        Public Function Parsing5UTR(data As Transcripts(), Genome As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken) As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
            Dim Parser As New LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader(Genome, False)
            Dim LQuery = (From Site As Transcripts In data.AsParallel Where Not (Site.IsRNA OrElse Site.Leaderless OrElse Site.TSSs = 0 OrElse Site.ATG = 0) Select Site).ToArray
            Dim Fasta = (From Site As Transcripts
                         In LQuery
                         Let Loci = Site.Get5UTRLeader(Parser)
                         Where Not Loci Is Nothing AndAlso Loci.Length >= 6
                         Select Loci).ToArray

            Return InternalTrimHead(Fasta)
        End Function

        ''' <summary>
        ''' TSSs位点附近的-5bp到5bp的片段的序列
        ''' </summary>
        ''' <returns></returns>
        Public Function ParsingTSSs(data As Transcripts(), Genome As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken) As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
            Dim Parser As New LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader(Genome, False)
            Dim LQuery = (From Site As Transcripts In data.AsParallel Where Not Site.TSSs = 0 Select Site).ToArray
            Dim Fasta = (From Site As Transcripts
                         In LQuery
                         Let FastaToken = Site.GetTSSLoci(Parser)
                         Where Not FastaToken Is Nothing AndAlso FastaToken.Length >= 6
                         Select FastaToken).ToArray

            Return InternalTrimHead(Fasta)
        End Function

        ''' <summary>
        ''' 解析出TGA到TTS之间的序列片段   3'UTR
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="Genome"></param>
        ''' <returns></returns>
        Public Function ParsingTTSs(data As Transcripts(), Genome As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken) As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
            Dim Parser As New LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader(Genome, False)
            Dim LQuery = (From Site As Transcripts In data.AsParallel Where Not (Site.IsRNA OrElse Site.TGA - Site.TTSs = 0 OrElse Site.TGA = 0 OrElse Site.TTSs = 0) Select Site).ToArray
            Dim Fasta = (From Site As Transcripts
                         In LQuery
                         Let _3UTR = Site.GetTTSsLoci(Parser)
                         Where Not _3UTR Is Nothing AndAlso _3UTR.Length >= 6
                         Select _3UTR).ToArray

            Return InternalTrimHead(Fasta)
        End Function

        ''' <summary>
        ''' 解析出-35区到TSS位点之间的序列片段，实际上是TSS到其上游的-50bp左右的位置
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="Genome"></param>
        ''' <returns></returns>
        Public Function ParsingPromoterBox(data As Transcripts(), Genome As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken) As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
            Dim Parser As New LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader(Genome, False)
            Dim LQuery = (From site As Transcripts In data.AsParallel Where site.TSSs > 0 Select site).ToArray
            Dim FastaFile = (From site As Transcripts
                                 In LQuery
                             Let Fasta = site.GetPromoterBoxLoci(Parser)
                             Where Not Fasta Is Nothing AndAlso Fasta.Length >= 6  '这个筛选条件是由于MEME软件所需求的，进行分析的序列必须是长度大于6bp的
                             Select Fasta).ToArray

            Return InternalTrimHead(FastaFile)
        End Function

        Private Function InternalTrimHead(ByRef Fasta As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken()) As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
            Dim TrimHead = (From obj In Fasta.AsParallel Select ID = obj.Attributes(0), obj Group By ID Into Group).ToArray
            For Each item In TrimHead
                Dim dat = item.Group.ToArray
                If dat.Count > 1 Then
                    For i As Integer = 0 To dat.Count - 1
                        dat(i).obj.SetAttribute(0, dat(i).obj.Attributes(0) & "-" & i + 1)
                    Next
                End If
            Next

            For Each obj In Fasta
                obj.SetAttribute(0, obj.Attributes(0).Replace(" ", "_"))
            Next

            Dim File = CType(Fasta, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile)
            Return File
        End Function

        ''' <summary>
        ''' 判断条件：基因号相同但是TSSs不同则就认为是
        ''' </summary>
        ''' <param name="condition1"></param>
        ''' <param name="condition2"></param>
        ''' <returns></returns>
        Public Function DifferentTSSs(condition1 As Transcripts(),
                                      condition2 As Transcripts(),
                                      KEGG As LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Pathway()) As TSSsDifferent()
            Dim c2Group = (From Transcript As Transcripts
                           In condition2.AsParallel
                           Where Not String.Equals(Transcript.Synonym, "predicted RNA", StringComparison.OrdinalIgnoreCase)
                           Select Transcript).ToArray
            Dim c2Hash = c2Group.ToDictionary(Function(Transcript) Transcript.Synonym)
            Dim LQuery = (From Transcript As Transcripts In condition1.AsParallel
                          Where c2Hash.ContainsKey(Transcript.Synonym) Select c1 = Transcript, c2 = c2Hash(Transcript.Synonym)).ToArray
            Dim Diff = (From Paired In LQuery.AsParallel
                        Where Not (Paired.c1.TSSs = Paired.c2.TSSs OrElse Paired.c1.TSSs = 0 OrElse Paired.c2.TSSs = 0)
                        Select New TSSsDifferent With {
                            .GeneID = Paired.c1.Synonym,
                            .TSSs_Condition1 = Paired.c1.TSSs,
                            .TSSs_Condition2 = Paired.c2.TSSs,
                            .Pathway = (From KO As LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Pathway
                                        In KEGG
                                        Where KO.IsContainsGeneObject(Paired.c1.Synonym)
                                        Select KO.EntryId).ToArray}).ToArray.ToDictionary(Function(obj) obj.GeneID)
            'TTS differents
            Dim TTSDiff = (From Paired In LQuery.AsParallel Where Not (Paired.c1.TTSs = Paired.c2.TTSs OrElse Paired.c1.TTSs = 0 OrElse Paired.c2.TTSs = 0) Select New TSSsDifferent With {
                            .GeneID = Paired.c1.Synonym,
                            .TTSs_Condition1 = Paired.c1.TTSs,
                            .TTSs_Condition2 = Paired.c2.TTSs,
                            .Pathway = (From KO As LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Pathway
                                        In KEGG
                                        Where KO.IsContainsGeneObject(Paired.c1.Synonym)
                                        Select KO.EntryId).ToArray}).ToArray

            For Each item In TTSDiff
                If Diff.ContainsKey(item.GeneID) Then
                    Dim obj = Diff(item.GeneID)
                    obj.TTSs_Condition1 = item.TTSs_Condition1
                    obj.TTSs_Condition2 = item.TTSs_Condition2
                Else
                    Call Diff.Add(item.GeneID, item)
                End If
            Next

            Return Diff.Values.ToArray
        End Function

        ''' <summary>
        ''' KEGG途径差异性分析
        ''' </summary>
        ''' <param name="TSSs"></param>
        ''' <param name="KEGG"></param>
        ''' <returns></returns>
        Public Function KEGGDifferent(TSSs As TSSsDifferent(),
                                      KEGG As LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Pathway()) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
            Dim data = (From item In TSSs Select IsTSSChnaged = item.TSSChanged, IsTTSChnaged = item.TTSChnaged, IsHaveBoth = item.HaveBoth, Gene = item).ToArray
            Dim HaveBoth = (From item In data Where item.IsHaveBoth Select item).ToArray
            Dim Report As New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
            Dim row As New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject
            Dim PwyClasses = LANS.SystemsBiology.Assembly.KEGG.DBGET.BriteHEntry.Pathway.LoadFromResource
            Dim kegg_dict = KEGG.ToDictionary(
                Function(obj) obj.EntryId,
                              elementSelector:=Function(obj) New With {
                                    .Pathway = obj,
                                    .Class = LANS.SystemsBiology.Assembly.KEGG.DBGET.BriteHEntry.Pathway.GetClass(obj.EntryId, PwyClasses)})

            Call row.AddRange({"Genes have both TSSs & TTSs predicted:", "", "", "", HaveBoth.Count})
            Call Report.Add(row)
            Dim BothChnaged = (From item In HaveBoth Where item.IsTSSChnaged AndAlso item.IsTTSChnaged Select item).ToArray
            row = New DocumentFormat.Csv.DocumentStream.RowObject
            Call row.AddRange({"", "TSSs & TTSs both changed:", "", "", BothChnaged.Count})
            Call Report.Add(row)
            Dim KEGG_ss = (From obj In (From item In BothChnaged Select (From id In item.Gene.Pathway Select id, item.Gene).ToArray).ToArray.MatrixToList Select obj Group obj By obj.id Into Group).ToArray
            For Each item In (From gg In KEGG_ss Select gg Order By gg.Group.Count Descending).ToArray
                row = New DocumentFormat.Csv.DocumentStream.RowObject
                Dim pwy = kegg_dict(item.id)

                Call row.AddRange({"", pwy.Class.Class, pwy.Class.Category, pwy.Pathway.EntryId, item.Group.Count, String.Join("; ", (From fff In item.Group Select fff.Gene.GeneID).ToArray), pwy.Pathway.Name.Split(CChar("-")).First.Trim, pwy.Pathway.Description})
                Call Report.Add(row)
            Next

            Call Report.AppendLine()

            row = New DocumentFormat.Csv.DocumentStream.RowObject
            Dim TSSsChanged = (From item In HaveBoth Where item.IsTSSChnaged AndAlso Not item.IsTTSChnaged Select item).ToArray
            Call row.AddRange({"", "TSSs changed:", "", "", TSSsChanged.Count})
            Call Report.Add(row)
            KEGG_ss = (From obj In (From item In TSSsChanged Select (From id In item.Gene.Pathway Select id, item.Gene).ToArray).ToArray.MatrixToList Select obj Group obj By obj.id Into Group).ToArray
            For Each item In (From gg In KEGG_ss Select gg Order By gg.Group.Count Descending).ToArray
                row = New DocumentFormat.Csv.DocumentStream.RowObject
                Dim pwy = kegg_dict(item.id)

                Call row.AddRange({"", pwy.Class.Class, pwy.Class.Category, pwy.Pathway.EntryId, item.Group.Count, String.Join("; ", (From fff In item.Group Select fff.Gene.GeneID).ToArray), pwy.Pathway.Name.Split(CChar("-")).First.Trim, pwy.Pathway.Description})
                Call Report.Add(row)
            Next


            Call Report.AppendLine()
            row = New DocumentFormat.Csv.DocumentStream.RowObject
            Dim TTSChnaged = (From item In HaveBoth Where item.IsTTSChnaged AndAlso Not item.IsTSSChnaged Select item).ToArray
            Call row.AddRange({"", "TTSs changed:", "", "", TTSChnaged.Count})
            Call Report.Add(row)

            KEGG_ss = (From obj In (From item In TTSChnaged Select (From id In item.Gene.Pathway Select id, item.Gene).ToArray).ToArray.MatrixToList Select obj Group obj By obj.id Into Group).ToArray
            For Each item In (From gg In KEGG_ss Select gg Order By gg.Group.Count Descending).ToArray
                row = New DocumentFormat.Csv.DocumentStream.RowObject
                Dim pwy = kegg_dict(item.id)

                Call row.AddRange({"", pwy.Class.Class, pwy.Class.Category, pwy.Pathway.EntryId, item.Group.Count, String.Join("; ", (From fff In item.Group Select fff.Gene.GeneID).ToArray), pwy.Pathway.Name.Split(CChar("-")).First.Trim, pwy.Pathway.Description})
                Call Report.Add(row)
            Next





            Dim GenesOnlyHasTSSPreicted = (From item In data.AsParallel Where item.Gene.HaveTSSs AndAlso Not item.Gene.HaveTTSs Select item).ToArray
            Call Report.AppendLine()
            row = New DocumentFormat.Csv.DocumentStream.RowObject
            Call row.AddRange({"Genes only have TSSs predicted:", "", "", "", GenesOnlyHasTSSPreicted.Count})
            Call Report.Add(row)
            row = New DocumentFormat.Csv.DocumentStream.RowObject
            TSSsChanged = (From item In GenesOnlyHasTSSPreicted Where item.IsTSSChnaged AndAlso Not item.IsTTSChnaged Select item).ToArray
            Call row.AddRange({"", "TSSs changed:", "", "", TSSsChanged.Count})
            Call Report.Add(row)
            KEGG_ss = (From obj In (From item In TSSsChanged Select (From id In item.Gene.Pathway Select id, item.Gene).ToArray).ToArray.MatrixToList Select obj Group obj By obj.id Into Group).ToArray
            For Each item In (From gg In KEGG_ss Select gg Order By gg.Group.Count Descending).ToArray
                row = New DocumentFormat.Csv.DocumentStream.RowObject
                Dim pwy = kegg_dict(item.id)

                Call row.AddRange({"", pwy.Class.Class, pwy.Class.Category, pwy.Pathway.EntryId, item.Group.Count, String.Join("; ", (From fff In item.Group Select fff.Gene.GeneID).ToArray), pwy.Pathway.Name.Split(CChar("-")).First.Trim, pwy.Pathway.Description})
                Call Report.Add(row)
            Next


            Dim GenesOnlyHasTTSPredicted = (From item In data.AsParallel Where item.Gene.HaveTTSs AndAlso Not item.Gene.HaveTSSs Select item).ToArray
            Call Report.AppendLine()
            row = New DocumentFormat.Csv.DocumentStream.RowObject
            Call row.AddRange({"Genes only have TTSs predicted:", "", "", "", GenesOnlyHasTTSPredicted.Count})
            Call Report.Add(row)
            row = New DocumentFormat.Csv.DocumentStream.RowObject
            TTSChnaged = (From item In GenesOnlyHasTTSPredicted Where item.IsTTSChnaged AndAlso Not item.IsTSSChnaged Select item).ToArray
            Call row.AddRange({"", "TTSs changed:", "", "", TTSChnaged.Count})
            Call Report.Add(row)

            KEGG_ss = (From obj In (From item In TTSChnaged Select (From id In item.Gene.Pathway Select id, item.Gene).ToArray).ToArray.MatrixToList Select obj Group obj By obj.id Into Group).ToArray
            For Each item In (From gg In KEGG_ss Select gg Order By gg.Group.Count Descending).ToArray
                row = New DocumentFormat.Csv.DocumentStream.RowObject
                Dim pwy = kegg_dict(item.id)

                Call row.AddRange({"", pwy.Class.Class, pwy.Class.Category, pwy.Pathway.EntryId, item.Group.Count, String.Join("; ", (From fff In item.Group Select fff.Gene.GeneID).ToArray), pwy.Pathway.Name.Split(CChar("-")).First.Trim, pwy.Pathway.Description})
                Call Report.Add(row)
            Next


            Return Report
        End Function

    End Module
End Namespace
