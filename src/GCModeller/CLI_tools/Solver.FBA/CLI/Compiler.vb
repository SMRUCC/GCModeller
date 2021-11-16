#Region "Microsoft.VisualBasic::8c66932f8aea9d157aa352a9a5ef5b7c, CLI_tools\Solver.FBA\CLI\Compiler.vb"

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

'#Region "Microsoft.VisualBasic::af59a07de42ddb057b3d831c0d05aaf0, CLI_tools\Solver.FBA\CLI\Compiler.vb"

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
''     Function: Compile, CompileMetaCyc, CompileSBML
'' 
'' /********************************************************************************/

'#End Region

'Imports Microsoft.VisualBasic.ApplicationServices.Terminal.STDIO
'Imports Microsoft.VisualBasic.CommandLine
'Imports Microsoft.VisualBasic.CommandLine.Reflection
'Imports SMRUCC.genomics.GCModeller.Assembly
'Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage
'Imports SMRUCC.genomics.Model.SBML.Level2.Elements

'Partial Module CLI

'    <ExportAPI("compile", Info:="Compile data source into a model file so that the fba program can using the data to performing the simulation calculation.",
'        Usage:="compile -i <input_file> -o <output_file> [-if <sbml/metacyc> -of <fba/fba2> -f <objective_function> -d <max/min>]",
'        Example:="compile -i /home/xieguigang/ecoli/ -o /home/xieguigang/ecoli.xml -if metacyc -of fba2 -f v2+v3 -d max")>
'    <Argument("-i", False,
'        Description:="The input datasource path of the compiled model, it can be a MetaCyc data directory or a xml file in sbml format, format was specific by the value of switch '-if'")>
'    <Argument("-o", False,
'        Description:="The output file path of the compiled model file.")>
'    <Argument("-if", True,
'        Description:="Optional, this switch specific the format of the input data source, the fba compiler just support the metacyc database and sbml model currently, default value if metacyc." & vbCrLf &
'                                 " metacyc - the input compiled data source is a metacyc database;" & vbCrLf &
'                                 "sbml - the input compiled data source is a standard sbml language model in level 2.")>
'    <Argument("-of", True,
'        Description:="Optional, this switch specific the format of the output compiled model, it can be a standard fba model or a advanced version of fba model, defualt is a standard fba model." & vbCrLf &
'                                 " fba - the output compiled model is a standard fba model;" & vbCrLf &
'                                 "fba2 - the output compiled model is a advanced version of fba model.")>
'    <Argument("-f", True,
'        Description:="Optional, you can specific the objective function using this switch, default value is the objective function that define in the sbml model file.")>
'    <Argument("-d", True,
'        Description:="Optional, the constraint direction of the objective function in the fba model, default value is maximum the objective function." & vbCrLf &
'                                 " max - the constraint direction is maximum;" & vbCrLf &
'                                 " min - the constraint direction is minimum.")>
'    Public Function Compile(CommandLine As CommandLine) As Integer
'        Dim DataSourceFomat As String = CommandLine("-if")

'        printf("FBA model compiler module [FBA.exe version: %s]", My.Application.Info.Version.ToString)
'        If String.IsNullOrEmpty(DataSourceFomat) Then
'            DataSourceFomat = "metacyc"
'            printf("User not specifc a data source format, use default value 'metacyc' database format.")
'        ElseIf Array.IndexOf(CompileMethods.Keys.ToArray, DataSourceFomat) = -1 Then
'            printf("No such a data source format \'%s\', automatically  select the data source format as default 'metacyc'.", DataSourceFomat)
'        End If
'        printf("Data source format is %s.", DataSourceFomat)
'        Return CompileMethods(DataSourceFomat)(CommandLine)
'    End Function

'    ''' <summary>
'    ''' The compiler support the metacyc database and sbml model file format currently.(编译器当前仅支持MetaCyc数据库和SBML标准模型数据源)
'    ''' </summary>
'    ''' <remarks></remarks>
'    ReadOnly CompileMethods As Dictionary(Of String, Func(Of CommandLine, Integer)) =
'        New Dictionary(Of String, Func(Of CommandLine, Integer)) From {
'            {"sbml", AddressOf CLI.CompileSBML},
'            {"metacyc", AddressOf CLI.CompileMetaCyc}}

'    ''' <summary>
'    ''' SBML --> FBA
'    ''' </summary>
'    ''' <param name="args"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Private Function CompileSBML(args As CommandLine) As Integer
'        'Dim SBML As SMRUCC.genomics.Assembly.SBML.Level2.XmlFile = CommandLine("-i")
'        'Dim FBAModel As SMRUCC.genomics.FBA.FBA = FBA.Compile(SBML)
'        'FBAModel.ObjectiveFunction = CommandLine("-f")
'        'FBAModel.Direction = CommandLine("-d")
'        'Call FileIO.FileSystem.WriteAllText(CommandLine("-o"), FBAModel.GetXml, append:=False)
'        'Return 0
'        Dim SBML As String = args("-i")
'        Dim FBAModel As GCMarkupLanguage.FBACompatibility.Model =
'            GCMarkupLanguage.FBACompatibility.API.Compile(SBMl2:=SBML)

'        Call GCMarkupLanguage.Replacer.ApplyReplacements(Of
'          speciesReference,
'          FBACompatibility.Model)(FBAModel, Program.Profile.Gcc.Filters)
'        Call FBAModel.Save(args("-o"))
'        Return 0
'    End Function

'    ''' <summary>
'    ''' metacyc --> FBA/FBA2
'    ''' </summary>
'    ''' <param name="CommandLine"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Private Function CompileMetaCyc(CommandLine As CommandLine) As Integer
'        If String.Equals(CommandLine("-of"), "fba") Then
'            'Dim SBML As SMRUCC.genomics.Assembly.SBML.Level2.XmlFile = CommandLine("-i") & "/metabolic-reactions.sbml"
'            'Dim FBAModel As SMRUCC.genomics.FBA.FBA_RScript_Builder = FBA.Compile()
'            'FBAModel.ObjectiveFunction = CommandLine("-f")
'            'FBAModel.Direction = CommandLine("-d")
'            'Call FileIO.FileSystem.WriteAllText(CommandLine("-o"), FBAModel.GetXml, append:=False)
'        Else
'            ' Dim Compiler As SMRUCC.genomics.Assembly.Xml.Model = SMRUCC.genomics.Assembly.Xml.Model.BuildFrom(CommandLine("-i"), {Program.Profile.Filter.Old, Program.Profile.Filter.[New]})
'            'Call Compiler.ApplyFilter(Program.Profile.Filter.Old, Program.Profile.Filter.[New])
'            'Call Compiler.Save(CommandLine("-o"))
'        End If
'        Return 0
'    End Function
'End Module
