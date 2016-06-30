Imports LANS.SystemsBiology.AnalysisTools.ComparativeGenomics
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.MotifScans
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.Similarity
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.ComponentModel
Imports LANS.SystemsBiology.SequenceModel
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace Analysis.MotifScans

    <PackageNamespace("MotifScansTools.Similarity", Publisher:="amethyst.asuka@gcmodeller.org")>
    Public Module TestAPI

        <ExportAPI("PWM")>
        Public Function PWM(<Parameter("Nt.Seq", "ATCG base, nucleotide sequence.")> sequence As String) As MotifPM()
            Dim Nt = New FASTA.FastaToken With {.SequenceData = sequence}
            Return MotifDeltaSimilarity.PWM(Nt)
        End Function

        <ExportAPI("NT")>
        Public Function Nt(sequence As String) As NucleotideModels.NucleicAcid
            Return New NucleotideModels.NucleicAcid(sequence)
        End Function

        <ExportAPI("PWM")>
        Public Function PWM(sequence As FASTA.FastaToken) As MotifPM()
            Return MotifDeltaSimilarity.PWM(sequence)
        End Function

        <ExportAPI("DeltaSimilarity")>
        Public Function DeltaSimilarity(f As IEnumerable(Of MotifPM), g As IEnumerable(Of MotifPM)) As Double
            Return MotifDeltaSimilarity.Sigma(f.ToArray, g.ToArray)
        End Function

        <ExportAPI("DeltaSimilarity")>
        Public Function DeltaSimilarity(f As FASTA.FastaToken, g As FASTA.FastaToken) As Double
            Return DifferenceMeasurement.Sigma(f, g)
        End Function

        <ExportAPI("DeltaSimilarity")>
        Public Function DeltaSimilarity(f As NucleotideModels.NucleicAcid, g As NucleotideModels.NucleicAcid) As Double
            Return DifferenceMeasurement.Sigma(f, g)
        End Function

        <ExportAPI("DeltaSimilarity")>
        Public Function DeltaSimilarity(f As AnnotationModel, g As AnnotationModel) As Double
            Return MotifDeltaSimilarity.Sigma(f.PspMatrix, g.PspMatrix)
        End Function

        <ExportAPI("DeltaSimilarity")>
        Public Function DeltaSimilarity(f As AnnotationModel, g As NucleotideModels.NucleicAcid) As Double
            Return MEME_Suite.Analysis.Similarity.Sigma(f.PspMatrix, g:=MEME_Suite.Analysis.Similarity.PWM(g.ToArray))
        End Function
    End Module
End Namespace