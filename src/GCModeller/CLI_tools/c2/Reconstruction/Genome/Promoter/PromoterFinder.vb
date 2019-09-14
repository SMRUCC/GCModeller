#Region "Microsoft.VisualBasic::46cfd1b3945fa36a0636920608adce52, CLI_tools\c2\Reconstruction\Genome\Promoter\PromoterFinder.vb"

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

    '         Class PromoterFinder
    ' 
    '             Properties: ReconstructedDNASegments
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Function: GetSegments, Performance, (+2 Overloads) TryParse
    '             Class Chromosome
    ' 
    '                 Properties: Complement, Length, Sequence, Title
    ' 
    '                 Constructor: (+1 Overloads) Sub New
    '                 Function: ToString
    ' 
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Extensions

Namespace Reconstruction : Partial Class Promoters

        ''' <summary>
        ''' 搜索所有可能的启动子序列
        ''' </summary>
        ''' <remarks>
        ''' 所使用的数据来自于MetaCyc数据库中的dnaseq.fsa和全基因组序列数据
        ''' </remarks>
        Public Class PromoterFinder : Inherits c2.Reconstruction.Operation

            ''' <summary>
            ''' 所有的基因对象的序列
            ''' </summary>
            ''' <remarks></remarks>
            Dim SpecieGenes As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
            ''' <summary>
            ''' 全基因组序列数据
            ''' </summary>
            ''' <remarks></remarks>
            Dim WholeGenome As Chromosome

            ''' <summary>
            ''' 一个染色体片段
            ''' </summary>
            ''' <remarks></remarks>
            Public Class Chromosome

                ''' <summary>
                ''' The name property for this chromosome object.
                ''' </summary>
                ''' <value></value>
                ''' <returns></returns>
                ''' <remarks></remarks>
                Public Property Title As String
                ''' <summary>
                ''' The sequence data of this chromosome object.
                ''' </summary>
                ''' <value></value>
                ''' <returns></returns>
                ''' <remarks></remarks>
                Public Property Sequence As String
                ''' <summary>
                ''' The complement sequence data of this chromemosome object.
                ''' </summary>
                ''' <value></value>
                ''' <returns></returns>
                ''' <remarks></remarks>
                Public Property Complement As String

                Public ReadOnly Property Length As Long
                    Get
                        Return Len(Sequence)
                    End Get
                End Property

                Sub New(fsa As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)
                    Title = fsa.ToString
                    Sequence = fsa.SequenceData
                    Complement = LANS.SystemsBiology.SequenceModel.FASTA.FastaToken.Complement(fsa).SequenceData
                End Sub

                Public Overrides Function ToString() As String
                    Return Title
                End Function
            End Class

            ''' <summary>
            ''' 在MetaCyc数据库中描述一个片段区域位置的正则表达式
            ''' </summary>
            ''' <remarks></remarks>
            Const METACYC_POSITION As String = "(\d+\.{2}\d+)|(complement\(\d+\.{2}\d+\))"

            Sub New(Session As OperationSession)
                Call MyBase.New(Session)
                Dim MetaCyc = Session.ReconstructedMetaCyc
                Me.SpecieGenes = MetaCyc.Database.FASTAFiles.DNAseq
                Me.WholeGenome = New PromoterFinder.Chromosome(fsa:=MetaCyc.Database.WholeGenome)
            End Sub

            Dim _ReconstructedDNASegments As LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader()

            Public ReadOnly Property ReconstructedDNASegments As LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader()
                Get
                    If _ReconstructedDNASegments.IsNullOrEmpty Then
                        Call Me.Performance()
                    End If
                    Return _ReconstructedDNASegments
                End Get
            End Property

            Public Function GetSegments() As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
                Dim FQuery = From Segment As LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader In Me.ReconstructedDNASegments Select Segment.GetFasta '转换为FASTA数据库文件
                Return CType(FQuery.ToArray, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile)
            End Function

            ''' <summary>
            ''' 尝试着从每一个CDS的上游的500bp的区域中解析出启动子序列
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Overrides Function Performance() As Integer
                Dim LQuery = From Gene As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken In SpecieGenes Select PromoterFinder.TryParse(Gene) '
                _ReconstructedDNASegments = LQuery.ToArray
                LQuery = From Segment As LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader In _ReconstructedDNASegments Select TryParse(WholeGenome, Segment) '获取每一个CDS的上游500bp长度的片段的序列数据
                _ReconstructedDNASegments = LQuery.ToArray
                Return _ReconstructedDNASegments.Count
            End Function

            Const SEGMENT_LENGTH As Integer = 450

            ''' <summary>
            ''' 尝试着解析出一个500bp的包含有启动子序列的核酸片段
            ''' </summary>
            ''' <param name="WholeGenome">目标物种的全基因组序列</param>
            ''' <param name="Segment">一个CDS序列片段区域</param>
            ''' <returns></returns>
            ''' <remarks>取从CDS的起始密码子开始到上游的150bp长度的序列</remarks>
            Public Shared Function TryParse(WholeGenome As PromoterFinder.Chromosome, Segment As LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader) As LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader
                If Segment.Complement Then
                    Segment.SequenceData = Mid(WholeGenome.Complement, Segment.Right - SEGMENT_LENGTH, SEGMENT_LENGTH)
                    Segment.SequenceData = Segment.SequenceData.Reverse.ToArray
                    Segment.Left += SEGMENT_LENGTH
                    Segment.Right += SEGMENT_LENGTH
                Else '直接取上游的500bp的序列片段
                    If Segment.Left < 100 Then
                        Segment.SequenceData = Mid(WholeGenome.Sequence, 1, Segment.Left)
                        Segment.Left = 1
                        Segment.Right -= Segment.Left
                    Else
                        Segment.SequenceData = Mid(WholeGenome.Sequence, Segment.Left - SEGMENT_LENGTH, SEGMENT_LENGTH)
                        Segment.Left -= SEGMENT_LENGTH
                        Segment.Right -= SEGMENT_LENGTH
                    End If
                End If
                Return Segment
            End Function

            ''' <summary>
            ''' 尝试着从一个FASTA文件之中解析出位置数据
            ''' </summary>
            ''' <param name="fsa"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Shared Function TryParse(fsa As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken) As LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader
                Dim p As String = Regex.Match(fsa.Title, METACYC_POSITION).Value
                Dim Segment As LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader = New LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader
                Segment.Complement = InStr(p, "complement", CompareMethod.Text) > 0
                Dim Ms = Regex.Matches(p, "\d+")
                Segment.Left = Val(Ms(0).Value)
                Segment.Right = Val(Ms(1).Value)
                Dim sc As String() = fsa.Attributes.Last.Split
                Segment.Title = String.Format("{0} {1}", sc(0), sc(1))
                Segment.Description = sc(2)

                Return Segment
            End Function
        End Class
    End Class
End Namespace
