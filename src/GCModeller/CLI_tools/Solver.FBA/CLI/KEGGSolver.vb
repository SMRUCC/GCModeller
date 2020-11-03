#Region "Microsoft.VisualBasic::38536795797ce3b2c5122ef66c2ef8a5, CLI_tools\Solver.FBA\CLI\KEGGSolver.vb"

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

    ' Module CLI
    ' 
    '     Function: KEGGSolver
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.Analysis.FBA_DP
Imports SMRUCC.genomics.Assembly.KEGG.Archives.Xml

Partial Module CLI

    <ExportAPI("/Solver.KEGG", Usage:="/Solver.KEGG /in <model.xml> /objs <locus.txt> [/out <outDIR>]")>
    <ArgumentAttribute("/objs", False,
                   Description:="This parameter defines the objective function in the FBA solver, is a text file which contains a list of genes locus, 
                   and these genes locus is associated to a enzyme reaction in the FBA model.")>
    Public Function KEGGSolver(args As CommandLine) As Integer
        Dim inModel As String = args("/in")
        Dim objs As String = args("/objs")
        Dim out As String = args.GetValue("/out", inModel.TrimSuffix & ", " & basename(objs) & "/")
        Dim model As XmlModel = inModel.LoadXml(Of XmlModel)
        Dim locus As String() = objs.ReadAllLines
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
