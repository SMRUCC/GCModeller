#Region "Microsoft.VisualBasic::5c749190ae968e5bcbd0b9b51ec457d2, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\LinkDB\Relationship.vb"

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

    '   Total Lines: 73
    '    Code Lines: 45
    ' Comment Lines: 18
    '   Blank Lines: 10
    '     File Size: 2.87 KB


    '     Enum Relationships
    ' 
    '         equivalent, indirect, original, reverse
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class Relationship
    ' 
    '         Properties: left, relationship, right
    ' 
    '         Function: GetLinkDb, LinkIterator, TryParseLine
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Assembly.KEGG.DBGET.LinkDB

    Public Enum Relationships

        unknown = 0

        ''' <summary>
        ''' links are special original links to signify equivalent contents between 
        ''' KEGG GENES, COMPOUND, DRUG, REACTION databases and databases other 
        ''' than KEGG.
        ''' </summary>
        equivalent
        ''' <summary>
        ''' links are derived by combining two or more original links. Currently, 
        ''' links from KEGG GENES to REACTION via KO, and to COMPOUND via REACTION 
        ''' are available.
        ''' </summary>
        indirect
        ''' <summary>
        ''' links are extracted from the database entries provided by the GenomeNet 
        ''' DBGET system.
        ''' </summary>
        original
        ''' <summary>
        ''' links are derived from the original links by exchanging a source entry 
        ''' and its target entry.
        ''' </summary>
        reverse
    End Enum

    Public Class Relationship

        Public Property left As String
        Public Property relationship As Relationships
        Public Property right As NamedValue(Of String)

        Public Shared Function TryParseLine(line As String) As Relationship
            Dim links$() = line.Matches("[<].+?[>]", RegexICSng) _
                               .Select(Function(l)
                                           Return l.GetStackValue("<", ">")
                                       End Function) _
                               .ToArray
            Dim rel As Relationships = links.ElementAtOrDefault(1).GetRelationship
            Dim entry$ = links(0).Split("/"c).Last
            Dim right$ = links(2)
            Dim name$ = Strings.Split(right, "//")(1).Split("/"c).First
            Dim value$ = right.Split("/"c, "?"c, "="c).Last

            Return New Relationship With {
                .left = entry,
                .relationship = rel,
                .right = New NamedValue(Of String) With {
                    .Name = name,
                    .Value = value,
                    .Description = right
                }
            }
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function LinkIterator(lines As IEnumerable(Of String)) As IEnumerable(Of Relationship)
            Return lines.Select(AddressOf TryParseLine)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function GetLinkDb(entry As String) As IEnumerable(Of Relationship)
            Return LinkIterator($"http://www.genome.jp/dbget-bin/get_linkdb?-N+{entry}".GET.LineTokens)
        End Function
    End Class
End Namespace
