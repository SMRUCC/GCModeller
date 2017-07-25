Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.ComponentModel.Settings.Inf
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports SMRUCC.genomics.Analysis.Microarray.DEGDesigner

<CLI> Module CLI

    Sub New()
        Call Settings.Templates.WriteExcelTemplate(Of Designer)()

        Dim path$ = Settings.Session.Templates & "/" & Parameters.DefaultFileName

        If Not path.FileExists Then
            Call New Parameters With {
                .ForceDirectedArgs = ForceDirectedArgs.DefaultNew
            }.WriteProfile(path)
        End If
    End Sub
End Module