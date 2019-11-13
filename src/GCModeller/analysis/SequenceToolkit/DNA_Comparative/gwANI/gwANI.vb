#Region "Microsoft.VisualBasic::b245a2a07def648a89d99a8bdafa2ec5, analysis\SequenceToolkit\DNA_Comparative\gwANI\gwANI.vb"

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

    '     Class gwANI
    ' 
    '         Properties: length_of_genome, number_of_samples, sequence_names
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: __calculate_and_output_gwani, __fast_calculate_gwani, calc_gwani_between_a_sample_and_everything_afterwards, calc_gwani_between_a_sample_and_everything_afterwards_memory, calculate_and_output_gwani
    '              check_input_file_and_calc_dimensions, Evaluate, fast_calculate_gwani, print_header
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Terminal.STDIO
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

Namespace gwANI

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

        Private Sub New(file As TextWriter)
            out = file
            If out Is Nothing Then
                out = Console.Out
            End If
        End Sub

        ''' <summary>
        ''' The result output stream
        ''' </summary>
        ReadOnly out As TextWriter

        Public ReadOnly Property length_of_genome As Integer
        Public ReadOnly Property number_of_samples As Integer
        Public ReadOnly Property sequence_names As String()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Sub Evaluate(in$, out$, fast As Boolean)
            Call gwANIExtensions.Evaluate([in], out, fast)
        End Sub

        Private Sub check_input_file_and_calc_dimensions(ByRef filename As String)
            _number_of_samples = 0
            _length_of_genome = 0
            _sequence_names = New String(DefineConstants.DEFAULT_NUM_SAMPLES - 1) {}

            For Each seq As FastaSeq In New FastaFile(filename)
                If number_of_samples = 0 Then
                    _length_of_genome = seq.Length
                ElseIf length_of_genome <> seq.Length Then
                    printf("Alignment %s contains sequences of unequal length. Expected length is %i but got %i in sequence %s \n\n", filename, length_of_genome, seq.Length, seq.Title)
                    App.Exit(-1)
                End If

                ' The sample name is initially set to a large number but make sure this can be increased dynamically

                If number_of_samples >= DefineConstants.DEFAULT_NUM_SAMPLES Then
                End If

                sequence_names(number_of_samples) = seq.Title
                _number_of_samples += 1
            Next    ' First pass of the file get the length of the alignment, number of samples and sample names
        End Sub

        ''' <summary>
        ''' 执行入口点
        ''' </summary>
        ''' <param name="filename"></param>
        ''' <param name="out">默认是打印在终端之上</param>
        Public Shared Sub calculate_and_output_gwani(ByRef filename As String, Optional out As TextWriter = Nothing)
            Call New gwANI(out).__calculate_and_output_gwani(filename)
        End Sub

        Private Sub __calculate_and_output_gwani(ByRef filename As String)
            Call check_input_file_and_calc_dimensions(filename)
            Call print_header()

            For i As Integer = 0 To number_of_samples - 1
                Dim similarity_percentage As Double() = New Double(number_of_samples) {}

                out.Write("{0}", sequence_names(i))
                calc_gwani_between_a_sample_and_everything_afterwards(filename, i, similarity_percentage)

                For j As Integer = 0 To number_of_samples - 1
                    If similarity_percentage(j) < 0 Then
                        out.Write(vbTab & "-")
                    Else
                        out.Write(vbTab & "{0:f}", similarity_percentage(j))
                    End If
                Next
                out.Write(vbLf)
            Next
        End Sub

        Private Sub print_header()
            Dim i As Integer
            For i = 0 To number_of_samples - 1
                Call out.Write(vbTab & "{0}", sequence_names(i))
            Next
            Call out.WriteLine()
        End Sub

        Private Sub calc_gwani_between_a_sample_and_everything_afterwards(ByRef filename As String, comparison_index As Integer, similarity_percentage As Double())
            Dim current_index As Integer = 0
            Dim bases_in_common As Integer
            Dim length_without_gaps As Integer
            Dim comparison_sequence As String = New String("-"c, length_of_genome + 1)

            For Each seq As FastaSeq In New FastaFile(filename)

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
        ''' <param name="filename"></param>
        ''' <param name="out">默认是打印在终端之上</param>
        Public Shared Sub fast_calculate_gwani(ByRef filename As String, Optional out As TextWriter = Nothing)
            Call New gwANI(out).__fast_calculate_gwani(filename)
        End Sub

        ''' <summary>
        ''' 执行入口点
        ''' </summary>
        ''' <param name="filename"></param>
        Private Sub __fast_calculate_gwani(ByRef filename As String)
            Call check_input_file_and_calc_dimensions(filename)
            Call print_header()

            ' initialise space to store entire genome
            Dim i As Integer
            Dim j As Integer
            Dim comparison_sequence As Char()() = New Char(number_of_samples())() {}

            ' Store all sequences in a giant array - eek

            For Each seq As FastaSeq In New FastaFile(filename)
                comparison_sequence(i) = seq.SequenceData
                i += 1
            Next

            For j = 0 To number_of_samples - 1
                For i = 0 To length_of_genome - 1
                    'standardise the input so that case doesnt matter and replace unknowns with single type
                    comparison_sequence(j)(i) = Char.ToUpper(comparison_sequence(j)(i))
                    If IsUnknown(comparison_sequence(j)(i)) <> 0 Then
                        comparison_sequence(j)(i) = "N"c
                    End If
                Next
            Next

            For i = 0 To number_of_samples - 1

                out.Write("{0}", sequence_names(i))

                Dim similarity_percentage As Double() = New Double(number_of_samples) {}

                Call calc_gwani_between_a_sample_and_everything_afterwards_memory(comparison_sequence, i, similarity_percentage)

                For j = 0 To number_of_samples - 1
                    If similarity_percentage(j) < 0 Then
                        out.Write(vbTab & "-")
                    Else
                        out.Write(vbTab & "{0:f}", similarity_percentage(j))
                    End If
                Next
                out.Write(vbLf)
            Next
        End Sub
    End Class
End Namespace
