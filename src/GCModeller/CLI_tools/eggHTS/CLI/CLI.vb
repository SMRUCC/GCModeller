Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports SMRUCC.genomics.Analysis.Microarray.DEGDesigner

<CLI> Module CLI

    Sub New()
        Call Settings.Templates.WriteExcelTemplate(Of Designer)()
    End Sub
End Module