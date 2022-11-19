#Region "Microsoft.VisualBasic::39f368659d793f989c13eab0a8bd0772, GCModeller\analysis\SequenceToolkit\gwANI\gwANI.vb"

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

    '   Total Lines: 284
    '    Code Lines: 140
    ' Comment Lines: 108
    '   Blank Lines: 36
    '     File Size: 10.96 KB


    ' Class gwANI
    ' 
    '     Function: __calculate_and_output_gwani, __fast_calculate_gwani
    ' 
    '     Sub: calc_gwani_between_a_sample_and_everything_afterwards, calc_gwani_between_a_sample_and_everything_afterwards_memory, check_input_file_and_calc_dimensions, Evaluate
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.STDIO
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language.C
Imports SMRUCC.genomics.SequenceModel.FASTA

'     *  Wellcome Trust Sanger Institute
'     *  Copyright (C) 2016  Wellcome Trust Sanger Institute
'     *  
'     *  This program is free software; you can redistribute it and/or
'     *  modify it under the terms of the GNU General Public License
'     *  as published by the Free Software Foundation; either version 3
'     *  of the License, or (at your option) any later version.
'     *  
'     *  This program is distributed in the hope that it will be useful,
'     *  but WITHOUT ANY WARRANTY; without even the implied warranty of
'     *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'     *  GNU General Public License for more details.
'     *  
'     *  You should have received a copy of the GNU General Public License
'     *  along with this program; if not, write to the Free Software
'     *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

