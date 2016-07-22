#Region "Microsoft.VisualBasic::fcac0b4f1146ec267308d53194facacd, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\Pathway.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel

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
            Dim TempFile As String = App.AppSystemTemp & "/KEGG_PATHWAYS.txt"
            Call IO.File.WriteAllText(TempFile, My.Resources.br08901, encoding:=System.Text.Encoding.ASCII)
            Return LoadData(TempFile)
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
            Dim Chunkbuffer As String() = (From strLine As String In IO.File.ReadAllLines(path)
                                           Where Not String.IsNullOrEmpty(strLine) AndAlso
                                               (strLine.First = "A"c OrElse strLine.First = "B"c OrElse strLine.First = "C"c)
                                           Select strLine).ToArray
            Dim [Class] As String = "", Category As String = ""
            Dim ItemList As New List(Of Pathway)

            For i As Integer = 0 To Chunkbuffer.Length - 1
                Dim strLine As String = Chunkbuffer(i)
                Dim Id As Char = strLine.First

                strLine = Mid(strLine, 2).Trim

                If Id = "A"c Then
                    [Class] = BriteHText.NormalizePath(strLine.GetValue)
                ElseIf Id = "B"c Then
                    Category = BriteHText.NormalizePath(strLine)
                ElseIf Id = "C"c Then
                    Dim IdNum As String = Regex.Match(strLine, "\d{5}").Value
                    strLine = strLine.Replace(IdNum, "").Trim
                    ItemList += New Pathway With {
                        .Category = Category,
                        .Class = [Class],
                        .Entry = New KeyValuePair With {
                            .Key = IdNum,
                            .Value = strLine
                        }
                    }
                End If
            Next

            Return ItemList.ToArray
        End Function

        Public Shared Function CombineDIR(entry As Pathway, ParentDIR As String) As String
            Return String.Join("/", ParentDIR, [Module].TrimPath(entry.Class), [Module].TrimPath(entry.Category))
        End Function

        Public ReadOnly Property EntryId As String Implements IReadOnlyId.Identity
            Get
                Return Entry.Key
            End Get
        End Property

        Public Shared Function GetClass(EntryID As String, data As Pathway()) As Pathway
            Dim MatchID As String = (From m As Match
                                     In Regex.Matches(EntryID, "\d{5}")
                                     Select m.Value).Last
            Dim LQuery As Pathway = (From pwy As Pathway
                                     In data
                                     Where String.Equals(MatchID, pwy.Entry.Key)
                                     Select pwy).FirstOrDefault
            Return LQuery
        End Function
    End Class
End Namespace
