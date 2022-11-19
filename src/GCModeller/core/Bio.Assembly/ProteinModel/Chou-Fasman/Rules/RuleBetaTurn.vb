#Region "Microsoft.VisualBasic::e5e581e8c1cd77738d4db741e37e6153, GCModeller\core\Bio.Assembly\ProteinModel\Chou-Fasman\Rules\RuleBetaTurn.vb"

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

    '   Total Lines: 41
    '    Code Lines: 29
    ' Comment Lines: 5
    '   Blank Lines: 7
    '     File Size: 1.62 KB


    '     Module RuleBetaTurn
    ' 
    '         Function: Invoke
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace ProteinModel.ChouFasmanRules.Rules

    Public Module RuleBetaTurn

        Const FT As Double = 7.5 * 10 ^ -5

        ''' <summary>
        ''' Invoke calculation
        ''' </summary>
        ''' <param name="SequenceData"></param>
        ''' <remarks></remarks>
        Public Function Invoke(SequenceData As AminoAcid()) As Integer
            Dim SequenceEnums As SequenceModel.Polypeptides.AminoAcid() = (From token In SequenceData Select token.AminoAcid).ToArray
            Dim ChunkBuffer As SequenceModel.Polypeptides.AminoAcid() = New SequenceModel.Polypeptides.AminoAcid(4 - 1) {}
            Dim Count_bt As Integer = 0

            For i As Integer = 0 To SequenceEnums.Count - (4 + 1)
                Call Array.ConstrainedCopy(SequenceEnums, i, ChunkBuffer, 0, 4)

                Dim Pi As Double = 1
                For p As Integer = 0 To 3
                    Pi *= ChouFasmanTable(ChunkBuffer(p)).f(p)
                Next
                If Pi > FT Then
                    Dim Pt As Double = Avg(ChunkBuffer, Function(t) t.Pt)
                    Dim Pa As Double = Avg(ChunkBuffer, Function(t) t.Pa)
                    Dim Pb As Double = Avg(ChunkBuffer, Function(t) t.Pb)

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
