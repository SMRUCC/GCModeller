Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.Linq
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Analysis.Metagenome.BEBaC
Imports SMRUCC.genomics.Analysis.Metagenome.gast
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.Entrez
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Fastaq
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.SAM

Partial Module CLI

    <ExportAPI("/Format.GI", Usage:="/Format.GI /in <txt> /gi <regex> /format <gi|{gi}> /out <out.txt>")>
    Public Function FormatGI(args As CommandLine) As Integer
        Dim [in] As String = args.GetFullFilePath("/in")
        Dim gi As String = args("/gi")
        Dim format As String = args("/format")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".Format_gi." & [in].Split("."c).Last)
        Dim regex As New Regex(gi, RegexICSng)

        Using write = out.OpenWriter(Encodings.ASCII)
            write.NewLine = vbLf

            For Each line As String In [in].IterateAllLines
                Dim ms = regex.Matches(line).ToArray

                For Each s As String In ms
                    line = line.Replace(s, format.Replace("{gi}", Regex.Match(s, "\d+").Value))
                Next

                Call write.WriteLine(line)
            Next
        End Using

        Return 0
    End Function

    <ExportAPI("/fq2fa", Usage:="/fq2fa /in <fastaq> [/out <fasta>]")>
    Public Function Fq2fa(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".fasta")
        Dim fastaq As FastaqFile = FastaqFile.Load([in])
        Dim fasta As FastaFile = fastaq.ToFasta
        Return fasta.Save(out, Encodings.ASCII)
    End Function

    <ExportAPI("/Clustering", Usage:="/Clustering /in <fq> /kmax <int> [/out <out.Csv>]")>
    Public Function Clustering(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim kmax As Integer = args.GetInt32("/kmax")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ",kmax=" & kmax & ".Csv")
        Dim fq As FastaqFile = FastaqFile.Load([in])
        Dim vectors = fq.Transform
        Dim Crude = vectors.InitializePartitions(kmax)
        Dim ppppp = Crude.First.PartitionProbability
        Dim ptes = Crude.MarginalLikelihood
    End Function

    <ExportAPI("/Co.Vector", Usage:="/Co.Vector /in <co.Csv/DIR> [/min 0.01 /max 0.05 /out <out.csv>]")>
    Public Function CorrelatesVector(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim min As Double = args.GetValue("/min", 0.01)
        Dim max As Double = args.GetValue("/max", 0.05)
        Dim EXPORT As String = args.GetValue("/out", [in].TrimDIR & $"/{[in].BaseName}.EXPORT.PR.Csv")
        Dim out As New DocumentStream.File

        out += {"X", "P", "R"}

        For Each file As String In ls - l - r - wildcards("*P*.Csv") <= [in]
            Dim rp As String = file.ParentPath & "/" & file.BaseName.Replace("P", "R") & ".Csv"
            Dim R As DocumentStream.File = DocumentStream.File.Load(rp)
            Dim P As DocumentStream.File = DocumentStream.File.Load(file)
            Dim Rs = R.Skip(1).ToDictionary(Function(x) x.First, Function(x) x(1))

            Call file.__DEBUG_ECHO
            Call rp.__DEBUG_ECHO

            For Each row In P.Skip(1)
                If row(1) <> "NA" Then
                    Dim pp As Double = Val(row(1))
                    If pp <= max AndAlso pp >= min Then
                        Dim srow As New List(Of String)
                        srow += row.First
                        srow += CStr(pp)
                        srow += Rs(srow.First)

                        out += srow

                        Call Console.Write(".")
                    End If
                End If
            Next
        Next

        Return out.Save(EXPORT, Encodings.ASCII)
    End Function

    <Extension>
    Private Function __writeFile(file As DocumentStream.File, out As String, min As Double, max As Double) As Boolean
        Dim rows As New List(Of String)

        For Each row In file.Skip(1)
            If row(1) <> "NA" Then
                Dim p As Double = Val(row(1))
                If p <= max AndAlso p >= min Then
                    rows += $"{row.First},{p}"
                End If
            End If
        Next

        Return rows.SaveTo(out)
    End Function

    <ExportAPI("/Export.SAM.Maps", Usage:="/Export.SAM.Maps /in <in.sam> [/contigs <NNNN.contig.Csv> /raw <ref.fasta> /out <out.Csv>]")>
    Public Function ExportSAMMaps(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".Maps.Csv")
        Dim reader As New SamStream([in])
        Dim result As New List(Of SimpleSegment)
        Dim NNNNcontig As String = args("/contigs")
        Dim genome As GenomeContextProvider(Of GeneBrief) = Nothing
        Dim tagsHash As Dictionary(Of String, String) = Nothing

        If NNNNcontig.FileExists Then
            Dim contigs = NNNNcontig.LoadCsv(Of SimpleSegment)
            Dim genes = contigs.ToArray(Function(x) x.ToPTTGene)
            genome = New GenomeContextProvider(Of GeneBrief)(genes)
            tagsHash = (From x As SimpleSegment
                        In contigs
                        Select x.ID.Split.First,
                            x
                        Group By First Into Group) _
                                .ToDictionary(Function(x) x.First,
                                              Function(x) x.Group.First.x.ID.Replace(x.First, "").Trim)
        End If

        For Each readMaps In reader.ReadBlock
            Dim reads As New SimpleSegment With {
                .ID = readMaps.RNAME,
                .Start = readMaps.POS,
                .Strand = readMaps.Strand.GetBriefCode,
                .SequenceData = readMaps.QUAL,
                .Complement = readMaps.QNAME,
                .Ends = readMaps.FLAG
            }

            If reads.ID <> "*" AndAlso genome IsNot Nothing Then
                Dim loci As New NucleotideLocation(readMaps.POS, readMaps.POS + reads.SequenceData.Length, False)
                Dim contig = genome.GetAroundRelated(loci,, 10)
                If Not contig.IsNullOrEmpty Then
                    reads.ID = contig.First.Gene.Synonym.Split.First
                    If contig.Length > 1 Then
                        Call reads.GetJson.__DEBUG_ECHO
                    End If
                Else
                    Call (reads.ID & " " & loci.ToString & " not found!!!").__DEBUG_ECHO
                End If
            End If

            result += reads
        Next

        Dim maps As New Dictionary(Of String, String) From {
 _
            {NameOf(SimpleSegment.Complement), NameOf(AlignmentReads.QNAME)},
            {NameOf(SimpleSegment.Ends), NameOf(AlignmentReads.FLAG)},
            {NameOf(SimpleSegment.SequenceData), NameOf(AlignmentReads.QUAL)}
        }

        If tagsHash Is Nothing Then
            Dim ref As String = args("/raw")
            If ref.FileExists Then
                Dim rawRef As New FastaFile(ref)
                tagsHash = (From x As FastaToken
                            In rawRef
                            Select x,
                                sid = x.Title.Split.First
                            Group By sid Into Group) _
                                    .ToDictionary(Function(x) x.sid,
                                                  Function(x) x.Group.First.x.Title.Replace(x.sid, "").Trim)
            End If
        End If

        Dim getValue As Func(Of String, String) =
            If(tagsHash Is Nothing,
            Function(s) "",
            Function(s) If(tagsHash.ContainsKey(s), tagsHash(s), ""))
        Dim stat As NamedValue(Of Integer)() =
            LinqAPI.Exec(Of NamedValue(Of Integer)) <= (From x As SimpleSegment
                                                        In result
                                                        Select x
                                                        Group x By x.ID Into Count) _
                   .Select(Function(x) New NamedValue(Of Integer)(x.ID, x.Count) With {.Description = getValue(x.ID)})
        Dim statOut As String = out.TrimSuffix & ".stat.Csv"

        Call (From x As NamedValue(Of Integer)
              In stat
              Where Not String.Equals(x.Name, "*")
              Select gi = Regex.Match(x.Name, "\d+").Value,
                  x.Name) _
                  .ToArray(Function(x) x.gi & vbTab & x.Name) _
                  .SaveTo(statOut.TrimSuffix & ".gi_Maps.tsv")
        Call New NamedValue(Of Integer)([in].BaseName, result.Count).Join(stat).SaveTo(statOut)

        Return result.SaveTo(out, maps:=maps)
    End Function

    <ExportAPI("/Associate.GI",
               Usage:="/Associate.GI /in <list.Csv.DIR> /gi <nt.gi.csv> [/out <out.DIR>]")>
    Public Function AssociateGI(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim ref As String = args("/gi")
        Dim EXPORT As String = args.GetValue("/out", [in].TrimDIR & "." & ref.BaseName & "/")
        Dim hash As Dictionary(Of String, String) =
            TaxiValue.BuildHash(ref.LoadCsv(Of TaxiValue))

        For Each file As String In ls - l - wildcards("*.Csv") <= [in]
            Dim inputs = file.LoadCsv(Of TaxiValue)
            Dim out As String = EXPORT & "/" & file.BaseName & ".Csv"

            For Each x In inputs
                Dim gi As String = Regex.Match(x.Name, "gi(_|\|)\d+", RegexICSng).Value
                gi = gi.Split("|"c).Last
                If hash.ContainsKey(gi) Then
                    x.Title = hash(gi)
                End If
            Next

            Call inputs.SaveTo(out)
        Next

        Return 0
    End Function
End Module
