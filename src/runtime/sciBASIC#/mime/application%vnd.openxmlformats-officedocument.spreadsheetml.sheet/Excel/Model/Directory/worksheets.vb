﻿#Region "Microsoft.VisualBasic::bf8a19c7dd385e74723ef6f046e9cd70, mime\application%vnd.openxmlformats-officedocument.spreadsheetml.sheet\Excel\Model\Directory\worksheets.vb"

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

    '     Class worksheets
    ' 
    '         Properties: _rels, worksheets
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: _name, GetWorksheet, HaveWorksheet
    ' 
    '         Sub: _loadContents, Add, Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.MIME.Office.Excel.XML._rels
Imports Microsoft.VisualBasic.MIME.Office.Excel.XML.xl
Imports Microsoft.VisualBasic.MIME.Office.Excel.XML.xl.worksheets

Namespace Model.Directory

    Public Class worksheets : Inherits Directory

        ''' <summary>
        ''' Key都是格式``sheet%d``的字符串
        ''' </summary>
        ''' <returns></returns>
        Public Property worksheets As Dictionary(Of String, XML.xl.worksheets.worksheet)
        Public Property _rels As Dictionary(Of String, rels)

        Sub New(ROOT$)
            Call MyBase.New(ROOT)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function HaveWorksheet(sheetID$) As Boolean
            Return worksheets.ContainsKey(sheetID) OrElse worksheets.ContainsKey("sheet" & sheetID)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub Add(sheetID As String, worksheet As XML.xl.worksheets.worksheet)
            Call worksheets.Add(sheetID, worksheet)
        End Sub

        Public Function GetWorksheet(sheetID$) As XML.xl.worksheets.worksheet
            If worksheets.ContainsKey(sheetID) Then
                Return worksheets(sheetID)
            Else
                For Each key In worksheets.Keys
                    If key.TextEquals(sheetID) Then
                        sheetID = key
                        Return worksheets(sheetID)
                    End If
                Next
            End If

            With "sheet" & sheetID
                If worksheets.ContainsKey(.ByRef) Then
                    Return worksheets(.ByRef)
                Else
                    Return Nothing
                End If
            End With
        End Function

        Protected Overrides Sub _loadContents()
            ' 2017-12-18 发现有时候会出现sheetID不一致的情况，这种情况可能会出现于用户手动的从Excel电子表格文件之中删除了前面的几个表
            ' 所以在这里不可以直接使用文件名来作为sheet的编号名称
            ' r:id是一致的
            worksheets = (ls - l - "*.xml" <= Folder) _
                .Select(Function(path) (path:=path, path.LoadXml(Of worksheet))) _
                .ToDictionary(Function(page) page.path.BaseName,
                              Function(page) page.Item2)
            _rels = (ls - l - "*.rels" <= (Folder & "/_rels")) _
                .ToDictionary(Function(path) path.BaseName,
                              Function(path) path.LoadXml(Of rels))
        End Sub

        Public Sub Save()
            Dim path$

            For Each sheet In worksheets
                path = $"{Folder}/{sheet.Key}.xml"
                sheet.Value _
                    .ToXML _
                    .SaveTo(path, Encoding.UTF8)
            Next
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function _name() As String
            Return NameOf(worksheets)
        End Function
    End Class
End Namespace
