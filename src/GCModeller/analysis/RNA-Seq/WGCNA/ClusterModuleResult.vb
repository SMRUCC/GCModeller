#Region "Microsoft.VisualBasic::ad39a46c13c13800d183a6dda5e68456, analysis\RNA-Seq\WGCNA\ClusterModuleResult.vb"

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

    '   Total Lines: 33
    '    Code Lines: 18 (54.55%)
    ' Comment Lines: 11 (33.33%)
    '    - Xml Docs: 90.91%
    ' 
    '   Blank Lines: 4 (12.12%)
    '     File Size: 1.16 KB


    ' Class ClusterModuleResult
    ' 
    '     Properties: [module], color, gene_id
    ' 
    '     Function: LoadTable
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.Framework

''' <summary>
''' tsv file model of the WGCNA module result exports
''' </summary>
Public Class ClusterModuleResult : Implements INamedValue

    Public Property gene_id As String Implements INamedValue.Key
    Public Property [module] As Integer
    Public Property color As String

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="tsv"></param>
    ''' <param name="prefix">
    ''' append gene id prefix?
    ''' </param>
    ''' <returns></returns>
    Public Shared Function LoadTable(tsv As String, Optional prefix As String = Nothing) As IEnumerable(Of ClusterModuleResult)
        If prefix.StringEmpty Then
            Return tsv.LoadCsv(Of ClusterModuleResult)(tsv:=True, mute:=True)
        Else
            Return tsv.LoadCsv(Of ClusterModuleResult)(tsv:=True, mute:=True) _
                .Select(Function(c)
                            c.gene_id = prefix & c.gene_id
                            Return c
                        End Function)
        End If
    End Function

End Class

