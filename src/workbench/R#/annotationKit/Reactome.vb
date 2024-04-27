#Region "Microsoft.VisualBasic::799b85df6386f0e2922d2a5593402601, G:/GCModeller/src/workbench/R#/annotationKit//Reactome.vb"

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


    ' Code Statistics:

    '   Total Lines: 27
    '    Code Lines: 16
    ' Comment Lines: 5
    '   Blank Lines: 6
    '     File Size: 723 B


    ' Module ReactomeTools
    ' 
    '     Function: loadPathwayList, pathwayjsonTree
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.Reactome

<Package("Reactome")>
Module ReactomeTools

    Sub Main()

    End Sub

    ''' <summary>
    ''' get reactome pathway list
    ''' </summary>
    ''' <param name="taxname"></param>
    ''' <returns></returns>
    <ExportAPI("pathway_list")>
    Public Function loadPathwayList(Optional taxname As String = Nothing) As Hierarchy
        Return Hierarchy.LoadInternal(taxname)
    End Function

    <ExportAPI("jsonTree")>
    Public Function pathwayjsonTree(tree As Hierarchy) As String
        Return Hierarchy.TreeJSON(tree)
    End Function

End Module
