Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("WGCNA.Modules", Publisher:="xie.guigang@gcmodeller.org", Category:=APICategories.ResearchTools)>
Public Module WGCNAModules

    <ExportAPI("Load.Modules")>
    Public Function LoadModules(path As String) As CExprMods()
        Dim tokens As String() = IO.File.ReadAllLines(path).Skip(1).ToArray
        Dim resultSet As CExprMods() = tokens.Select(Function(line) CExprMods.CreateObject(line)).ToArray

        Return resultSet
    End Function

    <ExportAPI("Mods.View")>
    <Extension>
    Public Function ModsView(mods As IEnumerable(Of CExprMods)) As Dictionary(Of String, String())
        Dim groups = (From entity As CExprMods
                      In mods
                      Select entity
                      Group entity By entity.nodesPresent Into Group).ToArray
        Dim resultSet As Dictionary(Of String, String()) =
            groups.ToDictionary(Function([mod]) [mod].nodesPresent,
                                Function([mod])
                                    Return [mod].Group _
                                        .Select(Function(entity) entity.nodeName) _
                                        .ToArray
                                End Function)
        Return resultSet
    End Function
End Module