''' <summary>
''' ### pANIto
''' Given a multi-FASTA alignment, output the genome wide average nucleotide identity (gwANI) 
''' For Each sample against all other samples. A matrix containing the percentages Is outputted. 
''' This software loads the whole file into memory.
'''
''' #### Usage
''' ```
''' $ panito
''' Usage: panito [-hV] &lt;file>
''' This program calculates the genome wide ANI for a multiFASTA alignment.
'''   -h          this help message
'''   -V          print version and exit
'''   &lt;file>   input alignment file which can optionally be gzipped
''' ```
'''
''' #### Input format
''' The input file must be a multi-FASTA file, where all sequences are the same length:
'''
''' ```
''' >sample1
''' AAAAAAAAAA
''' >sample2
''' AAAAAAAAAC
''' >sample3
''' AAAAAAAACC
''' ```
'''
''' #### Output
''' ```
'''         sample1	    sample2	    sample3
''' sample1	100.000000	90.00000	80.000000
''' sample2	-			100.000000	90.000000
''' sample3	-			-			100.000000
''' ```
'''
''' #### Etymology
''' pANIto has 'ani' in the middle. In Spanish it means babylon.
''' </summary>
''' <remarks>
''' ```vbnet
''' 
''' Public Sub print_version()
'''     Call Console.Write("{0} {1}" &amp; vbLf, DefineConstants.PROGRAM_NAME, PACKAGE_VERSION)
''' End Sub
'''
''' Public Sub Main(argc As Integer, args As String())
'''     Dim multi_fasta_filename As String() = {""}
'''     Dim output_filename As String() = {""}
'''
'''     Dim c As Integer
'''     Dim index As Integer
'''     Dim output_multi_fasta_file As Integer = 0
'''
'''     While (InlineAssignHelper(c, getopt(argc, args, "ho:V"))) &lt;> -1
'''         Select Case c
'''             Case "V"c
'''                 GlobalMembersMain.print_version()
'''            
'''             Case "o"c
'''                 output_filename = optarg.Substring(0, FILENAME_MAX)
'''                  
'''             Case "h"c
'''                 GlobalMembersMain.print_usage()
'''                 
'''             Case Else
'''                 output_multi_fasta_file = 1
'''                   
'''         End Select
'''     End While
'''
'''     If optind &lt; argc Then
'''         multi_fasta_filename = Convert.ToString(args(optind)).Substring(0, FILENAME_MAX)
'''         gwANI.calculate_and_output_gwani(multi_fasta_filename);
'''         gwANI.fast_calculate_gwani(multi_fasta_filename)
'''     Else
'''         Call print_usage()
'''     End If
''' End Sub
''' ```
''' </remarks>
Public NotInheritable Class gwANI

    Dim length_of_genome As Integer
    Dim number_of_samples As Integer
    Dim sequence_names As String()

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub Evaluate(in$, out$, fast As Boolean)
        Call gwANIExtensions.Evaluate([in], out, fast)
    End Sub

    Private Sub check_input_file_and_calc_dimensions(ByRef multipleSeq As FastaFile)
        number_of_samples = 0
        length_of_genome = 0
        sequence_names = New String(DefineConstants.DEFAULT_NUM_SAMPLES - 1) {}

        For Each seq As FastaSeq In multipleSeq
            If number_of_samples = 0 Then
                length_of_genome = seq.Length
            ElseIf length_of_genome <> seq.Length Then
                printf("Alignment contains sequences of unequal length. Expected length is %i but got %i in sequence %s \n\n", length_of_genome, seq.Length, seq.Title)
                App.Exit(-1)
            End If

            ' The sample name is initially set to a large number but make sure this can be increased dynamically
            If number_of_samples >= DefineConstants.DEFAULT_NUM_SAMPLES Then
            End If

            ' First pass of the file get the length of the alignment, number of samples and sample names
            sequence_names(number_of_samples) = seq.Title
            number_of_samples += 1
        Next
    End Sub

    Friend Iterator Function __calculate_and_output_gwani(multipleSeq As FastaFile) As IEnumerable(Of DataSet)
        Dim id As String
        Dim seq As DataSet
        Dim similarity_percentage As Double()

        Call check_input_file_and_calc_dimensions(multipleSeq)

        For i As Integer = 0 To number_of_samples - 1
            similarity_percentage = New Double(number_of_samples) {}
            id = sequence_names(i)
            seq = New DataSet With {.ID = id}

            calc_gwani_between_a_sample_and_everything_afterwards(multipleSeq, i, similarity_percentage)

            For j As Integer = 0 To number_of_samples - 1
                Call seq.Add(sequence_names(j), similarity_percentage(j))
            Next

            Yield seq
        Next
    End Function

    Private Sub calc_gwani_between_a_sample_and_everything_afterwards(ByRef multipleSeq As FastaFile, comparison_index As Integer, similarity_percentage As Double())
        Dim current_index As Integer = 0
        Dim bases_in_common As Integer
        Dim length_without_gaps As Integer
        Dim comparison_sequence As String = New String("-"c, length_of_genome + 1)

        For Each seq As FastaSeq In multipleSeq
            If current_index < comparison_index Then
                similarity_percentage(current_index) = -1
            ElseIf current_index = comparison_index Then
                similarity_percentage(current_index) = 100
                For i As Integer = 0 To length_of_genome - 1
                    'standardise the input so that case doesnt matter and replace unknowns with single type
                    comparison_sequence = ChangeCharacter(comparison_sequence, i, Char.ToUpper(seq.SequenceData(i)))
                    If IsUnknown(comparison_sequence(i)) <> 0 Then
                        comparison_sequence = ChangeCharacter(comparison_sequence, i, "N"c)
                    End If
                Next
            Else
                bases_in_common = 0
                length_without_gaps = length_of_genome
                For i As Integer = 0 To length_of_genome - 1

                    If comparison_sequence(i) = "N"c OrElse IsUnknown(seq.SequenceData(i)) <> 0 Then
                        length_without_gaps -= 1
                    ElseIf comparison_sequence(i) = Char.ToUpper(seq.SequenceData(i)) AndAlso IsUnknown(seq.SequenceData(i)) = 0 Then
                        bases_in_common += 1
                    End If
                Next
                If length_without_gaps > 0 Then
                    similarity_percentage(current_index) = (bases_in_common * 100.0) / length_without_gaps
                Else
                    similarity_percentage(current_index) = 0

                End If
            End If

            current_index += 1
        Next
    End Sub

    Private Sub calc_gwani_between_a_sample_and_everything_afterwards_memory(ByRef comparison_sequence As Char()(), comparison_index As Integer, similarity_percentage As Double())
        Dim current_index As Integer = 0
        Dim bases_in_common As Integer
        Dim length_without_gaps As Integer

        For j As Integer = 0 To number_of_samples - 1

            If current_index < comparison_index Then
                similarity_percentage(current_index) = -1
            ElseIf current_index = comparison_index Then
                similarity_percentage(current_index) = 100
            Else
                bases_in_common = 0
                length_without_gaps = length_of_genome

                For i As Integer = 0 To length_of_genome - 1

                    If comparison_sequence(comparison_index)(i) = "N"c OrElse comparison_sequence(j)(i) = "N"c Then
                        length_without_gaps -= 1
                    ElseIf comparison_sequence(comparison_index)(i) = comparison_sequence(j)(i) Then
                        bases_in_common += 1
                    End If
                Next

                If length_without_gaps > 0 Then
                    similarity_percentage(current_index) = (bases_in_common * 100.0) / length_without_gaps
                Else
                    similarity_percentage(current_index) = 0
                End If
            End If

            current_index += 1
        Next
    End Sub

    ''' <summary>
    ''' 执行入口点
    ''' </summary>
    ''' <param name="multipleSeq"></param>
    Friend Iterator Function __fast_calculate_gwani(multipleSeq As FastaFile) As IEnumerable(Of DataSet)
        Call check_input_file_and_calc_dimensions(multipleSeq)

        ' initialise space to store entire genome
        Dim i As Integer
        Dim j As Integer
        Dim comparison_sequence As Char()() = New Char(number_of_samples)() {}

        ' store all sequences in a giant array - eek
        For Each seq As FastaSeq In multipleSeq
            comparison_sequence(i) = seq.SequenceData.ToArray
            i += 1
        Next

        For j = 0 To number_of_samples - 1
            For i = 0 To length_of_genome - 1
                ' standardise the input so that case doesnt matter 
                ' and replace unknowns with single type
                comparison_sequence(j)(i) = Char.ToUpper(comparison_sequence(j)(i))

                If IsUnknown(comparison_sequence(j)(i)) <> 0 Then
                    comparison_sequence(j)(i) = "N"c
                End If
            Next
        Next

        Dim similarity_percentage As Double()
        Dim seqRow As DataSet

        For i = 0 To number_of_samples - 1
            seqRow = New DataSet With {
                .ID = sequence_names(i)
            }

            similarity_percentage = New Double(number_of_samples) {}

            Call calc_gwani_between_a_sample_and_everything_afterwards_memory(comparison_sequence, i, similarity_percentage)

            For j = 0 To number_of_samples - 1
                Call seqRow.Add(sequence_names(j), similarity_percentage(j))
            Next
        Next
    End Function
End Class
