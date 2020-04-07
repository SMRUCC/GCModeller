#Region "Microsoft.VisualBasic::281dbf857fb5cfefa2e15f3bb5ef5000, PLAS.NET\PLAS\CLI.vb"

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
    '     Function: Compile, Run
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SSystem

<Package("PLAS.CLI", Category:=APICategories.CLI_MAN, Publisher:="xie.guigang@gmail.com")>
Public Module CLI

    <ExportAPI("Run")>
    <Description("run a model file of the biochemical network system.")>
    <Usage("run -i <model_file> -f <script/model/sbml> [-o <output_csv> /time <-1> /ODEs]")>
    <Example("run -i ""/home/xieguigang/proj/xcc8004.sbml"" -f sbml -chart T -o ""/home/xieguigang/Desktop/xcc8004.csv""")>
    <Argument("-i", False, Description:="The file path of the input model file that will be run on the PLAS program.")>
    <Argument("-f", True,
        Description:="This parameter specific that the file format of the model file which will be run on the PLAS program." & vbCrLf &
                     " script - The input file that specific by the switch parameter ""-i"" is a PLAS script file," & vbCrLf &
                     " model - The input file is a compiled PLAS model, run it directly," & vbCrLf &
                     " sbml - The input file is a sbml model file, it needs to be compiled to a PLAS model first.")>
    <Argument("-chart", True,
        Description:="Optional, This switch specific that PLAS displaying a chart windows after the calculation or not, default is F for not displaying." & vbCrLf &
                     " T - (True) display a chart window after the calculation," & vbCrLf &
                     " F - (False) not display a chart window after the calculation.")>
    <Argument("-o", False, Description:="The file path of the output data file for the calculation.")>
    Public Function Run(args As CommandLine) As Integer
        Return RunMethods(args <= "-f")(args)
    End Function

    <ExportAPI("Compile")>
    <Description("Compile a script file or sbml file into the plas model file.")>
    <Usage("compile -i <file> -f <script/sbml> -o <output_file> [/auto-fix]")>
    <Example("compile -i ""/home/xieguigang/proj/metacyc/xcc8004/17.0/data/metabolic-reactions.sbml"" -f sbml -o ""/home/xieguigang/Desktop/xcc8004.xml""")>
    Public Function Compile(args As CommandLine) As Integer
        Return Compilers(args <= "-f")(args <= "-i", args <= "-o", args("/auto-fix")).CLICode
    End Function
End Module
