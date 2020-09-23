#Region "Microsoft.VisualBasic::2fddf5e8db4a4598a1761f7d082bcf8e, sub-system\FBA\FBA_DP\ShellScriptAPI.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Module ShellScriptAPI
    ' 
    '     Function: ExportResult, GetFluxDistribution, LoadProject, SaveData, SolveModel
    ' 
    '     Sub: SetObjectives
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.FBA_DP.FBA_OUTPUT

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
<Package("Solver.FBA",
                    Url:="http://code.google.com/p/genome-in-code",
                    Publisher:="xie.guigang@gmail.com",
                    Description:="Simple metabolism network optimization solver.")>
Public Module ShellScriptAPI

    <ExportAPI("Project.Load.Simpheny")>
    Public Function LoadProject(DIR As String) As Simpheny.Project
        Return Simpheny.Project.LoadProject(DIR)
    End Function

    '<ExportAPI("LoadModel.From.CsvTabular", Info:="Load FBA model file from the Csv tabular format GCModeller virtual cell model file.")>
    'Public Function LoadData(Model As XmlresxLoader) As DataModel.FluxObject()
    '    Dim LQuery = (From FluxObject As FileStream.MetabolismFlux In Model.MetabolismModel Select FluxObject.CreateObject).ToArray
    '    Return LQuery
    'End Function

    '<ExportAPI("get.full_objective")>
    'Public Function FullObjective(Model As DataModel.FluxObject()) As String()
    '    Return (From Flux In Model Select Flux.Identifier).ToArray
    'End Function

    '<ExportAPI("create_model.from_csvtabular")>
    'Public Function CreateModel(Model As DataModel.FluxObject(), ObjectiveFunction As Object()) As lpSolveRModel
    '    Return New Models.CsvTabular(Model, (From obj In ObjectiveFunction Let value As String = obj.ToString Select value).ToArray)
    'End Function

    <ExportAPI("set.objective_factors")>
    Public Sub SetObjectives(model As lpSolveRModel, factors As Object())
        Call model.SetObjectiveFunc((From obj In factors Let value As String = obj.ToString Select value).ToArray)
    End Sub

    <ExportAPI("FBA.Solve")>
    Public Function SolveModel(Model As lpSolveRModel,
                               <Parameter("R_HOME", "The R program installed location.")> R_HOME As String) As TabularOUT()
        Dim Solver As New FBAlpRSolver(rBin:=R_HOME)
        Dim Result = Solver.RSolving(Model)
        Dim out As TabularOUT() = Result.CreateDataFile(Model)
        Return out
    End Function

    <ExportAPI("Write.FBA_Result")>
    Public Function SaveData(result As KeyValuePair(Of Double, KeyValuePairObject(Of String, Double)()), saveto As String) As IO.File
        Dim ResultFile As IO.File = New IO.File + {"ObjectiveFunction", CStr(result.Key)} + From item In result.Value Select New IO.RowObject({item.Key, item.Value})
        Call ResultFile.Save(saveto, False)
        Return ResultFile
    End Function

    <ExportAPI("Result.Get.FluxDistributions")>
    Public Function GetFluxDistribution(result As KeyValuePair(Of Double, KeyValuePairObject(Of String, Double)())) As KeyValuePairObject(Of String, Double)()
        Return result.Value
    End Function

    <ExportAPI("SolverOutput2Csv")>
    Public Function ExportResult(<Parameter("Solver.Out")> SolverOut As KeyValuePair(Of Double, KeyValuePairObject(Of String, Double)())) As IO.File
        Dim Output As IO.File =
            New IO.File + {"ObjectiveFunction", CStr(SolverOut.Key)} + From item
                                                                                   In SolverOut.Value
                                                                                   Select New IO.RowObject From {item.Key, item.Value.ToString}
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
