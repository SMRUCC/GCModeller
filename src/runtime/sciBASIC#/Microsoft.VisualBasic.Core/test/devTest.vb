Imports Microsoft.VisualBasic.ApplicationServices.Development.NetCore5
Imports Microsoft.VisualBasic.Serialization.JSON

Module devTest

    Sub Main()
        Dim deps = "D:\GCModeller\src\R-sharp\App\net5.0\base.deps.json".LoadJsonFile(Of deps)
        Dim ref = deps.GetReferenceProject.ToArray

        Pause()
    End Sub
End Module
