Imports SMRUCC.genomics.Assembly.KEGG.Archives.Xml
Imports SMRUCC.genomics.GCModeller.AnalysisTools.ModelSolvers.FBA
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv

Partial Module CLI

    <ExportAPI("/Solver.KEGG", Usage:="/Solver.KEGG /in <model.xml> /objs <locus.txt> [/out <outDIR>]")>
    <ParameterInfo("/objs", False,
                   Description:="This parameter defines the objective function in the FBA solver, is a text file which contains a list of genes locus, 
                   and these genes locus is associated to a enzyme reaction in the FBA model.")>
    Public Function KEGGSolver(args As CommandLine.CommandLine) As Integer
        Dim inModel As String = args("/in")
        Dim objs As String = args("/objs")
        Dim out As String = args.GetValue("/out", inModel.TrimFileExt & ", " & IO.Path.GetFileNameWithoutExtension(objs) & "/")
        Dim model As XmlModel = inModel.LoadXml(Of XmlModel)
        Dim locus As String() = IO.File.ReadAllLines(objs)
        Dim FBAModel As New Models.KEGGXml(model)
        Call FBAModel.SetGeneObjectives(locus)
        Dim solver As New FBAlpRSolver(GCModeller.FileSystem.GetR_HOME)
        Dim script As String = ""
        Dim result = solver.RSolving(FBAModel, script)
        Dim outTable = result.CreateDataFile(FBAModel)
        Call result.Objective.SaveTo(out & "/ObjectiveFunction.txt")
        Call script.SaveTo(out & "/FBA.RScript.R")
        Return outTable.SaveTo(out & "/Fluxs.csv").CLICode
    End Function

End Module
