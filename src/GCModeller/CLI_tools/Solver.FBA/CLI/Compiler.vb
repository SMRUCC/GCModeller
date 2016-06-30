Imports LANS.SystemsBiology.Assembly
Imports LANS.SystemsBiology.Assembly.SBML.Level2.Elements
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic.Extensions

Partial Module CLI

    <ExportAPI("compile", Info:="Compile data source into a model file so that the fba program can using the data to performing the simulation calculation.",
        Usage:="compile -i <input_file> -o <output_file>[ -if <sbml/metacyc> -of <fba/fba2> -f <objective_function> -d <max/min>]",
        Example:="compile -i /home/xieguigang/ecoli/ -o /home/xieguigang/ecoli.xml -if metacyc -of fba2 -f v2+v3 -d max")>
    <ParameterInfo("-i", False,
        Description:="The input datasource path of the compiled model, it can be a MetaCyc data directory or a xml file in sbml format, format was specific by the value of switch '-if'",
        Example:="/home/xieguigang/ecoli/")>
    <ParameterInfo("-o", False,
        Description:="The output file path of the compiled model file.",
        Example:="/home/xieguigang/ecoli.xml")>
    <ParameterInfo("-if", True,
        Description:="Optional, this switch specific the format of the input data source, the fba compiler just support the metacyc database and sbml model currently, default value if metacyc." & vbCrLf &
                                 " metacyc - the input compiled data source is a metacyc database;" & vbCrLf &
                                 "sbml - the input compiled data source is a standard sbml language model in level 2.",
        Example:="metacyc")>
    <ParameterInfo("-of", True,
        Description:="Optional, this switch specific the format of the output compiled model, it can be a standard fba model or a advanced version of fba model, defualt is a standard fba model." & vbCrLf &
                                 " fba - the output compiled model is a standard fba model;" & vbCrLf &
                                 "fba2 - the output compiled model is a advanced version of fba model.",
        Example:="fba2")>
    <ParameterInfo("-f", True,
        Description:="Optional, you can specific the objective function using this switch, default value is the objective function that define in the sbml model file.",
        Example:="v2+v3")>
    <ParameterInfo("-d", True,
        Description:="Optional, the constraint direction of the objective function in the fba model, default value is maximum the objective function." & vbCrLf &
                                 " max - the constraint direction is maximum;" & vbCrLf &
                                 " min - the constraint direction is minimum.",
        Example:="max")>
    Public Function Compile(CommandLine As CommandLine) As Integer
        Dim DataSourceFomat As String = CommandLine("-if")

        Printf("FBA model compiler module [FBA.exe version: %s]", My.Application.Info.Version.ToString)
        If String.IsNullOrEmpty(DataSourceFomat) Then
            DataSourceFomat = "metacyc"
            Printf("User not specifc a data source format, use default value 'metacyc' database format.")
        ElseIf Array.IndexOf(CompileMethods.Keys.ToArray, DataSourceFomat) = -1 Then
            Printf("No such a data source format \'%s\', automatically  select the data source format as default 'metacyc'.", DataSourceFomat)
        End If
        Printf("Data source format is %s.", DataSourceFomat)
        Return CompileMethods(DataSourceFomat)(CommandLine)
    End Function

    ''' <summary>
    ''' The compiler support the metacyc database and sbml model file format currently.(编译器当前仅支持MetaCyc数据库和SBML标准模型数据源)
    ''' </summary>
    ''' <remarks></remarks>
    ReadOnly CompileMethods As Dictionary(Of String, Func(Of CommandLine, Integer)) =
        New Dictionary(Of String, Func(Of CommandLine, Integer)) From {
            {"sbml", AddressOf CLI.CompileSBML},
            {"metacyc", AddressOf CLI.CompileMetaCyc}}

    ''' <summary>
    ''' SBML --> FBA
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CompileSBML(args As CommandLine) As Integer
        'Dim SBML As LANS.SystemsBiology.Assembly.SBML.Level2.XmlFile = CommandLine("-i")
        'Dim FBAModel As LANS.SystemsBiology.FBA.FBA = FBA.Compile(SBML)
        'FBAModel.ObjectiveFunction = CommandLine("-f")
        'FBAModel.Direction = CommandLine("-d")
        'Call FileIO.FileSystem.WriteAllText(CommandLine("-o"), FBAModel.GetXml, append:=False)
        'Return 0
        Dim SBML As String = args("-i")
        Dim FBAModel As GCMarkupLanguage.FBACompatibility.Model =
            GCMarkupLanguage.FBACompatibility.API.Compile(SBMl2:=SBML)

        Call GCMarkupLanguage.Replacer.ApplyReplacements(Of
          speciesReference,
          FBACompatibility.Model)(FBAModel, Program.Profile.Gcc.Filters)
        Call FBAModel.Save(args("-o"))
        Return 0
    End Function

    ''' <summary>
    ''' metacyc --> FBA/FBA2
    ''' </summary>
    ''' <param name="CommandLine"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CompileMetaCyc(CommandLine As CommandLine) As Integer
        If String.Equals(CommandLine("-of"), "fba") Then
            'Dim SBML As LANS.SystemsBiology.Assembly.SBML.Level2.XmlFile = CommandLine("-i") & "/metabolic-reactions.sbml"
            'Dim FBAModel As LANS.SystemsBiology.FBA.FBA_RScript_Builder = FBA.Compile()
            'FBAModel.ObjectiveFunction = CommandLine("-f")
            'FBAModel.Direction = CommandLine("-d")
            'Call FileIO.FileSystem.WriteAllText(CommandLine("-o"), FBAModel.GetXml, append:=False)
        Else
            ' Dim Compiler As LANS.SystemsBiology.Assembly.Xml.Model = LANS.SystemsBiology.Assembly.Xml.Model.BuildFrom(CommandLine("-i"), {Program.Profile.Filter.Old, Program.Profile.Filter.[New]})
            'Call Compiler.ApplyFilter(Program.Profile.Filter.Old, Program.Profile.Filter.[New])
            'Call Compiler.Save(CommandLine("-o"))
        End If
        Return 0
    End Function
End Module
