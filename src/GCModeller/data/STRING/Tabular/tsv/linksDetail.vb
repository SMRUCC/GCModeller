Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Linq

Namespace Tabular.Tsv

    ''' <summary>
    ''' protein network data (incl. subscores per channel); commercial entities require a license.	
    ''' </summary>
    Public Class linksDetail

        Public Property protein1 As String
        Public Property protein2 As String
        Public Property neighborhood As Single
        Public Property neighborhood_transferred As Single
        Public Property fusion As Single
        Public Property cooccurence As Single
        Public Property homology As Single
        Public Property coexpression As Single
        Public Property coexpression_transferred As Single
        Public Property experiments As Single
        Public Property experiments_transferred As Single
        Public Property database_transferred As Single
        Public Property textmining_transferred As Single
        Public Property experimental As Single
        Public Property database As Single
        Public Property textmining As Single
        Public Property combined_score As Single

        Public Overrides Function ToString() As String
            Return $"[{protein1} ~ {protein2}]"
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

                If neighborhood > -1 Then link.neighborhood = Single.Parse(tokens(neighborhood))
                If neighborhood_transferred > -1 Then link.neighborhood_transferred = Single.Parse(tokens(neighborhood_transferred))
                If fusion > -1 Then link.fusion = Single.Parse(tokens(fusion))
                If cooccurence > -1 Then link.cooccurence = Single.Parse(tokens(cooccurence))
                If homology > -1 Then link.homology = Single.Parse(tokens(homology))
                If coexpression > -1 Then link.coexpression = Single.Parse(tokens(coexpression))
                If coexpression_transferred > -1 Then link.coexpression_transferred = Single.Parse(tokens(coexpression_transferred))
                If experiments > -1 Then link.experiments = Single.Parse(tokens(experiments))
                If experiments_transferred > -1 Then link.experiments_transferred = Single.Parse(tokens(experiments_transferred))
                If database_transferred > -1 Then link.database_transferred = Single.Parse(tokens(database_transferred))
                If textmining_transferred > -1 Then link.textmining_transferred = Single.Parse(tokens(textmining_transferred))
                If experimental > -1 Then link.experimental = Single.Parse(tokens(experimental))
                If database > -1 Then link.database = Single.Parse(tokens(database))
                If textmining > -1 Then link.textmining = Single.Parse(tokens(textmining))
                If combined_score > -1 Then link.combined_score = Single.Parse(tokens(combined_score))

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
                    .combined_score = Single.Parse(t(2))
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
            Dim revMaps As Dictionary(Of String, String) = maps _
                .ToDictionary(Function(x) x.Value,
                              Function(x)
                                  Return x.Key
                              End Function)

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

End Namespace