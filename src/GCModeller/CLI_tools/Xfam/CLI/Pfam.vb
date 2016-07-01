Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.AnalysisTools.ProteinTools.Sanger.Pfam.PfamString
Imports SMRUCC.genomics.AnalysisTools.ProteinTools.Sanger.Pfam
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.Linq
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.AnalysisTools.ProteinTools.Sanger.PfamHMMScan
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Linq
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.AnalysisTools.ProteinTools.Sanger.PfamHMMScan.hmmscan
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.SequenceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Partial Module CLI

    <ExportAPI("/Export.Pfam.UltraLarge", Usage:="/Export.Pfam.UltraLarge /in <blastOUT.txt> [/out <out.csv> /evalue <0.00001> /coverage <0.85> /offset <0.1>]")>
    Public Function ExportUltraLarge(args As CommandLine.CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimFileExt & ".Export.Csv")
        Dim evalue As Double = args.GetValue("/evalue", DomainParser.Evalue1En5)
        Dim coverage As Double = args.GetValue("/coverage", 0.85)
        Dim offset As Double = args.GetValue("/offset", 0.1)

        Using fileStream As New WriteStream(Of PfamString)(out)
            Dim save As Action(Of BlastPlus.Query) =
                Sub(query As BlastPlus.Query)
                    Dim lstBuffer = ToPfamString(query, evalue:=evalue, coverage:=coverage, offset:=offset)
                    Call fileStream.Flush({lstBuffer})
                End Sub
            Dim chunkSize As Long = 768 * 1024 * 1024

            Call $"{inFile.ToFileURL} ===> {out.ToFileURL}....".__DEBUG_ECHO
            Call BlastPlus.Parser.Transform(inFile, chunkSize, save)

            Return 0
        End Using
    End Function

    <ExportAPI("/Export.hmmscan",
               Usage:="/Export.hmmscan /in <input_hmmscan.txt> [/evalue 1e-5 /out <pfam.csv>]")>
    Public Function ExportHMMScan(args As CommandLine.CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & ".pfam.Csv")
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
                           Select locus.__getPfam(domains, l)).ToArray

        Call pfamStrings.SaveTo(out.TrimFileExt & ".pfam-string.Csv")
        Return result.SaveTo(out).CLICode
    End Function


    <ExportAPI("/Export.hmmsearch",
               Usage:="/Export.hmmsearch /in <input_hmmsearch.txt> [/prot <query.fasta> /out <pfam.csv>]")>
    Public Function ExportHMMSearch(args As CommandLine.CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & ".pfam.Csv")
        Dim doc As hmmsearch = hmmsearchParser.LoadDoc([in])
        Dim pro As Dictionary(Of String, AlignmentHit()) = doc.GetProfiles
        Dim pfams As New List(Of PfamString)
        Dim protHash As Dictionary(Of String, FASTA.FastaToken)

        If args.ContainsParameter("/prot", True) Then
            Dim prot As New FASTA.FastaFile(args - "/prot")
            protHash =
            prot.ToDictionary(Function(x) x.Attributes(Scan0).Split.First)
        Else
            protHash = New Dictionary(Of String, FASTA.FastaToken)
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
                Dim fa As FASTA.FastaToken = protHash(x.Key)
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
                            Distinct).ToArray(Function(s) $"{s}:{s}"),
                .ProteinId = x.Key,
                .Length = len,
                .Description = title
            }
        Next

        Return pfams.SaveTo(out).CLICode
    End Function

    <Extension>
    Private Function __getPfam(locus As String, domains As ScanTable(), l As Integer) As PfamString
        Dim ps As String() = domains.ToArray(Function(x) x.GetPfamToken)
        Dim ds As String() = domains.Select(Function(x) $"{x.model}:{x.model}").Distinct.ToArray

        Return New PfamString With {
            .ProteinId = locus,
            .Length = l,
            .Domains = ds,
            .PfamString = ps
        }
    End Function
End Module
