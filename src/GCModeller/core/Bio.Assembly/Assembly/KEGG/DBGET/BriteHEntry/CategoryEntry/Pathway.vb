#Region "Microsoft.VisualBasic::fa289d3c8d0c247e88381e8a5e0ec07c, Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\CategoryEntry\Pathway.vb"

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

    '     Class Pathway
    ' 
    '         Properties: [class], category, entry, EntryId
    ' 
    '         Function: GetClass, GetGlobalAndOverviewMaps, GetPathCategory, LoadData, (+3 Overloads) LoadDictionary
    '                   LoadFromResource, LoadStream, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.DBGET.BriteHEntry

    ''' <summary>
    ''' The brief entry information for the pathway objects in the KEGG database.
    ''' (KEGG数据库之中的代谢途径对象的分类以及入口点信息) 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Pathway : Implements IReadOnlyId

        ''' <summary>
        ''' A
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property [class] As String
        ''' <summary>
        ''' B
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property category As String

        ''' <summary>
        ''' **C**, example as: ``01100  Metabolic pathways``
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property entry As NamedValue

        ''' <summary>
        ''' <see cref="Entry"/>::<see cref="NamedValue.name"/>, ``\d+``
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <XmlIgnore>
        Public ReadOnly Property EntryId As String Implements IReadOnlyId.Identity
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return entry.name
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]{1}   {2}", [class], category, entry.ToString)
        End Function

        Public Shared Function GetGlobalAndOverviewMaps() As NamedValue()
            Dim brite As Pathway() = LoadFromResource()
            Dim globals As Pathway() = brite _
                .Where(Function(b)
                           Return b.class = "Metabolism" AndAlso b.category = "Global and overview maps"
                       End Function) _
                .ToArray

            Return globals _
                .Select(Function(b) b.entry) _
                .ToArray
        End Function

        ''' <summary>
        ''' 从程序的自身的资源文件之中加载数据
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LoadFromResource() As Pathway()
            Return LoadStream(My.Resources.br08901)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function LoadDictionary() As Dictionary(Of String, Pathway)
            Return LoadFromResource().DoCall(AddressOf LoadDictionary)
        End Function

        Public Shared Function LoadDictionary(res As IEnumerable(Of Pathway)) As Dictionary(Of String, Pathway)
            Dim dict = res.ToDictionary(Function(x) x.EntryId)
            Return dict
        End Function

        Public Shared Function LoadDictionary(res As String) As Dictionary(Of String, Pathway)
            Dim data As Pathway() = LoadData(res)
            Return LoadDictionary(data)
        End Function

        ''' <summary>
        ''' 从文件之中加载数据
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LoadData(path As String) As Pathway()
            Return LoadStream(path.ReadAllText)
        End Function

        Public Shared Function LoadStream(text$) As Pathway()
            Dim lines$() = LinqAPI.Exec(Of String) <=
                From line As String
                In text.LineTokens
                Where Not String.IsNullOrEmpty(line) AndAlso
                    (line.First = "A"c OrElse
                     line.First = "B"c OrElse
                     line.First = "C"c)
                Select line

            Dim [class] As String = "", category As String = ""
            Dim out As New List(Of Pathway)

            For Each line As String In lines
                Dim Id As Char = line.First

                line = Mid(line, 2).Trim

                If Id = "A"c Then
                    [class] = BriteHText.NormalizePath(line.GetValue)

                ElseIf Id = "B"c Then
                    category = BriteHText.NormalizePath(line)

                ElseIf Id = "C"c Then
                    Dim IdNum As String = Regex.Match(line, "\d{5}").Value

                    line = Mid(line, IdNum.Length + 1).Trim
                    out += New Pathway With {
                        .category = category,
                        .class = [class],
                        .entry = New NamedValue With {
                            .name = IdNum,
                            .text = line
                        }
                    }
                End If
            Next

            Return out.ToArray
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetPathCategory() As String
            Return {
                [Module].TrimPath([class]),
                [Module].TrimPath(category)
            }.JoinBy("/")
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="KO"></param>
        ''' <param name="data">``<see cref="Pathway.EntryId"/> -> <see cref="Pathway"/>``</param>
        ''' <returns></returns>
        Public Shared Function GetClass(KO As String, data As Dictionary(Of String, Pathway)) As Pathway
            Dim MatchID As String = Regex.Matches(KO, "\d{5}").ToArray.Last

            If data.ContainsKey(MatchID) Then
                Return data(MatchID)
            Else
                Return Nothing
            End If
        End Function
    End Class
End Namespace
