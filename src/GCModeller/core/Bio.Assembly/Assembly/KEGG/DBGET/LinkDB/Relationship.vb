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

    Module Parserhelper

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