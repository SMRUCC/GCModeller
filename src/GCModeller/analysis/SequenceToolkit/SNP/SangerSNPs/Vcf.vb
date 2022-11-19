#Region "Microsoft.VisualBasic::a80994a363061bb3c82762fe17a31095, GCModeller\analysis\SequenceToolkit\SNP\SangerSNPs\Vcf.vb"

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

    '   Total Lines: 248
    '    Code Lines: 151
    ' Comment Lines: 49
    '   Blank Lines: 48
    '     File Size: 10.40 KB


    '     Module Vcf
    ' 
    '         Function: __alternativeBases, __checkIfBaseInAlts, FormatAlleleIndex, FormatAlternativeBases
    ' 
    '         Sub: __outputVcfHeader, __outputVcfRowSamplesBases, __outputVcfsnps, create_vcf_file, output_vcf_row
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Diagnostics
Imports System.IO
Imports FILE = System.IO.StreamWriter

Namespace SangerSNPs

    Public Module Vcf

        '
        '	 *  Wellcome Trust Sanger Institute
        '	 *  Copyright (C) 2013  Wellcome Trust Sanger Institute
        '	 *  
        '	 *  This program is free software; you can redistribute it and/or
        '	 *  modify it under the terms of the GNU General Public License
        '	 *  as published by the Free Software Foundation; either version 3
        '	 *  of the License, or (at your option) any later version.
        '	 *  
        '	 *  This program is distributed in the hope that it will be useful,
        '	 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
        '	 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
        '	 *  GNU General Public License for more details.
        '	 *  
        '	 *  You should have received a copy of the GNU General Public License
        '	 *  along with this program; if not, write to the Free Software
        '	 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
        '	 

        Private Sub __outputVcfHeader(file As FILE,
                                  ByRef sequence_names As String(),
                                  number_of_samples As Integer,
                                  length_of_genome As UInteger)

            Call file.Write("##fileformat=VCFv4.1" & vbLf)
            Call file.Write("##contig=<ID=1,length=%i>" & vbLf, CInt(length_of_genome))
            Call file.Write("##FORMAT=<ID=GT,Number=1,Type=String,Description=""Genotype"">" & vbLf)
            Call file.Write("#CHROM" & vbTab & "POS" & vbTab & "ID" & vbTab & "REF" & vbTab & "ALT" & vbTab & "QUAL" & vbTab & "FILTER" & vbTab & "INFO" & vbTab & "FORMAT")

            For i As Integer = 0 To number_of_samples - 1
                Call file.Write(vbTab & sequence_names(i))
            Next

            Call file.Write(vbLf)
        End Sub

        Public Sub create_vcf_file(ByRef filename As String,
                               snp_locations As Integer(),
                               number_of_snps As Integer,
                               ByRef bases_for_snps As String(),
                               ByRef sequence_names As String(),
                               number_of_samples As Integer,
                               length_of_genome As UInteger,
                               ByRef pseudo_reference_sequence As String)

            Using vcf_file_pointer As FILE = New StreamWriter(New FileStream(filename, FileMode.OpenOrCreate))
                Vcf.__outputVcfHeader(vcf_file_pointer, sequence_names, number_of_samples, CInt(length_of_genome))
                Vcf.__outputVcfsnps(vcf_file_pointer, bases_for_snps, snp_locations, number_of_snps, number_of_samples, pseudo_reference_sequence)
            End Using
        End Sub

        Private Sub __outputVcfsnps(file As FILE,
                                ByRef bases_for_snps As String(),
                                snp_locations As Integer(),
                                number_of_snps As Integer,
                                number_of_samples As Integer,
                                ByRef pseudo_reference_sequence As String)

            For i As Integer = 0 To number_of_snps - 1
                Vcf.output_vcf_row(file, bases_for_snps(i), snp_locations(i), number_of_samples, pseudo_reference_sequence)
            Next
        End Sub

        Public Sub output_vcf_row(vcf_file_pointer As FILE,
                              ByRef bases_for_snp As String,
                              snp_location As Integer,
                              number_of_samples As Integer,
                              ByRef pseudo_reference_sequence As String)

            Dim reference_base As Char = pseudo_reference_sequence(snp_location)

            If reference_base = ControlChars.NullChar Then
                Throw New Exception("Couldnt get the reference base for coordinate " & snp_location)
            End If

            ' Chromosome
            vcf_file_pointer.Write("1" & vbTab)

            ' Position
            vcf_file_pointer.Write((CInt(snp_location) + 1) & vbTab)

            'ID
            vcf_file_pointer.Write("." & vbTab)

            ' REF
            vcf_file_pointer.Write(reference_base & vbTab)

            ' ALT
            ' Need to look through list and find unique characters


            'ORIGINAL LINE: sbyte * alt_bases = alternative_bases(reference_base, bases_for_snp, number_of_samples);
            Dim alt_bases As Char = Vcf.__alternativeBases(reference_base, bases_for_snp, number_of_samples)

            'ORIGINAL LINE: sbyte * alternative_bases_string = format_alternative_bases(alt_bases);
            Dim alternative_bases_string As Char = Vcf.FormatAlternativeBases(alt_bases)

            vcf_file_pointer.Write(alternative_bases_string & vbTab)

            ' QUAL
            vcf_file_pointer.Write("." & vbTab)

            ' FILTER
            vcf_file_pointer.Write("." & vbTab)

            ' INFO
            vcf_file_pointer.Write("." & vbTab)

            ' FORMAT
            vcf_file_pointer.Write("GT" & vbTab)

            ' Bases for each sample
            Vcf.__outputVcfRowSamplesBases(vcf_file_pointer, reference_base, alt_bases, bases_for_snp, number_of_samples)

            vcf_file_pointer.Write(vbLf)
        End Sub

        Private Sub __outputVcfRowSamplesBases(vcf_file_pointer As StreamWriter,
                                           reference_base As Char,
                                           ByRef alt_bases As String,
                                           ByRef bases_for_snp As String,
                                           number_of_samples As Integer)
            Dim format As String

            For i As Integer = 0 To number_of_samples - 1
                format = Vcf.FormatAlleleIndex(bases_for_snp(i), reference_base, alt_bases)
                vcf_file_pointer.Write(format)

                If i + 1 <> number_of_samples Then
                    vcf_file_pointer.Write(vbTab)
                End If
            Next
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="reference_base"></param>
        ''' <param name="bases_for_snp"></param>
        ''' <param name="number_of_samples">序列的数量</param>
        ''' <returns></returns>
        Private Function __alternativeBases(reference_base As Char, ByRef bases_for_snp As String, number_of_samples As Integer) As String
            Dim num_alt_bases As Integer = 0
            Dim alt_bases As Char() = New Char(MAXIMUM_NUMBER_OF_ALT_BASES) {}

            For i As Integer = 0 To number_of_samples - 1
                Dim current_base As Char = bases_for_snp(i)
                If current_base <> reference_base Then
                    If IsUnknown(current_base) <> 0 Then
                        ' VCF spec only allows ACGTN* for alts
                        current_base = "*"c
                    End If

                    If Vcf.__checkIfBaseInAlts(alt_bases, current_base, num_alt_bases) = 0 Then
                        If num_alt_bases >= MAXIMUM_NUMBER_OF_ALT_BASES Then
                            Throw New Exception(AltBasesNotFound)
                        Else
                            alt_bases(num_alt_bases) = current_base
                            num_alt_bases += 1
                        End If
                    End If
                End If
            Next

            alt_bases(num_alt_bases) = ControlChars.NullChar

            Return alt_bases
        End Function

        ''' <summary>
        ''' Unexpectedly large number Of alternative bases found between sequences.  Please check input file Is Not corrupted
        ''' </summary>
        Const AltBasesNotFound As String = "Unexpectedly large number Of alternative bases found between sequences.  Please check input file Is Not corrupted"

        ''' <summary>
        ''' 检查目标残基是否在可变的残基列表之中
        ''' </summary>
        ''' <param name="altBases"></param>
        ''' <param name="base"></param>
        ''' <param name="numAltBases"></param>
        ''' <returns></returns>
        Private Function __checkIfBaseInAlts(ByRef altBases As String, base As Char, numAltBases As Integer) As Integer
            Dim i As Integer
            For i = 0 To numAltBases - 1
                If altBases(i) = base Then
                    Return 1
                End If
            Next
            Return 0
        End Function

        Private Function FormatAlternativeBases(ByRef alt_bases As String) As String
            Dim number_of_alt_bases As Integer = alt_bases.Length
            Dim formatted_alt_bases As Char()

            Debug.Assert(number_of_alt_bases < DefineConstants.MAXIMUM_NUMBER_OF_ALT_BASES)

            If number_of_alt_bases = 0 Then
                formatted_alt_bases = New Char(2) {}
                formatted_alt_bases(0) = "."c
                Return formatted_alt_bases
            Else
                formatted_alt_bases = New Char(number_of_alt_bases * 2) {}
                formatted_alt_bases(0) = alt_bases(0)

                For i As Integer = 1 To number_of_alt_bases - 1
                    formatted_alt_bases(i * 2 - 1) = ","c
                    formatted_alt_bases(i * 2) = alt_bases(i)
                Next

                Return formatted_alt_bases
            End If
        End Function

        Private Function FormatAlleleIndex(base As Char, refBase As Char, ByRef altBases As String) As String
            Dim length_of_alt_bases As Integer = altBases.Length
            Dim result As String
            Dim index As Integer

            Call Debug.Assert(length_of_alt_bases < 100)

            If base.IsUnknown <> 0 Then
                base = "*"c
            End If

            If refBase = base Then
                result = "0"
            Else
                result = "."
                For index = 1 To length_of_alt_bases
                    If altBases(index - 1) = base Then
                        result = String.Format("{0:D}", index)
                        Exit For
                    End If
                Next
            End If
            Return result
        End Function
    End Module
End Namespace
