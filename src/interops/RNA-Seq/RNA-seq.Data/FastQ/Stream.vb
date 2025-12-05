#Region "Microsoft.VisualBasic::dfd570ac09e335b345ce1897dcbce6b9, RNA-Seq\RNA-seq.Data\FastQ\Stream.vb"

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

    '   Total Lines: 88
    '    Code Lines: 56 (63.64%)
    ' Comment Lines: 18 (20.45%)
    '    - Xml Docs: 27.78%
    ' 
    '   Blank Lines: 14 (15.91%)
    '     File Size: 4.26 KB


    '     Module Stream
    ' 
    '         Function: AsReadsNode, ReadAllLines, WriteFastQ
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Text
Imports ASCII = Microsoft.VisualBasic.Text.ASCII

Namespace FQ

    Public Module Stream

        Public Iterator Function ReadAllLines(path$, Optional encoding As Encodings = Encodings.Default) As IEnumerable(Of FastQ)
            Dim strBuffer As IEnumerable(Of String()) = TaskPartitions.SplitIterator(path.IterateAllLines, 4)

            For Each fq As FastQ In strBuffer.AsParallel.Select(AddressOf FastQ.FastaqParser)
                Yield fq
            Next
        End Function

        ''' <summary>
        ''' 将<see cref="FastQ"/>对象重新生成fq文件之中的一条reads数据
        ''' </summary>
        ''' <param name="fq"></param>
        ''' <returns></returns>
        <Extension>
        Public Function AsReadsNode(fq As FastQ) As String
            Dim lines$() = New String(4 - 1) {}

            ' 对测序结果原始图像数据利用软件Bcl2fastq(v2.17.1.14)进行图像碱基识别（Base Calling），
            ' 初步质量分析（在测序过程中， Illumina内置软件根据每个测序片段， 即read， 前25个碱基的
            ' 质量决定该read是保留还是舍弃），得到原始测序数据（Pass Filter Data），结果以 FASTQ 文件
            ' 格式存储， 其中包含测序序列信息（FASTQ格式第二行）及与其对应的测序质量信息（FASTQ格式第四行）。

            ' FASTQ格式文件中每个read由四行描述， 如下： 

            ' @GWZHISEQ01289:C3Y96ACXX : 6:1101:1704:2425 1:N : 0:GGCTAC
            ' GCTCTTTGCCCTTCTCGTCGAAAATTGTCTCCTCATTCGAAACTTCTCTGT
            ' +
            ' @@CFFFDEHHHHFIJJJ@FHGIIIEHIIJBHHHIJJEGIIJJIGHIGHCCF

            ' 每个序列共有4行， 第1行和第3行是序列名称（有的fq文件为了节省存储空间会省略第三行“＋”后面的序列名称）， 
            ' 由测序仪产生；第2行是序列；第4行是序列的测序质量， 每个字符对应第2行每个碱基， 第四行每个字符对应的
            ' ASCII值减去33， 即为该碱基的测序质量值， 比如@对应的ASCII值为64， 那么其对应的碱基质量值是31。
            ' 从Illumina GA Pipeline v1.8开始（目前为v1.9），碱基质量值范围为0到41。

            lines(Scan0) = fq.SEQ_ID.ToString
            lines(1) = fq.SequenceData
            lines(2) = If(fq.SEQ_Info.StringEmpty(, True), "+", fq.SEQ_Info.ToString)
            lines(3) = fq.Quality

            Return lines.JoinBy(ASCII.LF)
        End Function

        <Extension>
        Public Function WriteFastQ(data As IEnumerable(Of FastQ), save$, Optional encoding As Encodings = Encodings.ASCII) As Boolean
            Using file As IO.StreamWriter = save.OpenWriter(encoding)
                For Each fq As FastQ In data
                    Call file.WriteLine(fq.AsReadsNode)
                Next

                Return True
            End Using
        End Function
    End Module
End Namespace
