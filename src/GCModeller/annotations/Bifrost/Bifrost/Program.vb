Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.Annotation.MetaEuk
Imports SMRUCC.genomics.Annotation.Prodigal
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Program

    Public Sub PrintUsage()
        Console.WriteLine("Bifrost - Prokaryotic/Eukaryotic Genome Gene Prediction Tool")
        Console.WriteLine("Based on Prodigal/MetaEuk algorithm:")
        Console.WriteLine("   - Prodigal - PROkaryotic DYnamic programming Gene-finding ALgorithm")
        Console.WriteLine("   - MetaEuk- homology-based exon chain optimization")
        Console.WriteLine()
        Console.WriteLine("USAGE:")
        Console.WriteLine("  bifrost <prodigal/metaeuk> --contigs <MAGs.fasta> [--reference <proteins.fasta>] [--model <prodigal.model>] [options]")
        Console.WriteLine()
        Console.WriteLine("REQUIRED ARGUMENTS:")
        Console.WriteLine("  -c, --contigs <file>       Input MAGs contigs in FASTA format")
        Console.WriteLine("  -r, --reference <file>     Reference protein database in FASTA format")
        Console.WriteLine("  -m, --model <file>         Trained model for prodigal method")
        Console.WriteLine()
        Console.WriteLine("OUTPUT OPTIONS:")
        Console.WriteLine("  -o, --output <prefix>      Output file prefix (default: bifrost_out)")
        Console.WriteLine("                             Generates: <prefix>.faa, <prefix>.gff3, <prefix>.tsv")
        Console.WriteLine()
        Console.WriteLine("ALGORITHM PARAMETERS:")
        Console.WriteLine("  -e, --evalue <float>       E-value threshold (default: 1e-3)")
        Console.WriteLine("  --min-identity <float>     Minimum sequence identity fraction (default: 0.2)")
        Console.WriteLine("  --min-fragment-length <int> Minimum candidate fragment length in AA (default: 15)")
        Console.WriteLine("  --gap-penalty <float>      Gap penalty coefficient lambda (default: 0.5)")
        Console.WriteLine("  --max-intron <int>         Maximum intron length in bp (default: 50000)")
        Console.WriteLine("  --min-exon-score <float>   Minimum exon bitscore (default: 20.0)")
        Console.WriteLine("  --overlap-threshold <int>  Overlap bp for conflict detection (default: 10)")
        Console.WriteLine("  --exon-overlap-fraction <float> Exon overlap fraction for redundancy (default: 0.3)")
        Console.WriteLine()
        Console.WriteLine("OTHER OPTIONS:")
        Console.WriteLine("  -v, --verbose              Enable verbose output")
        Console.WriteLine("  -h, --help                 Show this help message")
        Console.WriteLine()
        Console.WriteLine("ALGORITHM PIPELINE:")
        Console.WriteLine("  1. Six-frame translation of contigs -> candidate coding fragments")
        Console.WriteLine("  2. Smith-Waterman local alignment against reference proteins")
        Console.WriteLine("  3. Group hits by (Target, Contig, Strand) -> TCS groups")
        Console.WriteLine("  4. Dynamic programming: optimal exon chain per TCS group")
        Console.WriteLine("  5. Redundancy removal: cluster overlapping predictions, pick representative")
        Console.WriteLine("  6. Same-strand conflict resolution: keep best E-value")
        Console.WriteLine("  7. Output: Protein FASTA (.faa), GFF3 (.gff3), TSV summary (.tsv)")
    End Sub

    Public Function Main(args As String()) As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    <ExportAPI("metaeuk")>
    Public Function MetaEuk(args As CommandLine) As Integer
        Dim config As New MetaEukConfig With {
            .ReferenceFile = args("--reference"),
            .ContigsFile = args("--contigs"),
            .OutputPrefix = args("--output")
        }
        Dim predicts As GenePrediction() = MetaEukWorker.Predict(config).ToArray

        Call MetaEukWorker.ExportResult(predicts, config)

        Return 0
    End Function

    <ExportAPI("prodigal")>
    Public Function Prodigal(args As CommandLine) As Integer
        Dim MAGs As String = args("--contigs")
        Dim outprefix As String = args("--output")
        Dim predicts = ProdigalWorker.GenePrediction(FastaFile.Read(MAGs)).ToArray

        Call ProdigalWorker.ExportResult(predicts, outprefix)

        Return 0
    End Function
End Module
