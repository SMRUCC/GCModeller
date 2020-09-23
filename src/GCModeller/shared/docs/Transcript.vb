#Region "Microsoft.VisualBasic::941100e6550a2079b77319671909accd, shared\docs\Transcript.vb"

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

    '     Class Transcript
    ' 
    '         Properties: _5UTR, ATG, Left, Length, Operon
    '                     Position, Raw, Right, Strand, Support
    '                     Synonym, TGA, TSS_ID, TSSs, TSSsShared
    '                     TTSs
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: __getMappingLoci, __merge, (+5 Overloads) Copy, (+2 Overloads) CreateObject, MergeJason
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Data.Linq.Mapping
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation
Imports stdNum = System.Math

Namespace DocumentFormat

    ''' <summary>
    ''' 转录本对象，包含有基本的基因结构：ATG-TGA，TSSs，TTS以及链的方向，表达量的高低
    ''' </summary>
    Public Class Transcript : Inherits SMRUCC.genomics.SequenceModel.NucleotideModels.Contig

        Public Property TSS_ID As String
        Public Property Operon As String

#Region "和方向无关的"

#Region "Transcription"

        ''' <summary>
        ''' <see cref="SMRUCC.genomics.SequenceModel.NucleotideModels.Contig"/>.Left (The transcription start coordinate.)
        ''' </summary>
        ''' <returns></returns>
        Public Property Left As Long
            Get
                Return _Left
            End Get
            Set(value As Long)
                _Left = value
                _MappingLocation = Nothing
            End Set
        End Property
        ''' <summary>
        ''' <see cref="SMRUCC.genomics.SequenceModel.NucleotideModels.Contig"/>.Right  (The transcription stop coordinate.)
        ''' </summary>
        ''' <returns></returns>
        Public Property Right As Long
            Get
                Return _Right
            End Get
            Set(value As Long)
                _Right = value
                _MappingLocation = Nothing
            End Set
        End Property

        Dim _Left, _Right As Long

#End Region

#End Region

#Region "和方向有关的"

        Public ReadOnly Property TSSs As Long
            Get
                If Me.MappingLocation.Strand = Strands.Forward Then
                    Return Left
                Else
                    Return Right
                End If
            End Get
        End Property
        Public ReadOnly Property TTSs As Long
            Get
                If Me.MappingLocation.Strand = Strands.Forward Then
                    Return Right
                Else
                    Return Left
                End If
            End Get
        End Property

        Protected Overrides Function __getMappingLoci() As NucleotideLocation
            Dim array As Integer() = {Left, Right}
            _Left = array.Min
            _Right = array.Max
            Return New SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation(Left, Right, Strand)
        End Function
#End Region

        <Column(Name:="5'UTR")> Public ReadOnly Property _5UTR As Integer
            Get
                Return stdNum.Abs(TSSs - ATG)
            End Get
        End Property

        ''' <summary>
        ''' <see cref="SMRUCC.genomics.SequenceModel.NucleotideModels.Contig"/>.Strands
        ''' </summary>
        ''' <returns></returns>
        Public Property Strand As String
            Get
                Return _Strand
            End Get
            Set(value As String)
                _Strand = value
                _MappingLocation = Nothing
            End Set
        End Property

        Dim _Strand As String

        Public ReadOnly Property Length As Integer
            Get
                Return MappingLocation.FragmentSize
            End Get
        End Property

        ''' <summary>
        ''' 位点和基因对象之间的位置关系的简要描述
        ''' </summary>
        ''' <returns></returns>
        Public Property Position As String
        Public Property Synonym As String

#Region "Translation"

        Public Property ATG As Long
        Public Property TGA As Long

#End Region

        ''' <summary>
        ''' 5'UTR左端的共享的reads计数
        ''' </summary>
        ''' <returns></returns>
        Public Property TSSsShared As Integer
        Public Property Support As Boolean

        ''' <summary>
        ''' Htseq-Count raw/GeneLength
        ''' </summary>
        ''' <returns></returns>
        Public Property Raw As Integer
        '   Public Property ExprConsi As Double

        Public Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return $"[ORF] ({MappingLocation.ToString})  {Synonym}"
        End Function

        ''' <summary>
        ''' 要确保左小右大
        ''' </summary>
        ''' <param name="contig"></param>
        ''' <returns></returns>
        Public Shared Function CreateObject(Of T As Transcript)(contig As NucleotideLocation) As T
            Dim Transcript As T = Activator.CreateInstance(Of T)
            Transcript.Left = contig.Left
            Transcript.Right = contig.Right
            Transcript.Strand = contig.Strand.GetBriefCode
            Transcript._MappingLocation = contig
            Return Transcript
        End Function

        Public Shared Function CreateObject(Of T As Transcript)(Gene As SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief) As T
            Dim Transcript = CreateObject(Of T)(Gene.Location)
            Transcript.Synonym = Gene.Synonym
            Transcript.ATG = Gene.ATG
            Transcript.TGA = Gene.TGA
            Transcript.Strand = If(Transcript.MappingLocation.Strand = Strands.Forward, "+", "-")

            Return Transcript
        End Function

