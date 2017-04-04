#Region "Microsoft.VisualBasic::cb4f2086224f59f39117ed03f3cd8d01, ..\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\Metabolites\MetabolitesDBGet.vb"

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

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Module MetabolitesDBGet

        Const URL = "http://www.kegg.jp/dbget-bin/www_bget?cpd:{0}"

        ''' <summary>
        ''' 使用KEGG compound的编号来下载代谢物数据
        ''' </summary>
        ''' <param name="ID">``cpd:ID``</param>
        ''' <returns></returns>
        Public Function DownloadCompound(ID As String) As Compound
            Return DownloadCompoundFrom(url:=String.Format(URL, ID))
        End Function

        ''' <summary>
        ''' 使用KEGG compound页面的url来下载代谢物数据
        ''' </summary>
        ''' <param name="url"></param>
        ''' <returns></returns>
        Public Function DownloadCompoundFrom(url As String) As Compound
            Return New WebForm(url).ParseCompound
        End Function

        <Extension>
        Public Function ParseCompound(html As WebForm) As Compound
            Dim links As DBLinks = GetDBLinks(html.GetValue("Other DBs").FirstOrDefault)
            Dim cpd As New Compound(links) With {
                .Entry = Regex.Match(html.GetValue("Entry").FirstOrDefault, "[GC]\d+").Value,
                .CommonNames = GetCommonNames(html.GetValue("Name").FirstOrDefault()),
                .Formula = html.GetValue("Formula").FirstOrDefault.Strip_NOBR.StripHTMLTags(stripBlank:=True),
                .KEGG_reactions = html _
                    .GetValue("Reaction") _
                    .FirstOrDefault _
                    .GetLinks() _
                    .Where(Function(s) Not s.IsShowAllLink) _
                    .ToArray,
                .Enzyme = html.GetValue("Enzyme").FirstOrDefault.GetLinks.Where(Function(s) Not s.IsShowAllLink).ToArray,
                .Pathway = __parseHTML_ModuleList(html.GetValue("Pathway").FirstOrDefault, LIST_TYPES.Pathway).Select(Function(s) String.Format("[{0}] {1}", s.Key, s.Value)).ToArray,
                .Module = __parseHTML_ModuleList(html.GetValue("Module").FirstOrDefault, LIST_TYPES.Module).Select(Function(s) String.Format("[{0}] {1}", s.Key, s.Value)).ToArray,
                .MolWeight = Val(html.GetValue("Mol weight").FirstOrDefault.Strip_NOBR.StripHTMLTags(stripBlank:=True)),
                .ExactMass = Val(html.GetValue("Exact mass").FirstOrDefault.Strip_NOBR.StripHTMLTags(stripBlank:=True)),
                .Remarks = html _
                    .GetValue("Remark") _
                    .Select(Function(s) s.Strip_NOBR.StripHTMLTags(stripBlank:=True).TrimNewLine) _
                    .ToArray
            }
            Return cpd
        End Function

        ''' <summary>
        ''' 下载指定编号集合的代谢物数据，并保存到指定的文件夹之中
        ''' </summary>
        ''' <param name="list">KEGG compound id list</param>
        ''' <param name="EXPORT"></param>
        ''' <returns>返回下载失败的对象的编号列表</returns>
        ''' <remarks></remarks>
        Public Function FetchTo(list As String(), EXPORT As String) As String()
            Dim failures As New List(Of String)
            Dim path$

            Call $"{list.Length} KEGG compounds are going to download!".__DEBUG_ECHO

            For Each cpdID As String In list

                path = String.Format("{0}/{1}.xml", EXPORT, cpdID)

                If Not path.FileExists Then
                    Dim CompoundData As Compound = DownloadCompound(cpdID)

                    If CompoundData Is Nothing Then
                        failures += cpdID
                    Else
                        Call CompoundData.GetXml.SaveTo(path)
                    End If
                End If
            Next

            Return failures
        End Function

        Const FLAG As String = "[a-z0-9-_.]+"
        Const regexpDBLink As String = FLAG & ": (\s*<a href="".+?"">" & FLAG & "</a>\s*)+"

        Friend Function GetDBLinks(html$) As DBLinks
            If String.IsNullOrEmpty(html) Then
                Return Nothing
            End If

            Dim t$() = html.DivInternals
            Dim LQuery As DBLink() = t _
                .SlideWindows(2, 2) _
                .Select(Function(s) TryParse(s(0).StripHTMLTags(stripBlank:=True).Trim(":"c).Trim, s(1))) _
                .IteratesALL _
                .ToArray
            Return New DBLinks(LQuery)
        End Function

        <Extension> Private Function TryParse(DBName$, values$) As DBLink()
            Dim IDs$() = values _
                .StripHTMLTags(stripBlank:=True) _
                .Split _
                .Select(AddressOf Trim) _
                .Where(Function(s) Not s.StringEmpty) _
                .ToArray
            Dim LQuery As String = LinqAPI.DefaultFirst(Of String) <=
 _
                From prefixName As String
                In DBLinks.PrefixDB
                Where InStr(DBName, prefixName, CompareMethod.Text) > 0
                Select prefixName

            DBName = If(String.IsNullOrEmpty(LQuery), DBName, LQuery)

            Return IDs.Select(
                Function(ID$) New DBLink With {
                    .DBName = DBName,
                    .Entry = ID
                }).ToArray
        End Function

        Friend Function GetCommonNames(str$) As String()
            If String.IsNullOrEmpty(str) Then
                Return New String() {}
            End If

            Dim buf As String() = str.Strip_NOBR.HtmlLines
            buf = LinqAPI.Exec(Of String) <=
 _
                From s As String
                In buf
                Let line As String = s.StripHTMLTags(stripBlank:=True).Trim(";"c, " "c)
                Where Not String.IsNullOrEmpty(line)
                Select line

            Return buf
        End Function

        ''' <summary>
        ''' 请注意，这个函数仅会根据文件名的前缀来判断类型
        ''' </summary>
        ''' <param name="xml$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function LoadCompoundObject(xml$) As Compound
            Dim ID$ = xml.BaseName

            If ID.First = "G"c Then
                Return xml.LoadXml(Of Glycan)(stripInvalidsCharacter:=True)
            Else
                Return xml.LoadXml(Of Compound)(stripInvalidsCharacter:=True)
            End If
        End Function
    End Module
End Namespace
