#Region "Microsoft.VisualBasic::87db795c3a3c2373a1daffc7c7c91491, ..\GCModeller\analysis\SequenceToolkit\SNP\SNPScan.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis
Imports SMRUCC.genomics.Analysis.SequenceTools.SNP.SangerSNPs
Imports SMRUCC.genomics.Interops
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Patterns

Public Module SNPScan

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
    Public Sub CLIUsage()
        Console.Write("Usage: snp-sites [-rmvpcbhV] [-o output_filename] <file>" & vbLf)
        Console.Write("This program finds snp sites from a multi FASTA alignment file." & vbLf)
        Console.Write(" -r     output internal pseudo reference sequence" & vbLf)
        Console.Write(" -m     output a multi fasta alignment file (default)" & vbLf)
        Console.Write(" -v     output a VCF file" & vbLf)
        Console.Write(" -p     output a phylip file" & vbLf)
        Console.Write(" -o STR specify an output filename" & vbLf)
        Console.Write(" -c     only output columns containing exclusively ACGT" & vbLf)
        Console.Write(" -b     output monomorphic sites, used for BEAST" & vbLf)
        Console.Write(" -h     this help message" & vbLf)
        Console.Write(" -V     print version and exit" & vbLf)
        Console.Write(" <file> input alignment file which can optionally be gzipped" & vbLf & vbLf)

        Console.Write("Example: creating files for BEAST" & vbLf)
        Console.Write(" snp-sites -cb -o outputfile.aln inputfile.aln" & vbLf & vbLf)

        Console.Write("If you use this program, please cite:" & vbLf)
        Console.Write("""SNP-sites: rapid efficient extraction of SNPs from multi-FASTA alignments""," & vbLf)
        Console.Write("Andrew J. Page, Ben Taylor, Aidan J. Delaney, Jorge Soares, Torsten Seemann, Jacqueline A. Keane, Simon R. Harris," & vbLf)
        Console.Write("Microbial Genomics 2(4), (2016). http://dx.doi.org/10.1099/mgen.0.000056" & vbLf)
    End Sub

    ReadOnly clustal As ClustalOrg.Clustal =
        ClustalOrg.Clustal.CreateSession

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="nt">
    ''' The file path of the input nt fasta sequence file.
    ''' </param>
    ''' <returns></returns>
    Public Function ScanRaw(nt As String) As SNPsAln
        Return __scanRaw(nt)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="nt">可以不经过任何处理，程序在这里会自动使用clustal进行对齐操作</param>
    ''' <returns></returns>
    <Extension>
    Public Function ScanRaw(nt As FASTA.FastaFile) As SNPsAln
        Dim tmp As String = App.GetAppSysTempFile(".fasta")
        Call nt.Save(tmp, Encodings.ASCII)
        Return __scanRaw(tmp)
    End Function

    Private Function __scanRaw([in] As String) As SNPsAln
        Dim nt As FASTA.FastaFile = clustal.MultipleAlignment([in])
        nt.FilePath = [in]
        Return nt.ScanSNPs(refInd:=Scan0)
    End Function

    ''' <summary>
    ''' Scan snp sites from the given fasta sequence file.
    ''' </summary>
    ''' <param name="nt">序列必须都是已经经过clustal对齐了的，并且拥有FileName属性值</param>
    ''' <returns></returns>
    <Extension>
    Public Function ScanSNPs(nt As FASTA.FastaFile,
                             refInd$,
                             Optional pureMode As Boolean = False,
                             Optional monomorphic As Boolean = False,
                             Optional ByRef vcf_output_filename$ = Nothing) As SNPsAln

        Dim index% = nt.Index(refInd)

        If index = -1 Then
            Throw New EvaluateException($"{refInd} is not a valid reference....")
        Else
            Call $"Using {nt(index).Title} as reference...".__DEBUG_ECHO
        End If

        Return nt.SNPSitesGeneric(1, 1, 1, App.GetAppSysTempFile, index, If(pureMode, 1, 0), If(monomorphic, 1, 0), vcf_output_filename)
    End Function
End Module
