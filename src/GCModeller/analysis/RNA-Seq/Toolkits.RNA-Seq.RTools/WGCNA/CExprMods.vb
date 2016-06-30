Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace WGCNA

    ''' <summary>
    ''' CytoscapeNodes
    ''' </summary>
    Public Class CExprMods
        Public Property NodeName As String
        Public Property altName As String
        Public Property NodesPresent As String

        Public Overrides Function ToString() As String
            Return $"{NodeName} @{NodesPresent}"
        End Function

        Friend Shared Function CreateObject(record As String) As CExprMods
            Dim Tokens As String() = Strings.Split(record, vbTab)
            Return New CExprMods With {
                .NodeName = Tokens(Scan0),
                .altName = Tokens(1),
                .NodesPresent = Tokens(2)
            }
        End Function
    End Class

    <PackageNamespace("WGCNA.Modules", Publisher:="xie.guigang@gcmodeller.org", Category:=APICategories.ResearchTools)>
    Public Module WGCNAModules

        <ExportAPI("Load.Modules")>
        Public Function LoadModules(path As String) As CExprMods()
            Dim Tokens As String() = IO.File.ReadAllLines(path).Skip(1).ToArray
            Dim resultSet As CExprMods() =
                Tokens.ToArray(Function(line) CExprMods.CreateObject(line),
                               Parallel:=True)
            Return resultSet
        End Function

        <ExportAPI("Mods.View")>
        Public Function ModsView(mods As IEnumerable(Of CExprMods)) As Dictionary(Of String, String())
            Dim Groups = (From entity As CExprMods
                          In mods
                          Select entity
                          Group entity By entity.NodesPresent Into Group).ToArray
            Dim resultSet As Dictionary(Of String, String()) =
                Groups.ToDictionary(Function([mod]) [mod].NodesPresent,
                                    Function([mod]) [mod].Group.ToArray.ToArray(Function(entity) entity.NodeName))
            Return resultSet
        End Function
    End Module
End Namespace