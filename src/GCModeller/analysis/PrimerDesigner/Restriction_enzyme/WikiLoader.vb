#Region "Microsoft.VisualBasic::6be4bb843b9b60382e8170fd8ca909c1, ..\GCModeller\analysis\PrimerDesigner\Restriction_enzyme\WikiLoader.vb"

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
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Restriction_enzyme

    ''' <summary>
    ''' Parser for wikipadia
    ''' </summary>
    Public Module WikiLoader

        ''' <summary>
        ''' Loads the enzyme data source from the offline wiki page directory
        ''' </summary>
        ''' <param name="DIR"></param>
        ''' <returns></returns>
        <Extension> Public Function LoadDIR(DIR As String) As Enzyme()
            Dim files = FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchTopLevelOnly, "*.html", "*.htm")
            Dim LQuery = (From page As String
                          In files'.AsParallel
                          Select page.HTMLParser).ToVector
            Return LQuery
        End Function

        ReadOnly __dataList As String() = New List(Of String) +
            "https://en.wikipedia.org/wiki/List_of_restriction_enzyme_cutting_sites:_A" +
            "https://en.wikipedia.org/wiki/List_of_restriction_enzyme_cutting_sites:_Ba%E2%80%93Bc#Whole_list_navigation" +
            "https://en.wikipedia.org/wiki/List_of_restriction_enzyme_cutting_sites:_Bd%E2%80%93Bp#Whole_list_navigation" +
            "https://en.wikipedia.org/wiki/List_of_restriction_enzyme_cutting_sites:_Bsa%E2%80%93Bso#Whole_list_navigation" +
            "https://en.wikipedia.org/wiki/List_of_restriction_enzyme_cutting_sites:_Bsp%E2%80%93Bss#Whole_list_navigation" +
            "https://en.wikipedia.org/wiki/List_of_restriction_enzyme_cutting_sites:_Bst%E2%80%93Bv#Whole_list_navigation" +
            "https://en.wikipedia.org/wiki/List_of_restriction_enzyme_cutting_sites:_C%E2%80%93D#Whole_list_navigation" +
            "https://en.wikipedia.org/wiki/List_of_restriction_enzyme_cutting_sites:_E%E2%80%93F#Whole_list_navigation" +
            "https://en.wikipedia.org/wiki/List_of_restriction_enzyme_cutting_sites:_G%E2%80%93K#Whole_list_navigation" +
            "https://en.wikipedia.org/wiki/List_of_restriction_enzyme_cutting_sites:_L%E2%80%93N#Whole_list_navigation" +
            "https://en.wikipedia.org/wiki/List_of_restriction_enzyme_cutting_sites:_O%E2%80%93R#Whole_list_navigation" +
            "https://en.wikipedia.org/wiki/List_of_restriction_enzyme_cutting_sites:_S#Whole_list_navigation" +
            "https://en.wikipedia.org/wiki/List_of_restriction_enzyme_cutting_sites:_T%E2%80%93Z#Whole_list_navigation"

        ''' <summary>
        ''' Download data from wiki
        ''' </summary>
        ''' <returns></returns>
        Public Function FromWiki() As Enzyme()
            Dim list As New List(Of Enzyme)

            For Each url As String In WikiLoader.__dataList
                list += url.HTMLParser
            Next

            Return list
        End Function

        Const TableStart As String = "<table.+?class=""sortable jquery-tablesorter"".+?</tbody>\s*<tfoot>\s*</tfoot>\s*</table>"
        Const EnzymeSection As String = "<span class=""mw-headline"" id=""Restriction_enzymes"">Restriction enzymes</span>"

        <Extension> Public Function HTMLParser(url As String) As Enzyme()
            Dim html As String = url.GET
            html = Strings.Split(html, EnzymeSection).Last
            html = Regex.Match(html, TableStart, RegexOptions.Singleline Or RegexOptions.IgnoreCase).Value
            Dim rows As String() = Regex.Matches(html, "<tr>.+?</tr>", RegexOptions.Singleline Or RegexOptions.IgnoreCase).ToArray
            Dim enzymes As Enzyme() = rows.Skip(1).ToArray(Function(row) row.__enzymeParser)
            Return enzymes
        End Function

        Const htmlTag As String = "<.+?>"

        <Extension> Private Function __enzymeParser(row As String) As Enzyme
            Dim cols As String() = Regex.Matches(row.Replace("&nbsp;", " "), "<td>.+?</td>", RegexOptions.Singleline Or RegexOptions.IgnoreCase) _
                .ToArray(Function(s) s.TrimVBCrLf.Trim)
            Dim enzyme As New Enzyme
            Dim p As New Pointer(Of String)

            Try
                enzyme.Enzyme = Regex.Replace(cols + p, htmlTag, "", RegexOptions.IgnoreCase Or RegexOptions.Singleline)
                enzyme.Source = Regex.Replace(cols + p, htmlTag, "", RegexOptions.Singleline Or RegexOptions.IgnoreCase)
                enzyme.Recognition = (cols + p).__recognitionParser
                enzyme.Cut = (cols + p).__cutsParser
                enzyme.Isoschizomers = (cols + p).__isoschizomersParser
            Catch ex As Exception
                ex = New Exception(enzyme.GetJson, ex)
                ex = New Exception(row, ex)
                Call App.LogException(ex)
                Call ex.PrintException
            End Try

            Return enzyme
        End Function

        <Extension>
        Private Function __recognitionParser(s As String) As Recognition
            Dim sides As String() = Regex.Matches(s, "<code>.+?</code>", RegexOptions.IgnoreCase Or RegexOptions.Singleline).ToArray(Function(ss) ss.GetValue)
            Dim sh = (From ss As String In sides Let sd As String = Regex.Match(ss, "\d'").Value Select sd, seq = ss.Replace(sd, "").Trim).ToArray
            Dim F As String = (From x In sh Where String.Equals(x.sd, "5'") Select x.seq).FirstOrDefault
            Dim R As String = (From x In sh Where String.Equals(x.sd, "3'") Select x.seq).FirstOrDefault
            Return New Recognition With {
                .Forwards = F,
                .Reversed = R
            }
        End Function

        <Extension> Public Function __cutsParser(s As String) As Cut()
            Dim sides As String() = Regex.Matches(s, "<code>.+?</code>", RegexOptions.IgnoreCase Or RegexOptions.Singleline).ToArray(Function(ss) Regex.Replace(ss.GetValue, htmlTag, "@"))
            Return sides.ToArray(Function(ss) Cut.Parser(ss))
        End Function

        <Extension> Public Function __isoschizomersParser(s As String) As String()
            s = Regex.Replace(s, htmlTag, "").Trim
            Dim tokens As String() = s.Split(","c).ToArray(Function(ss) ss.Trim)
            tokens = (From ss As String In tokens Where Not String.IsNullOrEmpty(ss) Select ss).ToArray
            Return tokens
        End Function
    End Module
End Namespace
