#Region "Microsoft.VisualBasic::8f67239a336805e046a883ff1d13628c, CLI_tools\Solver.FBA\CLI\Solver.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'#Region "Microsoft.VisualBasic::790e704d7a410daa9c7aa99bbf5bc299, CLI_tools\Solver.FBA\CLI\Solver.vb"

'' Author:
'' 
''       asuka (amethyst.asuka@gcmodeller.org)
''       xie (genetics@smrucc.org)
''       xieguigang (xie.guigang@live.com)
'' 
'' Copyright (c) 2018 GPL3 Licensed
'' 
'' 
'' GNU GENERAL PUBLIC LICENSE (GPL3)
'' 
'' 
'' This program is free software: you can redistribute it and/or modify
'' it under the terms of the GNU General Public License as published by
'' the Free Software Foundation, either version 3 of the License, or
'' (at your option) any later version.
'' 
'' This program is distributed in the hope that it will be useful,
'' but WITHOUT ANY WARRANTY; without even the implied warranty of
'' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'' GNU General Public License for more details.
'' 
'' You should have received a copy of the GNU General Public License
'' along with this program. If not, see <http://www.gnu.org/licenses/>.



'' /********************************************************************************/

'' Summaries:

'' Module CLI
'' 
''     Function: Solve
'' 
'' /********************************************************************************/

'#End Region

'Imports Microsoft.VisualBasic.ApplicationServices.Terminal.STDIO
'Imports Microsoft.VisualBasic.CommandLine
'Imports Microsoft.VisualBasic.CommandLine.Reflection
'Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage
'Imports SMRUCC.genomics.Model
'Imports SMRUCC.genomics.Model.SBML.FLuxBalanceModel
'Imports SMRUCC.genomics.Model.SBML.Level2.Elements

'Partial Module CLI

'    ''' <summary>
'    ''' Solve a metabolism network model using the FBA method, the model data was comes from a sbml model or compiled gcml model.
'    ''' (使用FBA方法对一个代谢网络问题进行求解，模型数据来自于一个SBML文件或者一个已经编译好的模型文件)
'    ''' </summary>
'    ''' <param name="args"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    <ExportAPI("/Solve", Info:="solve a FBA model from a specific (SBML) model file.",
'        Usage:="/solve -i <sbml_file> -o <output_result_dir> -d <max/min> [-m <sbml/model> -f <object_function> -knock_out <gene_id_list>]",
'        Example:="solve -i ""/home/xieguigang/BLAST/db/MetaCyc/xcc8004/fba.xml"" -o ""/home/xieguigang/Desktop/8004"" -m sbml -f default -d max -knock_out XC_1184,XC_3631")>
'    <Argument("-i", Description:="")>
'    <Argument("-o", Description:="The directory for the output result.")>
'    <Argument("-m", True, Description:="")>
'    <Argument("-f", True,
'        Description:="Optional, Set up the objective function for the fba linear programming problem, its value can be a expression, default or all." & vbCrLf &
'                     " <expression> - a user specific expression for objective function, it can be a expression or a text file name if the first character is @ in the switch value." & vbCrLf &
'                     " default - the program generate the objective function using the objective coefficient value which defines in each reaction object;" & vbCrLf &
'                     " all - set up all of the reaction objective coeffecient factor to 1, which means all of the reaction flux will use for objective function generation.")>
'    <Argument("-d", True,
'        Description:="Optional, the constraint direction of the objective function for the fba linear programming problem, " & vbCrLf &
'                     "if this switch option is not specific by the user then the program will use the direction which was defined in the FBA model file " & vbCrLf &
'                     "else if use specific this switch value then the user specific value will override the direction value in the FBA model.")>
'    <Argument("-knock_out", True,
'        Description:="Optional, this switch specific the id list that of the gene will be knock out in the simulation, this switch option only works in the advanced fba model file." & vbCrLf &
'                     "value string format: each id can be seperated by the comma character and the id value can be both of the genbank id or a metacyc unique-id value.")>
'    Public Function Solve(args As CommandLine) As Integer
'        Dim Model As I_FBAC2(Of speciesReference)
'        Dim Input As String = args("-i")
'        Dim Output As String = args("-o")

'        If String.Equals(args("-m"), "sbml") Then
'            printf("Ready to parse a FBA model from the sbml model file: %s", Input)
'            Model = SBML.Level2.XmlFile.Load(Input)
'        ElseIf String.Equals(args("-m"), "fba") OrElse String.Equals(args("-m"), "default") Then
'            printf("Ready for load a FBA model from compiled model file: %s", Input)
'            printf("Loading...")
'            Model = FBACompatibility.Model.Load(Input)
'        Else
'            Model = BacterialModel.Load(Input)
'        End If

'        'Using RSolver As FBA_RScript_Builder.RSolver = New FBA_RScript_Builder.RSolver(RScript:=FBA_RScript_Builder.Compile(Model))
'        '    Dim Csv As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = RSolver.RSolving(Model, Program.Profile.RBin, CommandLine("-f"), CommandLine("-d")) '计算得到代谢组的流量分布
'        '    If Csv Is Nothing Then
'        '        Return -1
'        '    Else
'        '        Printf("Saving the EXCEL to the location: %s", Output)
'        '        Call FileIO.FileSystem.CreateDirectory(Output)
'        '        Call Csv.Save(Output & "/fba.csv")
'        '    End If
'        'End Using

'        printf("Work completed!")

'        Return 0
'    End Function
'End Module

