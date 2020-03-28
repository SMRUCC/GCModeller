#Region "Microsoft.VisualBasic::e5bce657f16811d7845c37a4f66cce98, typescript\Linq.ts\TsBuild\TsBuild\Program.vb"

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

    ' Module Program
    ' 
    '     Function: BootstrapLoader, Compile, Main, ModuleBuilder
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Development.VisualStudio.vbproj
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports TsBuild.Bootstrap

Module Program

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    <ExportAPI("/declare")>
    <Usage("/declare /ts <*.d.ts> [/out <module.vb>]")>
    Public Function ModuleBuilder(args As CommandLine) As Integer
        Using output As StreamWriter = args.OpenStreamOutput("/out")
            Dim tokens = New ModuleParser() _
                .ParseIndex(args.ReadInput("/ts")) _
                .ToArray
            Dim vb$ = tokens.BuildVisualBasicModule

            Call output.WriteLine(vb)
        End Using

        Return 0
    End Function

    <ExportAPI("/bootstrap.loader")>
    <Usage("/bootstrap.loader /in <app.js> [/debug /out <app.directory>]")>
    Public Function BootstrapLoader(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or ([in].TrimSuffix & ".app/")
        Dim tokens = New JavaScriptSyntax() _
            .ParseTokens([in]) _
            .ToArray

        If args("/debug") Then
            Call New XmlList(Of Token) With {
                .items = tokens
            }.GetXml _
             .SaveTo($"{out}/syntax.xml")
        End If

        Dim js As New StringBuilder([in].ReadAllText.LineTokens.JoinBy(ASCII.LF))

        ' 下面的for循环保存的是最终所使用的app的class的定义
        ' 对于abstract属性的抽象模型，是保存于asset.js文件中
        ' 直接进行加载的
        For Each app As NamedValue(Of String) In tokens.PopulateModules(js.ToString)
            Call app.Value.SaveTo($"{out}/modules/{app.Name}.js")
            Call js.Replace(app.Value, "")
        Next

        Call js.SaveTo($"{out}/asset.js")

        ' 提供应用程序的启动框架
        ' 首先加载bootstrapLoader模块
        ' 然后根据当前的appName加载对应的app脚本模块
        ' 运行app class
        Call $"".SaveTo($"{out}/bootstrapLoader.js")

        Return 0
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
