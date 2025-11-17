#Region "Microsoft.VisualBasic::8476f005d6bd2b5681531bc212783f0b, core\Bio.Assembly\SequenceModel\FASTA\IO\FastaFile.vb"

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

'   Total Lines: 645
'    Code Lines: 410 (63.57%)
' Comment Lines: 135 (20.93%)
'    - Xml Docs: 94.81%
' 
'   Blank Lines: 100 (15.50%)
'     File Size: 25.65 KB


'     Class FastaFile
' 
'         Properties: _innerList, Count, FilePath, IsReadOnly, MimeType
' 
'         Constructor: (+8 Overloads) Sub New
' 
'         Function: [Select], AddRange, AsKSource, Clone, Contains
'                   Distinct, (+2 Overloads) DocParser, filterNulls, Find, Generate
'                   GetEnumerator, GetEnumerator1, IndexOf, IsValidFastaFile, LoadNucleotideData
'                   Match, ParseDocument, (+2 Overloads) Query, QueryAny, Read
'                   Remove, (+5 Overloads) Save, SaveData, SingleSequence, Take
'                   ToLower, ToString, ToUpper
' 
'         Sub: Add, AppendToFile, Clear, CopyTo, Insert
'              RemoveAt, Split
' 
'         Operators: (+3 Overloads) +, <, >
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq.JoinExtensions
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.SequenceModel.FASTA.Reflection
Imports Path = System.String

Namespace SequenceModel.FASTA

    ''' <summary>
    ''' A FASTA file that contains multiple sequence data.
    ''' (一个包含有多条序列数据的FASTA文件)
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <ActiveViewsAttribute(FastaSeq.SampleView, type:="bash")>
    Public Class FastaFile
        Implements IFileReference
        Implements IEnumerable(Of FastaSeq)
        Implements IList(Of FastaSeq)
        Implements ICloneable

        Public Overloads Shared Widening Operator CType(File As Path) As FastaFile
            Return FastaFile.Read(File)
        End Operator

        Public Overloads Shared Widening Operator CType(lstFiles As String()) As FastaFile
            Return lstFiles.Merge(True, False)
        End Operator

        Sub New()
        End Sub

        Sub New(source As List(Of FastaSeq))
            _innerList = source
        End Sub

        Sub New(source As IEnumerable(Of FastaSeq), path As String)
            Call Me.New(source.AsList)
            FilePath = path
        End Sub

        ''' <summary>
        ''' 构造函数会自动移除空值对象
        ''' </summary>
        ''' <param name="data"></param>
        Sub New(data As IEnumerable(Of FastaSeq))
            _innerList = LinqAPI.MakeList(Of FastaSeq) <= filterNulls(data)
        End Sub

        Private Shared Function filterNulls(data As IEnumerable(Of FastaSeq)) As IEnumerable(Of FastaSeq)
            Return From fa As FastaSeq
                   In data
                   Where Not fa Is Nothing AndAlso Not fa.SequenceData.StringEmpty
                   Select fa
        End Function

        Sub New(fa As FastaSeq)
            Call Me.New({fa})
        End Sub

        Sub New(fa As IEnumerable(Of FASTA.IAbstractFastaToken))
            Call Me.New(fa.Select(Function(x) New FastaSeq(x)))
        End Sub

        Sub New(fa As IEnumerable(Of FASTA.IFastaProvider))
            Call Me.New(fa.Select(Function(x) New FastaSeq(x)))
        End Sub

        Sub New(path As String, Optional deli As Char() = Nothing, Optional throwEx As Boolean = True)
            FilePath = path.FixPath
            _innerList = DocParser(
                path.ReadAllText(throwEx:=throwEx, suppress:=True),
                If(deli.IsNullOrEmpty, {"|"c}, deli))
        End Sub

        Protected Friend Overridable Property _innerList As New List(Of FastaSeq)

        Public Iterator Function AsKSource() As IEnumerable(Of KSeq)
            For Each fa As FastaSeq In _innerList
                Yield New KSeq With {
                    .Name = fa.Title,
                    .Seq = fa.SequenceData.ToCharArray
                }
            Next
        End Function

        ''' <summary>
        ''' 本FASTA数据文件对象的文件位置
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property FilePath As String Implements IFileReference.FilePath

        Public Function AddRange(FastaCol As IEnumerable(Of FastaSeq)) As Long
            Call __innerList.AddRange(FastaCol)
            Return __innerList.LongCount
        End Function

        Public Function AddRange(Of T As FastaSeq)(list As IEnumerable(Of T)) As Integer
            Call _innerList.AddRange(list)
            Return _innerList.Count
        End Function

        ''' <summary>
        ''' Get a new fasta2 object which is been clear the duplicated records in the collection.
        ''' (获取去除集合中的重复的记录新列表，原有列表中数据未被改变)
        ''' </summary>
        ''' <remarks></remarks>
        Public Function Distinct() As FastaFile
            Dim List = CType((From fasta As FastaSeq
                              In Me.__innerList
                              Select fasta
                              Order By fasta.Title Ascending).AsList, List(Of FastaSeq))
            For i As Integer = 1 To List.Count - 1
                If String.Equals(List(i).Title, List(i - 1).Title) Then
                    Call List.RemoveAt(i)
                End If

                If i = List.Count Then
                    Exit For
                End If
            Next

            Return List
        End Function

        Public Function ToUpper() As FastaFile
            For i As Integer = 0 To Me._innerList.Count - 1
                _innerList(i).SequenceData = _innerList(i).SequenceData.ToUpper
            Next
            Return New FastaFile(_innerList)
        End Function

        Public Function ToLower() As FastaFile
            For i As Integer = 0 To Me._innerList.Count - 1
                _innerList(i).SequenceData = _innerList(i).SequenceData.ToLower
            Next
            Return New FastaFile(_innerList)
        End Function

        ''' <summary>
        ''' Load the fasta file from the local filesystem.
        ''' </summary>
        ''' <param name="file"></param>
        ''' <param name="strict">当参数为真的时候，目标文件不存在则会抛出错误，反之则会返回一个空文件</param>
        ''' <param name="deli">Delimiter character for tokens in fasta sequence header.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Read(file As Path, Optional strict As Boolean = True, Optional deli As Char = "|"c) As FastaFile
            If file.StringEmpty OrElse Not file.FixPath.FileExists Then
                If strict Then
                    Throw New Exception($"File ""{file.ToFileURL}"" is not exists on the file system!")
                Else
                    Return New FastaFile
                End If
            End If

            Dim FastaReader As New FastaFile(DocParser(IO.File.ReadAllLines(file), {deli})) With {
                .FilePath = FileIO.FileSystem.GetFileInfo(file).FullName
            }
            Return FastaReader
        End Function

        ''' <summary>
        ''' 加载一个FASTA序列文件并检查其中是否全部为核酸序列数据，假若不是，则会将该序列移除
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LoadNucleotideData(path As String, Optional strict As Boolean = False) As FastaFile
            Dim seqs As FastaFile = FastaFile.Read(path)

            If seqs.IsNullOrEmpty Then
