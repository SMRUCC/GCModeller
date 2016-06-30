Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic

<[PackageNamespace]("Metacyc.Objects", Category:=APICategories.UtilityTools, Publisher:="xie.guigang@gmail.com")>
Public Class ObjectOperations

    Protected Friend Shared GetTableMethods As Dictionary(Of String, Func(Of LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Object())) =
        New Dictionary(Of String, Func(Of LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Object())) From {
            {"bind-rxns", Function(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder) MetaCyc.GetBindRxns},
            {"compounds", Function(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder) MetaCyc.GetCompounds},
            {"dna-binding-sites", Function(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder) MetaCyc.GetDNABindingSites},
            {"enz-rxns", Function(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder) MetaCyc.GetEnzrxns},
            {"genes", Function(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder) MetaCyc.GetGenes},
            {"pathways", Function(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder) MetaCyc.GetPathways},
            {"promoters", Function(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder) (From Promoter As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Promoter In MetaCyc.GetPromoters Select Promoter).ToArray},
            {"protein-features", Function(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder) MetaCyc.GetProteinFeature},
            {"proteins", Function(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder) (From Protein As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein In MetaCyc.GetProteins Select Protein).ToArray},
            {"protein-ligands", Function(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder) MetaCyc.GetProtLigandCplx},
            {"reactions", Function(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder) MetaCyc.GetReactions},
            {"regulations", Function(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder) MetaCyc.GetRegulations},
            {"terminators", Function(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder) MetaCyc.GetTerminators},
            {"transcipt-units", Function(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder) MetaCyc.GetTransUnits}}

    <ExportAPI("GetTable")>
    Public Shared Function GetTable(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, TableName As String) As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Object()
        Dim Method = GetTableMethods(TableName.ToLower)
        Dim ChunkBuffer = Method(MetaCyc)
        Return ChunkBuffer
    End Function

    <ExportAPI("GetTypes")>
    Public Shared Function GetTypes(table As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Object()) As String()
        Dim ChunkBuffer = (From item In table Select item.Types).ToArray
        Dim ListChunk As List(Of String) = New List(Of String)
        For Each line In ChunkBuffer
            Call ListChunk.AddRange(line)
        Next

        Return ListChunk.Distinct.ToArray
    End Function

    <ExportAPI("Write.Table.AsCsv")>
    Public Shared Function SaveTableAsCsv(table As Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Object), saveto As String) As Boolean
        Call table.ToArray.SaveTo(saveto, False)
        Return True
    End Function

    <ExportAPI("Table.Compounds.AsCsv")>
    Public Shared Function SaveCompoundsAsCsv(table As Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Compound), saveto As String) As Boolean
        Call table.SaveTo(saveto, False)
        Return True
    End Function
End Class
