#Region "Microsoft.VisualBasic::11b5bb431df4f9875b0ec7000bb4e63a, ..\GCModeller\core\Bio.Assembly\ComponentModel\DBLinkBuilder\DBLink.vb"

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
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace ComponentModel.DBLinkBuilder

    Public Class DBLink : Implements IKeyValuePairObject(Of String, String)
        Implements IDBLink
        Implements sIdEnumerable

        Public Property DBName As String Implements IKeyValuePairObject(Of String, String).Identifier, sIdEnumerable.Identifier, IDBLink.locusId
        Public Property Entry As String Implements IKeyValuePairObject(Of String, String).Value, IDBLink.Address

        Public Overrides Function ToString() As String
            Return ToString(Me)
        End Function

        Public Overloads Shared Function ToString(DBLink As DBLink) As String
            Return ToString(DBLink.DBName, DBLink.Entry)
        End Function

        Public Overloads Shared Function ToString(DBName As String, Entry As String) As String
            Return String.Format("[{0}] {1}", DBName, Entry)
        End Function

        Public Shared Function CreateObject(strData As String) As DBLink
            Dim Name As String = Regex.Match(strData, "\[.+?\] ").Value
            Dim Entry = strData.Replace(Name, "").Trim
            Return New DBLink With {
                .DBName = RemoveQuot(Name.Trim),
                .Entry = Entry
            }
        End Function

        Private Shared Function RemoveQuot(str As String) As String
            str = Mid(str, 2)
            str = Mid(str, 1, Len(str) - 1)
            Return str
        End Function

        Public Function GetFormatValue() As String Implements IDBLink.GetFormatValue
            Return ToString(Me)
        End Function
    End Class
End Namespace
