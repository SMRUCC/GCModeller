Imports LANS.SystemsBiology.DatabaseServices.Reactome.ObjectModels
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData

<[PackageNamespace]("Reactome", Cites:="", Publisher:="", Description:="", Url:="")>
Module ShellScriptAPI

    <ExportAPI("Owl.Extract")>
    Public Function ExtractOwl(path As String) As BioSystem
        Return Reactome.ExtractOwl.ExtractFile(path)
    End Function

    <ExportAPI("Export.Csv.Reactome")>
    Public Function SaveExtracted(Doc As BioSystem, <Parameter("DIR.Export")> ExportTo As String) As Boolean
        Call Doc.Metabolites.SaveTo(ExportTo & "/Metabolites.csv", False)
        Call Doc.Reactions.SaveTo(ExportTo & "/Reactions.csv", False)
        Return True
    End Function
End Module
