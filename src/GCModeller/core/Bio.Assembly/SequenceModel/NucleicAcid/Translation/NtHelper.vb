Imports System.Runtime.CompilerServices

Namespace SequenceModel.NucleotideModels.Translation

    Module NtHelper

        ''' <summary>
        ''' Check nt sequence direction by start and stop codon
        ''' </summary>
        ''' <param name="sequence"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function DoCheckNtDirection(translTable As TranslTable, sequence As String, ByRef operations As String()) As String
            Dim first As String = Mid(sequence, 1, 3)
            Dim last As String = Mid(sequence, Len(sequence) - 3)

            If translTable.IsInitCoden(first) Then
                ' 正常的序列
                Return sequence
            End If

            Dim lastAsInit As String = New String(last.Reverse.ToArray)

            If translTable.IsInitCoden(lastAsInit) Then
                ' 方向可能颠倒了
                operations = {"reverse"}
                Return New String(sequence.Reverse.ToArray)
            End If

            first = NucleicAcid.Complement(first)

            If translTable.IsInitCoden(first) Then
                ' 互补的序列
                operations = {"complement"}
                Return NucleicAcid.Complement(sequence)
            End If

            lastAsInit = NucleicAcid.Complement(lastAsInit)

            If translTable.IsInitCoden(lastAsInit) Then
                ' 方向可能颠倒了
                operations = {"reverse", "complement"}
                Return New String(NucleicAcid.Complement(sequence).Reverse.ToArray)
            End If

            ' 实在判断不出来了，只能够硬着头皮翻译下去了 
            operations = {"invalid"}
            Return sequence
        End Function
    End Module
End Namespace