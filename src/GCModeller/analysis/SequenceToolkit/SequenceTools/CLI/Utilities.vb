#Region "Microsoft.VisualBasic::c4c708e02018f8a062eb0952185602f8, analysis\SequenceToolkit\SequenceTools\CLI\Utilities.vb"

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


' Code Statistics:

'   Total Lines: 267
'    Code Lines: 185 (69.29%)
' Comment Lines: 55 (20.60%)
'    - Xml Docs: 58.18%
' 
'   Blank Lines: 27 (10.11%)
'     File Size: 12.66 KB


' Module Utilities
' 
'     Function: Complement, DrawClustalW, FindMotifs, PatternSearchA, PromoterRegionParser_gb
'               Reverse, SequenceLogo
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.ManView
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.SequenceTools
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.Extensions
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel.Promoter
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.FASTA.Reflection
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports std = System.Math

''' <summary>
''' Sequence Utilities
''' </summary>
''' <remarks></remarks>
''' 
<Package("Seqtools.Utilities.CLI",
                  Category:=APICategories.CLI_MAN,
                  Description:="Sequence operation utilities",
                  Publisher:="xie.guigang@gmail.com")>
<ExceptionHelp(Documentation:="https://github.com/smrucc/gcmodeller/src/analysis/",
               EMailLink:="xie.guigang@gcmodeller.org",
               Debugging:="http://gcmodeller.org")>
<GroupingDefine(CLIGrouping.FastaTools, Description:=CLIGrouping.FastaToolsDescription)>
<GroupingDefine(CLIGrouping.PalindromeTools, Description:=CLIGrouping.PalindromeToolsDescription)>
<CLI> Public Module Utilities

    '<ExportAPI("-321",
    '           Info:="Polypeptide sequence 3 letters to 1 lettes sequence.",
    '           Usage:="-321 /in <sequence.txt> [/out <out.fasta>]")>
    'Public Function PolypeptideBriefs(args As CommandLine) As Integer
    '    Dim [In] As String = args.GetString("/in")
    '    Dim Sequence As String = FileIO.FileSystem.ReadAllText([In]).Replace(vbCr, "").Replace(vbLf, "")
    '    Dim AA As New List(Of String)
    '    For i As Integer = 0 To Sequence.Length - 3 Step 3
    '        Call AA.Add(Mid(Sequence, i + 1, 3))
    '    Next
    '    Dim LQuery = New String((From a As String In AA Select Polypeptides.Abbreviate(a.ToUpper).First).ToArray)
    '    Dim MW As Double = CalcMW_Polypeptide(LQuery)
    '    Dim Fasta As New FastaSeq With {.SequenceData = LQuery, .Headers = {$"MW={MW}"}}
    '    Dim out As String = args("/out")
    '    If String.IsNullOrEmpty(out) Then
    '        out = [In] & ".fasta"
    '    End If
    '    Call Fasta.SaveAsOneLine(out)
    '    Return 0
    'End Function

    ''' <summary>
    ''' 进行核酸fasta序列的互补
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("-complement")>
    <Usage("-complement -i <input_fasta> [-o <output_fasta>]")>
    Public Function Complement(args As CommandLine) As Integer
        Dim InputFasta As String = args("-i")

        If String.IsNullOrEmpty(InputFasta) Then
            Call "No fasta sequence was input!".PrintException
        ElseIf Not FileIO.FileSystem.FileExists(InputFasta) Then
            Call $"Fasta file ""{InputFasta}"" is not exisist on your filesystem!".PrintException
        Else
            Dim outFasta As String = args.GetValue("-o", InputFasta.TrimSuffix & "-complement.fasta")
            Return FastaFile.LoadNucleotideData(InputFasta).Complement.Save(outFasta)
        End If

        Return -1
    End Function

    ''' <summary>
    ''' 对蛋白质序列或者核酸序列进行反向
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("-reverse")>
    <Usage("-reverse -i <input_fasta> [-o <output_fasta>]")>
    Public Function Reverse(args As CommandLine) As Integer
        Dim InputFasta As String = args("-i")

        If Not InputFasta.FileExists Then
            Call $"Fasta file ""{InputFasta}"" is not exisist on your filesystem or file empty!".PrintException
        Else
            Dim OutputFasta As String = args.GetValue("-o", InputFasta.TrimSuffix & "_reverse.fsa")
            Return FastaFile.Read(InputFasta).Reverse.Save(OutputFasta)
        End If

        Return -1
    End Function

    ''' <summary>
    ''' Using the regular expression to search the motif pattern on a target nucleotide sequence.(使用正则表达式搜索目标序列)
    ''' </summary>
    ''' <param name="argvs"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("-pattern_search")>
    <Description("Parsing the sequence segment from the sequence source using regular expression.")>
    <Usage("-pattern_search -i <file_name> -p <regex_pattern> [-o <output_directory> -f <format:fsa/gbk>]")>
    <Example("-pattern_search -i ~/xcc8004.txt -p TTA{3}N{1,2} -f fsa")>
    <ArgumentAttribute("-i",
        Description:="The sequence input data source file, it can be a fasta or genbank file.")>
    <ArgumentAttribute("-p",
        Description:="This switch specific the regular expression pattern for search the sequence segment,\n" &
                     "for more detail information about the regular expression please read the user manual.")>
    <ArgumentAttribute("-o", True,
        Description:="Optional, this switch value specific the output directory for the result data, default is user Desktop folder.")>
    <ArgumentAttribute("-f", True,
        Description:="Optional, specific the input file format for the sequence reader, default value is FASTA sequence file.\n" &
                     " fsa - The input sequence data file is a FASTA format file;\n" &
                     " gbk - The input sequence data file is a NCBI genbank flat file.")>
    Public Function PatternSearchA(argvs As CommandLine) As Integer
        Dim Format As String = argvs("-f")
        Dim Input As String = argvs("-i")
        Dim OutputFolder As String = argvs("-o")
        Dim FASTA As FASTA.FastaFile
        Dim pattern As String = argvs("-p").Replace("N", "[ATGCU]")

        If String.IsNullOrEmpty(OutputFolder) Then
            OutputFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        End If

        If String.IsNullOrEmpty(Format) OrElse String.Equals("fsa", Format, StringComparison.OrdinalIgnoreCase) Then 'fasta sequence
            FASTA = FastaFile.Read(file:=Input)
        Else 'gbk format
            Dim GbkFile As GBFF.File = GBFF.File.Load(path:=Input)
            FASTA = GbkFile.ExportProteins
        End If

        Dim File As String = Input.BaseName
        Dim Csv = SequencePatterns.Pattern.PatternSearch.Match(Seq:=FASTA, pattern:=pattern)
        Dim Complement = SequencePatterns.Pattern.PatternSearch.Match(Seq:=FASTA.Complement, pattern:=pattern)
        Dim Reverse = SequencePatterns.Pattern.PatternSearch.Match(Seq:=FASTA.Reverse, pattern:=pattern)
        Dim ComplementReverse = SequencePatterns.Pattern.PatternSearch.Match(Seq:=FASTA.Complement.Reverse, pattern:=pattern)

        Call Csv.Insert(rowId:=-1, Row:={"Match pattern:=", pattern})
        Call Complement.Insert(rowId:=-1, Row:={"Match pattern:=", pattern})
        Call Reverse.Insert(rowId:=-1, Row:={"Match pattern:=", pattern})
        Call ComplementReverse.Insert(rowId:=-1, Row:={"Match pattern:=", pattern})

        Call FileIO.FileSystem.CreateDirectory(OutputFolder)
        Call Csv.Save(OutputFolder & "/" & File & ".csv", False)
        Call Complement.Save(OutputFolder & "/" & File & "_complement.csv", False)
        Call Reverse.Save(OutputFolder & "/" & File & "_reversed.csv", False)
        Call ComplementReverse.Save(OutputFolder & "/" & File & "_complement_reversed.csv", False)

        Return 0
    End Function

    ''' <summary>
    ''' Drawing the sequence logo from the clustal alignment result.
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/logo")>
    <Description("* Drawing the sequence logo from the clustal alignment result.")>
    <Usage("/logo /in <clustal.fasta> [/out <out.png> /title """"]")>
    <ArgumentAttribute("/in", False, Description:="The file path of the clustal output fasta file.", AcceptTypes:={GetType(FastaFile)})>
    <ArgumentAttribute("/out", True, Description:="The output sequence logo image file path. default is the same name as the input fasta sequence file.")>
    <ArgumentAttribute("/title", True, Description:="The display title on the sequence logo, default is using the fasta file name.")>
    Public Function SequenceLogo(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".logo.png")
        Dim title As String = args("/title")
        Dim fa As New FastaFile([in])
        Dim logo As GraphicsData = SequencePatterns.SequenceLogo.DrawFrequency(fa, title)
        Return logo.Save(out)
    End Function

    <ExportAPI("/motifs")>
    <Description("Populate possible motifs from a give nt fasta sequence dataset.")>
    <Usage("/motifs /in <data.fasta> [/min.w <default=6> /max.w <default=20> /n.motifs <default=25> /n.occurs <default=6> /out <out.directory>]")>
    Public Function FindMotifs(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim param As New PopulatorParameter With {
            .maxW = args("/max.w") Or 20,
            .minW = args("/min.w") Or 6,
            .seedingCutoff = 0.95,
            .ScanMinW = 6,
            .ScanCutoff = 0.8
        }
        Dim leastN% = args("/n.occurs") Or 6
        Dim nmotifs% = args("/n.motifs") Or 25
        Dim motifs As SequenceMotif() = FastaFile.LoadNucleotideData([in]) _
            .PopulateMotifs(
                leastN:=leastN,
                param:=param
            ) _
            .OrderByDescending(Function(m) m.score / m.seeds.MSA.Length) _
            .Take(nmotifs) _
            .ToArray
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.motifs/"

        Call motifs _
            .GetJson(indent:=True) _
            .SaveTo(out & "/data.json")
        ' Call motifs.GetXml.SaveTo(out & "/data.xml")

        Dim i As i32 = 0

        For Each motif As SequenceMotif In motifs
            Call motif _
                .CreateDrawingModel _
                .InvokeDrawing(True) _
                .Save($"{out}/motif_{++i}.png")
        Next

        Call param.GetJson.SaveTo(out & "/args.json")

        Return 0
    End Function

    <ExportAPI("--Drawing.ClustalW")>
    <Usage("--Drawing.ClustalW /in <align.fasta> [/out <out.png> /dot.Size 10]")>
    Public Function DrawClustalW(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".png")
        Dim aln As New FASTA.FastaFile(inFile)
        Dim dotSize As Single = args("/dot.size") Or 10
        Dim res As Image = ClustalVisual.InvokeDrawing(aln, dotSize:=dotSize).AsGDIImage
        Return res.SaveAs(out, ImageFormats.Png)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 2017-2-9 经过KEGG测试，sequence cut功能没有问题
    ''' </remarks>
    <ExportAPI("/Promoter.Regions.Parser.gb")>
    <Usage("/Promoter.Regions.Parser.gb /gb <genbank.gb> [/out <out.DIR>]")>
    Public Function PromoterRegionParser_gb(args As CommandLine) As Integer
        Dim gb As String = args <= "/gb"
        Dim gbff As GBFF.File = NCBI.GenBank.GBFF.File.Load(gb)
        Dim nt As FastaSeq = gbff.Origin.ToFasta
        Dim genes = gbff.ExportGeneFeatures
        Dim out As String = args.GetValue("/out", gb.TrimSuffix)
        Dim PTT = gbff.GbffToPTT(ORF:=True)
        Dim parser As New PromoterRegionParser(nt, PTT)

        Call genes.SaveTo(out & "-genes.csv", nonParallel:=True)

        For Each l In parser.PromoterRegions
            Dim save$ = $"{out}-promoter-regions/-{l.Tag}bp.fasta"
            Call New FastaFile(l.Value.Values).Save(120, save, Encodings.ASCII)
        Next

        Return 0
    End Function

    <ExportAPI("/promoter_regions")>
    <Usage("/promoter_regions /nt <nt.fasta> /gff <genomics.gff3> [/upstream_len <default=500bp> /out <default=output.fasta>]")>
    Public Function PromoterRegionParser(args As CommandLine) As Integer
        Dim nt_file As String = args("/nt")
        Dim gff As GFFTable = GFFTable.LoadDocument(args("/gff"))
        Dim upstream_len As Integer = args("/upstream_len") Or 500
        Dim out_file As String = args("/out") Or $"{nt_file.ParentPath}/{nt_file.BaseName}_upstream_{upstream_len}bp.fasta"
        Dim bar As Tqdm.ProgressBar = Nothing

        Using s As Stream = out_file.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False),
            output As New FASTA.StreamWriter(s),
            input As Stream = nt_file.Open(FileMode.Open, doClear:=False, [readOnly]:=True)

            For Each nt As ChunkedNtFasta In ChunkedNtFasta.LoadDocument(input)
                Dim chr_genes As Feature() = (From gene As Feature
                                              In gff.features.AsParallel
                                              Where nt.title = gene.seqname).ToArray

                Call $"load genomics chromosome sequence {nt.ToString}".info

                Try
                    For Each gene As Feature In Tqdm.Wrap(chr_genes, bar:=bar)
                        Dim left As Integer = If(gene.strand = Strands.Forward, gene.left - 1, gene.right + 1)
                        Dim upstream As Integer = If(gene.strand = Strands.Forward, left - upstream_len, left + upstream_len)
                        Dim from = std.Min(left, upstream)
                        Dim [to] = std.Max(left, upstream)
                        Dim seq As String = nt.GetRegion(from, [to])

                        If gene.strand = Strands.Reverse Then
                            seq = NucleicAcid.Complement(seq).Reverse.CharString
                        End If

                        Dim promoter_region As New FastaSeq With {
                            .Headers = {nt.title, gene.ID, $"{from}-{[to]}"},
                            .SequenceData = seq
                        }

                        Call bar.SetLabel(promoter_region.Title)

                        Call output.Add(promoter_region)
                    Next
                Catch ex As Exception
                    Call $"error while processing chromosome {nt.title}".warning
                End Try
            Next
        End Using

        Return 0
    End Function

    <ExportAPI("/motif_scan")>
    <Usage("/motif_scan /in <input.fasta> /motif_db <motifs_db.xml> [/out <default=motif_scan.csv>]")>
    Public Function MotifScan(args As CommandLine) As Integer
        Dim in$ = args("/in")
        Dim motif_file As String = args("/motif_db")
        Dim out As String = args("/out") Or $"{[in].ParentPath}/{[in].BaseName}_vs_{motif_file.BaseName}_motif_scan.csv"
        Dim motifs As SequenceMotif() = motif_file.LoadXml(Of XmlList(Of MotifPWM)) _
            .AsEnumerable _
            .Select(Function(pwm)
                        Return New SequenceMotif With {
                            .tag = pwm.name,
                            .region = pwm.pwm _
                                .Select(Function(p)
                                            Dim pvec = p(pwm.alphabets) _
                                                .ToDictionary(Function(c) c.Key.ToString,
                                                                Function(c)
                                                                    Return c.Value
                                                                End Function)

                                            Return New SequencePatterns.Residue With {
                                                .index = p.site,
                                                .frequency = pvec
                                            }
                                        End Function) _
                                .ToArray
                        }
                    End Function) _
            .ToArray
        Dim target As FastaFile = FastaFile.Read([in])
        Dim scan As MotifMatch() = motifs.AsParallel _
            .Select(Iterator Function(motif) As IEnumerable(Of MotifMatch)
                        For Each seq As FastaSeq In target.AsEnumerable
                            For Each hit As MotifMatch In motif.ScanSites(seq)
                                Yield hit
                            Next
                        Next
                    End Function) _
            .ToArray

        Return scan.SaveTo(out).CLICode
    End Function
End Module
