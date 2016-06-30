Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Cytoscape.NetworkModel
Imports LANS.SystemsBiology.Assembly.SBML.Level2
Imports Microsoft.VisualBasic.CommandLine.Reflection

Partial Module CLI

    <ExportAPI("/Net.rFBA", Usage:="/Net.rFBA /in <metacyc.sbml> /fba.out <flux.Csv> [/out <outDIR>]")>
    Public Function net_rFBA(args As CommandLine.CommandLine) As Integer
        Dim inSBML As String = args("/in")
        Dim fbaResult As String = args("/fba.out")
        Dim outDIR As String = args.GetValue("/out", inSBML.TrimFileExt & "-" & fbaResult.GetJustFileName & "/")
        Dim net = SBMLrFBA.CreateNetwork(XmlFile.Load(inSBML), SBMLrFBA.LoadFBAResult(fbaResult))
        Return net.Save(outDIR, Encodings.ASCII.GetEncodings).CLICode
    End Function
End Module
