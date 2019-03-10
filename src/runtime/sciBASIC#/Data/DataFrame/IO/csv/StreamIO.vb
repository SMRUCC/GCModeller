﻿#Region "Microsoft.VisualBasic::c0c309eba867c337c42342839dd52e4b, Data\DataFrame\IO\csv\StreamIO.vb"

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

    '     Module StreamIO
    ' 
    '         Function: [GetType], SaveDataFrame
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.ComponentModels
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

Namespace IO

    Public Module StreamIO

        ''' <summary>
        ''' 根据文件的头部的定义，从<paramref name="types"/>之中选取得到最合适的类型的定义
        ''' </summary>
        ''' <param name="csv"></param>
        ''' <param name="types"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function [GetType](csv As File, ParamArray types As Type()) As Type
            Dim headers As Index(Of String) = csv.First.ToArray
            Dim scores As New List(Of (Type, Integer))

            For Each schema In types.Select(AddressOf SchemaProvider.CreateObject)
                Dim allNames$() = schema.Properties _
                                        .Select(Function(x) x.Name) _
                                        .ToArray
                Dim matches = Aggregate p As String
                              In allNames
                              Where headers.IndexOf(p) > -1
                              Into Sum(1)

                scores += (schema.DeclaringType, matches)
            Next

            Dim desc = From score As (type As Type, score%)
                       In scores
                       Select type = score.Item1, Value = score.Item2
                       Order By Value Descending
            Dim target As Type = desc.FirstOrDefault?.type
            Return target
        End Function

        Const NullLocationRef$ = "Sorry, the ``path`` reference to a null location!"

        ''' <summary>
        ''' Save this csv document into a specific file location <paramref name="path"/>.
        ''' </summary>
        ''' <param name="path">
        ''' 假若路径是指向一个已经存在的文件，则原有的文件数据将会被清空覆盖
        ''' </param>
        ''' <remarks>当目标保存路径不存在的时候，会自动创建文件夹</remarks>
        <Extension>
        Public Function SaveDataFrame(csv As IEnumerable(Of RowObject), Optional path$ = "", Optional encoding As Encoding = Nothing, Optional tsv As Boolean = False) As Boolean
            Dim stopwatch As Stopwatch = Stopwatch.StartNew
            Dim del$ = ","c Or ASCII.TAB.AsDefault(Function() tsv)

            If path.StringEmpty Then
                Throw New NullReferenceException(NullLocationRef)
            End If

            Using out As StreamWriter = path.OpenWriter(encoding Or UTF8)
                For Each line$ In csv.Select(Function(r) r.AsLine(del))
                    Call out.WriteLine(line)
                Next
            End Using

            Call $"Generate csv file document using time {stopwatch.ElapsedMilliseconds} ms.".__INFO_ECHO

            Return True
        End Function
    End Module
End Namespace
