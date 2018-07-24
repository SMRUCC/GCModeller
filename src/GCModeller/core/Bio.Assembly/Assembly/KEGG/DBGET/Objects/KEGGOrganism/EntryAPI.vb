#Region "Microsoft.VisualBasic::934f6edccff7781e60a515b8e52dacbb, Bio.Assembly\Assembly\KEGG\DBGET\Objects\KEGGOrganism\EntryAPI.vb"

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

    '     Module EntryAPI
    ' 
    '         Properties: Resources
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: __fillClass, __loadList, FromResource, GetKEGGSpeciesCode, GetOrganismList
    '                   GetOrganismListFromResource, GetValue
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Levenshtein

Namespace Assembly.KEGG.DBGET.bGetObject.Organism

    ''' <summary>
    ''' 
    ''' </summary>
    <Package("KEGG.DBGET.spEntry",
                      Publisher:="amethyst.asuka@gcmodeller.org",
                      Url:=EntryAPI.WEB_URL,
                      Category:=APICategories.UtilityTools,
                      Description:="KEGG Organisms: Complete Genomes")>
    Public Module EntryAPI

        Public ReadOnly Property Resources As KEGGOrganism

        ''' <summary>
        ''' {brief_sp, <see cref="organism"/>}
        ''' </summary>
        ReadOnly OrgCodes As Dictionary(Of String, Organism)

        Sub New()
            Try
                Dim mgr As New ResourcesSatellite(GetType(EntryAPI).Assembly)
                Resources = __loadList(mgr.GetString("KEGG_Organism_Complete_Genomes"))
                OrgCodes = Resources _
                    .ToArray _
                    .ToDictionary(Function(x) x.KEGGId)
            Catch ex As Exception
                Call App.LogException(ex)
                Call ex.PrintException
            End Try
        End Sub

        ''' <summary>
        ''' Gets the organism value from the KEGG database through the brief code, 
        ''' if the data is not exists in the database, Nothing will be returns.
        ''' </summary>
        ''' <param name="sp">The organism brief code in the KEGG database</param>
        ''' <returns></returns>
        Public Function GetValue(sp As String) As Organism
            If OrgCodes.ContainsKey(sp) Then
                Return OrgCodes(sp)
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 通过本地资源从基因组全名之中得到KEGG之中的三字母的简写代码
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("KEGG.spCode",
                   Info:="Convert the species genome full name into the KEGG 3 letters briefly code.")>
        Public Function GetKEGGSpeciesCode(Name As String) As Organism
            Dim LQuery = LinqAPI.Exec(Of (org As Organism, dist As DistResult)) _
 _
                () <= From org As Organism
                      In Resources _
                          .ToArray _
                          .AsParallel
                      Let dist As DistResult = LevenshteinDistance.ComputeDistance(Name, org.Species) ' StatementMatches.Match(Name, x.Species)
                      Where Not dist Is Nothing AndAlso dist.NumMatches >= 2
                      Order By dist.MatchSimilarity Descending
                      Select (org, dist)

            If LQuery.IsNullOrEmpty OrElse LQuery.First.dist.MatchSimilarity < 0.9 Then
                Call VBDebugger.Warning($"Could not found any entry for ""{Name}"" from KEGG...")
                Return Nothing
            Else
                Dim first = LQuery.First.org
                Return first
            End If
        End Function

        ''' <summary>
        ''' Load KEGG organism list from the internal resource.
        ''' </summary>
        ''' <returns></returns>
        <ExportAPI("list.Load", Info:="Load KEGG organism list from the internal resource.")>
        Public Function GetOrganismListFromResource() As KEGGOrganism
            Dim res As New ResourcesSatellite(GetType(EntryAPI).Assembly)
            Dim html As String = res.GetString("KEGG_Organisms__Complete_Genomes")
            Return __loadList(html)
        End Function

        Public Const WEB_URL As String = "http://www.genome.jp/kegg/catalog/org_list.html"
        Public Const DELIMITER As String = "</td>"
        Public Const CELL As String = "<tr .+?</tr>"

        Private Function __loadList(html As String) As KEGGOrganism
            Dim Tokens As String() = Regex.Matches(html, CELL, RegexICSng).ToArray.Skip(1).ToArray
            Dim eulst As Organism() = New Organism(Tokens.Length - 1) {}
            Dim i As Integer
            Dim prlst As Prokaryote() = New Prokaryote(Tokens.Length - i) {}

            For i = 0 To Tokens.Length - 1
                eulst(i) = Organism.__createObject(Tokens(i))
                If eulst(i) Is Nothing Then
                    Exit For
                End If
            Next

            Dim j As Integer
            For i = i + 1 To Tokens.Length - 1
                prlst(j) = New Prokaryote(Tokens(i))
                j += 1
            Next

            Dim LQuery As Organism() = LinqAPI.Exec(Of Organism) <=
 _
                From Handle As Integer
                In eulst.Sequence
                Let obj As Organism = eulst(Handle)
                Where Not obj Is Nothing
                Select obj.Trim

            Dim lstProk As Prokaryote() = LinqAPI.Exec(Of Prokaryote) <=
 _
                From handle As Integer
                In prlst.Sequence
                Let obj As Prokaryote = prlst(handle)
                Where Not obj Is Nothing
                Select DirectCast(obj.Trim, Prokaryote)

            Return New KEGGOrganism With {
                .Eukaryotes = LQuery,
                .Prokaryote = lstProk
            }.__fillClass
        End Function

        ''' <summary>
        ''' 从上往下填充物种分类信息
        ''' </summary>
        ''' <param name="org"></param>
        ''' <returns></returns>
        <Extension>
        Private Function __fillClass(org As KEGGOrganism) As KEGGOrganism
            Dim Phylum As String = ""
            Dim [Class] As String = ""

            For idx As Integer = 0 To org.Eukaryotes.Length - 1
                Dim Organism = org.Eukaryotes(idx)
                If Not String.IsNullOrEmpty(Organism.Class) Then
                    [Class] = Organism.Class
                Else
                    Organism.Class = [Class]
                End If
                If Not String.IsNullOrEmpty(Organism.Phylum) Then
                    Phylum = Organism.Phylum
                Else
                    Organism.Phylum = Phylum
                End If
            Next

            Dim Kingdom As String = ""
            Phylum = "" : [Class] = ""
            For idx As Integer = 0 To org.Prokaryote.Length - 1
                Dim Organism = org.Prokaryote(idx)
                If Not String.IsNullOrEmpty(Organism.Class) Then
                    [Class] = Organism.Class
                Else
                    Organism.Class = [Class]
                End If
                If Not String.IsNullOrEmpty(Organism.Phylum) Then
                    Phylum = Organism.Phylum
                Else
                    Organism.Phylum = Phylum
                End If
                If Not String.IsNullOrEmpty(Organism.Kingdom) Then
                    Kingdom = Organism.Kingdom
                Else
                    Organism.Kingdom = Kingdom
                End If
            Next

            Return org
        End Function

        ''' <summary>
        ''' Gets the latest KEGG organism list from query the KEGG database.
        ''' </summary>
        ''' <returns></returns>
        <ExportAPI("list.Get", Info:="Gets the latest KEGG organism list from query the KEGG database.")>
        Public Function GetOrganismList() As KEGGOrganism
            Dim html As String = WEB_URL.GET
            Return __loadList(html)
        End Function

        ''' <summary>
        ''' Data from the external resources.
        ''' </summary>
        ''' <param name="url"></param>
        ''' <returns></returns>
        Public Function FromResource(url As String) As KEGGOrganism
            Dim page As String = url.GET
            Return __loadList(page)
        End Function
    End Module
End Namespace
