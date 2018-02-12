#Region "Microsoft.VisualBasic::b17c7aa1d57b2490fde96aee51af781f, analysis\RNA-Seq\Toolkits.RNA-Seq.RTools\WGCNA\CExprMods.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Class CExprMods
    ' 
    '         Properties: altName, NodeName, NodesPresent
    ' 
    '         Function: CreateObject, ToString
    ' 
    '     Module WGCNAModules
    ' 
    '         Function: LoadModules, ModsView
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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

    <Package("WGCNA.Modules", Publisher:="xie.guigang@gcmodeller.org", Category:=APICategories.ResearchTools)>
    Public Module WGCNAModules

        <ExportAPI("Load.Modules")>
        Public Function LoadModules(path As String) As CExprMods()
            Dim Tokens As String() = IO.File.ReadAllLines(path).Skip(1).ToArray
            Dim resultSet As CExprMods() =
                Tokens.Select(Function(line) CExprMods.CreateObject(line)).ToArray
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
                                    Function([mod]) [mod].Group.ToArray.Select(Function(entity) entity.NodeName).ToArray)
            Return resultSet
        End Function
    End Module
End Namespace
