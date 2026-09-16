Imports System.IO

Namespace Script

    Module VBScript

        ' =========================================================================
        ' 函数1: 脚本源代码文件解析
        ' =========================================================================

        ''' <summary>
        ''' 对VB.NET脚本源代码文件进行解析处理
        ''' </summary>
        ''' <param name="scriptFile">.vb脚本源代码文件路径</param>
        ''' <param name="verbose">是否输出 #include 解析与代码重构的细节</param>
        Public Function ParseScript(scriptFile As String, Optional verbose As Boolean = False) As ScriptParseResult
            If Not File.Exists(scriptFile) Then
                Throw New FileNotFoundException("脚本源文件不存在: " & scriptFile, scriptFile)
            End If

            Dim fullScript As String = Path.GetFullPath(scriptFile)
            Dim source As String = fullScript.ReadAllText
            Dim baseDir As String = Path.GetDirectoryName(fullScript)

            If verbose Then
                Call Console.WriteLine("----- #include resolve -----")
            End If

            ' ---- Step1: 解析 #include 指令, 得到程序集 / 其它脚本 / nuget 包三类引用 ----
            Dim root0 As String = App.HOME
            Dim root1 As String = App.HOME & "/libs"             ' bin/libs/
            Dim root2 As String = App.HOME.ParentPath & "/libs"  ' ./bin/ ./libs/
            Dim searchRoots As String() = {baseDir, root0, root1, root2}

            Dim resolver As New IncludeResolver(searchRoots, verbose)
            Dim includes As IncludeSet = resolver.Resolve(source, fullScript)

            For Each warning As String In includes.Warnings
                Call Console.WriteLine("WARNING: " & warning)
            Next

            ' ---- Step2: 解析程序集元数据指令(#package/#author/#title/#version) ----
            Dim metadata As ScriptMetadata = ScriptMetadata.Parse(source)

            ' ---- Step3: 代码结构重构 ----
            ' 魔法方法与 #include 采用一致的相对路径搜索顺序(nuget 包目录追加在末尾)
            Dim magicRoots As String() = includes.MergeSearchRoots(searchRoots).ToArray()
            Dim magicSnippets As IEnumerable(Of String) = Magics.Build(fullScript, metadata, includes.Assemblies, magicRoots)
            Dim preprocessed As String = ScriptRefactor.PreprocessText(source)
            Dim code As String = New ScriptRefactor(metadata, magicSnippets, includes).RefactorPreprocessed(preprocessed)

            If verbose Then
                Call Console.WriteLine("----- generated code -----")
                Call Console.WriteLine(code)
            End If

            Return New ScriptParseResult With {
                .ScriptFile = fullScript,
                .CommandLine = Nothing,
                .Metadata = metadata,
                .Imports = New List(Of String)(includes.Assemblies),
                .ScriptIncludes = includes.Scripts,
                .NuGetPackages = includes.NuGetPackages,
                .IncludeWarnings = includes.Warnings,
                .SearchRoots = magicRoots,
                .PreprocessedCode = preprocessed,
                .GeneratedCode = code
            }
        End Function
    End Module
End Namespace
