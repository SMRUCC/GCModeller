#Region "Microsoft.VisualBasic::2d842e20835868507e776dcaaa61ae7b, GCModeller\models\Networks\KEGG\ReactionNetwork\Models\MapCache.vb"

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

    '   Total Lines: 132
    '    Code Lines: 82
    ' Comment Lines: 26
    '   Blank Lines: 24
    '     File Size: 4.42 KB


    '     Class MapCache
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CreateFromTable, FindAllPoints, FindPoints, indexKey, ParseText
    '                   Text
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports stdNum = System.Math

Namespace ReactionNetwork

    ''' <summary>
    ''' Mapping between the kegg compound tuple and reaction id, KO id, etc.
    ''' 
    ''' ``id1|id2 rxn,....``
    ''' </summary>
    Public Class MapCache

        ''' <summary>
        ''' the key in this hash table is the kegg compound id tuple in format like: ``id1|id2``
        ''' the values of the corresponding index hash key is the kegg reaction id and KO id list. 
        ''' </summary>
        Dim index As Dictionary(Of String, String())

        Sub New()
        End Sub

        ''' <summary>
        ''' found the related reaction id and KO id by given kegg compound tuple.
        ''' </summary>
        ''' <param name="c1$"></param>
        ''' <param name="c2$"></param>
        ''' <returns></returns>
        Public Function FindPoints(c1$, c2$) As String()
            Dim key = indexKey(c1, c2)

            If index.ContainsKey(key) Then
                Return index(key)
            Else
                Return {}
            End If
        End Function

        Public Function FindAllPoints(compounds As String()) As String()
            Dim result As String() = compounds _
                .AsParallel _
                .Select(Iterator Function(a) As IEnumerable(Of String())
                            For Each b As String In compounds
                                Yield FindPoints(a, b)
                            Next
                        End Function) _
                .IteratesALL _
                .IteratesALL _
                .Distinct _
                .ToArray

            Return result
        End Function

        ''' <summary>
        ''' Create index key from a given kegg compound id tuple
        ''' </summary>
        ''' <param name="c1">the kegg compound id</param>
        ''' <param name="c2">the kegg compound id</param>
        ''' <returns></returns>
        Private Shared Function indexKey(c1$, c2$) As String
            Dim i1 = Integer.Parse(c1.Substring(1))
            Dim i2 = Integer.Parse(c2.Substring(1))
            Dim key = stdNum.Min(i1, i2) & "|" & stdNum.Max(i1, i2)

            Return key
        End Function

        Public Function Text() As String
            Dim sb As New StringBuilder

            For Each index As KeyValuePair(Of String, String()) In Me.index
                sb.AppendLine(index.Key & " " & index.Value.JoinBy(","))
            Next

            Return sb.ToString
        End Function

        ''' <summary>
        ''' Parse cached index data from a text file readLines data result.
        ''' </summary>
        ''' <param name="text">the text lines</param>
        ''' <returns></returns>
        Public Shared Function ParseText(text As String()) As MapCache
            Dim cache As New Dictionary(Of String, String())
            Dim values As String()
            Dim key As String

            For Each line As String In text
                values = line.Split
                key = values(Scan0)
                values = values(1).Split(","c)

                Call cache.Add(key, values)
            Next

            Return New MapCache With {.index = cache}
        End Function

        Public Shared Function CreateFromTable(rxn As IEnumerable(Of ReactionTable)) As MapCache
            Dim cache As New Dictionary(Of String, List(Of String))
            Dim key As String

            For Each reaction As ReactionTable In rxn
                For Each i In reaction.substrates
                    For Each j In reaction.products
                        key = indexKey(i, j)

                        If Not cache.ContainsKey(key) Then
                            cache.Add(key, New List(Of String))
                        End If

                        cache(key).Add(reaction.entry)

                        If Not reaction.KO.IsNullOrEmpty Then
                            cache(key).AddRange(reaction.KO)
                        End If
                    Next
                Next
            Next

            Return New MapCache() With {
                .index = cache _
                    .ToDictionary(Function(t) t.Key,
                                  Function(t)
                                      Return t.Value.Distinct.ToArray
                                  End Function)
            }
        End Function

    End Class
End Namespace
