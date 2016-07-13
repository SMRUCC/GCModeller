Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Analysis.Metagenome.BEBaC
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Fastaq
Imports Microsoft.VisualBasic
Imports System.Runtime.CompilerServices

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

    <ExportAPI("/Export.SSU.Refs",
               Usage:="/Export.SSU.Refs /in <ssu.fasta> [/out <out.DIR>]")>
    Public Function ExportSSURefs(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim EXPORT As String = [in].TrimFileExt & ".EXPORT/"
        Return [in].ExportSILVA(EXPORT).CLICode
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
End Module
