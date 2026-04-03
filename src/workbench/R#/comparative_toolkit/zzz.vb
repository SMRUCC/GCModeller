Imports SMRUCC.Rsharp.Runtime.Interop

<Assembly: RPackageModule>

Public NotInheritable Class zzz

    Shared Sub New()
    End Sub

    Public Shared Sub onLoad()
        Call pangenome.Main()
    End Sub

End Class
