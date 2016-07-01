Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.Similarity.TOMQuery
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace Analysis.MotifScans

    ''' <summary>
    ''' 不太建议使用这个模块进行长序列的比对
    ''' </summary>
    <PackageNamespace("Site.Scanner", Publisher:="amethyst.asuka@gcmodeller.org")>
    Public Module SiteScanner

        <ExportAPI("Fa.LDM")>
        <Extension> Public Function CreateModel(fa As SequenceModel.FASTA.FastaToken) As AnnotationModel
            Dim seq As ResidueSite() = Time(Function() fa.SequenceData.ToArray(Function(x) New ResidueSite(x)))
            Return New AnnotationModel With {
                .Uid = fa.Title,
                .Sites = {
                    New Site With {
                        .Name = fa.Title,
                        .Start = 1,
                        .Right = fa.Length
                    }
                },
                .PWM = seq
            }
        End Function

        <ExportAPI("Scan")>
        <Extension> Public Function Scan(Motif As AnnotationModel, genome As SequenceModel.FASTA.FastaToken, params As Parameters) As Analysis.Similarity.TOMQuery.Output
            Return Motif.Scan(genome.CreateModel, params)
        End Function

        <ExportAPI("Scan")>
        <Extension> Public Function Scan(Motif As AnnotationModel, genome As AnnotationModel, params As Parameters) As Analysis.Similarity.TOMQuery.Output
            params.Parallel = True
            Return SWTom.Compare(Motif, genome, params)
        End Function
    End Module
End Namespace