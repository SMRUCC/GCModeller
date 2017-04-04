#Region "Microsoft.VisualBasic::199ba12f85c417c6828d2cf322d79dbf, ..\core\Bio.Assembly\SequenceModel\Polypeptides\Chou-Fasman\Rules\RuleBetaTurn.vb"

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

Namespace SequenceModel.Polypeptides.SecondaryStructure.ChouFasmanRules

    Public Module RuleBetaTurn

        Const FT As Double = 7.5 * 10 ^ -5

        ''' <summary>
        ''' Invoke calculation
        ''' </summary>
        ''' <param name="SequenceData"></param>
        ''' <remarks></remarks>
        Public Function Invoke(SequenceData As AminoAcid()) As Integer
            Dim SequenceEnums As SequenceModel.Polypeptides.Polypeptides.AminoAcid() = (From token In SequenceData Select token.AminoAcid).ToArray
            Dim ChunkBuffer As SequenceModel.Polypeptides.Polypeptides.AminoAcid() = New SequenceModel.Polypeptides.Polypeptides.AminoAcid(4 - 1) {}
            Dim Count_bt As Integer = 0

            For i As Integer = 0 To SequenceEnums.Count - (4 + 1)
                Call Array.ConstrainedCopy(SequenceEnums, i, ChunkBuffer, 0, 4)

                Dim Pi As Double = 1
                For p As Integer = 0 To 3
                    Pi *= ChouFasmanTable(ChunkBuffer(p)).f(p)
                Next
                If Pi > FT Then
                    Dim Pt As Double = Avg(ChunkBuffer, AddressOf ChouFasmanParameter.Get_Pt)
                    Dim Pa As Double = Avg(ChunkBuffer, AddressOf ChouFasmanParameter.Get_Pa)
                    Dim Pb As Double = Avg(ChunkBuffer, AddressOf ChouFasmanParameter.Get_Pb)

                    If Pt > 100 AndAlso (Pt > Pa AndAlso Pt > Pb) Then
                        For p As Integer = 0 To 3
                            SequenceData(p + i)._MastBetaTurn__ = True
                        Next
                        Count_bt += 1
                    End If
                End If
            Next

            Return Count_bt
        End Function
    End Module
End Namespace
