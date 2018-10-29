Imports System.ComponentModel
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Development.VisualStudio
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Module Program

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    <ExportAPI("/compile")>
    <Description("https://www.typescriptlang.org/docs/handbook/tsconfig-json.html")>
    <Usage("/compile /proj <*.njsproj> [/out <output.js>]")>
    Public Function Compile(args As CommandLine) As Integer
        Dim in$ = args <= "/proj"
        ' njsproj -> code files -> tsconfig.json => files -> tsc
        Dim njsproj As Project = [in].LoadXml(Of Project)
        Dim codes$() = njsproj.ItemGroups _
            .Select(Function(item)
                        Return item.TypeScriptCompiles.SafeQuery
                    End Function) _
            .IteratesALL _
            .Where(Function(tsc)
                       Return tsc.SubType = "Code"
                   End Function) _
            .Select(Function(tsc) tsc.Include) _
            .ToArray

        ' 接下来将会覆盖掉tsconfig之中的files数组
        Dim config$ = $"{[in].ParentPath}/tsconfig.json"
        Dim tsbuild$ = $"{[in].ParentPath}/tsbuild.json"
        Dim tsconfig As tsconfig = config.LoadJSON(Of tsconfig)
        Dim out$ = args("/out") Or tsconfig.compilerOptions?.outFile

        If out.StringEmpty Then
            out = $"bin/{[in].BaseName}.js"
        End If

        tsconfig.compilerOptions.outFile = out
        tsconfig.files = codes _
            .Select(Function(s) s.Replace("\", "/").Replace("//", "/")) _
            .ToArray
        tsconfig.SaveJson(tsbuild, indent:=True)

        Using envir = App.TemporaryEnvironment([in].ParentPath)
            Dim cli$ = $"--project {tsbuild.GetFullPath.CLIPath}"
            Dim proc = Process.Start($"tsc {cli}")

            Return proc.ExitCode
        End Using
    End Function
End Module
