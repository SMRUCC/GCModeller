#Region "Microsoft.VisualBasic::8e98ad8c4ad2763f9aa34f9aa95ae025, ..\GCModeller\CLI_tools\Solver.FBA\CLI\KEGGSolver.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports SMRUCC.genomics.Analysis.FBA_DP
Imports SMRUCC.genomics.Assembly.KEGG.Archives.Xml

Partial Module CLI

    <ExportAPI("/Solver.KEGG", Usage:="/Solver.KEGG /in <model.xml> /objs <locus.txt> [/out <outDIR>]")>
    <ParameterInfo("/objs", False,
                   Description:="This parameter defines the objective function in the FBA solver, is a text file which contains a list of genes locus, 
                   and these genes locus is associated to a enzyme reaction in the FBA model.")>
    Public Function KEGGSolver(args As CommandLine.CommandLine) As Integer
        Dim inModel As String = args("/in")
        Dim objs As String = args("/objs")
        Dim out As String = args.GetValue("/out", inModel.TrimSuffix & ", " & IO.Path.GetFileNameWithoutExtension(objs) & "/")
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

