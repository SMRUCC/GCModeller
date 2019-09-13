#Region "Microsoft.VisualBasic::43e6af4373f6014c5c6aa0768d7ea2f1, CLI_tools\Xfam\CLI\Pfam.vb"

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
'     Function: __getPfam, ExportHMMScan, ExportHMMSearch, ExportUltraLarge
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceTools.Sanger.PfamHMMScan
Imports SMRUCC.genomics.Analysis.SequenceTools.Sanger.PfamHMMScan.hmmscan
Imports SMRUCC.genomics.Data.Xfam.Pfam
Imports SMRUCC.genomics.Data.Xfam.Pfam.PfamString
Imports SMRUCC.genomics.Data.Xfam.Pfam.Pipeline.LocalBlast
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.SequenceModel

Partial Module CLI

    <ExportAPI("/Export.Pfam.UltraLarge")>
    <Usage("/Export.Pfam.UltraLarge /in <blastOUT.txt> [/out <out.csv> /evalue <0.00001> /coverage <0.85> /offset <0.1>]")>
    <Description("Export pfam annotation result from blastp based sequence alignment analysis.")>
    Public Function ExportUltraLarge(args As CommandLine) As Integer
        Dim inFile As String = args <= "/in"
        Dim out As String = args("/out") Or (inFile.TrimSuffix & ".pfamString.csv")
        Dim evalue As Double = args("/evalue") Or Annotation.Evalue1En5
        Dim coverage As Double = args("/coverage") Or 0.85
        Dim offset As Double = args("/offset") Or 0.1

        Using fileStream As New WriteStream(Of PfamString)(out)
            Dim pstring As PfamString
            Dim save As Action(Of BlastPlus.Query) =
                Sub(query As BlastPlus.Query)
                    pstring = query.ToPfamString(
                        evalue:=evalue,
                        coverage:=coverage,
                        offset:=offset
                    )

                    Call fileStream.Flush({pstring})
                End Sub
            Dim chunkSize As Long = 768 * 1024 * 1024

            Call $"{inFile.ToFileURL} ===> {out.ToFileURL}....".__DEBUG_ECHO
            Call BlastPlus.Parser.Transform(inFile, chunkSize, save)

            Return 0
        End Using
    End Function

    <ExportAPI("/Export.hmmscan",
               Usage:="/Export.hmmscan /in <input_hmmscan.txt> [/evalue 1e-5 /out <pfam.csv>]")>
    Public Function ExportHMMScan(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".pfam.Csv")
        Dim doc As hmmscan = hmmscanParser.LoadDoc([in])
        Dim result As ScanTable() = doc.GetTable
        Dim prots = From x As ScanTable
                    In result
                    Select x
                    Group x By x.name Into Group
        Dim evalue As Double = args.GetValue("/evalue", 0.00001)
        Dim pfamStrings = (From x
                           In prots.AsParallel
                           Let locus As String = x.name.Split.First
                           Let domains As ScanTable() = (From d As ScanTable
                                                         In x.Group
                                                         Where d.rank.Last <> "?"c AndAlso
                                                             d.BestEvalue <= evalue
                                                         Select d).ToArray
                           Let l As Integer = x.Group.First.len
                           Select locus.getPfam(domains, l)).ToArray

        Call pfamStrings.SaveTo(out.TrimSuffix & ".pfam-string.Csv")
        Return result.SaveTo(out).CLICode
    End Function


    <ExportAPI("/Export.hmmsearch",
               Usage:="/Export.hmmsearch /in <input_hmmsearch.txt> [/prot <query.fasta> /out <pfam.csv>]")>
    Public Function ExportHMMSearch(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".pfam.Csv")
        Dim doc As hmmsearch = hmmsearchParser.LoadDoc([in])
        Dim pro As Dictionary(Of String, AlignmentHit()) = doc.GetProfiles
        Dim pfams As New List(Of PfamString)
        Dim protHash As Dictionary(Of String, FASTA.FastaSeq)

        If args.ContainsParameter("/prot", True) Then
            Dim prot As New FASTA.FastaFile(args - "/prot")
            protHash =
            prot.ToDictionary(Function(x) x.Headers(Scan0).Split.First)
        Else
            protHash = New Dictionary(Of String, FASTA.FastaSeq)
        End If

        For Each x In pro
            Dim pfam As String() = LinqAPI.Exec(Of String) <=
                From d As AlignmentHit
                In x.Value
                Select From o As Align
                       In d.hits
                       Where DirectCast(o, IMatched).IsMatched
                       Select o.GetPfamToken(d.QueryTag)

            Dim len As Integer, title As String

            If protHash.ContainsKey(x.Key) Then
                Dim fa As FASTA.FastaSeq = protHash(x.Key)
                len = fa.Length
                title = fa.Title
            Else
                len = 0
                title = x.Key
            End If

            pfams += New PfamString With {
                .PfamString = pfam,
                .Domains = (From s As String
                            In pfam
                            Select s.Split("("c).First
                            Distinct).Select(Function(s) $"{s}:{s}"),
                .ProteinId = x.Key,
                .Length = len,
                .Description = title
            }
        Next

        Return pfams.SaveTo(out).CLICode
    End Function

    <Extension>
    Private Function getPfam(locus As String, domains As ScanTable(), l As Integer) As PfamString
        Dim ps As String() = domains.Select(Function(x) x.GetPfamToken)
        Dim ds As String() = domains.Select(Function(x) $"{x.model}:{x.model}").Distinct.ToArray

        Return New PfamString With {
            .ProteinId = locus,
            .Length = l,
            .Domains = ds,
            .PfamString = ps
        }
    End Function
End Module
