Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.MotifScans
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat.MAST.HTML
Imports LANS.SystemsBiology.DatabaseServices.Regprecise.WebServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace Analysis.MotifScans

    <PackageNamespace("MotifScansTools", Publisher:="xie.guigang@gcmodeller.org")>
    Public Module MotifScansTools

        <ExportAPI("Read.Xml.Regulations")>
        Public Function LoadRegulations(path As String) As Regulations
            Return path.LoadXml(Of Regulations)
        End Function

        <ExportAPI("ScanModel.Create.From.LDM")>
        Public Function CreateModel(MotifLDM As AnnotationModel, Regulations As Regulations,
                                    Optional delta As Double = 80, Optional delta2 As Double = 70, Optional offSet As Integer = 5) As MotifScans
            Return New MotifScans(MotifLDM, Regulations, delta, delta2, offSet)
        End Function

        <ExportAPI("ScanModel.Create.From.LDM")>
        Public Function CreateModel(<Parameter("LDM.Xml", "The file path of the motif ldm xml file.")>
                                    MotifLDM As String,
                                    Regulations As Regulations,
                                    Optional delta As Double = 80, Optional delta2 As Double = 70, Optional offSet As Integer = 5) As MotifScans
            Dim LDM As AnnotationModel = MotifLDM.LoadXml(Of AnnotationModel)
            Return New MotifScans(LDM, Regulations, delta, delta2, offSet)
        End Function

        <ExportAPI("MotifScan")>
        Public Function MotifScan(LDM As MotifScans, Nt As SequenceModel.FASTA.FastaToken) As MatchedSite()
            Return LDM.Mast(Nt)
        End Function
    End Module
End Namespace