Imports Microsoft.VisualBasic.CommandLine.Reflection

Partial Module CLI

    ' /Analysis.Phenotype /in <MetaCyc.Sbml> /reg <footprints.csv> /obj <list/path/module-xml> [/obj-type <lst/pathway/module> /params <rfba.parameters.xml> /stat <stat.Csv> /sample <sampleTable.csv> /modify <locus_modify.csv> /out <outDIR>]

    ''' <summary>
    ''' Batch task schedule for <see cref="CLI.rFBABatch(CommandLine.CommandLine)"/>
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/gcFBA.Batch",
               Usage:="/gcFBA.Batch /model <model.sbml> /phenotypes <KEGG_modules/pathways.DIR> /footprints <footprints.csv> [/obj-type <pathway/module> /params <rfba.parameters.xml> /stat <RPKM-stat.Csv> /sample <sampleTable.csv> /modify <locus_modify.csv> /out <outDIR> /parallel <2>]")>
    Public Function PhenotypeAnalysisBatch(args As CommandLine.CommandLine) As Integer
        Dim model As String = args("/model")
        Dim phenos As String = args("/phenotypes").Replace("\", "/")
        Dim out As String = args.GetValue("/out", model.TrimFileExt & "-" & phenos.Split("/"c).Last)
        Dim footprints As String = args("/footprints")
        Dim obj As String = args.GetValue("/obj-type", "pathway")
        Dim params As String = args("/params")
        Dim stat As String = args("/stat")
        Dim samples As String = args("/sample")
        Dim modifier As String = args("/modify")
        Dim parallel As Integer = args.GetValue("/parallel", 2)

        Dim phenoROOT As String = FileIO.FileSystem.GetDirectoryInfo(phenos).FullName
        Dim files = (From file As String
                     In FileIO.FileSystem.GetFiles(phenoROOT, FileIO.SearchOption.SearchAllSubDirectories, "*.xml")
                     Select file,
                         cat = file.TrimFileExt.Replace(phenoROOT, "")).ToArray
        Dim CLI = (From x
                   In files
                   Select __CLIBuilder(model, x.file, out & "/" & x.cat, footprints, obj, params, stat, samples, modifier))
        Call App.SelfFolks(CLI, parallel)

        Return 0
    End Function

    Private Function __CLIBuilder(model As String,
                                  phenos As String,
                                  out As String,
                                  footprints As String,
                                  obj As String,
                                  params As String,
                                  stat As String,
                                  samples As String,
                                  modifier As String) As String

        Dim args As New Dictionary(Of String, String) From {
 _
            {"/in", model},       ' /in <MetaCyc.Sbml>  
            {"/obj", phenos},     ' /obj <list/path/module-xml>
            {"/out", out},        ' /out <outDIR>
            {"/reg", footprints}, ' /reg <footprints.csv> 
            {"/obj-type", obj},   ' /obj-type <lst/pathway/module>
            {"/params", params},  ' /params <rfba.parameters.xml>
            {"/stat", stat},      ' /stat <stat.Csv>
            {"/sample", samples}, ' /sample <sampleTable.csv>
            {"/modify", modifier} ' /modify <locus_modify.csv>
        }
        Return CommandLine.SimpleBuilder("/Analysis.Phenotype", args)
    End Function
End Module