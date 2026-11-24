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

Imports System.Text

Namespace Assembly.NCBI.GenBank.GBFF

    ''' <summary>
    ''' Genbank(GBFF) 文件的读取选项。
    ''' </summary>
    ''' <remarks>
    ''' 默认的读取模式会把整个记录(包括序列数据)加载到内存之中；对于人基因组这种
    ''' 单条染色体的序列就具有250Mb的记录而言，可以通过<see cref="LoadSequence"/>=<c>False</c>
    ''' 打开低内存模式，此时序列数据会保留在原始的文件之中，在需要的时候才按需回读。
    ''' 
    ''' 请使用<see cref="LowMemory"/>这样的共享实例，而不是每次读取都创建新的选项对象。
    ''' </remarks>
    Public Class GbkReadOptions

        ''' <summary>
        ''' 文本编码，GenBank是ASCII文本，使用Latin1可以保证一个字节对应一个字符
        ''' </summary>
        Public Property Encoding As Encoding = Encoding.Latin1

        ''' <summary>
        ''' 顺序读取的时候所使用的缓冲区的大小
        ''' </summary>
        Public Property BufferSize As Integer = 1024 * 1024

        ''' <summary>
        ''' 是否将``ORIGIN``段的序列数据加载到内存之中。
        ''' 
        ''' 这个选项为<c>False</c>的时候即为低内存模式，此时只会记录序列数据在文件之中的
        ''' 偏移量信息，序列数据在需要的时候通过<see cref="FileBackedSequenceProvider"/>回读。
        ''' </summary>
        Public Property LoadSequence As Boolean = True

        ''' <summary>
        ''' 是否解析``FEATURES``段
        ''' </summary>
        Public Property LoadFeatures As Boolean = True

        ''' <summary>
        ''' 是否将记录的偏移量索引缓存到磁盘文件(<c>*.gbff.idx</c>)之中
        ''' </summary>
        Public Property BuildRecordIndexCache As Boolean = True

        ''' <summary>
        ''' 单条记录之中所允许跳过的非法行的最大数量，超过这个数量的时候会抛出<see cref="GbkFormatException"/>
        ''' </summary>
        Public Property MaxInvalidLineCount As Integer = 100

        ''' <summary>
        ''' 低内存模式下序列数据的锚点间隔(单位:行)。
        ''' 
        ''' 低内存模式回读序列的时候，会首先定位到目标位置之前最近的锚点，然后再向前扫描，
        ''' 这个值越大则锚点表越小，但是平均需要扫描的行数也越多。设置为0或者负数的时候只会在
        ''' ``ORIGIN``段的第一行建立锚点。
        ''' </summary>
        Public Property SequenceAnchorInterval As Integer = 10000

        ''' <summary>
        ''' 默认的读取选项: 序列数据与位点注释数据都加载到内存之中
        ''' </summary>
        Public Shared ReadOnly [Default] As New GbkReadOptions

        ''' <summary>
        ''' 低内存模式: 序列数据不加载到内存之中，在需要的时候从原始文件之中按需回读
        ''' </summary>
        Public Shared ReadOnly LowMemory As New GbkReadOptions With {.LoadSequence = False}
    End Class
End Namespace
