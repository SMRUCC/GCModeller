#Region "Microsoft.VisualBasic::e27b9426ac5b7fa8079c1545a27fb4b4, CLI_tools\RNA-seq\CLI\gast.vb"

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

' Module CLI
' 
'     Function: ClusterOTU, ContactsRef, ExportSSUBatch, ExportSSURefs, gastInvoke
'               ImportsRefFromNt, MergeLabels, RankStatics
'     Class LabelData
' 
'         Properties: label, Name, value
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports Parallel
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Analysis.Metagenome.gast
Imports SMRUCC.genomics.Metagenomics
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
                                       Select $"{api} /in {fa.CLIPath} /no-suffix"
        For Each arg As String In CLI
            Call arg.__DEBUG_ECHO
        Next

        Return BatchTasks.SelfFolks(CLI, LQuerySchedule.Recommended_NUM_THREADS)
    End Function

    Const Interval As String = "NNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNN"

    ''' <summary>
    ''' 这个函数是在做完了一次mapping之后，进行更近一步分析用的
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Contacts.Ref",
               Info:="This tools using for the furthering analysis when finish the first mapping.",
               Usage:="/Contacts.Ref /in <in.fasta> /maps <maps.sam> [/out <out.DIR>]")>
    Public Function ContactsRef(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim inSam As String = args("/maps")
        Dim i As Integer = 1
        Dim contigs As New List(Of SimpleSegment)
        Dim out As String = args.GetValue("/out", [inSam].TrimSuffix & ".Contigs/")
        Dim outNt As String = out & "/nt.fasta"
        Dim outContigs As String = out & "/contigs.csv"
        Dim il As Integer = Interval.Length
        Dim sam As New SAMStream(inSam)

        Using writer = outNt.OpenWriter(Encodings.ASCII)
            Dim seqHash = (From x As FastaSeq
                           In New FastaFile([in])
                           Select x
                           Group x By x.Title.Split.First Into Group) _
                                .ToDictionary(Function(x) x.First,
                                              Function(x) x.Group.First)

            Call writer.WriteLine("> " & [in].BaseName)

            Dim list As New List(Of FastaSeq)

            For Each readMap As AlignmentReads In sam.IteratesAllReads.Where(Function(x) Not x.RNAME = "*")
                list += seqHash(readMap.RNAME)
            Next

            For Each fa As FastaSeq In (From x As FastaSeq
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

    <ExportAPI("/Cluster.OTUs", Usage:="/Cluster.OTUs /in <contigs.fasta> [/similarity <default:97> /out <outDIR>]")>
    Public Function ClusterOTU(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim similarity As Double = args.GetValue("/similarity", 97.0#)
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & $"-OTUS.{similarity}/")
        Dim contigs As New FastaFile([in])

        Using alignment As StreamWriter = (out & "/NeedlemanWunsch.txt").OpenWriter
            Dim OTUs = contigs.BuildOTUClusters(alignment, similarity)
            Dim table As New List(Of OTUData)

            Using OTUSeqs As StreamWriter = (out & "/OTUs.fasta").OpenWriter(Encodings.ASCII)

                For Each OTU As NamedValue(Of String()) In OTUs
                    table += New OTUData With {
                        .OTU = OTU.Name,
                        .data = New Dictionary(Of String, String) From {
                            {"cluster", OTU.Value.JoinBy("; ")}
                        }
                    }
                    Call OTUSeqs.WriteLine(OTU.Description)
                Next
            End Using

            Return table.SaveTo(out & "/OTUs.csv").CLICode
        End Using
    End Function

    <ExportAPI("/Rank.Statics",
               Usage:="/Rank.Statics /in <relative.table.csv> [/out <EXPORT_DIR>]")>
    Public Function RankStatics(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim EXPORT As String =
            args.GetValue("/out", [in].TrimSuffix & ".EXPORT/")
        Dim source As OTUData() = [in].LoadCsv(Of OTUData)
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
        Dim maps As New NameMapping From {
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
                .ID = x.Name,
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
