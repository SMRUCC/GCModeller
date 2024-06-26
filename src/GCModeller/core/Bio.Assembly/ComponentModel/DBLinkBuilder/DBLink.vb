﻿#Region "Microsoft.VisualBasic::1bbb0e34d94b7bfa647b649a2e58fe59, core\Bio.Assembly\ComponentModel\DBLinkBuilder\DBLink.vb"

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

    '   Total Lines: 79
    '    Code Lines: 50 (63.29%)
    ' Comment Lines: 15 (18.99%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 14 (17.72%)
    '     File Size: 2.75 KB


    '     Class DBLink
    ' 
    '         Properties: DBName, entry, link
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: CreateObject, FromTagValue, GetFormatValue, (+3 Overloads) ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace ComponentModel.DBLinkBuilder

    ''' <summary>
    ''' Database xref data
    ''' </summary>
    Public Class DBLink : Implements IKeyValuePairObject(Of String, String)
        Implements IDBLink
        Implements INamedValue

        ''' <summary>
        ''' Database name
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Property DBName As String Implements IKeyValuePairObject(Of String, String).Key, INamedValue.Key, IDBLink.DbName

        ''' <summary>
        ''' Entity uid in the target database
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Property entry As String Implements IKeyValuePairObject(Of String, String).Value, IDBLink.EntryId

        ''' <summary>
        ''' the url link to the object in the <see cref="DBName"/> database
        ''' </summary>
        ''' <returns></returns>
        <XmlText> Public Property link As String

        Sub New()
        End Sub

        Sub New(DB$, ID$)
            DBName = DB
            entry = ID
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return ToString(Me)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Function ToString(DBLink As DBLink) As String
            Return ToString(DBLink.DBName, DBLink.entry)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Function ToString(DBName As String, Entry As String) As String
            Return String.Format("[{0}] {1}", DBName, Entry)
        End Function

        Public Shared Function CreateObject(strData As String) As DBLink
            Dim Name As String = Regex.Match(strData, "\[.+?\] ").Value
            Dim Entry = strData.Replace(Name, "").Trim

            Return New DBLink With {
                .DBName = Name.Trim.GetString,
                .entry = Entry
            }
        End Function

        Public Shared Function FromTagValue(s$, Optional tag$ = ":") As DBLink
            With s.GetTagValue(tag, trim:=True)
                Return New DBLink(.Name, .Value)
            End With
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetFormatValue() As String Implements IDBLink.GetFormatValue
            Return ToString(Me)
        End Function
    End Class
End Namespace
