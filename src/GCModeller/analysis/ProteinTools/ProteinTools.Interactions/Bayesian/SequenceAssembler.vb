#Region "Microsoft.VisualBasic::c236fa433fc62923a4e6a06c2a9ae37f, analysis\ProteinTools\ProteinTools.Interactions\Bayesian\SequenceAssembler.vb"

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

    ' Class SequenceAssembler
    ' 
    '     Function: Assemble, FileIO, initialize
    '     Class AlignmentColumn
    ' 
    '         Properties: Alphabets, ColIndex
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Compute, GetFrequency, GetResidueCollection, ToString
    '         Class Alphabet
    ' 
    '             Properties: [Chr], Counts
    ' 
    '             Function: Convert, ToString
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.STDIO
Imports Microsoft.VisualBasic.Linq.Extensions

''' <summary>
''' 在计算贝叶斯网络所需要的矩阵之前所进行的比对序列的拼接并进行向量化的程序模块
''' </summary>
''' <remarks></remarks>
Public Class SequenceAssembler

    ''' <summary>
    ''' 每一个AlignmentColumn对象可以看作为贝叶斯网络中的一个节点
    ''' </summary>
    ''' <param name="alignFiles">FASTA格式的蛋白质比对序列数据</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function initialize(alignFiles As String()) As AlignmentColumn()
        Dim SequenceData As String() = Assemble(FileIO(alignFiles)) '读取序列数据并进行拼接
        Dim bCheckLength As Boolean = (From str As String In SequenceData
                                       Select Len(str) Distinct).Count = 1 '检查所有的序列的长度是否一致

        If bCheckLength Then
            Dim LQuery = (From Handle As Integer
                          In (Len(SequenceData.First) - 1).Sequence
                          Let col As AlignmentColumn = New AlignmentColumn(Handle) With {.SequenceCount = SequenceData.Count}
                          Select col.Compute(SequenceCollection:=SequenceData)).ToArray '
            Return LQuery.ToArray
        Else
            Call Printf("Sequence length is not consistent!")
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' 执行序列拼接
    ''' </summary>
    ''' <param name="alignSeq">要求比对上的序列的集合中的序列的数目必须要一致</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function Assemble(alignSeq As String()()) As String()
        Dim sBuilderLst As StringBuilder() = (From Handle As Integer In alignSeq.First.Sequence Select New StringBuilder(4096)).ToArray
        For row As Integer = 0 To alignSeq.First.Count - 1
            For col As Integer = 0 To alignSeq.Count - 1
                Call sBuilderLst(row).Append(alignSeq(col)(row))
            Next
        Next

        Return (From sBuilder As StringBuilder In sBuilderLst Let seq = sBuilder.ToString Select seq).ToArray
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="FileList">FASTA文件列表</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function FileIO(FileList As String()) As String()()
        Dim FsaList = (From alignFile As String In FileList Select SMRUCC.genomics.SequenceModel.FASTA.FastaFile.Read(alignFile)).ToArray
        Dim ExtractList As List(Of String()) = New List(Of String())       '取出每一条序列
        For Each fsa In FsaList
            Call ExtractList.Add((From seq In fsa Select seq.SequenceData).ToArray)
        Next

        Return ExtractList.ToArray
    End Function

    Public Class AlignmentColumn

        Public Class Alphabet
            Public Property [Chr] As Char
            Public Property Counts As Long

            Public Overrides Function ToString() As String
                Return String.Format("{0} => {1}", Counts, Chr)
            End Function

            ''' <summary>
            ''' 将氨基酸残基转换为该类型的残基在字典中所指向的位置
            ''' </summary>
            ''' <param name="residue"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Shared Function Convert(residue As Char) As Integer
                Return AlignmentColumn.ProteinAlphabetDictionary(residue)
            End Function
        End Class

        ''' <summary>
        ''' 这个表记录了每一个残基对象在该列种的出现频数
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Alphabets As Alphabet()
        ''' <summary>
        ''' 即本列比对列在整个表中的列标号，在初始化开始必须要赋值
        ''' </summary>
        ''' <remarks></remarks>
        Dim Handle As Long

        ''' <summary>
        ''' 序列数目，即统计的样本数
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend SequenceCount As Integer
        ''' <summary>
        ''' 原始的序列数据，每一个元素代表一行中的某一个位置的残基元素
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend CharArray As Char()

        ''' <summary>
        ''' 获取特定残基在本列中的出现概率
        ''' </summary>
        ''' <param name="Residue">目标残基对象的字母代号</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFrequency(Residue As Char) As Double
            Dim handle As Integer = AlignmentColumn.ProteinAlphabetDictionary(Residue)
            Return Alphabets(handle).Counts / SequenceCount
        End Function

        Public ReadOnly Property ColIndex As Long
            Get
                Return Handle
            End Get
        End Property

        ''' <summary>
        ''' 氨基酸残基的代码名称和其在统计列种的位置，-符号表示比对结果中的空缺
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend Shared ReadOnly ProteinAlphabetDictionary As Dictionary(Of Char, Integer) =
            New Dictionary(Of Char, Integer) From {
                {"A"c, 0}, {"C"c, 1}, {"D"c, 2}, {"E"c, 3}, {"F"c, 4},
                {"G"c, 5}, {"H"c, 6}, {"I"c, 7}, {"K"c, 8}, {"L"c, 9},
                {"M"c, 10}, {"N"c, 11}, {"O"c, 12}, {"P"c, 13}, {"Q"c, 14},
                {"R"c, 15}, {"S"c, 16}, {"T"c, 17}, {"U"c, 18}, {"V"c, 19},
                {"W"c, 20}, {"Y"c, 21}, {"-"c, 22}}

        ''' <summary>
        ''' 获取所有的残基的集合
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetResidueCollection() As Char()
            Dim ChunkBuffer As Char() = New Char(22) {}
            Call AlignmentColumn.ProteinAlphabetDictionary.Keys.CopyTo(ChunkBuffer, 0)
            Return ChunkBuffer
        End Function

        Sub New(Handle As Long)
            Dim ChrAlphabets As Char() = New Char(23) {}

            Call ProteinAlphabetDictionary.Keys.CopyTo(ChrAlphabets, 0)

            Alphabets = New Alphabet(22) {}
            For i As Integer = 0 To 22
                Alphabets(i) = New Alphabet With {.Chr = ChrAlphabets(i)}
            Next

            Me.Handle = Handle
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="SequenceCollection">所有的序列数据都必须是等长的</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Compute(SequenceCollection As String()) As AlignmentColumn
            Me.CharArray = (From seq As String In SequenceCollection Select seq(Me.Handle)).ToArray
            For Each residue As Char In CharArray
                Dim hwnd As Integer = ProteinAlphabetDictionary(residue)
                Alphabets(hwnd).Counts += 1
            Next

            Return Me
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("Alignment Column ==> {0}", ColIndex)
        End Function
    End Class
End Class
