#Region "Microsoft.VisualBasic::49b83db7ca7c36de5916ad63aae9ec21, localblast\CLI_tools\CLI\gbTools.vb"

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

    '   Total Lines: 432
    '    Code Lines: 364 (84.26%)
    ' Comment Lines: 10 (2.31%)
    '    - Xml Docs: 80.00%
    ' 
    '   Blank Lines: 58 (13.43%)
    '     File Size: 20.12 KB


    ' Module CLI
    ' 
    '     Function: __EXPORTgpff, __trimName, AddLocusTag, AddNames, CopyFasta
    '               CopyPTT, exportAnnotatonTable, ExportBlastX, ExportGenbank, ExportGenesFasta
    '               EXPORTgpff, EXPORTgpffs, ExportProt, HitsIDList, MergeFaa
    '               Print
    ' 
    '     Sub: exportTo
    ' 
    ' Class NameAnno
    ' 
    '     Properties: Maximum, Minimum, Name
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.FileIO.Path
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Language.UnixBash.FileSystem
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.BlastX
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports Feature = SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF.Feature
Imports Features = SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF.Features

Partial Module CLI

    <ExportAPI("/Print", Usage:="/Print /in <inDIR> [/ext <ext> /out <out.Csv>]")>
    <Group(CLIGrouping.GenbankTools)>
    Public Function Print(args As CommandLine) As Integer
        Dim ext As String = args.GetValue("/ext", "*.*")
        Dim inDIR As String = args - "/in"
        Dim out As String = args.GetValue("/out", inDIR.TrimDIR & ".contents.Csv")
        Dim files As IEnumerable(Of String) =
            ls - l - r - wildcards(ext) <= inDIR
        Dim content As NamedValue(Of String)() =
            LinqAPI.Exec(Of NamedValue(Of String)) <= From file As String
                                                      In files
                                                      Let name As String = file.BaseName
                                                      Let genome As String = file.ParentDirName
                                                      Select New NamedValue(Of String) With {
                                                          .Name = genome,
                                                          .Value = name
                                                      }
        Return content.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Export.gpff", Usage:="/Export.gpff /in <genome.gpff> /gff <genome.gff> [/out <out.PTT>]")>
    <Group(CLIGrouping.GenbankTools)>
    Public Function EXPORTgpff(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".PTT")
        Dim PTT As PTT = __EXPORTgpff([in], args - "/gff")
        Return PTT.Save(out)
    End Function

    Private Function __EXPORTgpff([in] As String, gffFile As String) As PTT
        VBDebugger.Mute = True

        Dim gpff As IEnumerable(Of GBFF.File) = GBFF.File.LoadDatabase([in])
        Dim gff As GFFTable = GFFTable.LoadDocument(gffFile)
        Dim CDSHash = (From x As Feature
                       In gff.GetsAllFeatures(Features.CDS)
                       Select x
                       Group x By x.proteinId Into Group) _
                            .ToDictionary(Function(x) x.ProteinId,
                                          Function(x) x.Group.First)
        Dim genes As GeneBrief() =
            LinqAPI.Exec(Of GeneBrief) <= From gb As GBFF.File
                                          In gpff
                                          Let ORF As GeneBrief = gb.GPFF2Feature(gff:=CDSHash)
                                          Where Not ORF Is Nothing
                                          Select ORF
        VBDebugger.Mute = False

        Return New PTT(genes, gpff.First.Source.SpeciesName)
    End Function

    <ExportAPI("/Export.gpffs", Usage:="/Export.gpffs [/in <inDIR>]")>
    <Group(CLIGrouping.GenbankTools)>
    Public Function EXPORTgpffs(args As CommandLine) As Integer
        Dim inDIR As String = args.GetValue("/in", App.CurrentDirectory)
        Dim gpffs As IEnumerable(Of String) = ls - l - r - wildcards("*.gpff") <= inDIR
        Dim gffs As IEnumerable(Of String) = ls - l - r - wildcards("*.gff") <= inDIR

        Call $"Found {gpffs.Count} *.gpff and {gffs.Count} *.gff files....".debug

        For Each pair As PathMatch In PathMatch.Pairs(gpffs, gffs, AddressOf __trimName)
            Dim out As String = pair.Pair1.TrimSuffix & ".PTT"

            Try
                Call __EXPORTgpff(pair.Pair1, pair.Pair2).Save(out)
            Catch ex As Exception
                ex = New Exception(out, ex)
                ex = New Exception(pair.GetJson, ex)
                Throw ex
            End Try
        Next

        Return 0
    End Function

    Private Function __trimName(name As String) As String
        If name Is Nothing Then
            Return ""
        Else
            name = Regex.Replace(name, "_genomic", "", RegexICSng)
            name = Regex.Replace(name, "_protein", "", RegexICSng)

            Return name
        End If
    End Function

    <ExportAPI("/Copy.PTT", Usage:="/Copy.PTT /in <inDIR> [/out <outDIR>]")>
    <Group(CLIGrouping.GenbankTools)>
    Public Function CopyPTT(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim EXPORT As String = args("/out") Or (inDIR & "-Copy/")
        Dim PTTs As IEnumerable(Of String) = ls - l - r - "*.ptt" <= inDIR

        For Each file As String In PTTs
            Dim title As String = file.ReadFirstLine
            title = Regex.Replace(title, " [-] \d+\s\.\.\s\d+", "", RegexICSng).Trim
            Dim out As String = EXPORT & $"/{title.NormalizePathString(False)}.PTT"
            file.ReadAllText.SaveTo(out, Encodings.ASCII.CodePage)
        Next

        Return 0
    End Function

    <ExportAPI("/Copy.Fasta",
               Info:="Copy target type files from different sub directory into a directory.",
               Usage:="/Copy.Fasta /imports <DIR> [/type <faa,fna,ffn,fasta,...., default:=faa> /out <DIR>]")>
    Public Function CopyFasta(args As CommandLine) As Integer
        Dim in$ = args <= "/imports"
        Dim type$ = args("/type") Or "faa"
        Dim out As String = args("/out") Or ([in].TrimDIR & "." & type)

        type = "*." & type

        For Each DIR As String In ls - l - lsDIR <= [in]
            Dim dName$ = DIR.BaseName

            For Each file$ In ls - l - r - type <= DIR$
                Dim path$ = out & "/" & dName & "_" & file.FileName
                Call file.ReadAllText.SaveTo(path)
            Next
        Next

        Return 0
    End Function

    <ExportAPI("/Merge.faa", Usage:="/Merge.faa /in <DIR> /out <out.fasta>")>
    <Group(CLIGrouping.GenbankTools)>
    Public Function MergeFaa(args As CommandLine) As Integer
        Dim inDIR As String = args <= "/in"
        Dim out As String = args("/out") Or (inDIR & "/faa.fasta")
        Dim fasta As New FastaFile

        For Each file As String In ls - l - r - "*.faa" <= inDIR
            fasta.AddRange(FastaFile.Read(file))
        Next

        Return fasta.Save(out, Encodings.ASCII)
    End Function

    <ExportAPI("/Export.BlastX")>
    <Usage("/Export.BlastX /in <blastx.txt> [/top /Uncharacterized.exclude /out <out.csv>]")>
    <Description("Export the blastx alignment result into a csv table.")>
    <ArgumentAttribute("/top", True, CLITypes.Boolean,
              Description:="Only output the top first alignment result? Default is not.")>
    <ArgumentAttribute("/in", False, CLITypes.File,
              PipelineTypes.std_in,
              Extensions:="*.txt",
              Description:="The text file content output from the blastx command in NCBI blast+ suite.")>
    <Group(CLIGrouping.GenbankTools)>
    Public Function ExportBlastX(args As CommandLine) As Integer
        Dim [in] As String = args <= "/in"
        Dim top As Boolean = args.IsTrue("/top")
        Dim UncharacterizedExclude As Boolean = args.IsTrue("/Uncharacterized.exclude")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & $"{If(top, "top", "")}.blastx.csv")

        If top Then
            Call "The top one will be output...".info
        End If
        If UncharacterizedExclude Then
            Call "The Uncharacterized protein will be Excluded...".info
        End If

        Dim blastxOut As v228_BlastX = BlastX.TryParseOutput([in], UncharacterizedExclude)
        Dim result As BlastXHit() = blastxOut.BlastXHits

        If top Then
            Return result.TopBest.SaveTo(out).CLICode
        Else
            Return result.SaveTo(out).CLICode
        End If
    End Function

    <ExportAPI("/hits.ID.list")>
    <Usage("/hits.ID.list /in <bbhindex.csv> [/out <out.txt>]")>
    Public Function HitsIDList(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = (args <= "/out") Or $"{[in].TrimSuffix}.hits_id.list.txt".AsDefault
        Dim hits As BBHIndex() = [in].LoadCsv(Of BBHIndex)
        Dim list = hits.Select(Function(h) h.HitName).Distinct.ToArray
        Return list.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Export.gb")>
    <Description("Export the *.fna, *.faa, *.ptt file from the gbk file.")>
    <Usage("/Export.gb /gb <genbank.gb/DIR> [/flat /out <outDIR> /simple /batch]")>
    <ArgumentAttribute("/simple", True, CLITypes.Boolean, AcceptTypes:={GetType(Boolean)},
              Description:="Fasta sequence short title, which is just only contains locus_tag")>
    <ArgumentAttribute("/flat", True, CLITypes.Boolean, AcceptTypes:={GetType(Boolean)},
              Description:="If the argument is presented in your commandline input, then all of the files 
              will be saved in one directory, otherwise will group by genome locus_tag in seperated folders.")>
    <Group(CLIGrouping.GenbankTools)>
    Public Function ExportGenbank(args As CommandLine) As Integer
        Dim gb As String = args("/gb")
        Dim batch As Boolean = args("/batch")
        Dim simple As Boolean = args("/simple")
        Dim flat As Boolean = args("/flat")

        If batch Then
            Dim EXPORT As String = args("/out") Or $"{gb.TrimDIR}.EXPORT"

            For Each file As String In ls - l - r - {"*.gb", "*.gbff", "*.gbk"} <= gb
                Dim out As String = file.TrimSuffix

                For Each dbFile As GBFF.File In GBFF.File.LoadDatabase(file)
                    Call dbFile.exportTo(out, simple, flat)
                Next
            Next
        Else
            Dim out As String = args("/out") Or args("/gb").TrimSuffix

            For Each dbFile As GBFF.File In GBFF.File.LoadDatabase(gb)
                Call dbFile.exportTo(out, simple, flat)
            Next
        End If

        Return 0
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="gb"></param>
    ''' <param name="out$"></param>
    ''' <param name="simple"></param>
    ''' <param name="flat">
    ''' 如果为true，则所有的结果都会在一个文件夹之中
    ''' </param>
    <Extension> Private Sub exportTo(gb As GBFF.File, out$, simple As Boolean, flat As Boolean)
        Dim PTT As PTT = gb.GbffToPTT(ORF:=True)
        Dim Faa As New FastaFile(If(simple, gb.ExportProteins_Short, gb.ExportProteins))
        Dim Fna As FastaSeq = gb.Origin.ToFasta
        Dim GFF As GFFTable = gb.ToGff
        Dim name As String = gb.Source.SpeciesName  ' 
        Dim ffn As FastaFile = gb.ExportGeneNtFasta
        Dim geneList$() = ffn.Select(Function(fa) fa.Headers.First).ToArray

        ' blast+程序要求序列文件的路径之中不可以有空格，所以将空格替换掉，方便后面的blast操作
        name = name.NormalizePathString(False).Replace(" ", "_")

        If Not flat Then
            out = out & "/" & gb.Locus.AccessionID
        End If

        Call gb.exportAnnotatonTable.SaveTo($"{out}/{name}.Annotations.csv")

        Call PTT.Save(out & $"/{name}.ptt")
        Call Fna.SaveTo(120, out & $"/{name}.fna")
        Call Faa.Save(out & $"/{name}.faa")
        Call GFF.Save(out & $"/{name}.gff")
        Call ffn.Save(out & $"/{name}.ffn")
        Call geneList.SaveTo(out & $"/{name}.list")
        Call gb.Save($"{out}/{name}.gbff")
        Call Faa.Select(Function(fa)
                            Return Strings.Trim(fa.Title).GetTagValue
                        End Function) _
                .SaveTo($"{out}/{name}.csv")
    End Sub

    <Extension>
    Private Function exportAnnotatonTable(gb As GBFF.File) As EntityObject()
        Return gb.Features _
            .Where(Function(feature) feature.KeyName <> "source") _
            .Select(Function(feature)
                        Dim location = feature.Location.Location

                        Return New EntityObject With {
                            .ID = feature.EnsureNonEmptyLocusId,
                            .Properties = New Dictionary(Of String, String) From {
                                {"Type", feature.KeyName},
                                {"Minimum", location.left},
                                {"Maximum", location.right},
                                {"Length", location.FragmentSize},
                                {"Direction", "forward" Or "reverse".When(feature.Location.Complement)},
                                {"# Intervals", 1},
                                {"Document Name", gb.Source.SpeciesName},
                                {"Length (with gaps)", location.FragmentSize},
                                {"Max (original sequence)", location.right},
                                {"Max (with gaps)", location.right},
                                {"Min (original sequence)", location.left},
                                {"Min (with gaps)", location.left},
                                {"NCBI Feature Key", feature.KeyName},
                                {"Sequence", feature.SequenceData},
                                {"Sequence Name", gb.Source.SpeciesName},
                                {"Track Name", ""},
                                {"product", feature("product")},
                                {"translation", feature("translation")},
                                {"Length (with extension)", location.FragmentSize},
                                {"Sequence (with extension)", ""},
                                {"EC_number", feature("EC_number")},
                                {"db_xref", feature("db_xref")},
                                {"transl_table", feature("transl_table")},
                                {"genome_id", ""},
                                {"genome_md5", ""},
                                {"mol_type", ""},
                                {"organism", ""}
                            }
                        }
                    End Function) _
            .ToArray
    End Function

    <ExportAPI("/Export.gb.genes")>
    <Usage("/Export.gb.genes /gb <genbank.gb> [/locus_tag /geneName /out <out.fasta>]")>
    <ArgumentAttribute("/geneName", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="If this parameter is specific as True, then this function will try using geneName as the fasta sequence title, or using locus_tag value as default.")>
    <Group(CLIGrouping.GenbankTools)>
    Public Function ExportGenesFasta(args As CommandLine) As Integer
        Dim gb$ = args <= "/gb"
        Dim locus_tag As Boolean = args("/locus_tag")
        Dim geneName As Boolean = args("/geneName")
        Dim out As String = args("/out") Or (gb.TrimSuffix & ".genes.fasta")
        Dim genbank As GBFF.File = GBFF.File.Load(gb)
        Dim ffn As FastaFile = genbank.ExportGeneNtFasta(geneName, onlyLocus_tag:=locus_tag)

        Return ffn.Save(out, Encoding.ASCII).CLICode
    End Function

    <ExportAPI("/Export.Protein")>
    <Description("Export all of the protein sequence from the genbank database file.")>
    <Usage("/Export.Protein /gb <genome.gb> [/out <out.fasta>]")>
    <Group(CLIGrouping.GenbankTools)>
    Public Function ExportProt(args As CommandLine) As Integer
        Dim gb As String = args <= "/gb"
        Dim out As String = args("/out") Or (gb.TrimSuffix & "-protein.fasta")
        Dim gbk As GBFF.File = GBFF.File.Load(gb)
        Dim prot As FastaFile = gbk.ExportProteins_Short

        Return prot.Save(out).CLICode
    End Function

    <ExportAPI("/add.locus_tag",
               Info:="Add locus_tag qualifier into the feature slot.",
               Usage:="/add.locus_tag /gb <gb.gbk> /prefix <prefix> [/add.gene /out <out.gb>]")>
    <ArgumentAttribute("/add.gene", True, Description:="Add gene features?")>
    <Group(CLIGrouping.GenbankTools)>
    Public Function AddLocusTag(args As CommandLine) As Integer
        Dim gbFile As String = args("/gb")
        Dim prefix As String = args("/prefix")
        Dim out As String = args.GetValue("/out", gbFile.TrimSuffix & $".{prefix}.gb")
        Dim gb As GBFF.File = GBFF.File.Load(gbFile)
        Dim LQuery = (From x As GBFF.Keywords.FEATURES.Feature
                      In gb.Features
                      Where String.Equals(x.KeyName, "gene") OrElse
                          String.Equals(x.KeyName, "CDS", StringComparison.OrdinalIgnoreCase)
                      Let uid As String = x.Location.UniqueId
                      Select uid,
                          x
                      Group By uid Into Group).ToArray

        Dim idx As i32 = 1

        For Each geneX In LQuery
            Dim locusId As String =
                $"{prefix}_{STDIO.ZeroFill(++idx, 4)}"

            For Each gFeature In geneX.Group
                gFeature.x.SetValue(FeatureQualifiers.locus_tag, locusId)
            Next

            Call Console.Write(".")
        Next

        If args("/add.gene") Then
            Call gb.Features.AddGenes()
        End If

        Return gb.Save(out, Encoding.ASCII).CLICode
    End Function

    <ExportAPI("/add.names", Usage:="/add.names /anno <anno.csv> /gb <genbank.gbk> [/out <out.gbk> /tag <overrides_name>]")>
    <Group(CLIGrouping.GenbankTools)>
    Public Function AddNames(args As CommandLine) As Integer
        Dim inFile As String = args("/anno")
        Dim gbFile As String = args("/gb")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & "-" & gbFile.BaseName & ".gb")
        Dim tag As String = args.GetValue("/tag", "name")
        Dim annos As IEnumerable(Of NameAnno) = inFile.LoadCsv(Of NameAnno)
        Dim gb As GBFF.File = GBFF.File.Load(gbFile)

        For Each anno In annos
            Dim features = gb.Features.GetByLocation(anno.Minimum, anno.Maximum)
            For Each feature As GBFF.Keywords.FEATURES.Feature In features
                Call feature.Add(tag, anno.Name)
            Next
        Next

        Return gb.Save(out, Encodings.ASCII.CodePage).CLICode
    End Function
End Module

Public Class NameAnno
    Public Property Name As String
    Public Property Minimum As Integer
    Public Property Maximum As Integer

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
