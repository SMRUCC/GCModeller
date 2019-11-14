#Region "Microsoft.VisualBasic::2ba7b4d4bee7a2d5afedf9075649e4ff, core\Bio.Assembly\ProteinModel\Chou-Fasman\Rules\RuleAlphaHelix.vb"

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

    '     Module RuleAlphaHelix
    ' 
    '         Function: CalculateCore, ExtendCore, get_ProPosition, Invoke
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel

Namespace ProteinModel.ChouFasmanRules.Rules

    Public Module RuleAlphaHelix

        ''' <summary>
        ''' Invoke calculation
        ''' </summary>
        ''' <param name="SequenceData"></param>
        ''' <remarks></remarks>
        Public Function Invoke(SequenceData As AminoAcid()) As Integer
            Dim SequenceEnums As Polypeptides.AminoAcid() = (From token In SequenceData Select token.AminoAcid).ToArray
            Dim ChunkBuffer As Polypeptides.AminoAcid() = New SequenceModel.Polypeptides.AminoAcid(CORE_LENGTH - 1) {}
            Dim Count_a As Integer = 0
            Dim pp As Integer

            For i As Integer = 0 To SequenceEnums.Count - (CORE_LENGTH + 1)

                Call Array.ConstrainedCopy(SequenceEnums, i, ChunkBuffer, 0, CORE_LENGTH)

                If CalculateCore(ChunkBuffer) Then '计算核心
                    Dim Segment = ExtendCore(SequenceEnums, i, CORE_LENGTH) '延伸核心
                    If Segment.FragmentSize > 5 Then
                        Dim TempChunk As SequenceModel.Polypeptides.AminoAcid() = New SequenceModel.Polypeptides.AminoAcid(Segment.FragmentSize - 1) {}
                        Call Array.ConstrainedCopy(SequenceEnums, Segment.Left, TempChunk, 0, Segment.FragmentSize)
                        If Avg(TempChunk, Function(t) t.Pa) > Avg(TempChunk, Function(t) t.Pb) Then
                            For p As Integer = Segment.Left To Segment.Right
                                SequenceData(p)._MaskAlphaHelix = True
                            Next
                            i = If(Segment.Right > pp, Segment.Right, pp + 1)
                            pp = i
                        End If
                    End If
                End If
            Next

            Return Count_a
        End Function

        Private Function ExtendCore(SequenceData As Polypeptides.AminoAcid(), i As Integer, Length As Integer) As Location
            Dim ChunkBuffer As Polypeptides.AminoAcid() = New Polypeptides.AminoAcid(Length - 1) {}
            Dim Left, Right As Integer

            '向左端延伸
            For p As Integer = i To 0 Step -1
                Call Array.ConstrainedCopy(SequenceData, p, ChunkBuffer, 0, Length)
                Dim Pa_avg As Double = Avg(ChunkBuffer, Function(t) t.Pa)
                If Pa_avg < 100 OrElse p = 0 Then
                    Left = p
                    Exit For
                Else
                    Dim pp = p
                    Dim p_Pro = get_ProPosition(ChunkBuffer, pp:=Function(i_Pro As Integer) pp - i_Pro)

                    If Not p_Pro.IsNullOrEmpty Then
                        If p_Pro.Min > 3 AndAlso p_Pro.Min < SequenceData.Count - 3 Then
                            Left = p
                            Exit For
                        End If
                    End If
                End If
            Next

            Dim d As Integer = SequenceData.Count - Length - 1
            For p As Integer = i + 1 To d
                Call Array.ConstrainedCopy(SequenceData, p, ChunkBuffer, 0, Length)
                Dim Pa_avg As Double = Avg(ChunkBuffer, Function(t) t.Pa)
                If Pa_avg < 100 OrElse p = d Then
                    Right = p + Length
                    Exit For
                Else
                    Dim pp = p
                    Dim p_Pro = get_ProPosition(ChunkBuffer, pp:=Function(i_Pro As Integer) i_Pro + pp)

                    If Not p_Pro.IsNullOrEmpty Then
                        If p_Pro.Max < SequenceData.Count - 3 AndAlso p_Pro.Min > 3 Then
                            Right = p - 1 + Length
                            Exit For
                        End If
                    End If
                End If
            Next

            Return New Location With {.Left = Left, .Right = Right}
        End Function

        Private Function get_ProPosition(ChunkBuffer As SequenceModel.Polypeptides.AminoAcid(), pp As Func(Of Integer, Integer)) As Integer()
            Dim p_Pro = (From i_Pro As Integer In ChunkBuffer.Sequence Let Token As SequenceModel.Polypeptides.AminoAcid = ChunkBuffer(i_Pro)
                         Where Token = SequenceModel.Polypeptides.AminoAcid.Praline Select pp(i_Pro)).ToArray
            Return p_Pro
        End Function

        Private Function CalculateCore(ChunkBuffer As SequenceModel.Polypeptides.AminoAcid()) As Boolean
            Dim LQuery = (From token In ChunkBuffer Let p = ChouFasmanTable(token).Pa Where p > 100 Select 1).ToArray
            Return (LQuery.Count / ChunkBuffer.Count) > PROPORTION
        End Function
    End Module
End Namespace
