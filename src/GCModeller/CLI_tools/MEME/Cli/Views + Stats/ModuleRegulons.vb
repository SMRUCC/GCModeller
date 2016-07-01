Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.FootprintTraceAPI
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.DatabaseServices.ComparativeGenomics.AnnotationTools
Imports SMRUCC.genomics.Toolkits.RNA_Seq
Imports SMRUCC.genomics.DatabaseServices.Regprecise
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv

Partial Module CLI

    <ExportAPI("/Motif.BuildRegulons",
               Usage:="/Motif.BuildRegulons /meme <meme.txt.DIR> /model <FootprintTrace.xml> /DOOR <DOOR.opr> /maps <bbhmappings.csv> /corrs <name/DIR> [/cut <0.65> /out <outDIR>]")>
    Public Function BuildRegulons(args As CommandLine.CommandLine) As Integer
        Dim meme As String = args("/meme")
        Dim model As String = args("/model")
        Dim DOOR As String = args("/DOOR")
        Dim maps As String = args("/maps")
        Dim out As String = args.GetValue("/out", model.TrimFileExt & ".ModuleRegulons/")  ' 主要是需要存放TOMTOM全局比对的图片数据
        Dim LDM = AnnotationModel.LoadMEMEOUT(meme)
        Dim footprints = model.LoadXml(Of FootprintTrace)
        Dim opr = DOOR_API.Load(DOOR)
        Dim mapsTF = maps.LoadCsv(Of bbhMappings)
        Dim corrs = Correlation2.LoadAuto(args("/corrs"))
        Dim cut As Double = args.GetValue("/cut", 0.65)
        Dim istrue As IsTrue = corrs.IsTrueFunc(cut)
        Dim regulons As ModuleMotif() = ToRegulons(LDM, footprints, opr, mapsTF, istrue, out)
        Return regulons.SaveTo(out & "/ModuleRegulons.Csv").CLICode
    End Function
End Module
