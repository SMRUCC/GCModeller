Imports SMRUCC.genomics.GCModeller.AnalysisTools.ModelSolvers.FBA.FBA_OUTPUT
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.FileStream.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Scripting.MetaData

<Cite(Title:="What is flux balance analysis?",
      Abstract:="Flux balance analysis is a mathematical approach for analyzing the flow of metabolites through a metabolic network. 
      This primer covers the theoretical basis of the approach, several practical examples and a software toolbox for performing the calculations.",
      AuthorAddress:="University of California San Diego, La Jolla, California, USA.",
      Authors:="Orth, J. D.
Thiele, I.
Palsson, B. O.",
      DOI:="10.1038/nbt.1614",
      ISSN:="1546-1696 (Electronic)
1087-0156 (Linking)",
      Issue:="3",
      Journal:="Nat Biotechnol",
      Keywords:="Biomass
Escherichia coli/metabolism
*Metabolic Networks and Pathways
*Models, Biological
Oxygen/metabolism
Phenotype
Systems Biology/*methods",
      Notes:="Orth, Jeffrey D
Thiele, Ines
Palsson, Bernhard O
eng
R01 GM057089/GM/NIGMS NIH HHS/
R01 GM057089-12/GM/NIGMS NIH HHS/
Research Support, N.I.H., Extramural
2010/03/10 06:00
Nat Biotechnol. 2010 Mar;28(3):245-8. doi: 10.1038/nbt.1614.",
      Pages:="245-8",
      PubMed:=20212490,
      StartPage:=245,
      URL:="http://www.ncbi.nlm.nih.gov/pubmed/20212490",
      Volume:=28,
      Year:=2010)>
<[PackageNamespace]("Solver.FBA",
                    Url:="http://code.google.com/p/genome-in-code",
                    Publisher:="xie.guigang@gmail.com",
                    Description:="Simple metabolism network optimization solver.")>
Public Module ShellScriptAPI

    <ExportAPI("Project.Load.Simpheny")>
    Public Function LoadProject(DIR As String) As FBA.Simpheny.Project
        Return FBA.Simpheny.Project.LoadProject(DIR)
    End Function

    <ExportAPI("LoadModel.From.CsvTabular", Info:="Load FBA model file from the Csv tabular format GCModeller virtual cell model file.")>
    Public Function LoadData(Model As XmlresxLoader) As DataModel.FluxObject()
        Dim LQuery = (From FluxObject As FileStream.MetabolismFlux In Model.MetabolismModel Select FluxObject.CreateObject).ToArray
        Return LQuery
    End Function

    <ExportAPI("get.full_objective")>
    Public Function FullObjective(Model As DataModel.FluxObject()) As String()
        Return (From Flux In Model Select Flux.Identifier).ToArray
    End Function

    <ExportAPI("create_model.from_csvtabular")>
    Public Function CreateModel(Model As DataModel.FluxObject(), ObjectiveFunction As Object()) As lpSolveRModel
        Return New Models.CsvTabular(Model, (From obj In ObjectiveFunction Let value As String = obj.ToString Select value).ToArray)
    End Function

    <ExportAPI("set.objective_factors")>
    Public Sub SetObjectives(model As lpSolveRModel, factors As Object())
        Call model.SetObjectiveFunc((From obj In factors Let value As String = obj.ToString Select value).ToArray)
    End Sub

    <ExportAPI("FBA.Solve")>
    Public Function SolveModel(Model As lpSolveRModel,
                               <Parameter("R_HOME", "The R program installed location.")> R_HOME As String) As TabularOUT()
        Dim Solver As New FBA.FBAlpRSolver(rBin:=R_HOME)
        Dim Result = Solver.RSolving(Model)
        Dim out As TabularOUT() = Result.CreateDataFile(Model)
        Return out
    End Function

    <ExportAPI("Write.FBA_Result")>
    Public Function SaveData(result As KeyValuePair(Of Double, KeyValuePairObject(Of String, Double)()), saveto As String) As DocumentStream.File
        Dim ResultFile As DocumentStream.File = New DocumentStream.File + {"ObjectiveFunction", CStr(result.Key)} + From item In result.Value Select New DocumentStream.RowObject({item.Key, item.Value})
        Call ResultFile.Save(saveto, False)
        Return ResultFile
    End Function

    <ExportAPI("Result.Get.FluxDistributions")>
    Public Function GetFluxDistribution(result As KeyValuePair(Of Double, KeyValuePairObject(Of String, Double)())) As KeyValuePairObject(Of String, Double)()
        Return result.Value
    End Function

    <ExportAPI("SolverOutput2Csv")>
    Public Function ExportResult(<Parameter("Solver.Out")> SolverOut As KeyValuePair(Of Double, KeyValuePairObject(Of String, Double)())) As DocumentStream.File
        Dim Output As DocumentStream.File =
            New DocumentStream.File + {"ObjectiveFunction", CStr(SolverOut.Key)} + From item
                                                                                   In SolverOut.Value
                                                                                   Select New DocumentStream.RowObject From {item.Key, item.Value.ToString}
        Return Output
    End Function

    '<ExportAPI("CreateModel.From.Chipdata")>
    'Public Function CreateModel(Chipdata As MatrixFrame,
    '                            Optional SystemStableStatus As Boolean = False,
    '                            Optional ExperimentId As String = "1",
    '                            Optional PccCutoff As Double = 0.65,
    '                            Optional ObjectiveFunction As String() = Nothing) As Models.GeneExpressions
    '    Return New Models.GeneExpressions(Chipdata, SystemStableStatus, ExperimentId, PccCutoff, ObjectiveFunction)
    'End Function
End Module
