Imports Microsoft.VisualBasic

Namespace SequenceModel.Polypeptides.SecondaryStructure.ChouFasmanRules

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
                    Token.StructureType = ChouFasman.SecondaryStructures.Coils
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

        Private Sub SetStructureType(ChunkBuffer As List(Of AminoAcid), StructureType As ChouFasman.SecondaryStructures)
            For Each Token In ChunkBuffer
                Token.StructureType = StructureType
            Next
        End Sub

        Private Sub CalculateHelixSheetOverlap(HelixSheetOverlap As List(Of AminoAcid))
            If HelixSheetOverlap.Count = 0 Then
                Return
            End If

            Dim ChunkBuffer = (From Token In HelixSheetOverlap Select Token.AminoAcid).ToArray
            Dim Pa = Avg(ChunkBuffer, AddressOf ChouFasmanParameter.Get_Pa)
            Dim Pb = Avg(ChunkBuffer, AddressOf ChouFasmanParameter.Get_Pb)

            Dim StructureType = If(Pa > Pb, ChouFasman.SecondaryStructures.AlphaHelix, ChouFasman.SecondaryStructures.BetaSheet)
            Call SetStructureType(HelixSheetOverlap, StructureType)
        End Sub

        Private Sub CalculateHelixTurnOverlap(HelixTurnOverlap As List(Of AminoAcid))
            If HelixTurnOverlap.Count = 0 Then
                Return
            End If

            Dim ChunkBuffer = (From Token In HelixTurnOverlap Select Token.AminoAcid).ToArray
            Dim Pa = Avg(ChunkBuffer, AddressOf ChouFasmanParameter.Get_Pa)
            Dim Pt = Avg(ChunkBuffer, AddressOf ChouFasmanParameter.Get_Pt)

            Dim StructureType = If(Pa > Pt, ChouFasman.SecondaryStructures.AlphaHelix, ChouFasman.SecondaryStructures.BetaTurn)
            Call SetStructureType(HelixTurnOverlap, StructureType)
        End Sub

        Private Sub CalculateSheetTurnOverlap(SheetTurnOverlap As List(Of AminoAcid))
            If SheetTurnOverlap.Count = 0 Then
                Return
            End If

            Dim ChunkBuffer = (From Token In SheetTurnOverlap Select Token.AminoAcid).ToArray
            Dim Pb = Avg(ChunkBuffer, AddressOf ChouFasmanParameter.Get_Pb)
            Dim Pt = Avg(ChunkBuffer, AddressOf ChouFasmanParameter.Get_Pt)

            Dim StructureType = If(Pb > Pt, ChouFasman.SecondaryStructures.BetaSheet, ChouFasman.SecondaryStructures.BetaTurn)
            Call SetStructureType(SheetTurnOverlap, StructureType)
        End Sub
    End Module
End Namespace