NULL_DATA:      Call $"""{path.ToFileURL}"" fasta data isnull or empty!".debug
                Return Nothing
            End If

            For Each fa As FastaSeq In seqs._innerList
                fa.SequenceData = fa.SequenceData.ToUpper.Replace("N", "-")
            Next

            Dim LQuery = (From fa As FastaSeq
                          In seqs._innerList
                          Where Not fa.IsProtSource
                          Select fa).ToArray
            If strict Then
                LQuery = (From fa As FastaSeq
                          In LQuery
                          Where Not fa.HaveGaps
                          Select fa).ToArray
            End If

            If LQuery.IsNullOrEmpty Then
                GoTo NULL_DATA
            End If

            seqs = New FastaFile(LQuery, path)

            Return seqs
        End Function

        ''' <summary>
        ''' 当<paramref name="lines"/>参数为空的时候，会返回一个空集合而非返回空值
        ''' </summary>
        ''' <param name="lines"></param>
        ''' <param name="deli"></param>
        ''' <returns></returns>
        Public Shared Iterator Function DocParser(lines As IEnumerable(Of String), Optional deli As String = "|") As IEnumerable(Of FastaSeq)
            Dim faseq As New List(Of String)

            If lines Is Nothing Then
                Return
            ElseIf deli.StringEmpty Then
                deli = "|"
            End If

            For Each line As String In lines
                If String.IsNullOrEmpty(line) Then
                    Continue For
                ElseIf line.Chars(Scan0) = ">"c Then
                    ' New FASTA Object
                    If faseq > 0 Then
                        Yield FastaSeq.ParseFromStream(faseq.PopAll, deli)
                    End If

                    faseq *= 0
                End If

                Call faseq.Add(line)
            Next

            If faseq > 0 Then
                Yield FastaSeq.ParseFromStream(faseq.PopAll, deli)
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function DocParser(doc As String, deli As Char()) As IEnumerable(Of FastaSeq)
            Return DocParser(doc.LineTokens, deli)
        End Function

        ''' <summary>
        ''' Try parsing a fasta file object from the text file content <paramref name="doc"></paramref>
        ''' </summary>
        ''' <param name="doc">The file data content in the fasta file, not the path of the fasta file!</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ParseDocument(doc As String, Optional deli As Char() = Nothing) As FastaFile
            Dim Fasta As New FastaFile(DocParser(doc, If(deli.IsNullOrEmpty, {"|"c}, deli)))
            Return Fasta
        End Function

        Public Sub Split(saveDIR As Path)
            Dim Index As Integer

            For Each FASTA As FastaSeq In __innerList
                Index += 1
                FASTA.SaveTo(String.Format("{0}/{1}.fasta", saveDIR, Index))
            Next
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="KeyWord">A key string that to search in this fasta file.</param>
        ''' <param name="CaseSensitive">For text compaired method that not case sensitive, otherwise in the method od binary than case sensitive.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function QueryAny(KeyWord As String, Optional CaseSensitive As CompareMethod = CompareMethod.Text) As FastaFile
            Dim LQuery As IEnumerable(Of FastaSeq) = From Fasta As FastaSeq
                                                       In __innerList.AsParallel
                                                     Where Find(Fasta.Headers, KeyWord, CaseSensitive)
                                                     Select Fasta '
            Return New FastaFile With {
                .__innerList = LQuery.AsList
            }
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="KeyWord">A key string that to search in this fasta file.</param>
        ''' <param name="CaseSensitive">For text compaired method that not case sensitive, otherwise in the method od binary than case sensitive.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Query(KeyWord As String, Optional CaseSensitive As CompareMethod = CompareMethod.Text) As FastaSeq
            Dim LQuery = LinqAPI.DefaultFirst(Of FastaSeq) <=
                From fa As FastaSeq
                In __innerList.AsParallel
                Where Find(fa.Headers, KeyWord, CaseSensitive)
                Select fa '

            Return LQuery
        End Function

        Public Function Query(Keyword As String, index%, Optional CaseSensitive As CompareMethod = CompareMethod.Text) As FastaSeq()
            Dim list As IEnumerable(Of FastaSeq) =
                From fsa As FastaSeq
                In __innerList
                Where fsa.Headers.Length - 1 >= index
                Select fsa
            Dim LQuery As FastaSeq() = LinqAPI.Exec(Of FastaSeq) <=
                                                                   _
                From fa As FastaSeq
                In list
                Where InStr(fa.Headers(index), Keyword, CaseSensitive) > 0
                Select fa '

            Return LQuery
        End Function

        Private Shared Function Find(AttributeList As String(), KeyWord As String, CaseSensitive As CompareMethod) As Boolean
            For i As Integer = 0 To AttributeList.Length - 1
                If InStr(AttributeList(i), KeyWord, CaseSensitive) Then
                    Return True
                End If
            Next

            Return False
        End Function

        Public Function Take(keyWords As IEnumerable(Of String), Optional CaseSensitive As CompareMethod = CompareMethod.Text) As FastaFile
            Dim result As New List(Of FastaSeq)

            For Each keyWord As String In keyWords
                result += From FASTA As FastaSeq
                          In __innerList
                          Where InStr(FASTA.Title, keyWord, CaseSensitive) > 0
                          Select FASTA '
            Next

            Return New FastaFile With {
                .__innerList = result
            }
        End Function

        ''' <summary>
        ''' Where Selector
        ''' </summary>
        ''' <param name="prediction"></param>
        ''' <returns></returns>
        Public Function [Select](prediction As Func(Of FastaSeq, Boolean)) As FastaFile
            Dim LQuery = From fa As FastaSeq
                         In Me._innerList
                         Where True = prediction(fa)
                         Select fa

            Return New FastaFile(LQuery)
        End Function

        ''' <summary>
        ''' Save the fasta file into the local filesystem.
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <param name="encoding">
        ''' 不同的程序会对这个由要求，例如meme程序在linux系统之中要求序列文件为unicode编码格式而windows版本的meme程序则要求ascii格式
        ''' </param>
        ''' <remarks>this function will split sequence data by 60 chars per line</remarks>
        Public Overloads Function Save(path As String, encoding As Encoding) As Boolean
            Try
                Return Save(lineBreak:=60, path, encoding:=encoding)
            Catch ex As Exception
                Throw New Exception(path, ex)
            End Try
        End Function

        ''' <summary>
        ''' Save the fasta file into the local filesystem.
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <param name="encoding">
        ''' 不同的程序会对这个由要求，例如
        ''' + meme程序在linux系统之中要求序列文件为unicode编码格式
        ''' + 而windows版本的meme程序则要求ascii格式，
        ''' + blast+则要求必须为ASCII编码格式的。
        ''' </param>
        ''' <remarks></remarks>
        Public Overloads Function Save(lineBreak As Integer, Optional path As String = "", Optional deli As String = "|", Optional encoding As Encodings = Encodings.ASCII) As Boolean
            Try
                Return Save(lineBreak, path, deli, encoding.CodePage)
            Catch ex As Exception
                Throw New Exception(path, ex)
            End Try
        End Function

        Public Overloads Function Save(lineBreak As Integer, s As Stream, encoding As Encoding) As Boolean
            Using writer As New IO.StreamWriter(s, encoding)
                For Each seq In _innerList.AsParallel.Select(Function(fa) fa.GenerateDocument(lineBreak:=lineBreak))
                    Call writer.WriteLine(seq)
                Next
            End Using

            Return True
        End Function

        ''' <summary>
        ''' Save the fasta file into the local filesystem.
        ''' </summary>
        ''' <param name="path">the file path for save the fasta sequence data</param>
        ''' <param name="encoding">不同的程序会对这个由要求，例如meme程序在linux系统之中要求序列文件为unicode编码格式而windows版本的meme程序则要求ascii格式</param>
        ''' <remarks></remarks>
        Public Overloads Function Save(lineBreak As Integer, Optional path As String = "", Optional deli As String = "|", Optional encoding As Encoding = Nothing) As Boolean
            Static ASCII As [Default](Of Encoding) = Encoding.ASCII

            Using writer As IO.StreamWriter = (path Or FilePath.When(path.StringEmpty)).OpenWriter(encoding Or ASCII)
                For Each seq As String In From fa As FastaSeq
                                          In _innerList.AsParallel
                                          Select fa.GenerateDocument(lineBreak:=lineBreak, delimiter:=deli)

                    Call writer.WriteLine(seq)
                Next
            End Using

            Return True
        End Function

        ''' <summary>
        ''' 在写入数据到文本文件之前需要将目标对象转换为文本字符串
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Generate(Optional lineBreak As Integer = 60, Optional deli As String = "|") As String
            Dim sb As New StringBuilder(capacity:=4 * ByteSize.KB)
            Dim text As New StringWriter(sb)

            Call Write(text, lineBreak, deli)
            Call text.Flush()

            Return sb.ToString
        End Function

        Public Sub Write(file As TextWriter, lineBreak As Integer, delimiter As String)
            For Each fa As FastaSeq In __innerList
                Call file.WriteLine(fa.GenerateDocument(lineBreak:=lineBreak, delimiter:=delimiter))
            Next
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="file">
        ''' The target FASTA file that to append this FASTA sequences.(将要拓展这些FASTA序列的目标FASTA文件)
        ''' </param>
        ''' <remarks></remarks>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub AppendToFile(file As Path)
            Call FileSystem.WriteAllText(file, Generate, append:=True)
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0}; [{1} records]", FilePath, Count)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Shadows Widening Operator CType(source As FastaSeq()) As FastaFile
            Return New FastaFile(source)
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Shadows Widening Operator CType(source As List(Of FastaSeq)) As FastaFile
            Return New FastaFile(source)
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Shadows Widening Operator CType(fa As FastaSeq) As FastaFile
            Return New FastaFile(New FastaSeq() {fa})
        End Operator

        Public Shadows Iterator Function GetEnumerator() As IEnumerator(Of FastaSeq) Implements IEnumerable(Of FastaSeq).GetEnumerator
            For i As Integer = 0 To __innerList.Count - 1
                Yield __innerList(i)
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function

        ''' <summary>
        ''' 使用正则表达式来搜索在序列中含有特定模式的序列对象
        ''' </summary>
        ''' <param name="regxText">the regular expression pattern for matches of the fasta sequence data</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Iterator Function Match(regxText$, Optional options As RegexOptions = RegexOptions.Singleline) As IEnumerable(Of FastaSeq)
            Dim regexp As New Regex(regxText, options)

            For Each fasta As FastaSeq In _innerList
                If regexp.Match(fasta.SequenceData.ToUpper).Success Then
                    Yield fasta
                End If
            Next
        End Function

#Region "Implementation of IList(Of SequenceModel.FASTA.FASTA) interface."
        Public Sub Add(item As FastaSeq) Implements ICollection(Of FastaSeq).Add
            Call __innerList.Add(item)
        End Sub

        Public Sub Clear() Implements ICollection(Of FastaSeq).Clear
            Call _innerList.Clear()
        End Sub

        Public Function Contains(item As FastaSeq) As Boolean Implements ICollection(Of FastaSeq).Contains
            Return _innerList.Contains(item)
        End Function

        Public Overloads Sub CopyTo(array() As FastaSeq, arrayIndex As Integer) Implements ICollection(Of FastaSeq).CopyTo
            Call _innerList.CopyTo(array, arrayIndex)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Count和Item属性可以兼容``R#``脚本的索引语法
        ''' </remarks>
        Public ReadOnly Property Count As Integer Implements ICollection(Of FastaSeq).Count
            Get
                Return _innerList.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of FastaSeq).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public Function Remove(item As FastaSeq) As Boolean Implements ICollection(Of FastaSeq).Remove
            Return _innerList.Remove(item)
        End Function

        Public Function IndexOf(item As FastaSeq) As Integer Implements IList(Of FastaSeq).IndexOf
            Return _innerList.IndexOf(item)
        End Function

        Public Sub Insert(index As Integer, item As FastaSeq) Implements IList(Of FastaSeq).Insert
            Call _innerList.Insert(index, item)
        End Sub

        Default Public Property Item(index As Integer) As FastaSeq Implements IList(Of FastaSeq).Item
            Get
                Return _innerList(index)
            End Get
            Set(value As FastaSeq)
                _innerList(index) = value
            End Set
        End Property

        Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Sub RemoveAt(index As Integer) Implements IList(Of FastaSeq).RemoveAt
            Call _innerList.RemoveAt(index)
        End Sub
#End Region

        ''' <summary>
        ''' 可以使用这个方法来作为通用的继承与fasta对象的集合的序列的数据保存工作
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="data"></param>
        ''' <param name="path"></param>
        ''' <param name="encoding"></param>
        ''' <returns></returns>
        Public Shared Function SaveData(Of T As IAbstractFastaToken)(data As IEnumerable(Of T), path As String, Optional encoding As Encodings = Encodings.ASCII) As Boolean
            Dim LQuery$() = LinqAPI.Exec(Of String) <=
                From fa As T
                In data
                Let line As String = FastaSeq.GenerateDocumentText(fa)
                Select line & vbLf

            Return LQuery.FlushAllLines(path, encoding)
        End Function

        Public Overloads Shared Operator +(FASTA As FastaFile, fa As FastaSeq) As FastaFile
            Call FASTA.Add(fa)
            Return FASTA
        End Operator

        Public Overloads Shared Operator +(FASTA As FastaFile, source As IEnumerable(Of FastaSeq)) As FastaFile
            Call FASTA.AddRange(source)
            Return FASTA
        End Operator

        Public Overloads Shared Operator +(FASTA As FastaFile, source As IEnumerable(Of IEnumerable(Of FastaSeq))) As FastaFile
            Return FASTA + source.IteratesALL
        End Operator

        Public Shared Operator >(source As FastaFile, path As String) As Boolean
            Return source.Save(path, Encoding.ASCII)
        End Operator

        Public Shared Operator <(source As FastaFile, path As String) As Boolean
            Throw New NotSupportedException
        End Operator

        Public Shared Function IsValidFastaFile(path As String) As Boolean
            Dim firstLine$ = path.ReadFirstLine
            Return firstLine.First = ">"c
        End Function

        ''' <summary>
        ''' 完完全全的按值複製
        ''' </summary>
        ''' <returns></returns>
        Public Function Clone() As Object Implements ICloneable.Clone
            Dim list As New List(Of FastaSeq)

            For Each x In Me._innerList
                Call list.Add(x.Clone)
            Next

            Return New FastaFile(list)
        End Function

        ''' <summary>
        ''' 文件之中是否只包含有一条序列？
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <returns></returns>
        Public Shared Function SingleSequence(path$) As Boolean
            Dim i%

            For Each fasta As FastaSeq In New StreamIterator(path).ReadStream
                i += 1

                If i = 2 Then
                    ' 已经有两条序列了，肯定不是单一序列构成的，返回False
                    Return False
                End If
            Next

            Return True
        End Function

        Public Overloads Function Save(path As String, Optional encoding As Encodings = Encodings.ASCII) As Boolean
            Return Save(path, encoding.CodePage)
        End Function
    End Class
End Namespace
