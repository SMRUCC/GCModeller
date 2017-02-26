
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

            require("clusterProfiler")

            SyncLock R
                With R
                    .call = $"{go2name} <- {goBrief}[, {header}]"
                End With
            End SyncLock
        End Sub

        ''' <summary>
        ''' GO富集操作
        ''' </summary>
        ''' <param name="DEGs"></param>
        ''' <param name="term2gene$">
        ''' term2gene should provide background annotation of the whole genome
        ''' </param>
        ''' <param name="save$"></param>
        ''' <param name="noCuts"></param>
        ''' <returns></returns>
        Public Function Enrich(DEGs As IEnumerable(Of String), backgrounds As IEnumerable(Of String), term2gene$, save$, Optional noCuts As Boolean = False) As Boolean
            Dim deg$ = c(DEGs, stringVector:=True)
            Dim universe$ = c(backgrounds, stringVector:=True)
            Dim t2g$ = read.table(term2gene, header:=False)
            Dim result$

            If noCuts Then
                result = clusterProfiler.enricher(deg, universe, t2g, pvalueCutoff:=1, minGSSize:=1, qvalueCutoff:=1, TERM2NAME:=go2name)
            Else
                result = clusterProfiler.enricher(deg, universe, t2g, TERM2NAME:=go2name)
            End If

            Call write.csv(summary(result), save, rowNames:=False)

            Return save.FileExists(ZERO_Nonexists:=True)
        End Function

        Public Function Enrich(DEGs$, backgrounds$, term2gene$, save$, Optional noCuts As Boolean = False) As Boolean
            Return Enrich(DEGs.ReadAllLines, backgrounds.ReadAllLines, term2gene, save, noCuts)
        End Function
    End Class
End Namespace