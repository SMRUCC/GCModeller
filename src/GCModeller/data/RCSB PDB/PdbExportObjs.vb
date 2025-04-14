#Region "Microsoft.VisualBasic::eb522f26caa3ff660511a6890685ee2d, data\RCSB PDB\PdbExportObjs.vb"

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

    '   Total Lines: 26
    '    Code Lines: 21 (80.77%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (19.23%)
    '     File Size: 938 B


    ' Class PdbItem
    ' 
    '     Properties: ChainId, PdbId
    ' 
    '     Function: ToString
    ' 
    ' Class PdbComplexesAssembly
    ' 
    '     Properties: ChainId, InteractionChainId, PdbId
    ' 
    '     Function: Equals, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Public Class PdbItem
    Public Property PdbId As String
    Public Property ChainId As String

    Public Overrides Function ToString() As String
        Return String.Format("{0}-{1}", PdbId, ChainId)
    End Function
End Class

Public Class PdbComplexesAssembly
    <Column("PdbId")> Public Property PdbId As String
    Public Property ChainId As String
    <Column("InteractionChainId")>
    Public Property InteractionChainId As String()

    Public Overrides Function ToString() As String
        Return String.Format("{0}-{1}", PdbId, ChainId)
    End Function

    Public Overloads Function Equals(Entry As PdbItem) As Boolean
        Return String.Equals(Entry.ChainId, ChainId, StringComparison.OrdinalIgnoreCase) AndAlso
            String.Equals(Entry.PdbId, PdbId, StringComparison.OrdinalIgnoreCase)
    End Function
End Class
