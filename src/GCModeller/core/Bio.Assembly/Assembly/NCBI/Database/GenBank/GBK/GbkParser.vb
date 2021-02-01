#Region "Microsoft.VisualBasic::7ca73fbd3a42e91c8a177ec1ffc6fd27, core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\GbkParser.vb"

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

    '     Module GbkParser
    ' 
    '         Function: __originReadThread, bufferTrims, doLoadData, Internal_readBlock, Read
    ' 
    '         Sub: __readOrigin
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Assembly.NCBI.GenBank.GBFF

    Public Module GbkParser

        ''' <summary>
        ''' 将一个GBK文件从硬盘文件之中读取出来，当发生错误的时候，会抛出错误
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Read")> Public Function Read(Path As String) As NCBI.GenBank.GBFF.File
            Dim file As String() = IO.File.ReadAllLines(Path)
            Dim genbank = doLoadData(file, Path)

            Return genbank
        End Function

        Private Function __originReadThread(gb As NCBI.GenBank.GBFF.File, buf As String()) As NCBI.GenBank.GBFF.Keywords.ORIGIN
            Dim bufs As String() = Internal_readBlock(KeyWord.GBK_FIELD_KEY_ORIGIN, buf)

            If bufs.IsNullOrEmpty Then
                Call $"{gb.Locus} have no sequence data.".__DEBUG_ECHO

                Return New ORIGIN With {
                    .SequenceData = ""
                }
            Else
                Return CType(bufs.Skip(1).ToArray, ORIGIN)
            End If
        End Function

        Friend Function bufferTrims(buf As String()) As String()
            Dim i As Integer = 0

            If buf.Length < 5 Then
                Return Nothing
            End If

            Do While String.IsNullOrEmpty(buf.Read(i))
            Loop

            If i = 1 Then
                Return buf
            Else
                i -= 1
            End If

            buf = buf.Skip(i).ToArray
            Return buf
        End Function

        Friend Function doLoadData(innerBufs As String(), defaultAccession$) As NCBI.GenBank.GBFF.File
            Call "Start loading ncbi gbk file...".__DEBUG_ECHO

            Dim Sw As Stopwatch = Stopwatch.StartNew
            Dim gb As New File
            Dim ReadThread As Action(Of File, String()) = AddressOf __readOrigin
            Dim ReadThreadResult As IAsyncResult = ReadThread.BeginInvoke(gb, innerBufs, Nothing, Nothing)

            gb.Comment = Internal_readBlock(KeyWord.GBK_FIELD_KEY_COMMENT, innerBufs)
            gb.Features = Internal_readBlock(KeyWord.GBK_FIELD_KEY_FEATURES, innerBufs).Skip(1).ToArray.FeaturesListParser
            gb.Accession = ACCESSION.CreateObject(Internal_readBlock(KeyWord.GBK_FIELD_KEY_ACCESSION, innerBufs), defaultAccession)
            gb.Reference = REFERENCE.InternalParser(innerBufs)
            gb.Definition = Internal_readBlock(KeyWord.GBK_FIELD_KEY_DEFINITION, innerBufs)
            gb.Version = Internal_readBlock(KeyWord.GBK_FIELD_KEY_VERSION, innerBufs)
            gb.Source = Internal_readBlock(KeyWord.GBK_FIELD_KEY_SOURCE, innerBufs)
            gb.Locus = LOCUS.InternalParser(Internal_readBlock(KeyWord.GBK_FIELD_KEY_LOCUS, innerBufs).First)
            gb.Keywords = GBFF.Keywords.KEYWORDS.__innerParser(Internal_readBlock(KeyWord.GBK_FIELD_KEY_KEYWORDS, innerBufs))
            gb.DbLinks = GBFF.Keywords.DBLINK.Parser(Internal_readBlock(KeyWord.GBK_FIELD_KEY_DBLINK, innerBufs))

            gb.Accession.gb = gb
            gb.Comment.gb = gb
            gb.Definition.gb = gb
            gb.Features.gb = gb
            gb.Keywords.gb = gb
            gb.Locus.gb = gb
            gb.Reference.gb = gb
            gb.Source.gb = gb
            gb.Version.gb = gb
            gb.DbLinks.gb = gb

            Call gb.Features.LinkEntry()
            Call ReadThread.EndInvoke(ReadThreadResult)

            ' 由于使用线程进行读取的，所以不能保证在赋值的时候是否初始化基因组序列完成
            gb.Origin.gb = gb
            innerBufs = Nothing

            Return gb
        End Function

        Private Sub __readOrigin(gb As File, bufs As String())
            gb.Origin = __originReadThread(gb, buf:=bufs)
        End Sub

        ''' <summary>
        ''' 快速读取数据库文件中的某一个字段的文本块
        ''' </summary>
        ''' <param name="keyword">字段名</param>
        ''' <returns>该字段的内容</returns>
        ''' <remarks></remarks>
        Private Function Internal_readBlock(Keyword As String, ByRef innerBufs As String()) As String()
            Dim Regx As New Regex(String.Format("^{0}\s+.+$", Keyword))
            Dim LQuery As String() =
                LinqAPI.Exec(Of String) <= From str As String
                                           In innerBufs
                                           Where Regx.Match(str).Success OrElse
                                               String.Equals(str, Keyword)
                                           Select str
            Dim index As Integer
            Dim p As Integer
            Dim bufs() As String = Nothing

            For Each Head As String In LQuery
                index = Array.IndexOf(innerBufs, Head)
                p = index + 1

                Do While String.IsNullOrEmpty(innerBufs(p)) OrElse innerBufs(p).First = " "c
                    p += 1
                    If p = innerBufs.Length Then
                        Exit Do
                    End If
                Loop

                Dim sBuf As String() = New String(p - index - 1) {}

                Call Array.ConstrainedCopy(innerBufs,
                                           index,
                                           sBuf,
                                           Scan0,
                                           sBuf.Length)

                If bufs Is Nothing Then
                    index = Scan0
                    ReDim bufs(sBuf.Length - 1)
                Else
                    index = bufs.Length
                    ReDim Preserve bufs(bufs.Length + sBuf.Length - 1)
                End If

                Call Array.ConstrainedCopy(sBuf, Scan0, bufs, index, sBuf.Length)
            Next

            Return bufs
        End Function
    End Module
End Namespace
