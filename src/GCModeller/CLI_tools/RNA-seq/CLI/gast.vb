#Region "Microsoft.VisualBasic::a7ce9cc4e14a2de51466bfdc6d3b5f73, ..\RNA-seq\CLI\gast.vb"

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
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.DocumentStream
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Analysis.Metagenome.gast
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.SAM

Partial Module CLI

    <ExportAPI("/gast")>
    Public Function gastInvoke(args As CommandLine) As Integer
        Return gast.Invoke(args).CLICode
    End Function

    <ExportAPI("/Export.SSU.Refs",
           Usage:="/Export.SSU.Refs /in <ssu.fasta> [/out <out.DIR> /no-suffix]")>
    Public Function ExportSSURefs(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim EXPORT As String =
            If(args.GetBoolean("/no-suffix"),
            [in].TrimSuffix,
            [in].TrimSuffix & ".EXPORT/")
        EXPORT = args.GetValue("/out", EXPORT)
        Return [in].ExportSILVA(EXPORT).CLICode
    End Function

    <ExportAPI("/Export.SSU.Refs.Batch",
               Usage:="/Export.SSU.Refs /in <ssu.fasta.DIR> [/out <out.DIR>]")>
    Public Function ExportSSUBatch(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimDIR & ".EXPORT/")
        Dim api As String = GetType(CLI).API(NameOf(ExportSSURefs))
        Dim CLI As String() =
            LinqAPI.Exec(Of String) <= From fa As String
                                       In ls - l - r - wildcards("*.fna", "*.fasta", "*.fsa", "*.fa", "*.fas") <= [in]
                                       Select $"{api} /in {fa.CliPath} /no-suffix"
        For Each arg As String In CLI
            Call arg.__DEBUG_ECHO
        Next

        Return App.SelfFolks(CLI, LQuerySchedule.Recommended_NUM_THREADS)
    End Function

    Const Interval As String = "NNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNN"

    <ExportAPI("/Contacts.Ref", Usage:="/Contacts.Ref /in <in.fasta> /maps <maps.sam> [/out <out.DIR>]")>
    Public Function Contacts(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim inSam As String = args("/maps")
        Dim i As Integer = 1
        Dim contigs As New List(Of SimpleSegment)
        Dim out As String = args.GetValue("/out", [inSam].TrimSuffix & ".Contigs/")
        Dim outNt As String = out & "/nt.fasta"
        Dim outContigs As String = out & "/contigs.csv"
        Dim il As Integer = Interval.Length
        Dim sam As New SamStream(inSam)

        Using writer = outNt.OpenWriter(Encodings.ASCII)
            Dim seqHash = (From x As FastaToken
                           In New FastaFile([in])
                           Select x
                           Group x By x.Title.Split.First Into Group) _
                                .ToDictionary(Function(x) x.First,
                                              Function(x) x.Group.First)

            Call writer.WriteLine("> " & [in].BaseName)

            Dim list As New List(Of FastaToken)

            For Each readMap As AlignmentReads In sam.ReadBlock.Where(Function(x) Not x.RNAME = "*")
                list += seqHash(readMap.RNAME)
            Next

            For Each fa As FastaToken In (From x As FastaToken
                                          In list
                                          Select x
                                          Group x By x.Title.Split.First Into Group) _
                                               .Select(Function(x) x.Group.First)

                Call writer.Write(fa.SequenceData)
                Call writer.Write(Interval)

                Dim nx As Integer = i + fa.Length

                contigs += New SimpleSegment With {
                    .Start = i,
                    .Ends = nx,
                    .ID = fa.ToString,
                    .Strand = "+"
                }
                i = nx + il
            Next

            Call contigs.SaveTo(outContigs)
        End Using

        Return 0
    End Function

    <ExportAPI("/Imports.gast.Refs.NCBI_nt",
               Usage:="/Imports.gast.Refs.NCBI_nt /in <in.nt> /gi2taxid <dmp/txt/bin> /taxonomy <nodes/names.dmp_DIR> [/out <out.fasta>]")>
    Public Function ImportsRefFromNt(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim gi2taxid As String = args("/gi2taxid")
        Dim taxonomy As String = args("/taxonomy")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".gast.refs.fasta")
        Return gast_tools.ExportNt([in], gi2taxid, taxonomy, out)
    End Function

    <ExportAPI("/gast.stat.names",
               Usage:="/gast.stat.names /in <*.names> /gast <gast.out> [/out <out.Csv>]")>
    Public Function StateNames(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".stat.Csv")
        Dim gastOut As String = args("/gast")
        Dim result As Names() = ParseNames([in]).FillTaxonomy(gastOut).ToArray
        Call BIOM.Imports(result, 1000).GetJson.SaveTo(out.TrimSuffix & ".Megan.biom")
        Call MeganImports.Out(result).Save(out.TrimSuffix & ".Megan.Csv", Encodings.ASCII)
        Return result.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Export.Megan.BIOM",
               Usage:="/Export.Megan.BIOM /in <relative.table.csv> [/out <out.json.biom>]")>
    Public Function ExportToMegan(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".biom")
        Dim data As RelativeSample() = [in].LoadCsv(Of RelativeSample)()
        Dim result = data.EXPORT
        Return result.GetJson.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Rank.Statics",
               Usage:="/Rank.Statics /in <relative.table.csv> [/out <EXPORT_DIR>]")>
    Public Function RankStatics(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim EXPORT As String =
            args.GetValue("/out", [in].TrimSuffix & ".EXPORT/")
        Dim source As RelativeSample() = [in].LoadCsv(Of RelativeSample)
        Return source.ExportByRanks(EXPORT)
    End Function

    <ExportAPI("/Statics.Labels",
               Usage:="/Statics.Labels /in <in.csv> [/label <Label> /name <Name> /value <value> /out <out.csv>]")>
    Public Function MergeLabels(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".labels.Csv")
        Dim label As String = args.GetValue("/label", NameOf(LabelData.label))
        Dim iname As String = args.GetValue("/name", NameOf(LabelData.Name))
        Dim value As String = args.GetValue("/value", NameOf(LabelData.value))
        Dim maps As New Dictionary(Of String, String) From {
            {label, NameOf(LabelData.label)},
            {iname, NameOf(LabelData.Name)},
            {value, NameOf(LabelData.value)}
        }
        Dim inData As LabelData() = [in].LoadCsv(Of LabelData)(maps:=maps)
        Dim Groups = From x As LabelData
                     In inData
                     Select x
                     Group x By x.Name Into Group
        Dim result As DataSet() = LinqAPI.Exec(Of DataSet) <=
            From x
            In Groups
            Select New DataSet With {
                .Identifier = x.Name,
                .Properties = x.Group.ToDictionary(
                    Function(xx) xx.label,
                    Function(xx) xx.value)
            }
        Return result.SaveTo(out).CLICode
    End Function

    Public Class LabelData
        Public Property label As String
        Public Property Name As String
        Public Property value As Double
    End Class
End Module

