Imports Microsoft.VisualBasic.Scripting.ShoalShell
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ObjectModels

Module _DEBUG_MAIN
    Sub Main()
        Call CommandLines.RegisterModule("-register_modules ""E:\GCModeller\CompiledAssembly\LANS.SystemsBiology.Assembly.Plugins.Shoal.dll""")
        Call CommandLines.RegisterModule("-register_modules -path ""E:\GCModeller\GCI Project\Reference\ShellScript\TestShellScriptModule\bin\Debug\TestShellScriptModule.dll""")
        Call New Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ShellScript().EXEC(FileIO.FileSystem.ReadAllText("../release/linq_test.txt"))
    End Sub
End Module
