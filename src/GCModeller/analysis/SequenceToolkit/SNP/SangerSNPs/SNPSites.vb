#Region "Microsoft.VisualBasic::37db59be334a3f8919d588c72a1e3b00, GCModeller\analysis\SequenceToolkit\SNP\SangerSNPs\SNPSites.vb"

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

    '   Total Lines: 182
    '    Code Lines: 127
    ' Comment Lines: 33
    '   Blank Lines: 22
    '     File Size: 8.83 KB


    '     Module SNPSites
    ' 
    '         Function: __snpSitesGeneric, OutputSNPSites, OutputSNPSitesWithRef, OutputSNPSitesWithRefPureMono, SNPSitesGeneric
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

Namespace SangerSNPs

    ''' <summary>
    ''' SNP functions entry point at here
    ''' </summary>
    Public Module SNPSites

        Private Function __snpSitesGeneric(ByRef filename As String,
                                       output_multi_fasta_file As Integer,
                                       output_vcf_file As Integer,
                                       output_phylip_file As Integer,
                                       ByRef output_filename As String,
                                       output_reference As Integer,
                                       pure_mode As Integer,
                                       output_monomorphic As Integer) As SNPsAln

            Return New FastaFile(filename).SNPSitesGeneric(
                output_multi_fasta_file,
                output_vcf_file,
                output_phylip_file,
                output_filename,
                output_reference,
                pure_mode,
                output_monomorphic)
        End Function

        <Extension>
        Public Function SNPSitesGeneric(fasta As FastaFile,
                                        output_multi_fasta_file As Integer,
                                        output_vcf_file As Integer,
                                        output_phylip_file As Integer,
                                        ByRef output_filename As String,
                                        output_reference As Integer,
                                        pure_mode As Integer,
                                        output_monomorphic As Integer,
                                        Optional ByRef vcf_output_filename$ = Nothing) As SNPsAln

            Dim bases_for_snps As Char()() = New Char(fasta.Count - 1)() {}
            Dim args As New SNPsAln

            SNPsAlignment.DetectSNPs(fasta, pure_mode, output_monomorphic, args)
            SNPsAlignment.GetBasesForEachSNP(fasta, bases_for_snps, args)

            Dim SNPsBases As String() = bases_for_snps.MatrixTranspose.Select(Function(x) New String(x)).ToArray
            Dim output_filename_base As String = fasta.FilePath

            If output_vcf_file <> 0 Then
                vcf_output_filename = output_filename_base.TrimSuffix
                If (output_vcf_file + output_phylip_file + output_multi_fasta_file) > 1 OrElse (output_filename Is Nothing OrElse output_filename = ControlChars.NullChar) Then
                    vcf_output_filename += ".vcf"
                End If

                Vcf.create_vcf_file(vcf_output_filename,
                                    args.snp_locations,
                                    args.number_of_snps,
                                    SNPsBases,
                                    args.sequence_names,
                                    args.number_of_samples,
                                    args.length_of_genome,
                                    args.pseudo_reference_sequence)
            End If


            If output_phylip_file <> 0 Then
                Dim phylip_output_filename As New String(New Char(FILENAME_MAX - 1) {})
                phylip_output_filename = output_filename_base.TrimSuffix
                If (output_vcf_file + output_phylip_file + output_multi_fasta_file) > 1 OrElse (output_filename Is Nothing OrElse output_filename = ControlChars.NullChar) Then
                    phylip_output_filename += ".phylip"
                End If

                SNPsPhylib.PhylibOfSNPSites(
                    args.number_of_snps,
                    SNPsBases,
                    args.sequence_names,
                    args.number_of_samples,
                    output_reference,
                    args.pseudo_reference_sequence,
                    args.snp_locations).Doc.SaveTo(phylip_output_filename, Encoding.ASCII)
            End If

            If (output_multi_fasta_file) OrElse (output_vcf_file = 0 AndAlso output_phylip_file = 0 AndAlso output_multi_fasta_file = 0) Then
                Dim multi_fasta_output_filename As New String(New Char(FILENAME_MAX - 1) {})
                multi_fasta_output_filename = output_filename_base.TrimSuffix
                If (output_vcf_file + output_phylip_file + output_multi_fasta_file) > 1 OrElse (output_filename Is Nothing OrElse output_filename = ControlChars.NullChar) Then
                    multi_fasta_output_filename += ".snp_sites.aln"
                End If

                SNPsFasta.SNPSitesFasta(
                    args.number_of_snps,
                    SNPsBases,
                    args.sequence_names,
                    args.number_of_samples,
                    output_reference,
                    args.pseudo_reference_sequence,
                    args.snp_locations).Save(multi_fasta_output_filename, Encodings.ASCII)
            End If

            Return args
        End Function

        ''' <summary>
        ''' SNPs from the input alignment fasta file
        ''' </summary>
        ''' <param name="filename">input alignment file</param>
        ''' <param name="output_multi_fasta_file"></param>
        ''' <param name="output_vcf_file"></param>
        ''' <param name="output_phylip_file"></param>
        ''' <param name="output_filename"></param>
        ''' <returns></returns>
        Public Function OutputSNPSites(ByRef filename As String,
                                   output_multi_fasta_file As Integer,
                                   output_vcf_file As Integer,
                                   output_phylip_file As Integer,
                                   ByRef output_filename As String) As SNPsAln

            Return __snpSitesGeneric(filename,
                                 output_multi_fasta_file,
                                 output_vcf_file,
                                 output_phylip_file,
                                 output_filename, 0, 0, 0)
        End Function

        ''' <summary>
        ''' SNPs from the input alignment fasta file
        ''' </summary>
        ''' <param name="filename">input alignment file</param>
        ''' <param name="output_multi_fasta_file"></param>
        ''' <param name="output_vcf_file"></param>
        ''' <param name="output_phylip_file"></param>
        ''' <param name="output_filename"></param>
        ''' <returns></returns>
        Public Function OutputSNPSitesWithRef(ByRef filename As String,
                                          output_multi_fasta_file As Integer,
                                          output_vcf_file As Integer,
                                          output_phylip_file As Integer,
                                          ByRef output_filename As String) As SNPsAln

            Return __snpSitesGeneric(filename,
                                 output_multi_fasta_file,
                                 output_vcf_file,
                                 output_phylip_file,
                                 output_filename, 1, 0, 0)
        End Function

        ''' <summary>
        ''' SNPs from the input alignment fasta file
        ''' </summary>
        ''' <param name="filename">input alignment file</param>
        ''' <param name="output_multi_fasta_file"></param>
        ''' <param name="output_vcf_file"></param>
        ''' <param name="output_phylip_file"></param>
        ''' <param name="output_filename"></param>
        ''' <param name="output_reference"></param>
        ''' <param name="pure_mode"></param>
        ''' <param name="output_monomorphic"></param>
        ''' <returns></returns>
        Public Function OutputSNPSitesWithRefPureMono(ByRef filename As String,
                                                  output_multi_fasta_file As Integer,
                                                  output_vcf_file As Integer,
                                                  output_phylip_file As Integer,
                                                  ByRef output_filename As String,
                                                  output_reference As Integer,
                                                  pure_mode As Integer,
                                                  output_monomorphic As Integer) As SNPsAln

            Return __snpSitesGeneric(filename,
                                 output_multi_fasta_file,
                                 output_vcf_file,
                                 output_phylip_file,
                                 output_filename,
                                 output_reference,
                                 pure_mode,
                                 output_monomorphic)
        End Function
    End Module
End Namespace
