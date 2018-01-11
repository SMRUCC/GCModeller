#Region "Microsoft.VisualBasic::4ef23dabbaf3a8ad961139c8e253e65d, ..\GCModeller\data\functional_DATA\CARD\Tables.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy

Public Class ncbi_taxomony

    ''' <summary>
    ''' NCBI taxonomy id, can be using for build taxonomy lineage from <see cref="NcbiTaxonomyTree"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property Accession As String
    ''' <summary>
    ''' <see cref="SeqHeader.species"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property Name As String
    Public Property Description As String
End Class
