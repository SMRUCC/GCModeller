Imports Microsoft.VisualBasic.CommandLine.Reflection

Namespace UniprotSprot

    <[Namespace]("annotations.uniprot")>
    Module UniprotShellScriptAPI

        Sub New()
            If Not Settings.Initialized Then
                Call Settings.Initialize()
            End If
        End Sub

        <ExportAPI("uniprot_sprot.install")>
        Public Function InstallDatabase(UniprotSprot As String) As LANS.SystemsBiology.GCModeller.Workbench.DatabaseServices.Model_Repository.Uniprot()
            Return DbTools.InstallDatabase(UniprotSprot, Settings.SettingsFile.RepositoryRoot & "/UniprotSprot/")
        End Function
    End Module
End Namespace