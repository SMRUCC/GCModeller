#Region "Microsoft.VisualBasic::d8dadc4f4d947c2e4deb3bc46949c3e9, CLI_tools\gcc\CLI\MetaCyc.vb"

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

'#Region "Microsoft.VisualBasic::f55b760e2f3f1018d770188f8b245121, CLI_tools\gcc\CLI\MetaCyc.vb"

'    ' Author:
'    ' 
'    '       asuka (amethyst.asuka@gcmodeller.org)
'    '       xie (genetics@smrucc.org)
'    '       xieguigang (xie.guigang@live.com)
'    ' 
'    ' Copyright (c) 2018 GPL3 Licensed
'    ' 
'    ' 
'    ' GNU GENERAL PUBLIC LICENSE (GPL3)
'    ' 
'    ' 
'    ' This program is free software: you can redistribute it and/or modify
'    ' it under the terms of the GNU General Public License as published by
'    ' the Free Software Foundation, either version 3 of the License, or
'    ' (at your option) any later version.
'    ' 
'    ' This program is distributed in the hope that it will be useful,
'    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
'    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    ' GNU General Public License for more details.
'    ' 
'    ' You should have received a copy of the GNU General Public License
'    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



'    ' /********************************************************************************/

'    ' Summaries:

'    ' Module CLI
'    ' 
'    '     Function: AddNewPair, AddRule, CompileMetaCyc
'    ' 
'    ' /********************************************************************************/

'#End Region

'Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
'Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
'Imports Microsoft.VisualBasic.CommandLine.Reflection
'Imports Microsoft.VisualBasic.Data.csv
'Imports Microsoft.VisualBasic.Scripting.MetaData
'Imports SMRUCC.genomics.GCModeller.Assembly
'Imports SMRUCC.genomics.Model.SBML.Specifics.MetaCyc

'<Package("GCModeller.Compiler.CLI", Category:=APICategories.CLI_MAN,
'                  Description:="gcc=GCModeller Compiler; Compiler program for the GCModeller virtual cell system model",
'                  Url:="http://gcmodeller.org",
'                  Publisher:="GCModeller")>
'<CLI> Public Module CLI

'    ''' <summary>
'    ''' Compile a metacyc database into a gcml model file.(将一个MetaCyc数据库编译为gcml计算机模型文件)
'    ''' </summary>
'    ''' <param name="CommandLine"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    <ExportAPI("compile_metacyc", Info:="compile a metacyc database into a gcml(genetic clock markup language) model file.",
'        Usage:="compile_metacyc -i <data_dir> -o <output_file>",
'        Example:="compile_metacyc -i ~/Documents/ecoli/ -o ~/Desktop/ecoli.xml")>
'    <Argument("-i",
'        Description:="")>
'    <Argument("-o",
'        Description:="")>
'    Public Function CompileMetaCyc(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
'        Dim input As String = CommandLine("-i")
'        Dim output As String = CommandLine("-o")
'        Dim LogFile As New LogFile($"{Settings.LogDIR}/gcc_{Now.ToNormalizedPathString}.log")

'        If String.IsNullOrEmpty(input) Then
'            LogFile.WriteLine("Could not load metacyc database, value of the parameter ""-i"" is empty!", "gcc_main() -> compile_metacyc", MSG_TYPES.ERR)
'            LogFile.Dispose()
'            Return -1
'        End If

'        If String.IsNullOrEmpty(output) Then
'            output = My.Application.Info.DirectoryPath & "/" & input.Replace("\", "/").Split.Last & ".xml"
'            LogFile.WriteLine(String.Format("User not specific the output file for the compiled model, the compiled model file will be save to location:{0}  ""{1}""", vbCrLf, output),
'                              "gcc_main() -> compile_metacyc", MSG_TYPES.WRN)
'        End If

'        LogFile.WriteLine(String.Format("Start to compile metacyc database:{0}  ""{1}""", vbCrLf, input), "gcc_main() -> compile_metacyc", MSG_TYPES.INF)

'        Dim TagFilters = (From Filter In Settings.SettingsFile.Gcc.Filters.ToArray Select New Escaping With {
'                                                                                       .Original = Filter.NewReplaced, .Escape = Filter.Old}).ToArray
'        Dim Compiler As New GCMarkupLanguage.Builder.Compiler With {
'                .StringReplacements = TagFilters,
'                .LogFile = LogFile
'        }
'        Call Compiler.PreCompile(input)
'        Call Compiler.Compile()
'        Call Compiler.Return.Save(output)
'        LogFile.Dispose()

'        Return 0
'    End Function

'    <ExportAPI("-add_replacement", Usage:="-add_replacement -old <old_value> -new <new_value>")>
'    Public Function AddNewPair(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
'        Dim Old As String = CommandLine("-old")
'        Dim [New] As String = CommandLine("-new")

'        Call Settings.SettingsFile.Gcc.Filters.Add(New Settings.Programs.GCC.Replacement With {.Old = Old, .NewReplaced = [New]})
'        Call Settings.ProfileData.Save(Nothing)

'        Return 0
'    End Function

'    ''' <summary>
'    ''' 向一个编译好的计算机模型文件之中添加相互作用规则
'    ''' </summary>
'    ''' <param name="CommandLine"></param>
'    ''' <returns></returns>
'    ''' <remarks>
'    ''' 由于规则是按照结构域来标识的，故而，首先需要进行结构域的分析
'    ''' 之后将结构域拓展为具体的目标蛋白质
'    ''' 最后将拓展的新反应规则添加进入计算机模型之中
'    ''' </remarks>
'    <ExportAPI("-add_rule", Usage:="-add_rule -rulefile <path> -db <datadir> -model <path> [-grep <scriptText>]")>
'    <Argument("-rulefile", Description:="a file contains some protein interaction rules", Usage:="")>
'    <Argument("-db", Description:="original database for the target compiled model", Usage:="")>
'    <Argument("-model", Description:="Target model file for adding some new rules", Usage:="")>
'    <Argument("-grep", True, Description:="If null then the system will using the MeatCyc database unique-id parsing method as default.")>
'    Public Function AddRule(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
'        Dim MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder =
'            SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder.CreateInstance(CommandLine("-db"))
'        Dim Model As GCMarkupLanguage.BacterialModel = GCMarkupLanguage.BacterialModel.Load(CommandLine("-model"))
'        Dim RuleFile As IO.File = IO.File.Load(CommandLine("-rulefile"))

'        Return ProteinDomain.AddingRules(MetaCyc:=MetaCyc, Model:=Model, RuleFile:=RuleFile, GrepScript:=CommandLine("-grep"), modelFilePath:=CommandLine("-model"))
'    End Function
'End Module
