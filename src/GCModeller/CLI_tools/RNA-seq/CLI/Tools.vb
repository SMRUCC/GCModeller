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
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Fastaq
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.SAM
Imports SMRUCC.genomics.SequenceModel.SAM.DocumentElements

Partial Module CLI

    <ExportAPI("/fq2fa", Usage:="/fq2fa /in <fastaq> [/out <fasta>]")>
    Public Function Fq2fa(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & ".fasta")
        Dim fastaq As FastaqFile = FastaqFile.Load([in])
        Dim fasta As FastaFile = fastaq.ToFasta
        Return fasta.Save(out, Encodings.ASCII)
    End Function

    <ExportAPI("/Clustering", Usage:="/Clustering /in <fq> /kmax <int> [/out <out.Csv>]")>
    Public Function Clustering(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim kmax As Integer = args.GetInt32("/kmax")
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & ",kmax=" & kmax & ".Csv")
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

    <ExportAPI("/Export.SAM.Maps", Usage:="/Export.SAM.Maps /in <in.sam> [/out <out.Csv>]")>
    Public Function ExportSAMMaps(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & ".Maps.Csv")
        Dim reader As New SamStream([in])
        Dim result As New List(Of SimpleSegment)

        For Each readMaps In reader.ReadBlock
            result += New SimpleSegment With {
                .ID = readMaps.RNAME,
                .Start = readMaps.POS,
                .Strand = readMaps.Strand.GetBriefCode,
                .SequenceData = readMaps.QUAL,
                .Complement = readMaps.QNAME,
                .Ends = readMaps.FLAG
            }
        Next

        Dim maps As New Dictionary(Of String, String) From {
 _
            {NameOf(SimpleSegment.Complement), NameOf(AlignmentReads.QNAME)},
            {NameOf(SimpleSegment.Ends), NameOf(AlignmentReads.FLAG)},
            {NameOf(SimpleSegment.SequenceData), NameOf(AlignmentReads.QUAL)}
        }


        Dim stat As NamedValue(Of Integer)() =
            LinqAPI.Exec(Of NamedValue(Of Integer)) <= (From x As SimpleSegment
                                                        In result
                                                        Select x
                                                        Group x By x.ID Into Count) _
                   .Select(Function(x) New NamedValue(Of Integer)(x.ID, x.Count))
        Dim statOut As String = out.TrimFileExt & ".stat.Csv"

        Call New NamedValue(Of Integer)([in].BaseName, result.Count).Join(stat).SaveTo(statOut)

        Return result.SaveTo(out, maps:=maps)
    End Function

    <ExportAPI("/Associate.GI",
               Usage:="/Associate.GI /in <list.Csv.DIR> /gi <nt.gi.csv> [/out <out.DIR>]")>
    Public Function AssociateGI(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim ref As String = args("/gi")
        Dim EXPORT As String = args.GetValue("/out", [in].TrimDIR & "." & ref.BaseName & "/")
        Dim hash As Dictionary(Of String, String) = __buildHash(ref)

        For Each file As String In ls - l - wildcards("*.Csv") <= [in]
            Dim inputs = file.LoadCsv(Of TaxiValue)
            Dim out As String = EXPORT & "/" & file.BaseName & ".Csv"

            For Each x In inputs
                Dim gi As String = Regex.Match(x.Name, "gi\|\d+", RegexICSng).Value
                gi = gi.Split("|"c).Last
                If hash.ContainsKey(gi) Then
                    x.Title = hash(gi)
                End If
            Next

            Call inputs.SaveTo(out)
        Next

        Return 0
    End Function

    <ExportAPI("/Associate.Taxonomy",
               Usage:="/Associate.Taxonomy /in <in.DIR> /tax <ncbi_taxonomy:names,nodes> [/gi <nt.gi.csv> /out <out.DIR>]")>
    Public Function AssociateTaxonomy(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim tax As String = args("/tax")
        Dim EXPORT As String = args.GetValue("/out", [in].TrimDIR & ".NCBI.Taxonomy/")
        Dim ref As String = args("/gi")
        Dim taxTree As New NcbiTaxonomyTree(tax)
        Dim hash As Dictionary(Of String, String) =
            If(ref.FileExists,
            __buildHash(ref),
            New Dictionary(Of String, String))

        For Each file As String In ls - l - r - wildcards("*.Csv") <= [in]
            Dim data As IEnumerable(Of TaxiValue) = file.LoadCsv(Of TaxiValue)
            Dim out As String = EXPORT & "/" & file.BaseName & ".Csv"
            Dim LQuery = (From x As TaxiValue
                          In data
                          Let gi As String = Regex.Match(x.Name, "gi\|\d+", RegexICSng).Value.Split("|"c).Last
                          Select gi,
                              x).ToArray

            For Each x In LQuery
                If hash.ContainsKey(x.gi) Then
                    x.x.Title = hash(x.gi)
                End If
            Next

            Dim gis As String() = LinqAPI.Exec(Of String) <=
                From x
                In LQuery
                Where Not String.IsNullOrEmpty(x.gi)
                Select x.gi
            Dim taxis = TaxonomyWebAPI.efetch(gis)

            Call taxis.GetJson.SaveTo(EXPORT & "/" & file.BaseName & ".json")

            Dim taxiHash = (From x As TSeq
                            In taxis.TSeq
                            Select x
                            Group By x.TSeq_gi Into Group) _
                                    .ToDictionary(Function(x) x.TSeq_gi,
                                                  Function(x) x.Group.First)
            For Each x In LQuery
                If taxiHash.ContainsKey(x.gi) Then
                    x.x.taxid = taxiHash(x.gi).TSeq_taxid
                    x.x.TaxonomyTree = TaxonNode.Taxonomy(taxTree.GetAscendantsWithRanksAndNames({CInt(x.x.taxid)}).Values.First)
                End If
            Next

            data = LQuery.ToArray(Function(x) x.x)
            Call data.SaveTo(out)
        Next

        Return 0
    End Function

    <ExportAPI("/Export.GI", Usage:="/Export.GI /in <ncbi:nt.fasta> [/out <out.csv>]")>
    Public Function ExportGI(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & ".gi.Csv")
        Dim nt As New StreamIterator([in])

        Using writer As New WriteStream(Of TaxiValue)(out)

            writer.BaseStream.AutoFlush = True
            writer.BaseStream.NewLine = vbLf

            For Each fa As FastaToken In nt.ReadStream
                Dim title As String = fa.Title
                Dim gi As String = title.Match("gi\|\d+", RegexICSng).Split("|"c).Last  ' 由于bowetie程序建库的时候只取最开始的值，所以在这里只需要第一个match就行了
                Dim result As New TaxiValue With {
                    .Name = gi,
                    .Title = title
                }
                Call writer.Flush(result)
            Next
        End Using

        Return 0
    End Function

    Public Class TaxiValue
        Public Property Name As String
        Public Property x As String
        Public Property Title As String
        Public Property taxid As String
        Public Property TaxonomyTree As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    ''' <summary>
    ''' {gi, title}
    ''' </summary>
    ''' <returns></returns>
    Private Function __buildHash(nt As String) As Dictionary(Of String, String)
        Dim source As IEnumerable(Of TaxiValue) = nt.LoadCsv(Of TaxiValue)
        Dim Groups = From p
                     In source
                     Select p
                     Group By gi = p.Name Into Group
        Return Groups.ToDictionary(
            Function(x) x.gi,
            Function(x) x.Group.Select(
            Function(o) o.Title).JoinBy("; "))
    End Function
End Module
