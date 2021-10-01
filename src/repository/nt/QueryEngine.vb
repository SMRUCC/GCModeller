#Region "Microsoft.VisualBasic::ab55298995f722751bc20bad9eb80db9, nt\QueryEngine.vb"

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

    ' Class QueryEngine
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: ScanSeqDatabase, Search
    ' 
    '     Sub: __scan, ScanDatabase
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.IO.SearchEngine
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class QueryEngine

    ReadOnly __nt As New Dictionary(Of Index)
    ReadOnly __headers As New Dictionary(Of TitleIndex)
    'ReadOnly mysql As New mysqlClient

    '''' <summary>
    '''' 创建以及测试数据库连接
    '''' </summary>
    'Sub New(uri As ConnectionUri)
    '    If (mysql <= uri) = -1.0R Then
    '        Throw New Exception("No mysql connection!")
    '    End If
    'End Sub

    Sub New()
    End Sub

    Public Function ScanSeqDatabase(DATA$) As Long
        Dim scan As Action(Of String, String) =
            Sub(name$, nt$)
                Dim index As New Index(DATA, name, nt$.BaseName)
                Dim title As New TitleIndex(DATA, name, nt$.BaseName)

                Call __nt.Add(index)
                Call __headers.Add(title)
            End Sub

        Call __scan(DATA, scan)

        Return __nt.Values.Sum(Function(i) i.Size)
    End Function

    Private Shared Sub __scan(DATA$, scan As Action(Of String, String))
        For Each db$ In ls - l - lsDIR <= DATA
            Dim name$ = db$.BaseName

            If name.TextEquals("headers") OrElse name.TextEquals("index") Then
                Continue For
            End If

            Call $"Loading {name}...".__DEBUG_ECHO

            For Each nt$ In ls - l - r - wildcards("*.nt") <= db$
                Call scan(name$, nt$)
            Next
        Next
    End Sub

    ''' <summary>
    ''' Scaner for full NT database that can running on low memory machine.
    ''' </summary>
    ''' <param name="DATA$"></param>
    ''' <param name="query"></param>
    ''' <param name="EXPORT$"></param>
    ''' <param name="lineBreak%"></param>
    Public Shared Sub ScanDatabase(DATA$, query As Dictionary(Of NamedValue(Of Expression)), EXPORT$, Optional lineBreak% = 60)
        Dim writer As New Dictionary(Of String, StreamWriter)

        For Each x In query.Values
            Dim path$ = $"{EXPORT}/{x.Name.NormalizePathString}.fasta"
            writer(x.Name) = path.OpenWriter(Encodings.ASCII)
        Next

        Call __scan(DATA,
             Sub(name$, nt$)
                 Dim index As New Index(DATA, name, nt$.BaseName)
                 Dim title As New TitleIndex(DATA, name, nt$.BaseName)
                 Dim def As IObject = title.GetDef

                 For Each exp In query.Values
                     Dim LQuery = From x As NamedValue(Of String)
                                  In title.EnumerateTitles.AsParallel
                                  Where exp.Value.Evaluate(def, x)
                                  Select x
                     Dim file As StreamWriter = writer(exp.Name)

                     For Each m As NamedValue(Of String) In LQuery
                         Dim seq$ = index.ReadNT_by_gi(gi:=m.Name)
                         Dim fa As New FastaSeq With {
                             .Headers = {"gi", m.Name, m.Value},
                             .SequenceData = seq
                         }
                         Dim line$ = fa.GenerateDocument(lineBreak)

                         Call file.WriteLine(line$)
                     Next
                 Next

                 Call {name, nt}.GetJson.__DEBUG_ECHO
             End Sub)

        For Each file In writer.Values
            Call file.Flush()
            Call file.Close()
            Call file.Dispose()
        Next
    End Sub

    ''' <summary>
    ''' 请参考搜索引擎的语法，假若查询里面含有符号的话，会被当作分隔符来看待，所以假若符号也要被匹配出来的话，需要添加双引号
    ''' </summary>
    ''' <param name="query$"></param>
    ''' <returns></returns>
    Public Iterator Function Search(query$) As IEnumerable(Of FastaSeq)
        Dim LQuery = From db As TitleIndex
                     In __headers.Values.AsParallel
                     Let expression As Expression = Build(query$)
                     Let def As IObject = db.GetDef
                     Select db.EnumerateTitles _
                         .AsParallel _
                         .Where(Function(x) expression.Evaluate(def, x))

        For Each x As NamedValue(Of String) In LQuery.IteratesALL
            Dim seq$ = __nt(x.Description) _
                .ReadNT_by_gi(gi:=x.Name)

            Yield New FastaSeq With {
                .Headers = {"gi", x.Name, x.Value},
                .SequenceData = seq
            }
        Next
    End Function
End Class
