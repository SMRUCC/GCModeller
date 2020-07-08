#Region "Microsoft.VisualBasic::47bf742d2e7cde4e2813689c796027db, Bio.Assembly\Assembly\KEGG\DBGET\LinkDB\ParserHelper.vb"

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

    '     Module ParserHelper
    ' 
    '         Function: GetRelationship, LinkDbEntries
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.DBGET.LinkDB

    ''' <summary>
    ''' http://www.genome.jp/dbget-bin/www_bget?
    ''' </summary>
    Module ParserHelper

        Public Const URL_MODULE_GENES As String = "http://www.genome.jp/dbget-bin/get_linkdb?-t+genes+md:{0}"
        Public Const URL_PATHWAY_GENES As String = "http://www.genome.jp/dbget-bin/get_linkdb?-t+genes+path:{0}"

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function LinkDbEntries(url As String, Optional cacheRoot$ = "./", Optional offline As Boolean = False) As IEnumerable(Of NamedValue)
            Return GenericParser.LinkDbEntries(url, $"{cacheRoot}/.kegg/linkdb/", offline)
        End Function

        <Extension>
        Public Function GetRelationship(link As String) As Relationships
            If link.StringEmpty Then
                Return Relationships.unknown
            End If

            Dim type$ = link.Split("/"c).Last
            Dim value As Relationships = [Enum].Parse(GetType(Relationships), type)

            Return value
        End Function
    End Module
End Namespace
