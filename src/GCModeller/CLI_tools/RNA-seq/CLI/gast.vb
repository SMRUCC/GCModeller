Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Parallel.Linq
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Analysis.Metagenome.gast
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
                                       Select $"{api} /in {fa.CliPath} /no-suffix"
        For Each arg As String In CLI
            Call arg.__DEBUG_ECHO
        Next

        Return App.SelfFolks(CLI, LQuerySchedule.Recommended_NUM_THREADS)
    End Function

    Const Interval As String = "NNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNN"

    <ExportAPI("/Contacts.Ref", Usage:="/Contacts.Ref /in <in.fasta> /maps <maps.sam> [/out <out.DIR>]")>
    Public Function Contacts(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim inSam As String = args("/maps")
        Dim i As Integer = 1
        Dim contigs As New List(Of SimpleSegment)
        Dim out As String = args.GetValue("/out", [inSam].TrimSuffix & ".Contigs/")
        Dim outNt As String = out & "/nt.fasta"
        Dim outContigs As String = out & "/contigs.csv"
        Dim il As Integer = Interval.Length
        Dim sam As New SamStream(inSam)

        Using writer = outNt.OpenWriter(Encodings.ASCII)
            Dim seqHash = (From x As FastaToken
                           In New FastaFile([in])
                           Select x
                           Group x By x.Title.Split.First Into Group) _
                                .ToDictionary(Function(x) x.First,
                                              Function(x) x.Group.First)

            Call writer.WriteLine("> " & [in].BaseName)

            Dim list As New List(Of FastaToken)

            For Each readMap As AlignmentReads In sam.ReadBlock.Where(Function(x) Not x.RNAME = "*")
                list += seqHash(readMap.RNAME)
            Next

            For Each fa As FastaToken In (From x As FastaToken
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
End Module
