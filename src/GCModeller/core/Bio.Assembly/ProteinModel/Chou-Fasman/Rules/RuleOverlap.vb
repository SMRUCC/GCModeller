#Region "Microsoft.VisualBasic::992bd4f610a2d3442d0b5aa7f72e19a5, GCModeller\core\Bio.Assembly\ProteinModel\Chou-Fasman\Rules\RuleOverlap.vb"

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

    '   Total Lines: 83
    '    Code Lines: 63
    ' Comment Lines: 5
    '   Blank Lines: 15
    '     File Size: 3.76 KB


    '     Module RuleOverlap
    ' 
    '         Sub: CalculateHelixSheetOverlap, CalculateHelixTurnOverlap, CalculateSheetTurnOverlap, Invoke, SetStructureType
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic

Namespace ProteinModel.ChouFasmanRules.Rules

    Public Module RuleOverlap

        ''' <summary>
        ''' Invoke calculation
        ''' </summary>
        ''' <param name="SequenceData"></param>
        ''' <remarks></remarks>
        Public Sub Invoke(SequenceData As AminoAcid())
            Dim HelixSheetOverlap As New List(Of AminoAcid)
            Dim HelixTurnOverlap As New List(Of AminoAcid)
            Dim SheetTurnOverlap As New List(Of AminoAcid)

            For i As Integer = 0 To SequenceData.Count - 1
                Dim Token = SequenceData(i)

                If Token.Coil Then
                    Token.StructureType = SecondaryStructures.Coils
                ElseIf Token.HelixSheetOverlap Then
                    Call HelixSheetOverlap.Add(Token)
                    Call CalculateHelixTurnOverlap(HelixTurnOverlap) : Call HelixTurnOverlap.Clear()
                    Call CalculateSheetTurnOverlap(SheetTurnOverlap) : Call SheetTurnOverlap.Clear()
                ElseIf Token.HelixTurnOverlap Then
                    Call HelixTurnOverlap.Add(Token)
                    Call CalculateHelixSheetOverlap(HelixSheetOverlap) : Call HelixSheetOverlap.Clear()
                    Call CalculateSheetTurnOverlap(SheetTurnOverlap) : Call SheetTurnOverlap.Clear()
                ElseIf Token.SheetTurnOverlap Then
                    Call SheetTurnOverlap.Add(Token)
                    Call CalculateHelixSheetOverlap(HelixSheetOverlap) : Call HelixSheetOverlap.Clear()
                    Call CalculateHelixTurnOverlap(HelixTurnOverlap) : Call HelixTurnOverlap.Clear()
                End If
            Next
        End Sub

        Private Sub SetStructureType(ChunkBuffer As List(Of AminoAcid), StructureType As SecondaryStructures)
            For Each Token In ChunkBuffer
                Token.StructureType = StructureType
            Next
        End Sub

        Private Sub CalculateHelixSheetOverlap(HelixSheetOverlap As List(Of AminoAcid))
            If HelixSheetOverlap.Count = 0 Then
                Return
            End If

            Dim ChunkBuffer = (From Token In HelixSheetOverlap Select Token.AminoAcid).ToArray
            Dim Pa = Avg(ChunkBuffer, Function(t) t.Pa)
            Dim Pb = Avg(ChunkBuffer, Function(t) t.Pb)

            Dim StructureType = If(Pa > Pb, SecondaryStructures.AlphaHelix, SecondaryStructures.BetaSheet)
            Call SetStructureType(HelixSheetOverlap, StructureType)
        End Sub

        Private Sub CalculateHelixTurnOverlap(HelixTurnOverlap As List(Of AminoAcid))
            If HelixTurnOverlap.Count = 0 Then
                Return
            End If

            Dim ChunkBuffer = (From Token In HelixTurnOverlap Select Token.AminoAcid).ToArray
            Dim Pa = Avg(ChunkBuffer, Function(t) t.Pa)
            Dim Pt = Avg(ChunkBuffer, Function(t) t.Pt)

            Dim StructureType = If(Pa > Pt, SecondaryStructures.AlphaHelix, SecondaryStructures.BetaTurn)
            Call SetStructureType(HelixTurnOverlap, StructureType)
        End Sub

        Private Sub CalculateSheetTurnOverlap(SheetTurnOverlap As List(Of AminoAcid))
            If SheetTurnOverlap.Count = 0 Then
                Return
            End If

            Dim ChunkBuffer = (From Token In SheetTurnOverlap Select Token.AminoAcid).ToArray
            Dim Pb = Avg(ChunkBuffer, Function(t) t.Pb)
            Dim Pt = Avg(ChunkBuffer, Function(t) t.Pt)

            Dim StructureType = If(Pb > Pt, SecondaryStructures.BetaSheet, SecondaryStructures.BetaTurn)
            Call SetStructureType(SheetTurnOverlap, StructureType)
        End Sub
    End Module
End Namespace
