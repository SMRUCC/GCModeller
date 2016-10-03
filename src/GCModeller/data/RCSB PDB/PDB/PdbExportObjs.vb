#Region "Microsoft.VisualBasic::dbe8ec4027f67c491bc7a5ff1841b6fe, ..\GCModeller\data\RCSB PDB\PDB\PdbExportObjs.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

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
    <Collection("InteractionChainId", "; ")>
    Public Property InteractionChainId As String()

    Public Overrides Function ToString() As String
        Return String.Format("{0}-{1}", PdbId, ChainId)
    End Function

    Public Overloads Function Equals(Entry As PdbItem) As Boolean
        Return String.Equals(Entry.ChainId, ChainId, StringComparison.OrdinalIgnoreCase) AndAlso
            String.Equals(Entry.PdbId, PdbId, StringComparison.OrdinalIgnoreCase)
    End Function
End Class
