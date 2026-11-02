Imports SMRUCC.Rsharp.Runtime.Interop

<Assembly: RPackageModule>

Public Module zzz

    Sub onLoad()
        Call pdbTools.Main()
        Call molecular_tools.Main()
    End Sub
End Module
