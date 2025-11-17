#Region "Microsoft.VisualBasic::de27e353187bbb834044475fe60178d4, core\Bio.Assembly\ComponentModel\MimeTypes.vb"

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

    '   Total Lines: 17
    '    Code Lines: 5 (29.41%)
    ' Comment Lines: 9 (52.94%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 3 (17.65%)
    '     File Size: 830 B


    '     Module MimeTypes
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace ComponentModel

    Public Module MimeTypes

        ''' <summary>
        ''' FASTQ format is a text-based format for storing both a biological sequence 
        ''' (usually nucleotide sequence) and its corresponding quality scores. Both the
        ''' sequence letter and quality score are each encoded with a single ASCII 
        ''' character for brevity. It was originally developed at the Wellcome Trust 
        ''' Sanger Institute to bundle a FASTA formatted sequence and its quality data, 
        ''' but has recently become the de facto standard for storing the output of 
        ''' high-throughput sequencing instruments such as the Illumina Genome Analyzer.
        ''' </summary>
        Public Const FastQ As String = "text/plain, chemical/seq-na-fastq"
        ''' <summary>
        ''' Fasta file for protein sequence data
        ''' </summary>
        Public Const FastAProt As String = "text/plain, chemical/seq-aa"
        ''' <summary>
        ''' Fasta file for nucleotide sequence data
        ''' </summary>
        Public Const FastANucl As String = "text/plain, chemical/seq-na"
        Public Const GenBank As String = "text/plain, application/x-genbank"
        ''' <summary>
        ''' Generic Feature Format version 3
        ''' </summary>
        Public Const GFF3Table As String = "text/plain, text/x-gff3"

    End Module
End Namespace
