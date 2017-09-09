#Region "Microsoft.VisualBasic::98e5aeb026afc2d497d63e004365bb55, ..\localblast\CLI_tools\CLI\gbTools.vb"

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

Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Language.UnixBash.FileSystem
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Terminal
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.SequenceModel.FASTA

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
        Dim CDSHash = (From x As GFF.Feature
                       In gff.GetsAllFeatures(FeatureKeys.Features.CDS)
                       Select x
                       Group x By x.ProteinId Into Group) _
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

        Call $"Found {gpffs.Count} *.gpff and {gffs.Count} *.gff files....".__DEBUG_ECHO

        For Each pair In PathMatch.Pairs(gpffs, gffs, AddressOf __trimName)
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
        Dim EXPORT As String = args.GetValue("/out", inDIR & "-Copy/")
        Dim PTTs As IEnumerable(Of String) = ls - l - r - wildcards("*.ptt") <= inDIR

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
        Dim type$ = args.GetValue("/type", "faa")
        Dim out As String = args.GetValue("/out", [in].TrimDIR & "." & type)

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
        Dim inDIR As String = args - "/in"
        Dim out As String = args.GetValue("/out", inDIR & "/faa.fasta")
        Dim fasta As New FastaFile

        For Each file As String In ls - l - r - wildcards("*.faa") << FileHandles.OpenHandle(inDIR)
            fasta.AddRange(FastaFile.Read(file))
        Next

        Return fasta.Save(out, Encodings.ASCII)
    End Function

    <ExportAPI("/Export.BlastX", Usage:="/Export.BlastX /in <blastx.txt> [/out <out.csv>]")>
    <Group(CLIGrouping.GenbankTools)>
    Public Function ExportBlastX(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".blastx.csv")
        Dim blastx As BlastPlus.BlastX.v228_BlastX = BlastPlus.BlastX.TryParseOutput([in])
        Dim result = blastx.BlastXHits
        Return result.SaveTo(out)
    End Function

    <ExportAPI("/Export.gb",
               Info:="Export the *.fna, *.faa, *.ptt file from the gbk file.",
               Usage:="/Export.gb /gb <genbank.gb/DIR> [/out <outDIR> /simple /batch]")>
    <Argument("/simple", True, AcceptTypes:={GetType(Boolean)},
                   Description:="Fasta sequence short title, which is just only contains locus_tag")>
    <Group(CLIGrouping.GenbankTools)>
    Public Function ExportPTTDb(args As CommandLine) As Integer
        Dim gb As String = args("/gb")
        Dim batch As Boolean = args.GetBoolean("/batch")
        Dim simple As Boolean = args.GetBoolean("/simple")

        If batch Then
            Dim EXPORT As String = args.GetValue("/out", gb.TrimDIR & ".EXPORT")

            For Each file As String In ls - l - r - wildcards("*.gb", "*.gbff", "*.gbk") <= gb
                Dim out As String = file.TrimSuffix

                For Each x As GBFF.File In GBFF.File.LoadDatabase(file)
                    Call x.__exportTo(out, simple)
                Next
            Next
        Else
            Dim out As String = args.GetValue("/out", args("/gb").TrimSuffix)

            For Each x As GBFF.File In GBFF.File.LoadDatabase(gb)
                Call x.__exportTo(out, simple)
            Next
        End If

        Return 0
    End Function

    <Extension> Private Sub __exportTo(gb As GBFF.File, out As String, simple As Boolean)
        Dim PTT As TabularFormat.PTT = gb.GbffToORF_PTT
        Dim Faa As New FastaFile(If(simple, gb.ExportProteins_Short, gb.ExportProteins))
        Dim Fna As FastaToken = gb.Origin.ToFasta
        Dim GFF As GFFTable = gb.ToGff
        Dim name As String = gb.Source.SpeciesName  ' 
        Dim ffn As FastaFile = gb.ExportGeneNtFasta

        name = name.NormalizePathString(False).Replace(" ", "_") ' blast+程序要求序列文件的路径之中不可以有空格，所以将空格替换掉，方便后面的blast操作
        out = out & "/" & gb.Locus.AccessionID

        Call PTT.Save(out & $"/{name}.ptt")
        Call Fna.SaveTo(out & $"/{name}.fna")
        Call Faa.Save(out & $"/{name}.faa")
        Call GFF.Save(out & $"/{name}.gff")
        Call ffn.Save(out & $"/{name}.ffn")
    End Sub

    <ExportAPI("/Export.gb.genes",
               Usage:="/Export.gb.genes /gb <genbank.gb> [/geneName /out <out.fasta>]")>
    <Argument("/geneName", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="If this parameter is specific as True, then this function will try using geneName as the fasta sequence title, or using locus_tag value as default.")>
    <Group(CLIGrouping.GenbankTools)>
    Public Function ExportGenesFasta(args As CommandLine) As Integer
        Dim gb$ = args <= "/gb"
        Dim geneName As Boolean = args.GetBoolean("/geneName")
        Dim out As String = args.GetValue("/out", gb.TrimSuffix & ".genes.fasta")
        Dim genbank As GBFF.File = GBFF.File.Load(gb)
        Dim ffn As FastaFile = genbank.ExportGeneNtFasta(geneName)
        Return ffn.Save(out, Encoding.ASCII).CLICode
    End Function

    <ExportAPI("/add.locus_tag",
               Info:="Add locus_tag qualifier into the feature slot.",
               Usage:="/add.locus_tag /gb <gb.gbk> /prefix <prefix> [/add.gene /out <out.gb>]")>
    <Argument("/add.gene", True, Description:="Add gene features?")>
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

        Dim idx As int = 1

        For Each geneX In LQuery
            Dim locusId As String =
                $"{prefix}_{STDIO.ZeroFill(++idx, 4)}"

            For Each feature In geneX.Group
                feature.x.SetValue(FeatureQualifiers.locus_tag, locusId)
            Next

            Call Console.Write(".")
        Next

        If args.GetBoolean("/add.gene") Then
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
