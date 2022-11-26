#Region "Microsoft.VisualBasic::aa895e93399e30f524e76e08cae61abc, GCModeller\analysis\SequenceToolkit\SNP\SangerSNPs\AlignmentMinusfile.vb"

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

    '   Total Lines: 132
    '    Code Lines: 89
    ' Comment Lines: 14
    '   Blank Lines: 29
    '     File Size: 5.75 KB


    '     Module SNPsAlignment
    ' 
    '         Sub: (+2 Overloads) DetectSNPs, (+2 Overloads) GetBasesForEachSNP
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language.C
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace SangerSNPs

    Public Module SNPsAlignment

        Public Sub GetBasesForEachSNP(ByRef filename As String, ByRef bases_for_snps As Char()(), ByRef snps As SNPsAln)
            Call New FastaFile(filename).GetBasesForEachSNP(bases_for_snps, snps)
        End Sub

        <Extension>
        Public Sub GetBasesForEachSNP(fasta As FastaFile, ByRef bases_for_snps As Char()(), ByRef snps As SNPsAln)
            Dim sequenceNumber As Integer = 0
            Dim lenOfgenomeFound As UInteger = 0

            For i As Integer = 0 To bases_for_snps.Length - 1
                bases_for_snps(i) = New Char(snps.length_of_genome - 1) {}
            Next

            For Each fa As FastaSeq In fasta
                Dim seq As Char() = fa.SequenceData.ToCharArray

                If sequenceNumber = 0 Then
                    lenOfgenomeFound = fa.Length
                End If

                For i As Integer = 0 To snps.number_of_snps - 1
                    bases_for_snps(sequenceNumber)(i) = Char.ToUpper(seq(snps.snp_locations(i)))
                Next

                If seq.Length <> lenOfgenomeFound Then
                    Dim msg As String = sprintf(UnEqualLength, fasta.FilePath, lenOfgenomeFound, seq.Length, fa.Title)
                    Throw New Exception(msg)
                End If

                sequenceNumber += 1
            Next
        End Sub

        Const UnEqualLength As String = "Alignment %s contains sequences of unequal length. Expected length is %i but got %i in sequence %s" & vbLf & vbLf

        ''' <summary>
        ''' Detection of the SNP sites based on a set of fasta sequence.
        ''' </summary>
        ''' <param name="filename">The input fasta sequence.</param>
        ''' <param name="pure_mode"></param>
        ''' <param name="output_monomorphic"></param>
        Public Sub DetectSNPs(ByRef filename As String, pure_mode As Integer, output_monomorphic As Integer, ByRef snps As SNPsAln)
            Call New FastaFile(filename).DetectSNPs(pure_mode, output_monomorphic, snps)
        End Sub

        ''' <summary>
        ''' Detection of the SNP sites based on a set of fasta sequence.
        ''' </summary>
        ''' <param name="fasta">The input fasta sequence.</param>
        ''' <param name="pure_mode"></param>
        ''' <param name="output_monomorphic"></param>
        ''' 
        <Extension>
        Public Sub DetectSNPs(fasta As FastaFile, pure_mode As Integer, output_monomorphic As Integer, ByRef snps As SNPsAln)
            Dim first_sequence As String = Nothing

            snps.number_of_snps = 0
            snps.number_of_samples = 0
            snps.length_of_genome = 0
            snps.sequence_names = New String(DefineConstants.DEFAULT_NUM_SAMPLES - 1) {}

            For Each fa As FastaSeq In fasta

                If snps.number_of_samples = 0 Then
                    snps.length_of_genome = fa.Length
                    Memset(first_sequence, "N"c, snps.length_of_genome)
                    Memset(snps.pseudo_reference_sequence, "N"c, snps.length_of_genome)
                End If

                Dim seq As Char() = fa.SequenceData.ToCharArray

                For i As Integer = 0 To snps.length_of_genome - 1
                    If first_sequence(i) = "#"c Then
                        Continue For
                    End If

                    If first_sequence(i) = "N"c AndAlso IsUnknown(fa.SequenceData(i)) = 0 Then
                        first_sequence = CString.ChangeCharacter(first_sequence, i, Char.ToUpper(seq(i)))
                        snps.pseudo_reference_sequence(i) = Char.ToUpper(seq(i))
                    End If

                    ' in pure mode we only want /ACGT/i, if any other base is found the whole column is excluded
                    If pure_mode <> 0 AndAlso IsPure(seq(i)) = 0 Then
                        first_sequence = CString.ChangeCharacter(first_sequence, i, "#"c)
                        Continue For
                    End If

                    If first_sequence(i) <> ">"c AndAlso IsUnknown(seq(i)) = 0 AndAlso first_sequence(i) <> "N"c AndAlso first_sequence(i) <> Char.ToUpper(seq(i)) Then
                        first_sequence = CString.ChangeCharacter(first_sequence, i, ">"c)
                    End If
                Next

                If snps.number_of_samples >= DefineConstants.DEFAULT_NUM_SAMPLES Then
                End If

                snps.sequence_names(snps.number_of_samples) = fa.Title
                snps.number_of_samples += 1
            Next

            For i = 0 To snps.length_of_genome - 1
                If first_sequence(i) = ">"c OrElse (output_monomorphic <> 0 AndAlso first_sequence(i) <> "#"c) Then
                    snps.number_of_snps += 1
                End If
            Next

            If snps.number_of_snps = 0 Then
                Throw New Exception(NoSNPs)
            End If

            Dim current_snp_index As Integer = 0

            snps.snp_locations = New Int32(snps.length_of_genome - 1) {}

            For i As Integer = 0 To snps.length_of_genome - 1
                If first_sequence(i) = ">"c OrElse (output_monomorphic <> 0 AndAlso first_sequence(i) <> "#"c) Then
                    snps.snp_locations(current_snp_index) = i
                    current_snp_index += 1
                End If
            Next
        End Sub

        Const NoSNPs As String = "Warning: No SNPs were detected so there is nothing to output."
    End Module
End Namespace
