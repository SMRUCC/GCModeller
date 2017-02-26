
Imports RDotNET.Extensions.VisualBasic.API.utils
Imports RDotNET.Extensions.VisualBasic.API.base
Imports RDotNET.Extensions.VisualBasic

Namespace clusterProfiler

    Public Class Enricher

        Public ReadOnly Property go2name As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="goBrief$">``go_brief.csv``</param>
        Sub New(goBrief$, Optional goID$ = "goID", Optional name$ = "name")
            Dim header$ = c({goID, name}, stringVector:=True)

            goBrief = read.csv(goBrief)
            go2name = App.NextTempName

            SyncLock R
                With R
                    .call = $"{go2name} <- {goBrief}[, {header}]"
                End With
            End SyncLock
        End Sub
    End Class
End Namespace