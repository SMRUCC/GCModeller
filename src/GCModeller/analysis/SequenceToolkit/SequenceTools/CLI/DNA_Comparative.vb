Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998.CAI
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998.CAI.XML
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.gwANI
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module Utilities

    <ExportAPI("/Rule.dnaA_gyrB",
               Usage:="/Rule.dnaA_gyrB /genome <genbank.gb> [/out <out.fasta>]")>
    Public Function dnaA_gyrB_rule(args As CommandLine) As Integer
        Dim in$ = args <= "/genome"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "_dnaA-gyrB.fasta")
        Dim genome As GBFF.File = GBFF.File.Load(in$)
        Return genome _
            .dnaA_gyrB _
            .Save(out, Encodings.ASCII) _
            .CLICode
    End Function

    <ExportAPI("/Rule.dnaA_gyrB.Matrix",
               Usage:="/Rule.dnaA_gyrB.Matrix /genomes <genomes.gb.DIR> [/out <out.csv>]")>
    Public Function RuleMatrix(args As CommandLine) As Integer
        Dim in$ = args <= "/genomes"
        Dim out As String = args.GetValue("/out", [in].TrimDIR & ".dnaA-gyrB.sigma_matrix.csv")
        Dim genomes As GBFF.File() = (ls - l - r - {"*.gb", "*.gbk"} <= in$) _
            .Select(AddressOf GBFF.File.Load) _
            .ToArray
        Dim matrix As IdentityResult() = IdentityResult _
            .SigmaMatrix(genomes) _
            .ToArray
        Return matrix.SaveTo(out).CLICode
    End Function

    <ExportAPI("/gwANI", Usage:="/gwANI /in <in.fasta> [/fast /out <out.Csv>]")>
    <Group(CLIGrouping.DNA_ComparativeTools)>
    Public Function gwANI(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".gwANI.Csv")
        Dim fast As Boolean = args.GetBoolean("/fast")
        Call gwANIExtensions.Evaluate([in], out, fast)
        Return 0
    End Function

    ''' <summary>
    ''' 计算基因组序列的同质性
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
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
            fasta.Select(Function(x) x.Title))

        Using writer As New WriteStream(Of IdentityResult)(out, metaKeys:=keys)
            For Each x As IdentityResult In IdentityResult.SigmaMatrix(fasta, round, simple)
                Call writer.Flush(x)
            Next

            Return 0
        End Using
    End Function

    ''' <summary>
    ''' 基因组的密码子偏好性计算
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/CAI", Usage:="/CAI /ORF <orf_nt.fasta> [/out <out.XML>]")>
    <Argument("/ORF", False, CLITypes.File,
              PipelineTypes.std_in,
              AcceptTypes:={GetType(FastaFile), GetType(FastaToken)},
              Description:="If the target fasta file contains multiple sequence, then the CAI table xml will output to a folder or just output to a xml file if only one sequence in thye fasta file.")>
    <Group(CLIGrouping.DNA_ComparativeTools)>
    Public Function CAI(args As CommandLine) As Integer
        Dim orf$ = args <= "/ORF"
        Dim fasta As New FastaFile(orf)

        If fasta.NumberOfFasta = 1 Then
            Dim out$ = args.GetValue("/out", orf.TrimSuffix & "_CodonAdaptationIndex.XML")
            Dim prot As FastaToken = fasta.First
            Dim table As New CodonAdaptationIndex(New RelativeCodonBiases(prot))
            Return table.SaveAsXml(out).CLICode
        Else
            Dim out$ = args.GetValue("/out", orf.TrimSuffix & "_CodonAdaptationIndex/")

            For Each prot As FastaToken In fasta
                Dim table As New CodonAdaptationIndex(New RelativeCodonBiases(prot))
                Dim path$ = out & "/" & prot.Title.NormalizePathString & ".XML"
                Call table.SaveAsXml(path)
            Next

            Return 0
        End If
    End Function
End Module