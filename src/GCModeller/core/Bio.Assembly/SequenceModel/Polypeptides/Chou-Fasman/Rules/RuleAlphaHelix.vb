Namespace SequenceModel.Polypeptides.SecondaryStructure.ChouFasmanRules

    Public Module RuleAlphaHelix

        ''' <summary>
        ''' Invoke calculation
        ''' </summary>
        ''' <param name="SequenceData"></param>
        ''' <remarks></remarks>
        Public Function Invoke(SequenceData As AminoAcid()) As Integer
            Dim SequenceEnums As SequenceModel.Polypeptides.Polypeptides.AminoAcid() = (From token In SequenceData Select token.AminoAcid).ToArray
            Dim ChunkBuffer As SequenceModel.Polypeptides.Polypeptides.AminoAcid() = New SequenceModel.Polypeptides.Polypeptides.AminoAcid(CORE_LENGTH - 1) {}
            Dim Count_a As Integer = 0
            Dim pp As Integer

            For i As Integer = 0 To SequenceEnums.Count - (CORE_LENGTH + 1)

                Call Array.ConstrainedCopy(SequenceEnums, i, ChunkBuffer, 0, CORE_LENGTH)

                If CalculateCore(ChunkBuffer) Then '计算核心
                    Dim Segment = ExtendCore(SequenceEnums, i, CORE_LENGTH) '延伸核心
                    If Segment.FragmentSize > 5 Then
                        Dim TempChunk As SequenceModel.Polypeptides.Polypeptides.AminoAcid() = New SequenceModel.Polypeptides.Polypeptides.AminoAcid(Segment.FragmentSize - 1) {}
                        Call Array.ConstrainedCopy(SequenceEnums, Segment.Left, TempChunk, 0, Segment.FragmentSize)
                        If Avg(TempChunk, AddressOf ChouFasmanParameter.Get_Pa) > Avg(TempChunk, AddressOf ChouFasmanParameter.Get_Pb) Then
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

        Private Function ExtendCore(SequenceData As SequenceModel.Polypeptides.Polypeptides.AminoAcid(), i As Integer, Length As Integer) As LANS.SystemsBiology.ComponentModel.Loci.Location
            Dim ChunkBuffer As SequenceModel.Polypeptides.Polypeptides.AminoAcid() = New SequenceModel.Polypeptides.Polypeptides.AminoAcid(Length - 1) {}
            Dim Left, Right As Integer

            '向左端延伸
            For p As Integer = i To 0 Step -1
                Call Array.ConstrainedCopy(SequenceData, p, ChunkBuffer, 0, Length)
                Dim Pa_avg As Double = Avg(ChunkBuffer, AddressOf ChouFasmanParameter.Get_Pa)
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
                Dim Pa_avg As Double = Avg(ChunkBuffer, AddressOf ChouFasmanParameter.Get_Pa)
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

            Return New LANS.SystemsBiology.ComponentModel.Loci.Location With {.Left = Left, .Right = Right}
        End Function

        Private Function get_ProPosition(ChunkBuffer As SequenceModel.Polypeptides.Polypeptides.AminoAcid(), pp As Func(Of Integer, Integer)) As Integer()
            Dim p_Pro = (From i_Pro As Integer In ChunkBuffer.Sequence Let Token As SequenceModel.Polypeptides.Polypeptides.AminoAcid = ChunkBuffer(i_Pro)
                         Where Token = SequenceModel.Polypeptides.Polypeptides.AminoAcid.Praline Select pp(i_Pro)).ToArray
            Return p_Pro
        End Function

        Private Function CalculateCore(ChunkBuffer As SequenceModel.Polypeptides.Polypeptides.AminoAcid()) As Boolean
            Dim LQuery = (From token In ChunkBuffer Let p = ChouFasmanTable(token).P_a Where p > 100 Select 1).ToArray
            Return (LQuery.Count / ChunkBuffer.Count) > PROPORTION
        End Function
    End Module
End Namespace