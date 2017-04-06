Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis
Imports SMRUCC.genomics.Analysis.SequenceTools
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998.CAI
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998.CAI.XML
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.gwANI
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module Utilities

    <ExportAPI("/gwANI", Usage:="/gwANI /in <in.fasta> [/fast /out <out.Csv>]")>
    <Group(CLIGrouping.DNA_ComparativeTools)>
    Public Function gwANI(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".gwANI.Csv")
        Dim fast As Boolean = args.GetBoolean("/fast")

        Call gwANIExtensions.Evaluate([in], out, fast)
        Return 0
    End Function

    <ExportAPI("/Sigma",
               Usage:="/Sigma /in <in.fasta> [/out <out.Csv> /simple /round <-1>]")>
    <Group(CLIGrouping.DNA_ComparativeTools)>
    Public Function Sigma(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".Sigma.Csv")
        Dim fasta As New FastaFile([in])
        Dim simple As Boolean = args.GetBoolean("/simple")
        Dim round As Integer = args.GetValue("/round", -1)
        Dim keys As String() =
            If(simple,
            fasta.ToArray(AddressOf IdentityResult.SimpleTag),
            fasta.ToArray(Function(x) x.Title))

        Using writer As New WriteStream(Of IdentityResult)(out, metaKeys:=keys)
            For Each x As IdentityResult In IdentityResult.SigmaMatrix(fasta, round, simple)
                Call writer.Flush(x)
            Next

            Return 0
        End Using
    End Function

    <ExportAPI("/CAI", Usage:="/CAI /ORF <orf_nt.fasta> [/out <out.XML>]")>
    <Group(CLIGrouping.DNA_ComparativeTools)>
    Public Function CAI(args As CommandLine) As Integer
        Dim orf$ = args <= "/ORF"
        Dim out As String = args.GetValue("/out", orf.TrimSuffix & "_CodonAdaptationIndex.XML")
        Dim prot As FastaToken = FastaToken.LoadNucleotideData(orf)
        Dim table As New CodonAdaptationIndex(New RelativeCodonBiases(prot))
        Return table.SaveAsXml(out).CLICode
    End Function
End Module