﻿#Region "Microsoft.VisualBasic::d415d8206f223726c6e26d44ee78d8fb, Data\DataFrame\Linq\DataStream.vb"

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

    '     Delegate Function
    ' 
    ' 
    '     Class SchemaReader
    ' 
    '         Properties: Headers, SchemaOridinal
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: GetOrdinal, ToString
    ' 
    '     Module DataLinqStream
    ' 
    '         Function: AsLinq, CastObject, ForEach, OpenHandle
    ' 
    '     Class DataStream
    ' 
    '         Properties: SchemaOridinal
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: AsLinq, BufferProvider, GetOrdinal, OpenHandle
    ' 
    '         Sub: (+2 Overloads) Dispose, ForEach, ForEachBlock
    '         Structure __taskHelper
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Sub: RunTask
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.ComponentModels
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Text

Namespace IO.Linq

    ''' <summary>
    ''' 获取列在当前的数据集之中的编号
    ''' </summary>
    ''' <param name="Column"></param>
    ''' <returns></returns>
    Public Delegate Function GetOrdinal(column As String) As Integer

    Public Class SchemaReader : Implements ISchema

        Public ReadOnly Property SchemaOridinal As Dictionary(Of String, Integer) Implements ISchema.SchemaOridinal
        Public ReadOnly Property Headers As IReadOnlyCollection(Of String)

        Sub New(fileName$, Optional encoding As Encoding = Nothing)
            Call Me.New(RowObject.TryParse(fileName.ReadFirstLine(encoding)))
        End Sub

        Sub New(firstLineHeaders As RowObject)
            Headers = firstLineHeaders.ToArray
            SchemaOridinal = Headers _
                .SeqIterator _
                .ToDictionary(Function(x) x.value.ToLower,
                              Function(x) x.i)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetOrdinal(name As String) As Integer Implements ISchema.GetOrdinal
            Return _SchemaOridinal.TryGetValue(name.ToLower, [default]:=-1)
        End Function

        Public Overrides Function ToString() As String
            Return $"[{Headers.JoinBy(", ")}]"
        End Function
    End Class

    Public Module DataLinqStream

        ''' <summary>
        ''' 所返回来的tuple对象之中的table字段，是跳过了第一行标题行的Linq迭代器
        ''' </summary>
        ''' <param name="fileName$"></param>
        ''' <param name="encoding"></param>
        ''' <returns></returns>
        <Extension>
        Public Function OpenHandle(fileName$, Optional encoding As Encoding = Nothing) As (schema As SchemaReader, table As IEnumerable(Of RowObject))
            Dim schema As New SchemaReader(fileName, encoding)
            Dim source As IEnumerable(Of RowObject) = fileName _
                .IterateAllLines _
                .Skip(1) _
                .Select(Function(line)
                            Return New RowObject(line)
                        End Function)
            Return (schema, source)
        End Function

        <Extension>
        Public Function CastObject(Of T As Class)(schema As SchemaReader) As Func(Of RowObject, T)
            Dim provider As SchemaProvider = SchemaProvider _
                .CreateObject(Of T)(strict:=False) _
                .CopyWriteDataToObject
            Dim rowBuilder As New RowBuilder(provider)
            Dim type As Type = GetType(T)

            Call rowBuilder.IndexOf(schema)
            Call rowBuilder.SolveReadOnlyMetaConflicts()

            Return Function(row As RowObject) As T
                       Dim obj As Object = Activator.CreateInstance(type)
                       Dim data As Object = rowBuilder.FillData(row, obj)

                       Return DirectCast(data, T)
                   End Function
        End Function

        <Extension>
        Public Function AsLinq(Of T As Class)(handle As (schema As SchemaReader, table As IEnumerable(Of RowObject)), Optional parallel As Boolean = False) As IEnumerable(Of T)
            Dim castObject = handle.schema.CastObject(Of T)
            Return handle.table.Populate(parallel,).Select(castObject)
        End Function

        <Extension>
        Public Iterator Function ForEach(Of T As Class)(handle As (schema As SchemaReader, table As IEnumerable(Of RowObject)),
                                                        apply As Action(Of T),
                                                        Optional parallel As Boolean = False) As IEnumerable(Of Exception)
            For Each obj As T In handle.AsLinq(Of T)(parallel)
                Call apply(obj)
            Next
        End Function
    End Module

    ''' <summary>
    ''' Buffered large text dataset Table reader
    ''' </summary>
    Public Class DataStream : Inherits BufferedStream
        Implements ISchema
        Implements IDisposable

        ''' <summary>
        ''' The title row, which is the mapping source of the class property name.
        ''' </summary>
        ReadOnly _title As RowObject

        ''' <summary>
        ''' The columns and their index order
        ''' </summary>
        Public ReadOnly Property SchemaOridinal As Dictionary(Of String, Integer) Implements ISchema.SchemaOridinal

        Sub New()
            _schema = New Dictionary(Of String, Integer)
            _title = New RowObject
        End Sub

        Sub New(file$, Optional encoding As Encoding = Nothing, Optional bufSize% = 64 * 1024 * 1024)
            Call MyBase.New(file, encoding, bufSize)

            Dim first As String = file.ReadFirstLine

            _title = RowObject.TryParse(first)
            _schema = _title.Select(
                Function(colName, idx) New With {
                    .colName = colName,
                    .ordinal = idx}) _
                    .ToDictionary(Function(x) x.colName.ToLower,
                                  Function(x) x.ordinal)
            Me.FileName = file

            Call $"{file.ToFileURL} handle opened...".__DEBUG_ECHO
        End Sub

        Public Function GetOrdinal(Name As String) As Integer Implements ISchema.GetOrdinal
            Name = Name.ToLower

            If _SchemaOridinal.ContainsKey(Name) Then
                Return _SchemaOridinal(Name)
            Else
                Return -1
            End If
        End Function

        Dim __firstBlock As Boolean = True

        ''' <summary>
        ''' Providers the data buffer for the <see cref="RowObject"/>
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 这个函数主要是为了处理第一行数据
        ''' 因为在构造函数部分已经读取了第一行来解析schema，所以在这里需要对第一个数据块做一些额外的处理
        ''' </remarks>
        Public Overrides Function BufferProvider() As String()
            Dim buffer As String() = MyBase.BufferProvider()

            If __firstBlock Then
                __firstBlock = False
                buffer = buffer.Skip(1).ToArray
            Else         '  不是第一个数据块，则不需要额外处理，直接返回
            End If

            Return buffer
        End Function

        ''' <summary>
        ''' For each item in the source data fram, invoke a specific task
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="invoke"></param>
        Public Sub ForEach(Of T As Class)(invoke As Action(Of T))
            Dim schema As SchemaProvider =
                SchemaProvider.CreateObject(Of T)(False).CopyWriteDataToObject
            Dim RowBuilder As New RowBuilder(schema)
            Dim type As Type = GetType(T)

            Call RowBuilder.IndexOf(Me)
            Call RowBuilder.SolveReadOnlyMetaConflicts()

            Do While True
                Dim buffer As String() = BufferProvider()

                For Each line As String In buffer
                    Dim row As RowObject = RowObject.TryParse(line)
                    Dim obj As Object = Activator.CreateInstance(type)

                    obj = RowBuilder.FillData(row, obj)

                    Call invoke(DirectCast(obj, T))
                Next

                If EndRead Then
                    Exit Do
                Else
                    Call EchoLine("Process next block....")
                End If
            Loop
        End Sub

        ''' <summary>
        ''' Processing large dataset in block partitions.(以分块任务的形式来处理一个非常大的数据集)
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="invoke">task of this block buffer</param>
        ''' <param name="blockSize">Lines of the data source.(行数)</param>
        ''' <remarks>
        ''' 2016.06.19  代码已经经过测试，没有数据遗漏的bug，请放心使用
        ''' </remarks>
        Public Sub ForEachBlock(Of T As Class)(invoke As Action(Of T()), Optional blockSize As Integer = 10240 * 5)
            Dim schema As SchemaProvider =
                SchemaProvider.CreateObject(Of T)(False).CopyWriteDataToObject ' 生成schema映射模型
            Dim RowBuilder As New RowBuilder(schema)
            Dim type As Type = GetType(T)

            Call RowBuilder.IndexOf(Me)
            Call RowBuilder.SolveReadOnlyMetaConflicts()

            Do While True
                Dim chunks As IEnumerable(Of String()) =
                    TaskPartitions.SplitIterator(BufferProvider(), blockSize)

                For Each block As String() In chunks
                    Dim LQuery As RowObject() =
                        LinqAPI.Exec(Of RowObject) <=
 _
                            From line As String
                            In block.AsParallel
                            Select RowObject.TryParse(line)

                    Dim values As T() = LinqAPI.Exec(Of T) <=
 _
                        From row As RowObject
                        In LQuery.AsParallel
                        Let obj As Object = Activator.CreateInstance(type)
                        Let data = RowBuilder.FillData(row, obj)
                        Select DirectCast(data, T)

                    Call Time(AddressOf New __taskHelper(Of T)(values, invoke).RunTask)
                    Call cat(".")
                Next

                If EndRead Then
                    Exit Do
                Else
                    Call "Process next block....".__INFO_ECHO
                End If
            Loop
        End Sub

        ''' <summary>
        ''' 为了减少Lambda表达式所带来的性能损失而构建的一个任务运行帮助对象
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        Private Structure __taskHelper(Of T)

            ''' <summary>
            ''' 赋值任务和数据源
            ''' </summary>
            ''' <param name="source"></param>
            ''' <param name="invoke"></param>
            Sub New(source As T(), invoke As Action(Of T()))
                Me.__source = source
                Me.__task = invoke
            End Sub

            Dim __task As Action(Of T())
            Dim __source As T()

            ''' <summary>
            ''' 运行当前的这个任务
            ''' </summary>
            Public Sub RunTask()
                Call __task(__source)
            End Sub
        End Structure

        ''' <summary>
        ''' Csv to LINQ
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <returns></returns>
        Public Iterator Function AsLinq(Of T As Class)(Optional parallel As Boolean = False) As IEnumerable(Of T)
            Dim schema As SchemaProvider = SchemaProvider _
                .CreateObject(Of T)(False) _
                .CopyWriteDataToObject
            Dim RowBuilder As New RowBuilder(schema)
            Dim type As Type = GetType(T)

            Call RowBuilder.IndexOf(Me)
            Call RowBuilder.SolveReadOnlyMetaConflicts()

            Do While Not EndRead

                Dim LQuery As IEnumerable(Of T) =
                    From line As String
                    In If(parallel,
                        DirectCast(BufferProvider.AsParallel, IEnumerable(Of String)),
                        DirectCast(BufferProvider(), IEnumerable(Of String)))
                    Let row As RowObject = RowObject.TryParse(line)
                    Let obj As Object = Activator.CreateInstance(type)
                    Let data As Object = RowBuilder.FillData(row, obj)
                    Select DirectCast(data, T)

                For Each x As T In LQuery
                    Yield x
                Next
            Loop

            Call Reset()
        End Function

        ''' <summary>
        ''' Open the data frame reader for the specific csv document.
        ''' </summary>
        ''' <param name="file">*.csv data file.</param>
        ''' <param name="encoding">The text encoding. default is using <see cref="Encodings.Default"/></param>
        ''' <returns></returns>
        Public Shared Function OpenHandle(
                               file As String,
                      Optional encoding As Encoding = Nothing,
                      Optional bufSize As Integer = 64 * 1024 * 1024) As DataStream

            Return New DataStream(file, encoding, bufSize)
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    __innerBuffer = Nothing
                    __innerStream = Nothing

                    Call FlushMemory()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Overloads Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace
