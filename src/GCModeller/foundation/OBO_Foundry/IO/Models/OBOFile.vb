#Region "Microsoft.VisualBasic::10a04d4472fae05f60dfb71078263113, foundation\OBO_Foundry\IO\Models\OBOFile.vb"

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

    '   Total Lines: 319
    '    Code Lines: 107 (33.54%)
    ' Comment Lines: 186 (58.31%)
    '    - Xml Docs: 83.33%
    ' 
    '   Blank Lines: 26 (8.15%)
    '     File Size: 11.46 KB


    '     Class OBOFile
    ' 
    '         Properties: header
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: createRawTerm, GetRawTerms, populateLines, ToString
    ' 
    '         Sub: (+2 Overloads) Dispose, parseHeader
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

Namespace IO.Models

    ''' <summary>
    ''' OBO file reader
    ''' </summary>
    ''' <remarks>
    ''' ### OBO文件格式1.2
    '''
    ''' 该文件每一行都是一个键值对， 基本格式为
    ''' ```
    ''' 键: 值!注释
    ''' ```
    '''
    ''' ##### 总体结构
    ''' ```
    ''' 文件头
    ''' !包含若干行总体说明
    ''' 词条1![词条类型]占第一行， 后跟若干行说明
    ''' 词条2!不同的词条可用于描述同一对象的不同方面
    ''' .
    ''' .
    ''' .
    ''' !中间可以有若干空白行
    ''' !OBO文件中可以在任意地方插入注释， 其注释以'!'开头
    ''' ```
    ''' 
    ''' ##### 文件头
    ''' ```
    ''' format-version:  1.2    !本文件所使用的OBO版本。本条目是必需的，以下文件头说明是可选的。
    ''' data-version:  2012-12-09    !当前ontology的版本
    ''' date :  07:12:2012 17:25    !当前日期
    ''' saved-by: tanyaberardini!最后保存该文件的用户
    ''' auto-generated - by: OBO-Edit 2.2    !生成该文件的程序
    ''' subsetdef: goslim_aspergillus "Aspergillus GO slim"!对术语子集的描述
    ''' .
    ''' .
    ''' .
    ''' synonymtypedef: systematic_synonym "Systematic synonym" EXACT    !用户自定义的同义词类型
    ''' default-namespace: gene_ontology!缺省的名称使用范围
    ''' remark: !重要注释
    ''' default-relationship-id-prefix:     !缺省关系作用范围
    ''' id-mapping: !将一个术语或关系对应到另一个术语或关系上
    ''' idspace: !全局id和局部id的对应
    ''' ontology: go
    ''' ```
    '''
    ''' ##### 词条：词条类型包括[Term]、[Typedef]、[Instance]
    '''
    ''' ###### [Term]
    ''' ```
    ''' [Term]
    ''' id: GO : 0000001    !一个id即一个对象。一般情况下，id是全局性的，即唯一对应的，在任何地方都是一样的
    ''' name: mitochondrion inheritance    !术语名，只能有一个
    ''' namespace:  biological_process    !名称使用范围
    ''' def: !该术语的定义
    ''' synonym: "mitochondrial inheritance" EXACT []    !同义词
    ''' is_a: GO:0048308 ! organelle inheritance    !该术语是上级类别的一个亚类
    ''' is_a: GO:0048311 ! mitochondrion distribution
    ''' alt_id: !备选id， 一个术语可以有多个备选id
    ''' is_anonymous: !说明为true的话， 则该词条为局部id， 其id不是固定的， 仅在当前文件有效
    ''' comment: !重要注释
    ''' subset: !该术语从属的子集， 该子集必须是文件头定义的
    ''' subset: !一个术语可以从属于多个子集
    ''' xref: !其他词汇表中的类似词汇
    ''' xref: !一个术语可有多个类似词汇
    ''' intersection_of: !该术语是其他几个术语的交集
    ''' intersection_of: !至少要有两个
    ''' union_of: !该术语是其他几个术语的并集
    ''' union_of: !至少要有两个
    ''' disjoint_from: !该术语和另一个术语互斥
    ''' relationship: !该术语和另一个术语的关系， 必须使用[Typedef]中定义的关系id
    ''' is_obsolete: !该术语是否被淘汰
    ''' replaced_by: !替代淘汰词的术语
    ''' consider: !淘汰词备选的、还未被审核的替换术语
    ''' created_by: !术语创造者
    ''' creation_date: !术语创造时间
    ''' ```
    ''' 
    ''' ###### [Typedef]
    ''' ```
    ''' [Typedef]
    ''' id: !通常是有一定含义的字符串， 而不是数字
    ''' is_anonymous
    ''' name
    ''' Namespace
    ''' alt_id
    ''' def
    ''' comment
    ''' subset
    ''' synonym
    ''' xref
    ''' domain: !该关系仅对domain指定术语的亚类起作用
    ''' range: !任何具有这个关系的术语都属于range指定术语的亚类
    ''' is_anti_symmetric
    ''' is_cyclic: !可否构建循环作用
    ''' is_reflexive: !是否自反
    ''' is_symmetric: !是否对称
    ''' is_transitive: !传递关系？
    ''' is_a
    ''' inverse_of: !和另一关系相反。适用于原关系的两个术语，可以反方向适用另一关系。
    ''' transitive_over: !将关系传递给下一个
    ''' relationship
    ''' is_obsolete
    ''' replaced_by
    ''' consider
    ''' ```
    ''' 
    ''' ###### [Instance]
    ''' ```
    ''' [Instance]
    ''' id
    ''' is_anonymous
    ''' name
    ''' Namespace
    ''' alt_id
    ''' comment
    ''' synonym
    ''' xref
    ''' instance_of
    ''' property_value
    ''' is_obsolete
    ''' replaced_by
    ''' consider
    ''' ```
    ''' 
    ''' ##### 缺省的词条定义
    ''' ```
    ''' [Typedef]
    ''' id: is_a
    ''' name: is_a
    ''' range: OBO:TERM_OR_TYPE
    ''' domain: OBO:TERM_OR_TYPE
    ''' def: The basic subclassing relationship [OBO:defs]
    '''
    ''' [Typedef]
    ''' id: disjoint_from
    ''' name: disjoint_from
    ''' range: OBO:TERM
    ''' domain: OBO:TERM
    ''' def: Indicates that two classes are disjoint [OBO:defs]
    '''
    ''' [Typedef]
    ''' id: instance_of
    ''' name: instance_of
    ''' range: OBO:TERM
    ''' domain: OBO:INSTANCE
    ''' def: Indicates the type Of an instance [OBO:defs]
    '''
    ''' [Typedef]
    ''' id: inverse_of
    ''' name: inverse_of
    ''' range: OBO:TYPE
    ''' domain: OBO:TYPE
    ''' def: Indicates that one relationship type Is the inverse Of another [OBO:defs]
    '''
    ''' [Typedef]
    ''' id: union_of
    ''' name: union_of
    ''' range: OBO:TERM
    ''' domain: OBO:TERM
    ''' def: Indicates that a term Is the union Of several others [OBO:defs]
    ''' 
    ''' [Typedef]
    ''' id: intersection_of
    ''' name: intersection_of
    ''' range: OBO:TERM
    ''' domain: OBO:TERM
    ''' def: Indicates that a term Is the intersection Of several others [OBO:defs] 
    ''' ```
    ''' </remarks>
    Public Class OBOFile : Implements IDisposable

        Public Property header As header

        ReadOnly file As String
        ReadOnly reader As StreamReader
        ReadOnly cache As New List(Of RawTerm)

        Sub New(file$, Optional encoding As Encodings = Encodings.UTF8)
            If file.StringEmpty Then
                Throw New ArgumentNullException("File path can not be empty!")
            Else
                Me.file = file
                Me.reader = file.OpenReader(encoding.CodePage)
            End If

            Call parseHeader()
        End Sub

        Sub New(file As Stream, Optional encoding As Encodings = Encodings.UTF8)
            If TypeOf file Is FileStream Then
                Me.file = DirectCast(file, FileStream).Name
            End If

            reader = New StreamReader(file, encoding.CodePage)
            parseHeader()
        End Sub

        Private Sub parseHeader()
            Dim str As New Value(Of String)
            Dim bufs As New List(Of String)

            Do While Not String.IsNullOrEmpty(str = reader.ReadLine)
                bufs += str
            Loop

            header = bufs.LoadData(Of header)()
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return file.ToFileURL
        End Function

        ''' <summary>
        ''' read term data from obo file stream
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' this function has cache data, first time call this function will cache term data into memory
        ''' then second time call this function will populate data from the memory cache directly. 
        ''' </remarks>
        Public Iterator Function GetRawTerms() As IEnumerable(Of RawTerm)
            If cache.IsNullOrEmpty Then
                Dim term As RawTerm

                ' read from file at first time call of this function
                ' term will be parsed from the file stream
                ' and then cached into the cache list
                For Each block As NamedCollection(Of String) In populateLines()
                    term = createRawTerm(block)
                    cache.Add(term)
                    Yield term
                Next
            Else
                ' use the in-memory cache data
                For Each term As RawTerm In cache
                    Yield term
                Next
            End If
        End Function

        Private Function createRawTerm(block As NamedCollection(Of String)) As RawTerm
            Dim nameGroup = From line As String
                            In block
                            Select x = line.GetTagValue(": ")
                            Group x By id = x.Name Into Group

            Dim data = LinqAPI.Exec(Of NamedValue(Of String())) _
 _
                () <= From x
                      In nameGroup
                      Let values = x _
                          .Group _
                          .Select(Function(o) o.Value) _
                          .ToArray
                      Select New NamedValue(Of String()) With {
                          .Name = x.id,
                          .Value = values
                      }

            Return New RawTerm With {
                .type = block.Name,
                .data = data
            }
        End Function

        Private Iterator Function populateLines() As IEnumerable(Of NamedCollection(Of String))
            Dim s As New Value(Of String)
            Dim bufs As New List(Of String)

            Do While Not reader.EndOfStream
                Dim name As String = reader.ReadLine

                Do While Not String.IsNullOrEmpty(s = reader.ReadLine)
                    bufs += s.Value
                Loop

                Yield New NamedCollection(Of String) With {
                    .Name = name,
                    .Value = bufs.PopAll
                }
            Loop
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call reader.Close()
                    Call reader.Dispose()
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
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace
