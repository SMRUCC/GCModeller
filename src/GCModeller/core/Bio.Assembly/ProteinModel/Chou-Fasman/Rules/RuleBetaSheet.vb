#Region "Microsoft.VisualBasic::6df0d96f589d14de463f64e18c33f640, GCModeller\core\Bio.Assembly\ProteinModel\Chou-Fasman\Rules\RuleBetaSheet.vb"

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

    '   Total Lines: 77
    '    Code Lines: 57
    ' Comment Lines: 6
    '   Blank Lines: 14
    '     File Size: 3.44 KB


    '     Module RuleBetaSheet
    ' 
    '         Function: CalculateCore, ExtendCore, Invoke
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Loci

Namespace ProteinModel.ChouFasmanRules.Rules

    Public Module RuleBetaSheet

        ''' <summary>
        ''' Invoke calculation
        ''' </summary>
        ''' <param name="SequenceData"></param>
        ''' <remarks></remarks>
        Public Function Invoke(SequenceData As AminoAcid()) As Integer
            Dim SequenceEnums As SequenceModel.Polypeptides.AminoAcid() = (From token In SequenceData Select token.AminoAcid).ToArray
            Dim ChunkBuffer As SequenceModel.Polypeptides.AminoAcid() = New SequenceModel.Polypeptides.AminoAcid(CORE_LENGTH - 1) {}
            Dim Count_bs As Integer = 0
            Dim pp As Integer

            For i As Integer = 0 To SequenceEnums.Count - (CORE_LENGTH + 1)

                Call Array.ConstrainedCopy(SequenceEnums, i, ChunkBuffer, 0, CORE_LENGTH)

                If CalculateCore(ChunkBuffer) Then '计算核心
                    Dim Segment = ExtendCore(SequenceEnums, i, CORE_LENGTH) '延伸核心
                    Dim TempChunk As SequenceModel.Polypeptides.AminoAcid() = New SequenceModel.Polypeptides.AminoAcid(Segment.FragmentSize - 1) {}
                    Call Array.ConstrainedCopy(SequenceEnums, Segment.Left, TempChunk, 0, Segment.FragmentSize)

                    Dim Avg_Pa As Double = Avg(TempChunk, Function(t) t.Pa), Avg_Pb As Double = Avg(TempChunk, Function(t) t.Pb)

                    If Avg_Pb > 105 AndAlso Avg_Pb > Avg_Pa Then
                        For p As Integer = Segment.Left To Segment.Right
                            SequenceData(p)._MaskBetaSheet_ = True
                        Next
                        i = If(Segment.Right > pp, Segment.Right, pp + 1)
                        pp = i
                    End If
                End If
            Next

            Return Count_bs
        End Function

        Private Function ExtendCore(SequenceData As SequenceModel.Polypeptides.AminoAcid(), i As Integer, Length As Integer) As Location
            Dim ChunkBuffer As SequenceModel.Polypeptides.AminoAcid() = New SequenceModel.Polypeptides.AminoAcid(Length - 1) {}
            Dim Left, Right As Integer

            '向左端延伸
            For p As Integer = i To 0 Step -1
                Call Array.ConstrainedCopy(SequenceData, p, ChunkBuffer, 0, Length)
                Dim Pa_avg As Double = Avg(ChunkBuffer, Function(t) t.Pa)
                If Pa_avg < 100 OrElse p = 0 Then
                    Left = p
                    Exit For
                End If
            Next

            Dim d As Integer = SequenceData.Count - Length - 1
            For p As Integer = i + 1 To d
                Call Array.ConstrainedCopy(SequenceData, p, ChunkBuffer, 0, Length)
                Dim Pa_avg As Double = Avg(ChunkBuffer, Function(t) t.Pa)
                If Pa_avg < 100 OrElse p = d Then
                    Right = p + Length
                    Exit For
                End If
            Next

            Return New Location With {
                .Left = Left,
                .Right = Right
            }
        End Function

        Private Function CalculateCore(ChunkBuffer As SequenceModel.Polypeptides.AminoAcid()) As Boolean
            Dim LQuery = (From token In ChunkBuffer Let p = ChouFasmanTable(token).Pb Where p > 100 Select 1).ToArray
            Return (LQuery.Count / ChunkBuffer.Count) > PROPORTION
        End Function
    End Module
End Namespace
