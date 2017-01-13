#Region "Microsoft.VisualBasic::d0aee2fbe807627e0a24021e885b9336, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\Pathway.vb"

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
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.HtmlParser

Namespace Assembly.KEGG.DBGET.BriteHEntry

    ''' <summary>
    ''' The brief entry information for the pathway objects in the KEGG database.(KEGG数据库之中的代谢途径对象的入口点信息) 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Pathway : Implements IReadOnlyId

        ''' <summary>
        ''' A
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property [Class] As String
        ''' <summary>
        ''' B
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Category As String

        ''' <summary>
        ''' C
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Entry As KeyValuePair

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]{1}   {2}", [Class], Category, Entry.ToString)
        End Function

        ''' <summary>
        ''' 从程序的自身的资源文件之中加载数据
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LoadFromResource() As Pathway()
            Return LoadStream(My.Resources.br08901)
        End Function

        Public Shared Function LoadDictionary() As Dictionary(Of String, Pathway)
            Dim data = LoadFromResource()
            Return LoadDictionary(data)
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
                In text.lTokens
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

                    line = line.Replace(IdNum, "").Trim
                    out += New Pathway With {
                        .Category = category,
                        .Class = [class],
                        .Entry = New KeyValuePair With {
                            .Key = IdNum,
                            .Value = line
                        }
                    }
                End If
            Next

            Return out.ToArray
        End Function

        Public Shared Function CombineDIR(entry As Pathway, ParentDIR As String) As String
            Return String.Join("/", ParentDIR, [Module].TrimPath(entry.Class), [Module].TrimPath(entry.Category))
        End Function

        ''' <summary>
        ''' <see cref="Entry"/>::<see cref="KeyValuePair.Key"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property EntryId As String Implements IReadOnlyId.Identity
            Get
                Return Entry.Key
            End Get
        End Property

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
