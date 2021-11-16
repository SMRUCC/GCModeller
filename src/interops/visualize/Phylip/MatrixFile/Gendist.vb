#Region "Microsoft.VisualBasic::e46b5a4f458021b7adf69750557a5076, visualize\Phylip\MatrixFile\Gendist.vb"

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

    '     Class Gendist
    ' 
    '         Properties: NumberOfAlleles, NumberOfLoci, SpeciesCount
    ' 
    '         Function: CreateMotifDistrMAT, GenerateDocument, LoadDocument, SubMatrix
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace MatrixFile

    ''' <summary>
    ''' 在做motif的分布密度的时候，将每一种类型的motif看作为一个等位基因
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Gendist : Inherits MatrixFile

#Region "第一行的数据"

        ''' <summary>
        ''' 基因组的数目
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property SpeciesCount As Integer
            Get
                Return _innerMATRaw.Count - 1
            End Get
        End Property

        ''' <summary>
        ''' motif的数目
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property NumberOfLoci As Integer
            Get
                Return _innerMATRaw.First.Count - 1
            End Get
        End Property
#End Region

#Region "第二行的数据"

        ''' <summary>
        ''' There then follows a line which gives the numbers of alleles at each locus, in order. This must be the full number of alleles,
        ''' not the number of alleles which will be input: i. e. for a two-allele locus the number should be 2, not 1.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property NumberOfAlleles As Integer()

#End Region

        ''' <summary>
        ''' 主要是生成没有设置有A选项的文件数据
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function GenerateDocument() As String
            Dim DocBuilder As StringBuilder = New StringBuilder(1024)
            Call DocBuilder.AppendLine(String.Format("     {0}     {1}", SpeciesCount, NumberOfLoci))
            Call DocBuilder.AppendLine(String.Join(" ", (From n As Integer In NumberOfAlleles Select CStr(n)).ToArray))
            Call __createMatrix(DocBuilder)

            Return DocBuilder.ToString
        End Function

        Public Shared Function CreateMotifDistrMAT(Csv As File) As Gendist
            Dim MAT As Gendist = CreateObject(Of Gendist)(Csv)
            MAT.NumberOfAlleles = (From n In MAT.NumberOfLoci.Sequence Select 2).ToArray
            Return MAT
        End Function

        ''' <summary>
        ''' 加载已经生成的gendist矩阵文件之中的数据
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LoadDocument(Path As String) As Gendist
            Dim FileContent As String() = IO.File.ReadAllLines(Path).Skip(2).ToArray
            Dim MAT = (From strLine As String In FileContent.AsParallel
                       Let ID As String = Mid(strLine, 1, 10).Trim, strData As String = Mid(strLine, 11).Trim
                       Let Value As String() = strData.Split
                       Let Row As String()() = {New String() {ID}, Value}
                       Select CType(Row.Unlist, RowObject)).ToArray

            Dim MATList As New List(Of RowObject)
            Call MATList.Add(New String() {"GENOME_ID"})
            Call MATList.First.AddRange((From i As Integer In MAT.First.Sequence Select s = i.ToString).ToArray)
            Call MATList.AddRange(MAT)

            Dim CsvMAT As File = CType(MATList, File)
            Return Gendist.CreateMotifDistrMAT(CsvMAT)
        End Function

        ''' <summary>
        ''' 采集至少<paramref name="Count"></paramref>数量的和<paramref name="MainIndex"></paramref>相近的基因组
        ''' </summary>
        ''' <param name="Count"></param>
        ''' <param name="MainIndex"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SubMatrix(Count As Integer, MainIndex As String) As Gendist
            Dim ChunkBuffer = Me._innerMATRaw.FindAtColumn(MainIndex, 0)
            If ChunkBuffer.IsNullOrEmpty Then
                Throw New Exception(String.Format("Could not found the required genome named ""{0}""!", MainIndex))
            End If
            Dim MainRow As RowObject = ChunkBuffer.First
            Dim OtherGenome = (From row As RowObject
                               In _innerMATRaw.Skip(1).AsParallel
                               Where Not row.Equals(MainRow)
                               Select row).ToArray
            Dim MainRowCache = (From col As String In MainRow.Skip(1) Select Val(col)).ToArray
            Dim SelectLQuery = (From row In OtherGenome.AsParallel
                                Let InternalSamplingCounts = Function() As Integer
                                                                 Dim LQuery = (From i As Integer
                                                                               In MainRowCache.Sequence
                                                                               Where MainRowCache(i) > 0 AndAlso Val(row(i + 1)) > 0
                                                                               Select i).ToArray
                                                                 Return LQuery.Count
                                                             End Function()
                                Select InternalSamplingCounts, row
                                Order By InternalSamplingCounts Descending).ToArray.Take(Count).ToArray
            Dim SubMAT = New File
            Call SubMAT.Add(_innerMATRaw.First)
            Dim TempList = (From row In SelectLQuery Select row.row).AsList
            Call TempList.Add(MainRow)
            TempList = TempList.Shuffles.AsList

            Call SubMAT.AppendRange(TempList)

            Return Gendist.CreateMotifDistrMAT(SubMAT)
        End Function
    End Class
End Namespace
