#Region "Microsoft.VisualBasic::7c8578c71d655ca6ac3314b80f1e145c, analysis\RNA-Seq\TSSsTools\Transcriptome.UTRs\TSSsIdentification.vb"

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

    ' Module TSSsIdentification
    ' 
    '     Function: __createObjectLeft, __createObjectRight, (+2 Overloads) Enrichment, LoadTranscripts, MergeHtseq
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.ComponentModel.Loci

<Package("Assembler.TSSs",
                    Publisher:="xie.guigang@gcmodeller.org",
                    Description:="Package for TSSs identification.")>
Public Module TSSsIdentification

    '''' <summary>
    '''' The TSSs sites was raw identified by the 30 5'UTR ends shared reads at least.(按照文献之中的左端共享30条reads以上的的方法来鉴定TSSs)
    '''' </summary>
    '''' <param name="Assembler"></param>
    '''' <param name="Transcripts"></param>
    '''' <param name="sharedThreshold">文献之中的数据是30</param>
    'Public Sub IdentifyTSSs(Assembler As Assembler, ByRef Transcripts As DocumentFormat.Transcript(), sharedThreshold As Integer)

    '    Dim ForwardsReadsBuffer = Assembler.ForwardsReadsBuffer.ToDictionary(Function(obj) obj.TAG, elementSelector:=Function(obj) obj.InitReads)
    '    Dim ReversedReadsBuffer = Assembler.ReversedReadsBuffer.ToDictionary(Function(obj) obj.TAG, elementSelector:=Function(obj) obj.InitReads)

    '    Dim LQuery = (From Transcript As DocumentFormat.Transcript In Transcripts.AsParallel
    '                  Let strand = Transcript.MappingLocation.Strand
    '                  Let readsBuffer = If(strand = SMRUCC.genomics.ComponentModel.Loci.Strands.Forward, ForwardsReadsBuffer, ReversedReadsBuffer)
    '                  Let TSSs = Transcript.TSSs
    '                  Let support = If(readsBuffer.ContainsKey(TSSs), readsBuffer(TSSs), -1)
    '                  Select Transcript.InvokeSet(Of Integer)(NameOf(Transcript.TSSsShared), support) _
    '                                   .InvokeSet(Of Boolean)(NameOf(Transcript.Support), support >= sharedThreshold)).ToArray
    '    Transcripts = LQuery
    'End Sub

    <ExportAPI("TSSs.Shared.Enrichment",
             Info:="Reads shared a common strand specific start site. This function export all of this grouped start site shared data for the downstream TSSs identification.")>
    Public Function Enrichment(mappings As Generic.IEnumerable(Of SMRUCC.genomics.SequenceModel.NucleotideModels.Contig),
                               <Parameter("Shared", "Actually this parameter is not so important, this function just did simply data group operations, 
and the shared number of the start site just lets you have a simple glimp on your data set.")> Optional [shared] As Integer = 30) _
        As DocumentFormat.Transcript()

        Call $"Partitioning Forwards reads....".__DEBUG_ECHO
        Dim Forwards = (From reads In mappings.AsParallel Where reads.MappingLocation.Strand = Strands.Forward Select reads).ToArray
        Call $"Partitioning Reversed reads.....".__DEBUG_ECHO
        Dim Reversed = (From reads In mappings.AsParallel Where reads.MappingLocation.Strand = Strands.Reverse Select reads).ToArray

        Call $"There are {Forwards.Length} reads on forward strand and {Reversed.Length} reads on reversed strand...".__DEBUG_ECHO
        Call $"Start to group alignment reads....".__DEBUG_ECHO
        Dim ForwardGroup = (From Reads In Forwards.AsParallel
                            Select Reads
                            Group Reads By Reads.MappingLocation.Left Into Group).ToArray  'Forwards.ParallelGroup(Of Integer)(Function(Reads) Reads.POS)
        Call $"{NameOf(ForwardGroup)} job done! start to group {Reversed} reads....".__DEBUG_ECHO
        Dim ReversedGroup = (From Reads In Reversed.AsParallel
                             Select Reads
                             Group Reads By Reads.MappingLocation.Right Into Group).ToArray  'Reversed.ParallelGroup(Of Integer)(Function(Reads) Reads.POS)
        Call $"Create transcript object....".__DEBUG_ECHO
        Dim CreateObjectLQuery = (From obj In ForwardGroup.AsParallel
                                  Let array = obj.Group.ToArray
                                  Select __createObjectLeft(array, [shared], array.Length)).AsList
        Call CreateObjectLQuery.AddRange((From obj In ReversedGroup.AsParallel
                                          Let array = obj.Group.ToArray
                                          Select __createObjectRight(array, [shared], array.Length)).ToArray)
        Call "[DONE!]".__DEBUG_ECHO
        Return CreateObjectLQuery.ToArray
    End Function

    <ExportAPI("TSSs.Shared.Enrichment", Info:="Batch mode.")>
    Public Function Enrichment(<Parameter("Dir.Source", "The directory which contains the blastn mapping data set in Csv data file format.")> mappings As String,
                               Optional [Shared] As Integer = 30) As Boolean
        Dim Files = mappings.LoadSourceEntryList({"*.csv"})
        Dim outDir As String = FileIO.FileSystem.GetDirectoryInfo(mappings & "/Enrichment.Out/").FullName
        Dim Scripts = (From file In Files.AsParallel
                       Select script = My.Resources.TSSs_Enrichment.Replace("{reads.csv}", file.Value.CLIPath).Replace("{saved.csv}", $"{outDir}/{file.Key}.csv"),
                                       path = file.Value).ToArray
        Call Settings.Session.Initialize()
        Dim LQuery = (From script
                      In Scripts' 在这里可能会内存占用比较大，故而不适用并行了
                      Select Settings.Session.FolkShoalThread(script.script, script.path)).ToArray
        Return True
    End Function

    Private Function __createObjectRight(array As SMRUCC.genomics.SequenceModel.NucleotideModels.Contig(), [shared] As Integer, Enrichment As Long) As DocumentFormat.Transcript
        Dim MaxLength As Integer = (From obj In array Select obj.MappingLocation.Left).Max
        Dim Deputy = array(Scan0)
        Dim Transcript As New DocumentFormat.Transcript With {
            .Left = MaxLength,
            .Right = Deputy.MappingLocation.Right,
            .Strand = Deputy.MappingLocation.Strand.ToString,
            .TSSsShared = Enrichment,
            .Support = Enrichment >= [shared]}
        Return Transcript
    End Function

    Private Function __createObjectLeft(array As SMRUCC.genomics.SequenceModel.NucleotideModels.Contig(), [shared] As Integer, Enrichment As Long) As DocumentFormat.Transcript
        Dim MaxLength As Integer = (From obj In array Select obj.MappingLocation.Right).ToArray.Max
        Dim Deputy = array(Scan0)
        Dim Transcript As New DocumentFormat.Transcript With {
            .Left = Deputy.MappingLocation.Left,
            .Right = MaxLength,
            .Strand = Deputy.MappingLocation.Strand.ToString,
            .TSSsShared = Enrichment,
            .Support = Enrichment >= [shared]}
        Return Transcript
    End Function

    '<ExportAPI("Expr.Consistency", Info:="Testing if the TSSs site is belong to the gene based on the htseq-count raw result")>
    'Public Function ExprConsistency(data As Generic.IEnumerable(Of DocumentFormat.Transcript),
    '                                <Parameter("Path.htseq", "")> raw As String,
    '                                <Parameter("Path.PTT")> PTT As String,
    '                                <Parameter("Reads.Len", "90 bp in rna-seq dataset")> Optional readsAvgLen As Integer = 90) As DocumentFormat.Transcript()
    '    Dim ExprRaw = SMRUCC.genomics.Toolkits.RNASeq.Assembler.ExprConsistency.Normalize(raw, PTT, readsAvgLen)
    '    Dim sw = Stopwatch.StartNew
    '    Call $"Start expression htseq raw count consistency analysis....".__DEBUG_ECHO
    '    Dim Result = SMRUCC.genomics.Toolkits.RNASeq.Assembler.ExprConsistency.ApplyProperty(data, ExprRaw)
    '    Call $"Analysis job DONE! ..... {sw.ElapsedMilliseconds}ms.".__DEBUG_ECHO
    '    Return Result
    'End Function

    '<ExportAPI("Expr.Consistency", Info:="Testing if the TSSs site is belong to the gene based on the htseq-count raw result")>
    'Public Function ExprConsistency(data As Generic.IEnumerable(Of DocumentFormat.Transcript),
    '                                <Parameter("htseq-raw", "")> raw As Dictionary(Of String, Integer),
    '                                <Parameter("Path.PTT")> PTT As String,
    '                                <Parameter("Reads.Len", "90 bp in rna-seq dataset")> Optional readsAvgLen As Integer = 90) As DocumentFormat.Transcript()
    '    Dim ExprRaw = SMRUCC.genomics.Toolkits.RNASeq.Assembler.ExprConsistency.Normalize(raw, PTT, readsAvgLen)
    '    Dim sw = Stopwatch.StartNew
    '    Call $"Start expression htseq raw count consistency analysis....".__DEBUG_ECHO
    '    Dim Result = SMRUCC.genomics.Toolkits.RNASeq.Assembler.ExprConsistency.ApplyProperty(data, ExprRaw)
    '    Call $"Analysis job DONE! ..... {sw.ElapsedMilliseconds}ms.".__DEBUG_ECHO
    '    Return Result
    'End Function

    ''' <summary>
    ''' 当进行TSSs的预测的时候，假若数据源是有多个样本合并而来的，则相对应的一致性的检查的htseq-count的计数也应该是这些样本的计数的总和，这个函数提供了将这些计数数据合并在一起的方法
    ''' </summary>
    ''' <param name="Files"></param>
    ''' <returns></returns>
    <ExportAPI("Htseq.Merge")>
    Public Function MergeHtseq(Files As Generic.IEnumerable(Of String)) As Dictionary(Of String, Integer)
        Dim LQuery = (From file As String In Files.AsParallel
                      Let Lines = file.ReadAllLines
                      Select (From line As String In Lines
                              Let Tokens As String() = Strings.Split(line, vbTab)
                              Select ID = Tokens(Scan0), Expr = CInt(Val(Tokens(1)))).ToArray).ToArray.Unlist
        Dim ExprRaw = (From obj In LQuery
                       Select obj
                       Group obj By obj.ID Into Group).ToArray.ToDictionary(Function(obj) obj.ID, elementSelector:=Function(obj) (From nn In obj.Group Select nn.Expr).ToArray.Sum)
        Return ExprRaw
    End Function

    <ExportAPI("Read.Csv.Transcripts")>
    Public Function LoadTranscripts(path As String) As DocumentFormat.Transcript()
        Return path.LoadCsv(Of DocumentFormat.Transcript)(False).ToArray
    End Function
End Module
