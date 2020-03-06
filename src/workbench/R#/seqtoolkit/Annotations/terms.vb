Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("annotation.terms", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
Module terms

    <ExportAPI("assign.KO")>
    Public Function KOannotations()

    End Function

    <ExportAPI("assign.COG")>
    Public Function COGannotations()

    End Function

    <ExportAPI("assign.Pfam")>
    Public Function Pfamannotations()

    End Function

    <ExportAPI("assign.GO")>
    Public Function GOannotations()

    End Function

End Module
