#Region "Microsoft.VisualBasic::c986bccdc44279ff1d6bc76b216e5791, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\Compound.vb"

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

Imports System.Text.RegularExpressions
Imports System.Text
Imports System.Xml.Serialization
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.HtmlParser

Namespace Assembly.KEGG.DBGET.bGetObject

    <XmlRoot("KEGG.Compound", Namespace:="http://www.kegg.jp/dbget-bin/www_bget?cpd:compound_id")>
    Public Class Compound : Implements ICompoundObject

        Public Overridable Property Entry As String Implements ICompoundObject.Key, ICompoundObject.locusId
        Public Property CommonNames As String() Implements ICompoundObject.CommonNames
        Public Property Formula As String
        Public Property MolWeight As Double

        ''' <summary>
        ''' The <see cref="Entry">compound</see> was involved in these reactions. (http://www.kegg.jp/dbget-bin/www_bget?rn:[KEGG_Reaction_ID])
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property InvolvedReactions As String()
        Public Property Pathway As String()
        Public Property [Module] As String()

        Dim _DBLinks As DBLinks
        Public Property DbLinks As String() 'Implements MetaCyc.Schema.CompoundsMapping.ICompoundObject.DBLinks
            Get
                If _DBLinks Is Nothing Then
                    Return New String() {}
                End If
                Return _DBLinks.DBLinks
            End Get
            Set(value As String())
                _DBLinks = New DBLinks(value)
            End Set
        End Property

        Public Function GetDBLinkManager() As DBLinks
            Return _DBLinks
        End Function

        Public Sub DownloadStructureImage(SavedPath As String)
            Dim Url As String = String.Format("http://www.kegg.jp/Fig/compound/{0}.gif", Entry)
            Call Url.DownloadFile(SavedPath)
        End Sub

        Public Function GetPathways() As KeyValuePair(Of String, String)()
            Return GetKeyValuePair(Pathway)
        End Function

        Public Function GetModules() As KeyValuePair(Of String, String)()
            Return GetKeyValuePair([Module])
        End Function

        Public Function GetDBLinks() As DBLink()
            Return _DBLinks.DBLinkObjects.ToArray
        End Function

        Private Shared Function GetKeyValuePair(strData As String()) As KeyValuePair(Of String, String)()
            Dim LQuery = (From s As String
                          In strData
                          Let Id As String = Regex.Match(s, "[.+?]").Value
                          Let value As String = s.Replace(Id, "").Trim
                          Select New KeyValuePair(Of String, String)(Id, value)).ToArray
            Return LQuery
        End Function

        Const URL = "http://www.kegg.jp/dbget-bin/www_bget?cpd:{0}"

        Public Shared Function Download(Id As String) As Compound
            Return DownloadFrom(url:=String.Format(URL, Id))
        End Function

        Public Shared Function DownloadFrom(url As String) As Compound
            Dim WebForm As New KEGG.WebServices.InternalWebFormParsers.WebForm(url)
            Dim Compound As New Compound With {
                .Entry = Regex.Match(WebForm.GetValue("Entry").FirstOrDefault, "[GC]\d+").Value
            }
            Compound.CommonNames = GetCommonNames(WebForm.GetValue("Name").FirstOrDefault())
            Compound.Formula = WebForm.GetValue("Formula").FirstOrDefault.Replace("<br>", "")
            Compound.InvolvedReactions = GetReactionList(WebForm.GetValue("Reaction").FirstOrDefault)
            Compound.Pathway = (From item In KEGG.WebServices.InternalWebFormParsers.WebForm.parseList(WebForm.GetValue("Pathway").FirstOrDefault, "<a href="".*/kegg-bin/show_pathway\?.+?"">.+?</a>") Select String.Format("[{0}] {1}", item.Key, item.Value)).ToArray
            Compound.Module = (From item In KEGG.WebServices.InternalWebFormParsers.WebForm.parseList(WebForm.GetValue("Module").FirstOrDefault, "<a href="".*/kegg-bin/show_module\?.+?"">.+?</a>") Select String.Format("[{0}] {1}", item.Key, item.Value)).ToArray
            Compound._DBLinks = GetDBLinks(WebForm.GetValue("Other DBs").FirstOrDefault)
            Compound.MolWeight = Val(WebForm.GetValue("Mol weight").FirstOrDefault)

            Return Compound
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", Entry, Me.Formula)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="lstId"></param>
        ''' <param name="EXPORT"></param>
        ''' <returns>返回成功下载的对象的数目</returns>
        ''' <remarks></remarks>
        Public Shared Function FetchTo(lstId As String(), EXPORT As String) As Integer
            Dim i As Integer = 0

            Call $"{lstId} go to download!".__DEBUG_ECHO

            For Each Id As String In lstId
                Dim path As String = String.Format("{0}/{1}.xml", EXPORT, Id)

                If Not path.FileExists Then
                    Dim CompoundData As Compound = Compound.Download(Id)

                    If CompoundData Is Nothing Then
#If DEBUG Then
                        Call Console.WriteLine("{0} download failure!", Id)
#End If
                        Continue For
                    End If
                    i += 1
                    Call CompoundData.GetXml.SaveTo(path)
                End If
            Next

            Return i
        End Function

        Const FLAG As String = "[a-z0-9-_.]+"
        Const REGEX_DBLINK As String = FLAG & ": (\s*<a href="".+?"">" & FLAG & "</a>\s*)+"

        Friend Shared Function GetDBLinks(strData As String) As DBLinks
            If String.IsNullOrEmpty(strData) Then
                Return Nothing
            End If

            Dim Tokens As String() = (From m As Match
                                      In Regex.Matches(strData, REGEX_DBLINK, RegexOptions.IgnoreCase)
                                      Select m.Value).ToArray
            Dim LQuery As IEnumerable(Of DBLink()) = From s As String In Tokens Select TryParse(s)
            Return New DBLinks(LQuery.IteratesALL)
        End Function

        Private Shared Function TryParse(str As String) As DBLink()
            Dim TempChunk As String() = Strings.Split(str, ": ")
            Dim DBName As String = TempChunk.First
            Dim Entries As String() = GetValues(TempChunk.Last)
            Dim LQuery As String = (From prefixName As String
                                    In ComponentModel.DBLinkBuilder.DBLinks.PrefixDB
                                    Where InStr(DBName, prefixName, CompareMethod.Text) > 0
                                    Select prefixName).FirstOrDefault

            DBName = If(String.IsNullOrEmpty(LQuery), DBName, LQuery)

            Return (From s As String
                    In Entries
                    Select New DBLink With {.DBName = DBName, .Entry = s}).ToArray
        End Function

        Private Shared Function GetValues(str As String) As String()
            Dim buf As String() =
                Regex.Matches(str, "<a href="".+?"">.+?</a>") _
               .ToArray(AddressOf HtmlStrips.GetValue)
            Return buf
        End Function

        Friend Shared Function GetReactionList(strData As String) As String()
            If String.IsNullOrEmpty(strData) Then
                Return New String() {}
            End If

            Dim buf As String() =
                Regex.Matches(strData, "<a href="".+?"">.+?</a>", RegexOptions.Singleline) _
               .ToArray(AddressOf HtmlStrips.GetValue)
            Return buf
        End Function

        Friend Shared Function GetCommonNames(strData As String) As String()
            If String.IsNullOrEmpty(strData) Then
                Return New String() {}
            End If

            Dim buf As String() = Strings.Split(strData, "<br>")
            buf = (From s As String
                   In buf
                   Let strItem As String = s.Replace(";", "").Trim
                   Where Not String.IsNullOrEmpty(strItem)
                   Select strItem).ToArray

            Return buf
        End Function

        Public Property CHEBI As String() Implements ICompoundObject.CHEBI
            Get
                If _DBLinks.IsNullOrEmpty OrElse _DBLinks.CHEBI.IsNullOrEmpty Then
                    Return Nothing
                End If
                Return (From item In _DBLinks.CHEBI Select item.Entry).ToArray
            End Get
            Set(value As String())
                _DBLinks.AddEntry(New DBLink With {.DBName = "CHEBI", .Entry = value.First})
            End Set
        End Property

        Public Property PUBCHEM As String Implements ICompoundObject.PUBCHEM
            Get
                If _DBLinks.IsNullOrEmpty OrElse _DBLinks.PUBCHEM Is Nothing Then
                    Return ""
                End If
                Return _DBLinks.PUBCHEM.Entry
            End Get
            Set(value As String)
                _DBLinks.AddEntry(New DBLink With {.DBName = "PUBCHEM", .Entry = value})
            End Set
        End Property
    End Class
End Namespace
