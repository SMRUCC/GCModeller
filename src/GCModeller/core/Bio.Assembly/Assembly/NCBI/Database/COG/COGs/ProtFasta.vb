#Region "Microsoft.VisualBasic::d23e521ed91717dbf8304e2d8b5148b3, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\COG\COGs\ProtFasta.vb"

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

    '   Total Lines: 55
    '    Code Lines: 35
    ' Comment Lines: 11
    '   Blank Lines: 9
    '     File Size: 2.06 KB


    '     Class ProtFasta
    ' 
    '         Properties: GenomeName, Ref
    ' 
    '         Function: __genomeNameParser, LoadDocument, Parser, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Assembly.NCBI.COG.COGs

    ''' <summary>
    ''' prot2003-2014.fa.gz
    ''' Sequences of all proteins with assigned COG domains in FASTA format
    ''' (gzipped)
    '''
    ''' The first word of the defline always starts with "gi|&lt;protein-id>".
    ''' </summary>
    Public Class ProtFasta : Inherits SequenceDump.Protein

        Public Property Ref As String
        Public Property GenomeName As String

        ''' <summary>
        ''' >gi|103485499|ref|YP_615060.1| chromosomal replication initiation protein [Sphingopyxis alaskensis RB2256]
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return $"gi|{GI}|ref|{Ref}| {Description} [{GenomeName}]"
        End Function

        Public Shared Function Parser(fasta As FastaSeq) As ProtFasta
            Dim Describ As String = fasta.Headers(4)
            Dim genome As String = __genomeNameParser(Describ)

            Describ = Describ.Replace(genome, "").Trim
            genome = Mid(genome, 2, Len(genome) - 2)

            Return New ProtFasta With {
                .SequenceData = fasta.SequenceData,
                .Headers = fasta.Headers,
                .GI = fasta.Headers(1),
                .Ref = fasta.Headers(3),
                .Description = Describ,
                .GenomeName = genome
            }
        End Function

        Private Shared Function __genomeNameParser(attr As String) As String
            Dim values$() = Regex.Matches(attr, "\[.*?\]").ToArray
            Return values.LastOrDefault
        End Function

        Public Overloads Shared Function LoadDocument(File As String) As ProtFasta()
            Dim fasta = FastaFile.Read(File)
            Call $"Load fasta stream job done! Start fasta parsing job".__DEBUG_ECHO
            Return fasta.Select(Function(fa) ProtFasta.Parser(fa)).ToArray
        End Function
    End Class
End Namespace
