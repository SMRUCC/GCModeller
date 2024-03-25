Imports SMRUCC.Rsharp.Runtime.Interop

<Assembly: RPackageModule>

Public Class zzz

    Public Shared Sub onLoad()
        Call pubmed.Main()
        Call meshTools.Main()
    End Sub
End Class
