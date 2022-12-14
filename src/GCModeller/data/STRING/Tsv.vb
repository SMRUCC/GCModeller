#Region "Microsoft.VisualBasic::a360d21ed7dadfa783e127c5af2a7504, data\STRING\Tsv.vb"

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

'     Class LinkAction
' 
'         Properties: a_is_acting, action, item_id_a, item_id_b, mode
'                     score
' 
'         Function: LoadText
' 
'     Class linksDetail
' 
'         Properties: coexpression, combined_score, cooccurence, database, experimental
'                     fusion, neighborhood, protein1, protein2, textmining
' 
'         Function: IteratesLinks, LoadFile, Selects, ToString
' 
'     Class entrez_gene_id_vs_string
' 
'         Properties: Entrez_Gene_ID, STRING_Locus_ID
' 
'         Function: BuildMaps, BuildMapsFromFile, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace StringDB.Tsv

    ''' <summary>
    ''' interaction types for protein links.
    ''' (从String-DB之中下载的蛋白质互作网络数据，例如：``9606.protein.actions.v10.txt``，
    ''' 这个对象只是存在注释数据的互作关系，只是所有的互作关系之中研究比较明白的网络部分，
    ''' 假若查看所有的网络数据在``9606.protein.links.v10.txt``文件之中)
    ''' </summary>
    Public Class LinkAction

        Public Property item_id_a As String
        Public Property item_id_b As String
        Public Property mode As String
        Public Property action As String
        Public Property a_is_acting As String
        Public Property score As String

        Public Shared Iterator Function LoadText(path As String) As IEnumerable(Of LinkAction)
            For Each line As String In path.IterateAllLines.Skip(1)
                Dim tokens As String() = line.Split(Text.ASCII.TAB)

                Yield New LinkAction With {
                    .item_id_a = tokens(0),
                    .item_id_b = tokens(1),
                    .mode = tokens(2),
                    .action = tokens(3),
                    .a_is_acting = tokens(4),
                    .score = tokens(5)
                }
            Next
        End Function
    End Class

    ''' <summary>
    ''' protein network data (incl. subscores per channel); commercial entities require a license.	
    ''' </summary>
    Public Class linksDetail

        Public Property protein1 As String
        Public Property protein2 As String
        Public Property neighborhood As String
        Public Property neighborhood_transferred As Double
        Public Property fusion As String
        Public Property cooccurence As String
        Public Property homology As Double
        Public Property coexpression As String
        Public Property coexpression_transferred As Double
        Public Property experiments As Double
        Public Property experiments_transferred As Double
        Public Property database_transferred As Double
        Public Property textmining_transferred As Double
        Public Property experimental As String
        Public Property database As String
        Public Property textmining As String
        Public Property combined_score As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        ''' <summary>
        ''' parse the string-db table file
        ''' </summary>
        ''' <param name="path">
        ''' the string db protein links data files, example like:
        ''' 
        ''' 1. 9606.protein.links.v11.5.txt
        ''' 2. 9606.protein.links.full.v11.5.txt
        ''' 3. 9606.protein.links.detailed.v11.5.txt
        ''' </param>
        ''' <returns></returns>
        Public Shared Iterator Function LoadFile(path As String) As IEnumerable(Of linksDetail)
            Dim headers As Index(Of String) = path.ReadFirstLine.StringSplit("\s+").Indexing
            Dim neighborhood As Integer = headers.IndexOf(NameOf(linksDetail.neighborhood))
            Dim neighborhood_transferred As Integer = headers.IndexOf(NameOf(linksDetail.neighborhood_transferred))
            Dim fusion As Integer = headers.IndexOf(NameOf(linksDetail.fusion))
            Dim cooccurence As Integer = headers.IndexOf(NameOf(linksDetail.cooccurence))
            Dim homology As Integer = headers.IndexOf(NameOf(linksDetail.homology))
            Dim coexpression As Integer = headers.IndexOf(NameOf(linksDetail.coexpression))
            Dim coexpression_transferred As Integer = headers.IndexOf(NameOf(linksDetail.coexpression_transferred))
            Dim experiments As Integer = headers.IndexOf(NameOf(linksDetail.experiments))
            Dim experiments_transferred As Integer = headers.IndexOf(NameOf(linksDetail.experiments_transferred))
            Dim database_transferred As Integer = headers.IndexOf(NameOf(linksDetail.database_transferred))
            Dim textmining_transferred As Integer = headers.IndexOf(NameOf(linksDetail.textmining_transferred))
            Dim experimental As Integer = headers.IndexOf(NameOf(linksDetail.experimental))
            Dim database As Integer = headers.IndexOf(NameOf(linksDetail.database))
            Dim textmining As Integer = headers.IndexOf(NameOf(linksDetail.textmining))
            Dim combined_score As Integer = headers.IndexOf(NameOf(linksDetail.combined_score))

            For Each line As String In path.IterateAllLines.Skip(1)
                Dim tokens As String() = line.Split(" "c)
                Dim link As New linksDetail With {
                    .protein1 = tokens(0),
                    .protein2 = tokens(1)
                }

                If neighborhood > -1 Then link.neighborhood = tokens(neighborhood)
                If neighborhood_transferred > -1 Then link.neighborhood_transferred = tokens(neighborhood_transferred)
                If fusion > -1 Then link.fusion = tokens(fusion)
                If cooccurence > -1 Then link.cooccurence = tokens(cooccurence)
                If homology > -1 Then link.homology = tokens(homology)
                If coexpression > -1 Then link.coexpression = tokens(coexpression)
                If coexpression_transferred > -1 Then link.coexpression_transferred = tokens(coexpression_transferred)
                If experiments > -1 Then link.experiments = tokens(experiments)
                If experiments_transferred > -1 Then link.experiments_transferred = tokens(experiments_transferred)
                If database_transferred > -1 Then link.database_transferred = tokens(database_transferred)
                If textmining_transferred > -1 Then link.textmining_transferred = tokens(textmining_transferred)
                If experimental > -1 Then link.experimental = tokens(experimental)
                If database > -1 Then link.database = tokens(database)
                If textmining > -1 Then link.textmining = tokens(textmining)
                If combined_score > -1 Then link.combined_score = tokens(combined_score)

                Yield link
            Next
        End Function

        ''' <summary>
        ''' ``9606.protein.links.v10.txt``，这个文件之中只有3个值：a, b以及分数
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <returns></returns>
        Public Shared Iterator Function IteratesLinks(path$) As IEnumerable(Of linksDetail)
            For Each line As String In path.IterateAllLines.Skip(1)
                Dim t$() = line.Split(" "c)

                Yield New linksDetail With {
                    .protein1 = t(0),
                    .protein2 = t(1),
                    .combined_score = t(2)
                }
            Next
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="maps"></param>
        ''' <returns></returns>
        Public Shared Iterator Function Selects(
                                        source As IEnumerable(Of EntityObject),
                                        links As IEnumerable(Of linksDetail),
                               Optional maps As Dictionary(Of String, String) = Nothing) _
                                             As IEnumerable(Of EntityObject)
            If maps Is Nothing Then
                maps = New Dictionary(Of String, String)
            End If

            Dim FromHash As Dictionary(Of String, linksDetail()) = (
                From x As linksDetail
                In links
                Select x
                Group x By x.protein1 Into Group) _
                     .ToDictionary(Function(x) x.protein1,
                                   Function(x) x.Group.ToArray)
            Dim ToHash As Dictionary(Of String, linksDetail()) = (
                From x As linksDetail
                In FromHash.Values.IteratesALL
                Select x
                Group x By x.protein2 Into Group) _
                     .ToDictionary(Function(x) x.protein2,
                                   Function(x) x.Group.ToArray)
            Dim revMaps As Dictionary(Of String, String) =
                maps.ToDictionary(Function(x) x.Value, Function(x) x.Key)

            For Each x As EntityObject In source
                Dim key As String = x.ID
                Dim STRINGmap As String

                If maps.ContainsKey(x.ID) Then
                    STRINGmap = maps(x.ID)
                Else
                    STRINGmap = x.ID
                End If

                If FromHash.ContainsKey(STRINGmap) Then
                    For Each part As linksDetail In FromHash(STRINGmap)
                        Dim copy As EntityObject = x.Copy

                        copy.Properties.Add(NameOf(part.textmining), part.textmining)
                        copy.Properties.Add(NameOf(part.neighborhood), part.neighborhood)
                        copy.Properties.Add(NameOf(part.fusion), part.fusion)
                        copy.Properties.Add(NameOf(part.experimental), part.experimental)
                        copy.Properties.Add(NameOf(part.database), part.database)
                        copy.Properties.Add(NameOf(part.cooccurence), part.cooccurence)
                        copy.Properties.Add(NameOf(part.combined_score), part.combined_score)
                        copy.Properties.Add(NameOf(part.coexpression), part.coexpression)
                        copy.Properties.Add("Part To", part.protein2)

                        If revMaps.ContainsKey(part.protein2) Then
                            copy.Properties.Add("(NCBI)Part To", revMaps(part.protein2))
                        End If

                        Yield copy
                    Next
                End If
                If ToHash.ContainsKey(STRINGmap) Then
                    For Each part As linksDetail In ToHash(STRINGmap)
                        Dim copy As EntityObject = x.Copy

                        copy.Properties.Add(NameOf(part.textmining), part.textmining)
                        copy.Properties.Add(NameOf(part.neighborhood), part.neighborhood)
                        copy.Properties.Add(NameOf(part.fusion), part.fusion)
                        copy.Properties.Add(NameOf(part.experimental), part.experimental)
                        copy.Properties.Add(NameOf(part.database), part.database)
                        copy.Properties.Add(NameOf(part.cooccurence), part.cooccurence)
                        copy.Properties.Add(NameOf(part.combined_score), part.combined_score)
                        copy.Properties.Add(NameOf(part.coexpression), part.coexpression)
                        copy.Properties.Add("Part From", part.protein1)

                        If revMaps.ContainsKey(part.protein1) Then
                            copy.Properties.Add("(NCBI)Part From", revMaps(part.protein1))
                        End If

                        Yield copy
                    Next
                End If
            Next
        End Function
    End Class

    ''' <summary>
    ''' separate identifier mapping files, for several frequently used name_spaces...
    ''' </summary>
    Public Class entrez_gene_id_vs_string

        <Column("#Entrez_Gene_ID")> Public Property Entrez_Gene_ID As String
        Public Property STRING_Locus_ID As String

        Public Overrides Function ToString() As String
            Return $"{Entrez_Gene_ID} <--> {STRING_Locus_ID}"
        End Function

        Public Shared Function BuildMapsFromFile(path As String, Optional tsv As Boolean = True) As Dictionary(Of String, String)
            If tsv Then
                Return BuildMaps(path.Imports(Of entrez_gene_id_vs_string)(vbTab))
            Else
                Return BuildMaps(path.LoadCsv(Of entrez_gene_id_vs_string))
            End If
        End Function

        Public Shared Function BuildMaps(source As IEnumerable(Of entrez_gene_id_vs_string)) As Dictionary(Of String, String)
            Return source.ToDictionary(Function(x) x.Entrez_Gene_ID, Function(x) x.STRING_Locus_ID)
        End Function
    End Class
End Namespace
