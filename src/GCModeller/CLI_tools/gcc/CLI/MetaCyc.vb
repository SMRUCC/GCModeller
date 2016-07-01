Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.SBML.Specifics.MetaCyc

<PackageNamespace("GCModeller.Compiler.CLI", Category:=APICategories.CLI_MAN,
                  Description:="gcc=GCModeller Compiler; Compiler program for the GCModeller virtual cell system model",
                  Url:="http://gcmodeller.org",
                  Publisher:="GCModeller")>
Public Module CLI

    ''' <summary>
    ''' Compile a metacyc database into a gcml model file.(将一个MetaCyc数据库编译为gcml计算机模型文件)
    ''' </summary>
    ''' <param name="CommandLine"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("compile_metacyc", Info:="compile a metacyc database into a gcml(genetic clock markup language) model file.",
        Usage:="compile_metacyc -i <data_dir> -o <output_file>",
        Example:="compile_metacyc -i ~/Documents/ecoli/ -o ~/Desktop/ecoli.xml")>
    <ParameterInfo("-i",
        Description:="",
        Example:="")>
    <ParameterInfo("-o",
        Description:="",
        Example:="")>
    Public Function CompileMetaCyc(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
        Dim input As String = CommandLine("-i")
        Dim output As String = CommandLine("-o")
        Dim LogFile As Microsoft.VisualBasic.Logging.LogFile =
            New Microsoft.VisualBasic.Logging.LogFile($"{Settings.LogDIR}/gcc_{Now.ToNormalizedPathString}.log")

        If String.IsNullOrEmpty(input) Then
            LogFile.WriteLine("Could not load metacyc database, value of the parameter ""-i"" is empty!", "gcc_main() -> compile_metacyc", Microsoft.VisualBasic.Logging.MSG_TYPES.ERR)
            LogFile.SaveLog(appendToLogFile:=True)
            Return -1
        End If

        If String.IsNullOrEmpty(output) Then
            output = My.Application.Info.DirectoryPath & "/" & input.Replace("\", "/").Split.Last & ".xml"
            LogFile.WriteLine(String.Format("User not specific the output file for the compiled model, the compiled model file will be save to location:{0}  ""{1}""", vbCrLf, output),
                              "gcc_main() -> compile_metacyc", Microsoft.VisualBasic.Logging.MSG_TYPES.WRN)
        End If

        LogFile.WriteLine(String.Format("Start to compile metacyc database:{0}  ""{1}""", vbCrLf, input), "gcc_main() -> compile_metacyc", Microsoft.VisualBasic.Logging.MSG_TYPES.INF)

        Dim TagFilters = (From Filter In Settings.SettingsFile.Gcc.Filters.ToArray Select New Escaping With {
                                                                                       .Original = Filter.NewReplaced, .Escape = Filter.Old}).ToArray
        Dim Compiler As SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.Builder.Compiler =
            New SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.Builder.Compiler With {
                .StringReplacements = TagFilters,
                .LogFile = LogFile
        }
        Call Compiler.PreCompile(input)
        Call Compiler.Compile()
        Call Compiler.Return.Save(output)
        LogFile.SaveLog(appendToLogFile:=False)

        Return 0
    End Function

    <ExportAPI("-add_replacement", Usage:="-add_replacement -old <old_value> -new <new_value>")>
    Public Function AddNewPair(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
        Dim Old As String = CommandLine("-old")
        Dim [New] As String = CommandLine("-new")

        Call Settings.SettingsFile.Gcc.Filters.Add(New Settings.Programs.GCC.Replacement With {.Old = Old, .NewReplaced = [New]})
        Call Settings.ProfileData.Save()

        Return 0
    End Function

    ''' <summary>
    ''' 向一个编译好的计算机模型文件之中添加相互作用规则
    ''' </summary>
    ''' <param name="CommandLine"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 由于规则是按照结构域来标识的，故而，首先需要进行结构域的分析
    ''' 之后将结构域拓展为具体的目标蛋白质
    ''' 最后将拓展的新反应规则添加进入计算机模型之中
    ''' </remarks>
    <ExportAPI("-add_rule", Usage:="-add_rule -rulefile <path> -db <datadir> -model <path> [-grep <scriptText>]")>
    <ParameterInfo("-rulefile", Description:="a file contains some protein interaction rules", Usage:="", Example:="")>
    <ParameterInfo("-db", Description:="original database for the target compiled model", Usage:="", Example:="")>
    <ParameterInfo("-model", Description:="Target model file for adding some new rules", Usage:="", Example:="")>
    <ParameterInfo("-grep", True, Description:="If null then the system will using the MeatCyc database unique-id parsing method as default.")>
    Public Function AddRule(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
        Dim MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder =
            SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder.CreateInstance(CommandLine("-db"))
        Dim Model As SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.BacterialModel =
            SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.BacterialModel.Load(CommandLine("-model"))
        Dim RuleFile As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File =
            Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load(CommandLine("-rulefile"))

        Return ProteinDomain.AddingRules(MetaCyc:=MetaCyc, Model:=Model, RuleFile:=RuleFile, GrepScript:=CommandLine("-grep"))
    End Function
End Module
