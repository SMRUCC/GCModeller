﻿Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text

Namespace IO

    Public Module FileLoader

        ''' <summary>
        ''' If you are sure about your csv data document have no character such like " or, in a cell, then you can try using this fast load method to load your csv data.
        ''' if not, please using the <see cref="load"></see> method to avoid of the data damages.
        ''' (假若你确信你的数据文件之中仅含有数字之类的数据，则可以尝试使用本方法进行快速加载，假若文件之中每一个单元格还含有引起歧义的例如双引号或者逗号，则请不要使用本方法进行加载)
        ''' </summary>
        ''' <param name="path"></param>
        ''' <param name="encoding"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 因为这个函数是直接通过利用逗号做切割，所以效率是非常高的，但是对文本的格式要求非常高
        ''' </remarks>
        Public Function FastLoad(path As String,
                                 Optional parallel As Boolean = True,
                                 Optional encoding As Encoding = Nothing,
                                 Optional skipWhile As NamedValue(Of Func(Of String, Boolean)) = Nothing) As File

            Dim sw As Stopwatch = Stopwatch.StartNew
            Dim lines As String() = path.MapNetFile.ReadAllLines(encoding Or DefaultEncoding)
            Dim headers As New RowObject(lines(Scan0))
            Dim cData As New File
            Dim headerIndex As Integer = headers.IndexOf(skipWhile.Name)

            If parallel Then
                Dim cache = (From x As SeqValue(Of String) In lines.SeqIterator Select x)
                Dim Rows = From line As SeqValue(Of String)
                           In cache.AsParallel
                           Let __innerList As RowObject = New RowObject(line.value.Split(","c))
                           Select i = line.i,
                                data = __innerList
                           Order By i Ascending

                If headerIndex >= 0 Then
                    cData._innerTable = (From item In Rows Where Not skipWhile.Value(item.data(headerIndex)) Select item.data).AsList
                Else
                    cData._innerTable = (From item In Rows Select item.data).AsList
                End If
            Else
                Dim Rows = From strLine As String In lines
                           Let InternalList As RowObject = New RowObject(strLine.Split(","c))
                           Select InternalList

                If headerIndex >= 0 Then
                    cData._innerTable = Rows _
                        .Where(Function(r)
                                   Return Not skipWhile.Value(r(headerIndex))
                               End Function) _
                        .AsList
                Else
                    cData._innerTable = Rows.AsList
                End If
            End If

            Call $"CSV data ""{path.ToFileURL}"" load done!   {sw.ElapsedMilliseconds}ms.".__DEBUG_ECHO

            Return cData
        End Function

        ''' <summary>
        ''' 排序操作在这里会不会大幅度的影响性能？
        ''' </summary>
        ''' <param name="buf"></param>
        ''' <param name="trimBlanks">如果这个选项为真，则会移除所有全部都是逗号分隔符``,,,,,,,,,``的空白行</param>
        ''' <returns></returns>
        Public Function Load(buf As String(), trimBlanks As Boolean, Optional skipWhile As NamedValue(Of Func(Of String, Boolean)) = Nothing) As List(Of RowObject)
            Dim first As New RowObject(buf(Scan0))
            Dim test As Func(Of String, Boolean)
            Dim headerIndex As Integer = first.IndexOf(skipWhile.Name)

            If trimBlanks Then
                test = Function(s) Not s.IsEmptyRow(","c)
            Else
                test = Function(s) True
            End If

            If headerIndex = -1 AndAlso Not skipWhile.Name.StringEmpty Then
                Call $"Required test for skip on field: [{skipWhile.Name}], but no such field exists in current file data...".Warning
            End If

            Dim parallelLoad = Function() As IEnumerable(Of RowObject)
                                   Dim loader = From s As SeqValue(Of String)
                                                In buf.Skip(1).SeqIterator.AsParallel
                                                Where test(s.value)
                                                Select row = New RowObject(s.value), i = s.i
                                                Order By i Ascending
                                   Dim testForSkip = skipWhile.Value

                                   If headerIndex = -1 Then
                                       ' returns all
                                       Return loader.Select(Function(r) r.row)
                                   Else
                                       Return loader _
                                           .Where(Function(r)
                                                      If testForSkip(r.row(headerIndex)) Then
                                                          ' is a row not needed...
                                                          Return False
                                                      Else
                                                          Return True
                                                      End If
                                                  End Function) _
                                           .Select(Function(r) r.row)
                                   End If
                               End Function

            Return first + parallelLoad().AsList
        End Function
    End Module
End Namespace