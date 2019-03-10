#Region "Microsoft.VisualBasic::1594102674e807fb8d704b1d4c9d1547, data\Reactome\ShellScriptAPI.vb"

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

    ' Module ShellScriptAPI
    ' 
    '     Function: ExtractOwl, SaveExtracted
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.Reactome.ObjectModels

<Package("Reactome", Cites:="", Publisher:="", Description:="", Url:="")>
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
