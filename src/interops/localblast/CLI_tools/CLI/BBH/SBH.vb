#Region "Microsoft.VisualBasic::c6d4653b6935ee24d83a6498bcba4dd1, CLI_tools\CLI\BBH\SBH.vb"

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
'     Function: __evalueRow, __HitsRow, _2_KOBASOutput, EvalueMatrix, ExportBBHLarge
'               ExportOverviews, ExportParalog, ExportSBH, SBH_topHits
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.BBHLogs
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus

Partial Module CLI

    <ExportAPI("/to.kobas")>
    <Usage("/to.kobas /in <sbh.csv> [/out <kobas.tsv>]")>
    Public Function _2_KOBASOutput(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & ".tsv")
        Dim row As RowObject

        Using writer As StreamWriter = out.OpenWriter
            For Each match As BestHit In [in].LoadCsv(Of BestHit)
                With match
                    row = { .QueryName, .HitName, .coverage, .length_hsp, 0, 0, 0, 0, 0, 0, .evalue, .Score}
                End With

                Call writer.WriteLine(row.AsLine(ASCII.TAB))
            Next
        End Using

        Return 0
    End Function

    <ExportAPI("/Paralog", Usage:="/Paralog /blastp <blastp.txt> [/coverage 0.5 /identities 0.3 /out <out.csv>]")>
    Public Function ExportParalog(args As CommandLine) As Integer
        Dim [in] As String = args - "/blastp"
        Dim blastp As v228 = BlastPlus.ParsingSizeAuto([in])
        Dim coverage As Double = args.GetValue("/coverage", 0.5)
        Dim identities As Double = args.GetValue("/identities", 0.3)
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".paralogs.csv")
        Dim paralogs As BestHit() = Paralog.ExportParalog(blastp, coverage, identities)
        Return paralogs.SaveTo(out).CLICode
    End Function

    Private Function __evalueRow(hitsTags As String(),
                                 queryName As String,
                                 hashHits As Dictionary(Of String, BestHit()),
                                 flip As Boolean) As RowObject

        Dim row As New IO.RowObject From {queryName}

        For Each hit As String In hitsTags

            If flip Then

                If hashHits.ContainsKey(hit) Then
                    Dim e As Double = hashHits(hit).First.evalue

                    If e = 0R Then
                        Call row.Add("1")
                    Else
                        Call row.Add(CStr(1 - e))
                    End If

                Else
                    Call row.Add("0")
                End If

            Else
                If hashHits.ContainsKey(hit) Then
                    Call row.Add(hashHits(hit).First.evalue)
                Else
                    Call row.Add("-1")
                End If
            End If


        Next

        Return row
    End Function

    Private Function __HitsRow(hitsTags As String(),
                               queryName As String,
                               hashHits As Dictionary(Of String, BestHit())) As RowObject

        Dim row As New IO.RowObject From {queryName}

        For Each hit As String In hitsTags
            If hashHits.ContainsKey(hit) Then
                Call row.Add(hashHits(hit).Length)
            Else
                Call row.Add("0")
            End If
        Next

        Return row
    End Function

    <ExportAPI("/MAT.evalue", Usage:="/MAT.evalue /in <sbh.csv> [/out <mat.csv> /flip]")>
    Public Function EvalueMatrix(args As CommandLine) As Integer
        Dim sbh As List(Of BestHit) = args("/in").LoadCsv(Of BestHit)
        Dim out As String = args("/out") Or (args("/in").TrimSuffix & ".Evalue.Csv")
        Dim contigs = (From x As BestHit
                       In sbh
                       Select x
                       Group x By x.QueryName Into Group) _
                            .ToDictionary(Function(x) x.QueryName,
                                          Function(x) (From y As BestHit
                                                       In x.Group
                                                       Select y
                                                       Group y By y.HitName Into Group) _
                                                            .ToDictionary(Function(xx) xx.HitName,
                                                                          Function(xx) xx.Group.ToArray))
        Dim hitsTags As String() = (From x As BestHit In sbh Select x.HitName Distinct).ToArray
        Dim flip As Boolean = args.GetBoolean("/flip")
        Dim LQuery = (From contig In contigs.AsParallel Select __evalueRow(hitsTags, contig.Key, contig.Value, flip)).ToArray
        Dim hits = (From contig In contigs.AsParallel Select __HitsRow(hitsTags, contig.Key, contig.Value)).ToArray

        Dim Csv As csv.IO.File = New csv.IO.File + New RowObject("+".Join(hitsTags))
        Csv += LQuery
        Csv.Save(out, Encoding:=Encoding.ASCII)

        Csv = New csv.IO.File + New RowObject("+".Join(hitsTags))
        Csv += hits

        Return Csv.Save(out & ".Hits.Csv", Encoding:=System.Text.Encoding.ASCII).CLICode
    End Function

    <ExportAPI("/SBH.Export.Large")>
    <Description("Using this command for export the sbh result of your blastp raw data.")>
    <Usage("/SBH.Export.Large /in <blastp_out.txt> [/top.best /trim-kegg /out <sbh.csv> /s.pattern <default=-> /q.pattern <default=-> /identities 0.15 /coverage 0.5]")>
    <Argument("/trim-KEGG", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="If the fasta sequence source is comes from the KEGG database, and you want to removes the kegg species brief code for the locus_tag, then enable this option.")>
    <Argument("/out", True, CLITypes.File,
              AcceptTypes:={GetType(String)},
              Description:="The sbh result output csv file location.")>
    <Argument("/in", False, CLITypes.File,
              AcceptTypes:={GetType(String)},
              Description:="The blastp raw result input file path.")>
    <Group(CLIGrouping.BBHTools)>
    Public Function ExportBBHLarge(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".sbh.Csv")
        Dim idetities As Double = args.GetValue("/identities", 0.15)
        Dim coverage As Double = args.GetValue("/coverage", 0.5)
        Dim sPattern = args.GetValue("/s.pattern", "-").BuildGrepScript
        Dim qPattern = args.GetValue("/q.pattern", "-").BuildGrepScript
        Dim i As VBInteger = 0
        Dim topBest As Boolean = args("/top.best")

        Using IO As New WriteStream(Of BestHit)(out)
            Dim handle As Action(Of Query) = IO _
                .ToArray(Of Query)(
                [ctype]:=Iterator Function(query)
                             Dim hits = v228.SBHLines(query, coverage:=coverage, identities:=idetities, grepHitId:=sPattern)

                             If topBest Then
                                 Yield hits.First
                             Else
                                 For Each output As BestHit In hits
                                     Yield output
                                 Next
                             End If
                         End Function)

            For Each query As Query In BlastpOutputReader.RunParser(inFile)
                query.QueryName = qPattern(query.QueryName)

                Call handle(query)

                If ++i Mod 25 = 0 Then
                    Console.Write(i)
                    Console.Write(vbTab)
                End If
            Next
        End Using

        Return 0
    End Function

    <ExportAPI("/SBH.tophits")>
    <Description("Filtering the sbh result with top SBH Score")>
    <Usage("/SBH.tophits /in <sbh.csv> [/uniprotKB /out <out.csv>]")>
    Public Function SBH_topHits(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & ".sbh.tophits.csv")
        Dim data = [in].LoadCsv(Of BestHit)
        Dim groups = data.GroupBy(Function(hit) hit.QueryName)
        Dim tophits = groups.Select(Function(g) g.OrderByDescending(Function(hit) hit.SBHScore).First).ToArray

        If args.IsTrue("/uniprotKB") Then
            For Each x In tophits
                With x.HitName.GetTagValue(" ", trim:=True)
                    x.HitName = .Name
                    x.description = .Value
                End With
            Next
        End If

        Return tophits.SaveTo(out).CLICode
    End Function

    <ExportAPI("--Export.SBH", Usage:="--Export.SBH /in <in.DIR> /prefix <queryName> /out <out.csv> [/txt]")>
    Public Function ExportSBH(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim query As String = args("/prefix")
        Dim isTxtLog As Boolean = args.GetBoolean("/txt")
        Dim out As String = args("/out")
        Dim lst As String() = LoadSBHEntry(inDIR, query)
        Dim blastp As BBH.BestHit()()

        If isTxtLog Then
            blastp = (From x As String
                      In lst
                      Select BlastPlus.Parser.TryParse(x).ExportAllBestHist).ToArray
        Else
            blastp = lst.Select(Function(x) x.LoadCsv(Of BBH.BestHit).ToArray)
        End If

        Dim LQuery As BBH.BestHit() =
            LinqAPI.Exec(Of BestHit) <= From x As BBH.BestHit()
                                        In blastp
                                        Select x.Where(Function(xx) xx.Matched)
        Return LQuery.SaveTo(out).CLICode
    End Function

    <ExportAPI("--Export.Overviews", Usage:="--Export.Overviews /blast <blastout.txt> [/out <overview.csv>]")>
    Public Function ExportOverviews(args As CommandLine) As Integer
        Dim inFile As String = args("/blast")
        Dim fileInfo = FileIO.FileSystem.GetFileInfo(inFile)
        Dim blastOut As v228

        If fileInfo.Length >= 768 * 1024 * 1024 Then
            blastOut = BlastPlus.TryParseUltraLarge(inFile)
        Else
            blastOut = BlastPlus.TryParse(inFile)
        End If

        Dim overviews As BestHit() = blastOut.ExportOverview.GetExcelData
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & "Overviews.csv")

        Return overviews.SaveTo(out).CLICode
    End Function
End Module
