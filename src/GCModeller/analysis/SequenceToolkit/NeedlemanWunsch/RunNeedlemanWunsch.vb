Imports Microsoft.VisualBasic
Imports System
Imports LANS.SystemsBiology.SequenceModel

''' <summary>
''' Application of the Needleman-Wunsch Algorithm
''' Bioinformatics 1, WS 15/16
''' Dr. Kay Nieselt and Alexander Seitz
''' 
''' * Benjamin Schroeder
''' * Jonas Ditz
''' 
''' </summary>
Public Module RunNeedlemanWunsch

    ''' <summary>
    ''' Run the Needleman-Wunsch Algorithm </summary>
    ''' <param name="args"> commandline arguments </param>
    ''' <exception cref="Exception"> </exception>
    Public Sub RunAlign(fasta1 As FASTA.FastaToken, fasta2 As FASTA.FastaToken, [single] As Boolean, outfile As String)
        Dim nw As New NeedlemanWunsch(fasta1.SequenceData, fasta2.SequenceData)

        ' display input
        Console.WriteLine("Input:")
        Console.WriteLine(vbTab & "seq1 = " & nw.Query)
        Console.WriteLine(vbTab & "seq2 = " & nw.Subject)
        Console.WriteLine()

        ' run algorithm
        nw.compute()

        If [single] Then
            If outfile Is Nothing Then
                Console.WriteLine("Alignment :")
                Console.WriteLine(vbTab & "aligned1 = " & nw.getAligned1(0))
                Console.WriteLine(vbTab & "aligned2 = " & nw.getAligned2(0))
                Console.WriteLine("--------------------------------")
                Console.WriteLine("Alignment-Score = " & nw.Score)
            Else
                nw.writeAlignment(outfile, True)
            End If
        Else
            If outfile Is Nothing Then
                ' display all possible optimal alignments
                For i As Integer = 0 To nw.NumberOfAlignments - 1
                    Console.WriteLine("Alignment " & (i + 1) & ":")
                    Console.WriteLine(vbTab & "aligned1 = " & nw.getAligned1(i))
                    Console.WriteLine(vbTab & "aligned2 = " & nw.getAligned2(i))
                Next

                Console.WriteLine("--------------------------------")
                Console.WriteLine("Alignment-Score = " & nw.Score)
            Else
                nw.writeAlignment(outfile, False)
            End If
        End If
    End Sub
End Module