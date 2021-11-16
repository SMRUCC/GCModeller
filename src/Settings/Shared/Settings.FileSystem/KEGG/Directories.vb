#Region "Microsoft.VisualBasic::0ed832df89f5bf23844f2cb21722c4f0, Shared\Settings.FileSystem\KEGG\Directories.vb"

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

    '     Module Directories
    ' 
    ' 
    '     Module Directories
    ' 
    '         Function: GetCompounds, GetGlycan, GetMetabolites, GetPathways, GetReactions
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace GCModeller.FileSystem.KEGG

#If ENABLE_API_EXPORT Then
    <Package("GCModeller.Repository.KEGG.Directory", Publisher:="amethyst.asuka@gcmodeller.org")>
    Module Directories
#Else
    Module Directories
#End If
        ''' <summary>
        ''' /KEGG/Reactions/
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <ExportAPI("GetReactions")>
        Public Function GetReactions() As String
            Return FileSystem.GetRepositoryRoot & "/KEGG/Reactions/"
        End Function

        <ExportAPI("GetMetabolites")>
        Public Function GetMetabolites() As String
            Return FileSystem.GetRepositoryRoot & "/KEGG/Metabolites/"
        End Function

        <ExportAPI("GetCompounds")>
        Public Function GetCompounds() As String
            Return Directories.GetMetabolites & "/Compounds/"
        End Function

        <ExportAPI("GetGlycan")>
        Public Function GetGlycan() As String
            Return Directories.GetMetabolites() & "/Glycan/"
        End Function

        <ExportAPI("GetPathways")>
        Public Function GetPathways() As String
            Return FileSystem.GetRepositoryRoot & "/KEGG/Pathways/"
        End Function
    End Module
End Namespace
