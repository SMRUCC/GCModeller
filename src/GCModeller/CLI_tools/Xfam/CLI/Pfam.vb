#Region "Microsoft.VisualBasic::adc0ddc79e542248635095697292f6e2, CLI_tools\Xfam\CLI\Pfam.vb"

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
    '     Function: ExportHMMScan, ExportHMMSearch, ExportPfamHits, ExportUltraLarge, getPfam
    '               Pfam2Go, PfamAnnotation
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.SequenceTools.Sanger.PfamHMMScan
Imports SMRUCC.genomics.Analysis.SequenceTools.Sanger.PfamHMMScan.hmmscan
Imports SMRUCC.genomics.Data.GeneOntology.Annotation
Imports SMRUCC.genomics.Data.GeneOntology.Annotation.xref2go
Imports SMRUCC.genomics.Data.Xfam.Pfam
Imports SMRUCC.genomics.Data.Xfam.Pfam.PfamString
Imports SMRUCC.genomics.Data.Xfam.Pfam.Pipeline.LocalBlast
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.SequenceModel
Imports Query = SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Query

Partial Module CLI

    <ExportAPI("/pfam2go")>
    <Usage("/pfam2go /in <pfamhits.csv> /togo <pfam2go.txt> [/out <annotations.xml>]")>
    <Description("Do go annotation based on the pfam mapping to go term.")>
    <Group(Program.PfamCliTools)>
    Public Function Pfam2Go(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim toGoFile$ = args <= "/togo"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.2go_terms.Xml"
        Dim pfamhits As IEnumerable(Of PfamHit) = [in] _
           .OpenHandle _
           .AsLinq(Of PfamHit)
        Dim toGoMaps As Dictionary(Of String, toGO()) = toGO.Parse2GO(toGoFile) _
            .GroupBy(Function(pfam) pfam.entry) _
            .ToDictionary(Function(pfam) pfam.Key.Split(":"c).Last,
                          Function(group)
                              Return group.ToArray
                          End Function)
        Dim annotations As AnnotationClusters = pfamhits.PfamAssign(toGoMaps)

        Call annotations.ToAnnotationTable.SaveTo($"{out.TrimSuffix}.csv")

        Return annotations _
            .GetXml _
            .SaveTo(out) _
            .CLICode
    End Function

    <ExportAPI("/Export.PfamHits")>
    <Usage("/Export.PfamHits /in <blastp_vs_pfamA.txt> [/alt.direction /evalue <1e-5> /coverage <0.8> /identities <0.7> /out <pfamhits.csv>]")>
    <ArgumentAttribute("/out", True, CLITypes.File, PipelineTypes.std_out,
              AcceptTypes:={GetType(PfamHit)},
              Extensions:="*.csv",
              Description:="The output pfam hits result which is parsed from the pfam_vs_protein blastp result.")>
    <ArgumentAttribute("/in", False, CLITypes.File, PipelineTypes.std_in,
              Extensions:="*.txt",
              Description:="The blastp alignment output of pfamA align with query proteins.")>
    <ArgumentAttribute("/alt.direction", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="By default, this cli tools processing the blastp alignment result in direction ``protein_vs_pfam``, 
              apply this option argument in cli to switch the processor in direction ``pfam_vs_protein``.")>
    <ArgumentAttribute("/evalue", True, CLITypes.Double,
              AcceptTypes:={GetType(Double)},
              Description:="E-value cutoff of the blastp alignment result.")>
    <ArgumentAttribute("/coverage", True, CLITypes.Double,
              AcceptTypes:={GetType(Double)},
              Description:="The coverage cutoff of the pfam domain sequence. This argument is not the coverage threshold of your query protein.")>
    <Group(Program.PfamCliTools)>
    Public Function ExportPfamHits(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim alt_direction As Boolean = Not args("/alt.direction")
        Dim evalue# = args("/evalue") Or 0.00001
        Dim coverage# = args("/coverage") Or 0.8
        Dim identities# = args("/identities") Or 0.7
        Dim i As i32 = 0
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.pfamhits.csv"
        Dim hits As PfamHit()

        Call $"Parse {[in]}...".__INFO_ECHO
        Call New Dictionary(Of String, Double) From {
            {NameOf(evalue), evalue},
            {NameOf(coverage), coverage},
            {NameOf(identities), identities}
        }.GetJson _
         .__DEBUG_ECHO

        Using pfamhits As New WriteStream(Of PfamHit)(out)
            For Each query As Query In BlastpOutputReader.RunParser(in$, fast:=False)
                If alt_direction Then
                    hits = query.ParseProteinQuery.ToArray
                Else
                    hits = query.ParseDomainQuery.ToArray
                End If

                Call hits _
                    .Where(Function(hit)
                               Return hit.ApplyDomainFilter(evalue, coverage, identities)
                           End Function) _
                    .DoCall(AddressOf pfamhits.Flush)

                If alt_direction Then
                    Console.Write(".")
                ElseIf ++i Mod 50 = 0 Then
                    Console.Write(i)
                    Console.Write(vbTab)
                End If
            Next
        End Using

        Return 0
    End Function

    <ExportAPI("/Pfam.Annotation")>
    <Usage("/Pfam.Annotation /in <pfamhits.csv> [/out <out.pfamstring.csv>]")>
    <Description("Do pfam functional domain annotation based on the pfam hits result.")>
    <ArgumentAttribute("/in", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(PfamHit)},
              Extensions:="*.csv",
              Description:="The pfam hits result from the blastp query output or hmm search output.")>
    <ArgumentAttribute("/out", True, CLITypes.File, PipelineTypes.std_out,
              AcceptTypes:={GetType(PfamString)},
              Extensions:="*.csv",
              Description:="The annotation output.")>
    <Group(Program.PfamCliTools)>
    Public Function PfamAnnotation(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.pfam_string.csv"
        Dim pfamhits As IEnumerable(Of NamedCollection(Of PfamHit)) = [in] _
            .OpenHandle _
            .AsLinq(Of PfamHit) _
            .DoHitsGrouping
        Dim annotations As PfamString() = pfamhits _
            .CreateAnnotations _
            .OrderBy(Function(q) q.ProteinId) _
            .ToArray

        Return annotations.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 这个函数相当于<see cref="ExportPfamHits"/> + <see cref="PfamAnnotation"/>的合并，
    ''' 只不过使用上面的两个函数来进行注释在数据处理方面会更加的灵活
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Export.Pfam.UltraLarge")>
    <Usage("/Export.Pfam.UltraLarge /in <blastOUT.txt> [/out <out.csv> /evalue <0.00001> /coverage <0.85> /offset <0.1>]")>
    <Description("Export pfam annotation result from blastp based sequence alignment analysis.")>
    <Group(Program.PfamCliTools)>
    <ArgumentAttribute("/in", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(String)},
              Extensions:="*.txt",
              Description:="The blastp raw output file of alignment in direction protein query vs pfam database.")>
    <ArgumentAttribute("/out", True, CLITypes.File, PipelineTypes.std_out,
              AcceptTypes:={GetType(PfamString)},
              Extensions:="*.csv",
              Description:="The pfam annotation output.")>
    <ArgumentAttribute("/offset", True, CLITypes.Double,
              AcceptTypes:={GetType(Double)},
              Description:="The max allowed offset value of the length delta between ``length_query`` and ``length_hit``.")>
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

    <ExportAPI("/Export.hmmscan")>
    <Usage("/Export.hmmscan /in <input_hmmscan.txt> [/evalue 1e-5 /out <pfam.csv>]")>
    <Description("Export result from HMM search based domain annotation result.")>
    <Group(Program.PfamCliTools)>
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
    <Group(Program.PfamCliTools)>
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
