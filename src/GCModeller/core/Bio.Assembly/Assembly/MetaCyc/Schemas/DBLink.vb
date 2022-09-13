#Region "Microsoft.VisualBasic::67633229b7a2d575c4925d8acfdfe8ae, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\Schemas\DBLink.vb"

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


    ' Code Statistics:

    '   Total Lines: 180
    '    Code Lines: 120
    ' Comment Lines: 30
    '   Blank Lines: 30
    '     File Size: 8.39 KB


    '     Class DBLinkManager
    ' 
    '         Properties: CHEBI, IsEmpty, PUBCHEM
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: CreateFromMetaCycFormat, CreateObject, ToString
    '         Class DBLink
    ' 
    '             Properties: AccessionId, attributes, DBName
    ' 
    '             Function: CreateObject, GetFormatValue, GetMetaCycFormatValue, GetUniprotId, ToString
    '                       TryParse, TryParseMetaCycDBLink
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Text
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.MetaCyc.Schema

    ''' <summary>
    ''' MetaCyc database format dblink manager
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DBLinkManager : Inherits DBLinksManager(Of DBLink)

        ''' <summary>
        ''' 与其他的数据库之间的外键链接
        ''' </summary>
        ''' <remarks></remarks>
        Public Class DBLink : Implements IDBLink

            Public Property DBName As String Implements IDBLink.DbName
            Public Property AccessionId As String Implements IDBLink.EntryId
            Public Property attributes As String()

            Public Const SPLIT_REGX_EXPRESSION As String = " (?=(?:[^""]|""[^""]*"")*$)"

            Public Overrides Function ToString() As String
                Return String.Format("{0} -> {1}", DBName, AccessionId)
            End Function

            ''' <summary>
            ''' 解析来自于MetaCyc数据库中的*.dat文件中的DBLinks数据
            ''' </summary>
            ''' <param name="strData"></param>
            ''' <returns></returns>
            ''' <remarks>本方法和<see cref="TryParse">另外一个解析方法</see>的解析格式相似，仅在于TryParse方法是使用%进行分割的，由于在Csv文件中使用的是"进行分割，所以使用%符号可以避免一些不必要的字符串解析BUG</remarks>
            Public Shared Function TryParseMetaCycDBLink(strData As String) As DBLink
                Dim Tokens As String() = Regex.Split(Mid(strData, 2, Len(strData) - 2), SPLIT_REGX_EXPRESSION)

                If Tokens.IsNullOrEmpty Then
                    Return Nothing
                Else
                    Dim DBLink As DBLink = New DBLink With {
                        .attributes = Tokens.Skip(2).ToArray,
                        .DBName = Tokens(0),
                        .AccessionId = Tokens(1)}
                    DBLink.AccessionId = Mid(DBLink.AccessionId, 2, Len(DBLink.AccessionId) - 2)
                    Return DBLink
                End If
            End Function

            Public Shared Function GetUniprotId(DBLinks As Generic.IEnumerable(Of DBLink)) As String
                Return (From DBLink In DBLinks.SafeQuery Where String.Equals("UNIPROT", DBLink.DBName) Select DBLink.AccessionId).FirstOrDefault
            End Function

            ''' <summary>
            ''' 解析来自于Csv文件的DBLinks数据
            ''' </summary>
            ''' <param name="strData"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Shared Function TryParse(strData As String) As DBLink
                Dim Data As String = Regex.Match(strData, "\(\S+ %\S+?%").Value

                If String.IsNullOrEmpty(Data) Then
                    Return Nothing
                End If

                Dim DBName As String = Regex.Match(Data, "\(\S+").Value
                Dim EntryId As String = Regex.Match(Data, " %\S+%").Value

                DBName = Mid(DBName, 2)
                EntryId = Mid(EntryId, 3, Len(EntryId) - 3)
                Data = Regex.Match(strData, "\[\^ATTRIBUTES - .+?\]").Value
                If Not String.IsNullOrEmpty(Data) Then '有附加属性，则进行解析
                    Dim attrs As String = Mid(Data, 15).Trim
                    attrs = Mid(attrs, 1, Len(attrs) - 1)
                    Return New DBLink With {.DBName = DBName, .AccessionId = EntryId, .attributes = Strings.Split(attrs, "/")}
                Else
                    Return New DBLink With {.DBName = DBName, .AccessionId = EntryId, .attributes = New String() {}}
                End If
            End Function

            ''' <summary>
            ''' 向Csv文件写入数据所需求的一个方法
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function GetFormatValue() As String Implements IDBLink.GetFormatValue
                If attributes.IsNullOrEmpty Then
                    Return String.Format("({0} %{1}%)", DBName, AccessionId)
                End If

                Dim sBuilder As StringBuilder = New StringBuilder(128)
                For Each strToken As String In attributes
                    Call sBuilder.Append(strToken & "/")
                Next
                Call sBuilder.Remove(sBuilder.Length - 1, 1)

                Return String.Format("({0} %{1}% [^ATTRIBUTES - {2}])", DBName, AccessionId, sBuilder.ToString)
            End Function

            Public Shared Function CreateObject(DBLinks As Generic.IEnumerable(Of String)) As List(Of DBLink)
                Dim LQuery = (From strData As String In DBLinks Select DBLink.TryParse(strData)).AsList
                Return LQuery
            End Function

            ''' <summary>
            ''' 向MetaCyc数据库中的*.dat文件写入数据所需求
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function GetMetaCycFormatValue() As String
                If attributes.IsNullOrEmpty Then
                    Return String.Format("({0} ""{1}"")", DBName, AccessionId)
                End If

                Dim sBuilder As StringBuilder = New StringBuilder(1024)
                For Each strToken As String In attributes
                    Call sBuilder.Append(" " & strToken)
                Next

                Return String.Format("({0} ""{1}""{2})", DBName, AccessionId, sBuilder.ToString)
            End Function
        End Class

        Public Shared Function CreateFromMetaCycFormat(strData As Generic.IEnumerable(Of String)) As DBLinkManager
            Dim DBLinkManager As DBLinkManager = New DBLinkManager
            DBLinkManager.DBLinkObjects = (From strValue As String In strData Select DBLink.TryParseMetaCycDBLink(strValue)).ToArray
            DBLinkManager._CheBI = (From item In DBLinkManager.DBLinkObjects Where String.Equals(item.DBName, "chebi", StringComparison.OrdinalIgnoreCase) Select item).ToArray
            Dim LQuery = (From item In DBLinkManager.DBLinkObjects Where String.Equals(item.DBName, "pubchem", StringComparison.OrdinalIgnoreCase) Select item).ToArray
            If Not LQuery.IsNullOrEmpty Then
                DBLinkManager._PubChem = LQuery.First
            End If
            Return DBLinkManager
        End Function

        Sub New(strData As Generic.IEnumerable(Of String))
            MyBase._DBLinkObjects = DBLink.CreateObject(strData)

            _CheBI = (From item In MyBase.DBLinkObjects Where String.Equals(item.DBName, "chebi", StringComparison.OrdinalIgnoreCase) Select item).ToArray
            Dim LQuery = (From item In MyBase.DBLinkObjects Where String.Equals(item.DBName, "pubchem", StringComparison.OrdinalIgnoreCase) Select item).ToArray
            If Not LQuery.IsNullOrEmpty Then
                _PubChem = LQuery.First
            End If
        End Sub

        Protected Friend Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0} database entries.", DBLinkObjects.Count)
        End Function

        Public Shared Function CreateObject(dblinks As KeyValuePair(Of String, String)()) As DBLinkManager
            Dim LQuery = (From item In dblinks Select New Schema.DBLinkManager.DBLink With {.DBName = item.Key, .AccessionId = item.Value}).AsList
            Return New DBLinkManager With {._DBLinkObjects = LQuery}
        End Function

        Dim _CheBI As DBLink(), _PubChem As DBLink

        Public ReadOnly Property CHEBI As DBLink()
            Get
                Return _CheBI
            End Get
        End Property

        Public ReadOnly Property PUBCHEM As DBLink
            Get
                Return _PubChem
            End Get
        End Property

        Public Overrides ReadOnly Property IsEmpty As Boolean
            Get
                Return _CheBI.IsNullOrEmpty AndAlso _PubChem Is Nothing
            End Get
        End Property
    End Class
End Namespace
