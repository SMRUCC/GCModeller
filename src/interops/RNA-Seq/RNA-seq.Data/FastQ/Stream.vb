#Region "Microsoft.VisualBasic::7ba95fe763947395affba6cf6379e02f, ..\interops\RNA-Seq\RNA-seq.Data\FastQ\Stream.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Text

Namespace Fastaq

    Public Module Stream

        Public Iterator Function ReadAllLines(path$, Optional encoding As Encodings = Encodings.Default) As IEnumerable(Of FastQ)
            Dim sw As Stopwatch = Stopwatch.StartNew

            Call $"Start to load file data from handle *{path.ToFileURL}".__DEBUG_ECHO

            Dim stream As New BufferedStream(path)

            Call $"[Job Done!] {sw.ElapsedMilliseconds}ms...".__DEBUG_ECHO
            Call "Start to parsing the fastq format data...".__DEBUG_ECHO

            sw = Stopwatch.StartNew

            Dim sBufs As IEnumerable(Of String()) =
                TaskPartitions.SplitIterator(stream.ReadAllLines, 4)

            For Each fq As FastQ In From buf As String()
                                    In sBufs.AsParallel
                                    Select FastQ.FastaqParser(buf)
                Yield fq
            Next

            Call $"[Job Done!] Fastq format data parsing in {sw.ElapsedMilliseconds}ms...".__DEBUG_ECHO
        End Function

        ''' <summary>
        ''' 将<see cref="FastQ"/>对象重新生成fq文件之中的一条reads数据
        ''' </summary>
        ''' <param name="fq"></param>
        ''' <returns></returns>
        <Extension>
        Public Function AsReadsNode(fq As FastQ) As String
            Dim lines$() = New String(4 - 1) {}

            lines(Scan0) = fq.SEQ_ID.ToString
            lines(1) = fq.SequenceData
            lines(2) = fq.SEQ_ID2.ToString
            lines(3) = fq.Quantities

            Return lines.JoinBy(ASCII.LF)
        End Function

        <Extension>
        Public Function WriteFastQ(data As IEnumerable(Of FastQ), save$) As Boolean
            Using file As IO.StreamWriter = save.OpenWriter(Encodings.ASCII)
                For Each fq As FastQ In data
                    Call file.WriteLine(fq.AsReadsNode)
                Next

                Return True
            End Using
        End Function
    End Module
End Namespace