#Region "这这里请注意在修改了类之中的定义之后，假若有新添加的属性请务必要在这里修改，否则会出现bUG的"

        Public Function Copy(Of T As Transcript)() As T
            Dim Transcript As T = Activator.CreateInstance(Of T)
            Call Microsoft.VisualBasic.Serialization.ShadowsCopy.ShadowCopy(Of Transcript)(Me, Transcript)
            Return Transcript
        End Function

        ''' <summary>
        ''' 单个的ORF
        ''' </summary>
        ''' <returns></returns>
        Public Function Copy(Of T As Transcript)(Gene As SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief) As T
            Dim Transcript As T = Me.Copy(Of T)
            Transcript.ATG = Gene.ATG
            Transcript.TGA = Gene.TGA
            Transcript.Left = Left
            Transcript.Position = "ORF"
            Transcript.Right = Right
            Transcript.Strand = Strand
            Transcript.Synonym = Gene.Synonym
            Return Transcript
        End Function

        Public Function Copy(Of T As Transcript)(Gene As SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief, Operon As String) As T
            Dim Transcript As T = Me.Copy(Of T)
            Transcript.ATG = Gene.ATG
            Transcript.TGA = Gene.TGA
            Transcript.Left = Left
            Transcript.Position = NameOf(Operon)
            Transcript.Right = Right
            Transcript.Strand = Strand
            Transcript.Synonym = Gene.Synonym
            Transcript.Operon = Operon
            Return Transcript
        End Function

        Public Function Copy(Of T As Transcript)(Antisense As String) As T
            Dim Transcript As T = Me.Copy(Of T)
            Transcript.ATG = ATG
            Transcript.Left = Left
            Transcript.Operon = Operon
            Transcript.Position = SegmentRelationships.InnerAntiSense.ToString
            Transcript.Right = Right
            Transcript.Strand = Strand
            Transcript.Synonym = Antisense
            Transcript.TGA = TGA
            Return Transcript
        End Function

        Public Function Copy(Of T As Transcript)(Position As SegmentRelationships, Synonym As String) As T
            Dim Transcript As T = Me.Copy(Of T)
            Transcript.Left = Left
            Transcript.Right = Right
            Transcript.Position = Position.ToString
            Transcript.Synonym = Synonym
            Transcript.Strand = Strand
            Transcript.ATG = ATG
            Transcript.TGA = TGA
            Return Transcript
        End Function
#End Region

        Public Shared Function MergeJason(source As Transcript(), Optional offset As Integer = 3) As Transcript()
            Dim LQuery = (From x In source Select x Group x By x.Synonym Into Group) _
                .ToDictionary(Function(x) x.Synonym,
                              Function(array) (From x In array.Group Select x Order By x.TSSs Ascending).ToArray)
            Dim arrayMerged As Transcript() = (From x In LQuery Select __merge(x.Value, offset)).ToVector
            Return arrayMerged
        End Function

        Private Shared Function __merge(source As Transcript(), offset As Integer) As Transcript()
            If source.Length = 1 Then
                Return source
            End If

            If source(Scan0).MappingLocation.Strand = Strands.Forward Then
                Dim list As New List(Of Transcript)
                Dim last As Transcript = source(Scan0)

                For Each loci In source.Skip(1)
                    If stdNum.Abs(loci.TSSs - last.TSSs) <= offset Then
                        last.TSSsShared += loci.TSSsShared
                    Else
                        Call list.Add(last)
                        last = loci
                    End If
                Next
                Call list.Add(last)

                Return list.Distinct.ToArray
            Else
                source = source.Reverse.ToArray

                Dim list As New List(Of Transcript)
                Dim last As Transcript = source(Scan0)

                For Each loci In source.Skip(1)
                    If stdNum.Abs(loci.TSSs - last.TSSs) <= offset Then
                        last.TSSsShared += loci.TSSsShared
                    Else
                        Call list.Add(last)
                        last = loci
                    End If
                Next
                Call list.Add(last)

                Return list.Distinct.ToArray
            End If
        End Function
    End Class
End Namespace
