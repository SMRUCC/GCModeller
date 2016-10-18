#Region "Microsoft.VisualBasic::d146a548795ca0f4023c4b4a5a7c443e, ..\GCModeller\analysis\SequenceToolkit\SequenceTools\CLI\FastaTools.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
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

Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.ComponentModels
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.FASTA.Reflection
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Partial Module Utilities

    <ExportAPI("/Compare.By.Locis", Usage:="/Compare.By.Locis /file1 <file1.fasta> /file2 </file2.fasta>")>
    <Group(CLIGrouping.FastaTools)>
    Public Function CompareFile(args As CommandLine) As Integer
        Dim f1$ = args("/file1")
        Dim f2$ = args("/file2")
        Dim fa1 As New FastaFile(f1$)
        Dim fa2 As New FastaFile(f2$)

        If fa1.NumberOfFasta <> fa2.NumberOfFasta Then
            Call $"file1:={fa1.NumberOfFasta}, file2:={fa2.NumberOfFasta} is not equals!".Warning
        End If

        Dim f2Dict = (From x In fa2 Let id = x.Attributes.First.Split.First Select x, id Group By id Into Group).ToDictionary(Function(x) x.id)

        For Each x In fa1
            Dim id = x.Attributes.First.Split.First
            If Not f2Dict.ContainsKey(id) Then
                Call $"{x.Title} is not exists in  ""{f2}""".__DEBUG_ECHO
            End If
        Next

        Dim f1Dict = (From x In fa1 Let id = x.Attributes.First.Split.First Select x, id Group By id Into Group).ToDictionary(Function(x) x.id)

        For Each x In fa2
            Dim id = x.Attributes.First.Split.First
            If Not f1Dict.ContainsKey(id) Then
                Call $"{x.Title} is not exists in  ""{f1}""".__DEBUG_ECHO
            End If
        Next

        Return 0
    End Function

    <ExportAPI("/Select.By_Locus",
               Usage:="/Select.By_Locus /in <locus.txt> /fa <fasta/.inDIR> [/out <out.fasta>]")>
    <Group(CLIGrouping.FastaTools)>
    Public Function SelectByLocus(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim fa As String = args("/fa")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-" & fa.BaseName & ".fasta")
        Dim fasta As FastaToken() =
            StreamIterator.SeqSource(fa, {"*.faa", "*.fasta", "*.fsa", "*.fa"}).ToArray
        Dim locus As String() = [in].ReadAllLines

        Call $"Found {fasta.Length} fasta files in source DIR  {fa}".__DEBUG_ECHO

        Dim seqHash As Dictionary(Of String, FastaToken()) =
            (From x As FastaToken
             In fasta
             Let uid As String = x.Attributes.First.Split.First.Trim
             Select x,
                 uid
             Group x By uid Into Group) _
                .ToDictionary(Function(x) x.uid,
                              Function(x) x.Group.ToArray)
        Call $"Files loads {seqHash.Count} sequence...".__DEBUG_ECHO

        Dim LQuery As IEnumerable(Of FastaToken()) = From sId As String
                                                     In locus
                                                     Where seqHash.ContainsKey(sId)
                                                     Select seqHash(sId)
        Dim outFa As New FastaFile(LQuery.MatrixAsIterator)

        Return outFa.Save(out, Encodings.ASCII)
    End Function

    <ExportAPI("/To_Fasta",
               Usage:="/To_Fasta /in <anno.csv> [/out <out.fasta> /attrs <gene;locus_tag;gi;location,...> /seq <Sequence>]",
               Info:="Convert the sequence data in a excel annotation file into a fasta sequence file.")>
    <Group(CLIGrouping.FastaTools)>
    Public Function ToFasta(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".Fasta")
        Dim attrs As String = args("/attrs")
        Dim lstAttrs As String() = If(String.IsNullOrEmpty(attrs), {"gene", "locus_tag", "gi", "location", "product"}, attrs.Split(";"c))
        Dim seq As String = args.GetValue("/seq", "sequence")
        Dim Csv = DocumentStream.DataFrame.CreateObject(DocumentStream.DataFrame.Load(inFile))
        Dim readers = Csv.CreateDataSource
        Dim attrSchema = (From x In Csv.GetOrdinalSchema(lstAttrs) Where x > -1 Select x).ToArray
        Dim seqOrd As Integer = Csv.GetOrdinal(seq)
        Dim Fa As IEnumerable(Of FastaToken) =
            From row As DynamicObjectLoader
            In readers.AsParallel
            Let attributes As String() = row.GetValues(attrSchema)
            Let seqData As String = row.GetValue(seqOrd)
            Let seqFa As FASTA.FastaToken = New FASTA.FastaToken With {
                .Attributes = attributes,
                .SequenceData = seqData
            }
            Select seqFa
        Dim Fasta As New FASTA.FastaFile(Fa)
        Return Fasta.Save(out, Encodings.ASCII).CLICode
    End Function

    <ExportAPI("/Merge.Simple",
               Info:="This tools just merge the fasta sequence into one larger file.",
               Usage:="/Merge.Simple /in <DIR> [/exts <default:*.fasta,*.fa> /line.break 120 /out <out.fasta>]")>
    <GroupAttribute(CLIGrouping.FastaTools)>
    Public Function SimpleMerge(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", inDIR.TrimDIR & ".fasta")
        Dim lineBreak As Integer =
            args.GetValue("/line.break", 120)
        Dim exts As String() =
            args.GetValue("/exts", "*.fasta,*.fa").Split(","c)

        Using writer As StreamWriter = out.OpenWriter(Encodings.ASCII)
            Call Tasks.Parallel.ForEach(
                StreamIterator.SeqSource(inDIR, ext:=exts, debug:=True),
                Sub(fa)
                    Dim line$ = fa.GenerateDocument(lineBreak)
                    SyncLock writer
                        Call writer.WriteLine(line)
                        Call writer.Flush()
                    End SyncLock
                End Sub)

            Return 0
        End Using
    End Function

    <ExportAPI("/Merge",
               Usage:="/Merge /in <fasta.DIR> [/out <out.fasta> /trim /unique /ext <*.fasta> /brief]",
               Info:="Only search for 1 level folder, dit not search receve.")>
    <Group(CLIGrouping.FastaTools)>
    Public Function Merge(args As CommandLine) As Integer
        Dim inDIR As String = args.GetFullDIRPath("/in")
        Dim out As String = args.GetValue("/out", inDIR.TrimDIR & ".fasta")
        Dim ext As String = args("/ext")
        Dim fasta As FASTA.FastaFile
        Dim raw As Boolean = Not args.GetBoolean("/brief")

        If String.IsNullOrEmpty(ext) Then
            fasta = FastaExportMethods.Merge(inDIR, args.GetBoolean("/trim"), raw)
        Else
            fasta = FastaExportMethods.Merge(inDIR, ext, args.GetBoolean("/trim"), raw)
        End If

        If args.GetBoolean("/unique") Then
            Dim Groups = From f As FastaToken
                         In fasta
                         Let sid As String = f.Title.Split.First
                         Select sid,
                             f
                         Group By sid Into Group
            Dim uniques As IEnumerable(Of FastaToken) =
                Groups.Select(Function(x) x.Group.First.f)
            fasta = New FastaFile(uniques)
        End If

        Return fasta.Save(out).CLICode
    End Function

    ''' <summary>
    ''' 取单个片段的方法
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("-segment",
               Usage:="-segment /fasta <Fasta_Token> [-loci <loci>] [/left <left> /length <length> /right <right> [/reverse]] [/ptt <ptt> /geneID <gene_id> /dist <distance> /downstream] -o <saved> [-line.break 100]")>
    <Group(CLIGrouping.FastaTools)>
    Public Function GetSegment(args As CommandLine) As Integer
        Dim FastaFile As String = args("/fasta")
        Dim Loci As String = args("-loci")
        Dim SaveTo As String = args("-o")
        Dim LociData As NucleotideLocation

        If String.IsNullOrEmpty(Loci) Then

            Dim PTT As String = args("/ptt")

            If String.IsNullOrEmpty(PTT) Then

                Dim Left As Integer = args.GetInt32("/left")
                Dim Right As Integer = args.GetInt32("/right")
                Dim Length As Integer = args.GetInt32("/length")
                Dim Reverse As Boolean = args.HavebFlag("/reverse")

                If Length > 0 Then
                    LociData = NucleotideLocation.CreateObject(Left, Length, Reverse)
                Else
                    LociData = New NucleotideLocation(Left, Right, If(Reverse, Strands.Forward, Strands.Reverse))
                End If

            Else

                Dim GeneID As String = args("/geneid")
                Dim Distance As Integer = args.GetInt32("/dist")
                Dim PTTData = TabularFormat.PTT.Load(PTT)
                Dim GeneObject = PTTData.GeneObject(GeneID)
                Dim DownStream As Boolean = args.HavebFlag("/downstream")

                If Not DownStream Then
                    LociData = GeneObject.Location.GetUpStreamLoci(Distance)
                Else
                    LociData = GeneObject.Location.GetDownStream(Distance)
                End If
            End If
        Else
            LociData = LociAPI.TryParse(Loci)
        End If

        Dim SegmentFasta As FASTA.FastaToken = FASTA.FastaToken.Load(FastaFile)

        If SegmentFasta Is Nothing Then
            Return -10
        Else
            SegmentFasta = New NucleotideModels.SegmentReader(SegmentFasta).TryParse(LociData).GetFasta
        End If

        Dim LineBreak As Integer = If(args.ContainsParameter("-line.break", False), args.GetInt32("-line.break"), 100)  ' 默认100个碱基换行

        Return If(SegmentFasta.SaveTo(LineBreak, SaveTo), 0, -1)
    End Function

    <ExportAPI("--segments", Usage:="--segments /regions <regions.csv> /fasta <nt.fasta> [/complement /reversed /brief-dump]")>
    <Argument("/reversed", True, Description:="If the sequence is on the complement strand, reversed it after complement operation?")>
    <Argument("/complement", True,
                          Description:="If this Boolean switch is set on, then all of the reversed strand segment will be complemenet and reversed.")>
    <Argument("/brief-dump", True,
                          Description:="If this parameter is set up true, then only the locus_tag of the ORF gene will be dump to the fasta sequence.")>
    <Group(CLIGrouping.FastaTools)>
    Public Function GetSegments(args As CommandLine) As Integer
        Dim Regions As List(Of SimpleSegment) = args.GetObject("/regions", AddressOf LoadCsv(Of SimpleSegment))
        Dim Fasta As New FASTA.FastaToken(args("/fasta"))
        Dim Reader As New SegmentReader(Fasta)
        Dim Complement As Boolean = args.GetBoolean("/complement")
        Dim reversed As Boolean = args.GetBoolean("/reversed")
        Dim Segments = Regions.ToArray(Function(region) __fillSegment(region, Reader, Complement, reversed))
        Dim briefDump As Boolean = args.GetBoolean("/brief-dump")
        Dim dumpMethod As attrDump = [If](Of attrDump)(briefDump, AddressOf __attrBrief, AddressOf __attrFull)
        Dim input As String = args("/regions").TrimSuffix

        Segments.SaveTo(input & ".sequenceData.csv")

        Dim SequenceFasta = Segments.ToArray(
            Function(segment) New FASTA.FastaToken With {
                    .SequenceData = segment.SequenceData,
                    .Attributes = dumpMethod(segment)})
        Dim Complements As FastaToken() =
            LinqAPI.Exec(Of FastaToken) <= From segment As SimpleSegment
                                           In Segments
                                           Where segment.MappingLocation.Strand <> Strands.Forward
                                           Select New FastaToken With {
                                               .SequenceData = segment.Complement,
                                               .Attributes = dumpMethod(segment)
                                           }
        Dim PTT As PTT = Segments.CreatePTTObject
        PTT.Title = IO.Path.GetFileNameWithoutExtension(args("/fasta"))
        PTT.Size = Fasta.Length

        Call PTT.Save(input & ".ptt")
        Call CType(SequenceFasta, FASTA.FastaFile).Save(input & ".sequenceData.fasta")
        Call CType(Complements, FASTA.FastaFile).Save(input & ".complements.fasta")

        Return 0
    End Function

    Private Delegate Function attrDump(segment As SimpleSegment) As String()

    Private Function __attrFull(segment As NucleotideModels.SimpleSegment) As String()
        Return New String() {segment.ID, segment.MappingLocation.ToString, Len(segment.SequenceData)}
    End Function

    Private Function __attrBrief(segment As NucleotideModels.SimpleSegment) As String()
        Return New String() {segment.ID}
    End Function

    Private Function __fillSegment(region As NucleotideModels.SimpleSegment,
                                   reader As NucleotideModels.SegmentReader,
                                   Complement As Boolean,
                                   Reversed As Boolean) As NucleotideModels.SimpleSegment
        Dim seq As String =
            reader.GetSegmentSequence(region.MappingLocation.Left + 1,
                                      region.MappingLocation.Right + 1)

        If region.MappingLocation.Strand = Strands.Reverse Then
            If Complement Then
                region.Complement = NucleotideModels.NucleicAcid.Complement(seq)
                If Reversed Then
                    region.Complement = New String(region.Complement.Reverse.ToArray)
                End If
            End If

            region.SequenceData = region.Complement
        Else
            region.SequenceData = seq
        End If

        Return region
    End Function

    ''' <summary>
    ''' 假若你的fasta序列里面既有大写字母又有小写字母，并且希望将序列放在一行显示，则可以使用这个方法来统一这些序列的格式
    ''' 这个函数还会删除零长度的序列
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--Trim",
               Usage:="--Trim /in <in.fasta> [/case <u/l> /break <-1/int> /out <out.fasta> /brief]",
               Info:="")>
    <Argument("/case", True,
                   Description:="Adjust the letter case of your sequence, l for lower case and u for upper case. Default value is upper case.")>
    <Argument("/break", True,
                   Description:="Adjust the sequence break when this program write the fasta sequence, default is -1 which means no break, write all sequence in one line.")>
    <Group(CLIGrouping.FastaTools)>
    Public Function Trim(args As CommandLine) As Integer
        Dim Input As String = args("/in")
        Dim UpperCase As Boolean = Not String.Equals("l", args.GetValue("/case", "u"), StringComparison.OrdinalIgnoreCase)
        Dim break As Integer = args.GetValue("/break", -1)
        Dim out As String = args.GetValue("/out", Input.TrimSuffix & "-Trim.fasta")
        Dim Fasta As FastaFile = FastaFile.Read(Input)
        Dim brief As Boolean = args.GetBoolean("/brief")

        Fasta = New FastaFile(From fa As FastaToken
                              In Fasta
                              Where Not String.IsNullOrEmpty(fa.SequenceData.Trim)
                              Select fa) ' 过滤掉零长度的序列

        Dim setSeq = New SetValue(Of FastaToken) <= NameOf(FastaToken.SequenceData)

        If UpperCase Then
            Fasta = New FastaFile(Fasta.ToArray(Function(fa) setSeq(fa, fa.SequenceData.ToUpper)))
        Else
            Fasta = New FastaFile(Fasta.ToArray(Function(fa) setSeq(fa, fa.SequenceData.ToLower)))
        End If

        If brief Then
            Dim setAttrs = New SetValue(Of FastaToken) <= NameOf(FastaToken.Attributes)
            Fasta = New FastaFile(Fasta.ToArray(Function(fa) setAttrs(fa, {fa.Attributes.First})))
        End If

        Return Fasta.Save(break, out, Encoding.ASCII)
    End Function

    <ExportAPI("/subset", Usage:="/subset /lstID <lstID.txt> /fa <source.fasta>")>
    Public Function SubSet(args As CommandLine) As Integer
        Dim lstID As String() = IO.File.ReadAllLines(args("/lstID"))
        Dim fa As New FASTA.FastaFile(args("/fa"))
        Dim LQuery As FASTA.FastaToken() = (From id As String
                                            In lstID
                                            Where Not String.IsNullOrEmpty(id)
                                            Select (From x As FASTA.FastaToken
                                                    In fa
                                                    Where String.Equals(id, x.Title.Split.First, StringComparison.OrdinalIgnoreCase)
                                                    Select x).FirstOrDefault).ToArray
        fa = New FASTA.FastaFile(LQuery)
        Dim path As String = args("/lstID").TrimSuffix & ".subset.fasta"
        Return fa.Save(path).CLICode
    End Function

    <ExportAPI("/Split", Usage:="/Split /in <in.fasta> [/n <4096> /out <outDIR>]")>
    Public Function Split(args As CommandLine) As Integer
        Dim inFa As String = args("/in")
        Dim out As String = args.GetValue("/out", inFa.TrimSuffix & "/")
        Dim n As Long = args.GetValue("/n", 4096L)
        Dim stream As New FASTA.StreamIterator(inFa)
        Dim i As Integer = 0
        Dim name As String = inFa.BaseName

        For Each subFasta As FASTA.FastaFile In stream.Split(n)
            Dim path As String = out & $"/{name}-{ZeroFill(i, 3)}.fasta"
            Call subFasta.Save(path, Encodings.ASCII)
            i += 1
        Next

        Return 0
    End Function

    <ExportAPI("/Get.Locis", Usage:="/Get.Locis /in <locis.csv> /nt <genome.nt.fasta> [/out <outDIR>]")>
    Public Function GetSimpleSegments(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim nt As String = args("/nt")
        Dim out As String = args.GetValue("/out", [in].ParentPath)
        Dim locis As IEnumerable(Of Loci) = [in].LoadCsv(Of Loci)
        Dim parser As New SegmentReader(New FASTA.FastaToken(nt))

        For Each loci In locis
            loci.SequenceData = parser.TryParse(loci.MappingLocation).SequenceData
        Next

        Call locis.SaveTo(out & $"/{[in].BaseName}.Csv")
        Call New FastaFile(From x In locis Select x.ToFasta).Save(out & $"/{[in].BaseName}.fasta")

        Return 0
    End Function

    <ExportAPI("/Distinct",
               Info:="Distinct fasta sequence by sequence content.",
               Usage:="/Distinct /in <in.fasta> [/out <out.fasta> /by_Uid <uid_regexp>]")>
    Public Function Distinct(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".Distinct.fasta")
        Dim fasta As New FASTA.FastaFile([in], {"|"c, "/"c})
        Dim uidRegx As String = args("/by_uid")

        fasta = New FastaFile(From x In fasta Select New FastaToken(x.Attributes, x.SequenceData.Trim("-"c)))

        If Not String.IsNullOrEmpty(uidRegx) Then
            out = out.TrimSuffix & ".unique_uid.fasta"
            Call $"uidRegexp using {uidRegx}".__DEBUG_ECHO

            Dim uids = From fa As FastaToken
                       In fasta
                       Select fa,
                           uid = Regex.Match(fa.Title, uidRegx, RegexICSng).Value
                       Group By uid Into Group
            fasta = New FastaFile(uids.Select(Function(x) New FastaToken({x.uid, x.Group.First.fa.Title}, x.Group.First.fa.SequenceData)).OrderByDescending(Function(fa) fa.Length))
        Else
            Dim uids = From fa As FastaToken
                   In fasta
                       Let seq As String = fa.SequenceData.ToUpper
                       Select fa,
                           seq
                       Group By seq Into Group
            fasta = New FastaFile(From x
                                  In uids
                                  Let fa = x.Group.First
                                  Select New FastaToken(fa.fa.Attributes, fa.seq))
        End If

        Return fasta.Save(out, Encodings.ASCII)
    End Function

    <ExportAPI("/Gff.Sites",
               Usage:="/Gff.Sites /fna <genomic.fna> /gff <genome.gff> [/out <out.fasta>]")>
    Public Function GffSites(args As CommandLine) As Integer
        Dim [in] As String = args("/fna")
        Dim sites As String = args("/gff")
        Dim out As String =
            args.GetValue("/out", [in].TrimSuffix & "-" & sites.BaseName & ".fasta")
        Dim fna = FastaToken.LoadNucleotideData([in])
        Dim gff As GFF = TabularFormat.GFF.LoadDocument(sites)
        Dim nt As New SegmentReader(fna)
        Dim result = From loci As Feature
                     In gff.Features
                     Where (Not loci.attributes.ContainsKey("gbkey")) OrElse
                         (Not String.Equals(loci.attributes("gbkey"), "Src", StringComparison.OrdinalIgnoreCase))
                     Let seq = nt.TryParse(loci.MappingLocation)
                     Select New FastaToken With {
                         .SequenceData = seq.SequenceData,
                         .Attributes = {loci.Synonym, loci.Product, loci.MappingLocation.ToString}
                     }
        Dim fasta As New FastaFile(result)

        Return fasta.Save(-1, out, Encoding.UTF8)
    End Function
End Module

Public Class Loci : Inherits Contig

    Public Property ID As String
    Public Property st As Long
    Public Property sp As Long
    Public Property SequenceData As String

    Public Function ToFasta() As FASTA.FastaToken
        Return New FastaToken({ID, $"{st},{sp}"}, SequenceData)
    End Function

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Protected Overrides Function __getMappingLoci() As NucleotideLocation
        Return New NucleotideLocation(st, sp, Strands.Forward)
    End Function
End Class
