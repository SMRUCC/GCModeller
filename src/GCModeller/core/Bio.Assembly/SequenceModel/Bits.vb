#Region "Microsoft.VisualBasic::9207337d1ce0a3b1b28d43518f4a155e, GCModeller\core\Bio.Assembly\SequenceModel\Bits.vb"

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

    '   Total Lines: 119
    '    Code Lines: 79
    ' Comment Lines: 22
    '   Blank Lines: 18
    '     File Size: 4.29 KB


    '     Class Bits
    ' 
    '         Properties: length, seqType, title
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: FromNucleotide, FromPolypeptide, GetSequenceData, overlapSize, OverlapSize
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.Polypeptides
Imports stdNum = System.Math

Namespace SequenceModel

    ''' <summary>
    ''' Sequence model in bytes
    ''' </summary>
    Public Class Bits : Implements IFastaProvider

        ReadOnly bytes As Byte()

        Public ReadOnly Property seqType As SeqTypes
        Public ReadOnly Property title As String Implements IFastaProvider.title
        Public ReadOnly Property length As Integer
            Get
                Return bytes.Length
            End Get
        End Property

        Private Sub New(title$, type As SeqTypes, bytes As Byte())
            Me.bytes = bytes
            Me.seqType = type
            Me.title = title
        End Sub

        ''' <summary>
        ''' 注意这个函数并不是用于比较序列相似度, 而是精准的计算比较两条序列从头开始的重叠片段的大小
        ''' </summary>
        ''' <param name="another"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 主要是一个用于序列装配的帮助函数
        ''' </remarks>
        Public Function OverlapSize(another As Bits) As Integer
            Dim minW As Integer = stdNum.Min(another.length, Me.length) * 0.5
            Dim size1, size2 As Integer

            size1 = overlapSize(bytes, another.bytes)

            If size1 >= minW Then
                Return size1
            End If

            size2 = overlapSize(another.bytes, bytes)

            Return stdNum.Max(size1, size2)
        End Function

        Private Shared Function overlapSize(a As Byte(), b As Byte()) As Integer
            Dim first As Byte = b(Scan0)
            Dim break As Boolean = False

            For i As Integer = 0 To a.Length - 1
                If a(i) = first Then
                    ' 从这里开始往后面进行一一比较
                    For j As Integer = 0 To b.Length - 1
                        If i + j = a.Length Then
                            ' 当前的序列已经到头了
                            ' 说明已经找到了一个重叠区域
                            ' 这个重叠区域的长度为j
                            Return j
                        End If
                        If b(j) <> a(i + j) Then
                            ' 中间有一个位点不一致
                            ' 这个不是一个有效的重叠区域
                            break = True
                            Exit For
                        End If
                    Next

                    If Not break Then
                        ' another序列已经到头了
                        ' 并且没有不一致的位点
                        ' 说明存在一个有效的重叠区域
                        ' 区域的长度为another序列的长度
                        Return b.Length
                    End If
                End If
            Next

            ' 没有重叠片段
            Return 0
        End Function

        Public Overrides Function ToString() As String
            Return $"{seqType.Description} {title}"
        End Function

        Public Shared Function FromNucleotide(nucl As IAbstractFastaToken) As Bits
            Return New Bits(
                title:=nucl.title,
                type:=SeqTypes.DNA,
                bytes:=NucleicAcid.Enums(nucl.SequenceData).ToArray
            )
        End Function

        Public Shared Function FromPolypeptide(prot As IAbstractFastaToken) As Bits
            Return New Bits(
                title:=prot.title,
                type:=SeqTypes.Protein,
                bytes:=Polypeptide.ConstructVector(prot.SequenceData).ToArray
            )
        End Function

        Public Function GetSequenceData() As String Implements ISequenceProvider.GetSequenceData
            Select Case seqType
                Case SeqTypes.DNA
                    Return NucleicAcid.ToString(bytes)
                Case SeqTypes.Protein
                    Return Polypeptide.ToString(bytes)
                Case Else
                    Throw New NotImplementedException(seqType.Description)
            End Select
        End Function
    End Class
End Namespace
