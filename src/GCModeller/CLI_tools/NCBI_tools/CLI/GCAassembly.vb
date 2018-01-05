Imports System.IO
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/gpff.fasta")>
    <Usage("/gpff.fasta /in <gpff.txt> [/out <out.fasta>]")>
    Public Function gpff2Fasta(args As CommandLine) As Integer
        Using out As StreamWriter = args.OpenStreamOutput("/out", Encodings.ASCII)
            For Each seq In GBFF.File.LoadDatabase(args <= "/in")
                Dim fasta As New FastaToken With {
                    .Attributes = {
                        seq.Locus.AccessionID,
                        seq.Definition.Value
                    },
                    .SequenceData = seq.Origin.SequenceData
                }

                Call out.WriteLine(fasta.GenerateDocument(120))
            Next
        End Using

        Return 0
    End Function
End Module