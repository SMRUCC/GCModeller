#Region "Microsoft.VisualBasic::38df4f3dec80f3f3545748532baddb3e, ..\core\Bio.Assembly\Assembly\KEGG\Web\QueryEntry.vb"

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

Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports Microsoft.VisualBasic.Text.Levenshtein
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism

Namespace Assembly.KEGG.WebServices

    ''' <summary>
    ''' Meta data for query KEGG database
    ''' </summary>
    ''' <remarks>
    ''' The example format as:
    ''' 
    ''' ```
    ''' Nostoc sp. PCC 7120
    ''' #
    ''' alr4156
    ''' alr4157
    ''' alr1320
    ''' all0862
    ''' all2134
    ''' all2133
    ''' ......
    ''' ```
    ''' </remarks>
    Public Class QuerySource

        ''' <summary>
        ''' The genome name.
        ''' </summary>
        ''' <returns></returns>
        Public Property genome As String
        ''' <summary>
        ''' The list of gene locus id that using for the query.
        ''' </summary>
        ''' <returns></returns>
        Public Property locusId As String()

        ''' <summary>
        ''' Gets the brief code of the organism name in the KEGG database.
        ''' (获取得到KEGG数据库里面的物种的简称)
        ''' </summary>
        ''' <returns></returns>
        ''' <param name="offline">Work in offline mode?</param>
        Public Function QuerySpCode(Optional offline As Boolean = False) As String
            Dim sp As Organism = GetKEGGSpeciesCode(genome)

            If Not sp Is Nothing Then Return sp.KEGGId

            If offline Then
                Return ""   ' 本地查找不到，并且是工作在离线模式之下的，则无法得到数据
            End If

            Dim i As Integer

            For Each locus As String In locusId   ' 假若本地数据库没有查询到数据的话，则会查询在线数据库
                Dim entry As QueryEntry = GetQueryEntry(locus)   ' 这里还需要进行验证，因为基因号可能会在物种之间有重复

                If entry Is Nothing Then

                Else
                    sp = EntryAPI.GetValue(entry.SpeciesId)

                    If sp Is Nothing Then
                        Return ""
                    Else
                        ' 可能菌株的编号不是一样的，在这里修正
                        Dim lev As DistResult = LevenshteinDistance.ComputeDistance(sp.Species, genome)
                        If lev Is Nothing OrElse lev.NumMatches < 2 Then
                            Return ""
                        Else
                            Return entry.SpeciesId
                        End If
                    End If
                End If

                If i > 5 Then
                    Return ""
                Else
                    i += 1
                End If
            Next

            Return ""
        End Function

        Public Function GetDoc() As String
            Dim sbr As StringBuilder = New StringBuilder(genome & vbCrLf)
            Call sbr.AppendLine("#")
            Call sbr.AppendLine(locusId.JoinBy(vbCrLf))
            Return sbr.ToString
        End Function

        Public Shared Function DocParser(path As String) As QuerySource
            Dim Tokens As String() = IO.File.ReadAllLines(path)
            Dim name As String = Tokens(Scan0)
            Dim lstId As String() = Tokens.Skip(2).ToArray

            lstId = LinqAPI.Exec(Of String) <=
 _
                From s As String
                In lstId
                Where Not String.IsNullOrEmpty(s)
                Select s.Split.First

            Return New QuerySource With {
                .genome = name,
                .locusId = lstId
            }
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>&lt;a href="/dbget-bin/www_bget?ko:K00001">K00001&lt;/a>               E1.1.1.1, adh; alcohol dehydrogenase [EC:1.1.1.1]</remarks>
    Public Structure ListEntry

        ''' <summary>
        ''' The entry data which can be using for downloads data using the KEGG DBGET system.
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Dim EntryID As String
        ''' <summary>
        ''' The brief description information about this <see cref="EntryID">object</see>.
        ''' </summary>
        ''' <remarks></remarks>
        Dim Description As String

        ''' <summary>
        ''' The url which indicates the DBGET resource of this <see cref="EntryID">object</see>.
        ''' </summary>
        ''' <remarks></remarks>
        Dim Url As String

        Public Overrides Function ToString() As String
            Return String.Format("{0}:  {1}", EntryID, Description)
        End Function

        Public Shared Function InternalParser(s As String) As ListEntry
            Dim urlEntry As String = Regex.Match(s, "<a href="".+?"">.+?</a>").Value
            Dim descr As String = s.Replace(urlEntry, "").Trim
            Dim url As String = "http://www.genome.jp" & urlEntry.href
            Dim EntryID As String = urlEntry.GetValue

            Return New ListEntry With {
                .Description = descr,
                .EntryID = EntryID,
                .Url = url
            }
        End Function
    End Structure

    Public Class QueryEntry

        ''' <summary>SMRUCC.genomics.Assembly.KEGG.WebServices.WebRequest.QueryEntry
        ''' KEGG species id, the general species string in NCBI database can be mapping through the organism list which can 
        ''' be get from method <see cref="KEGG.DBGET.bGetObject.Organism.GetOrganismList"></see>.(KEGG
        ''' 数据库中的物种ID编号的简写形式，NCBI数据库中的标准的物种编号可以通过方法<see cref="KEGG.DBGET.bGetObject.Organism.GetOrganismList"></see>
        ''' 来进行映射)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property SpeciesId As String
        ''' <summary>
        ''' LocusId in the NCBI database.(NCBI数据库中的基因号)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property LocusId As String
        <XmlText>
        Public Property Description As String

        Sub New(str As String, Description As String)
            Dim Tokens As String() = str.Split(CChar(":"))
            SpeciesId = Tokens.First
            LocusId = Tokens.Last
            Me.Description = Description
        End Sub

        Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0}:{1}", SpeciesId, LocusId)
        End Function

        Public Shared Widening Operator CType(strArray As String()) As QueryEntry
            If strArray.IsNullOrEmpty OrElse strArray.Length < 2 Then
                Return New QueryEntry
            Else
                If strArray.Length = 2 Then
                    Return New QueryEntry With {
                        .SpeciesId = strArray(0),
                        .LocusId = strArray(1)
                    }
                Else
                    Return New QueryEntry With {
                        .SpeciesId = strArray(0),
                        .LocusId = strArray(1),
                        .Description = strArray(2)
                    }
                End If
            End If
        End Operator
    End Class
End Namespace
