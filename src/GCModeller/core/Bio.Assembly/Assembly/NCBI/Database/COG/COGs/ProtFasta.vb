#Region "Microsoft.VisualBasic::0c8bb86b6c05a66d74b8e46c2c93f343, ..\core\Bio.Assembly\Assembly\NCBI\Database\COG\COGs\ProtFasta.vb"

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

        Public Shared Function Parser(fasta As FastaToken) As ProtFasta
            Dim Describ As String = fasta.Attributes(4)
            Dim genome As String = __genomeNameParser(Describ)

            Describ = Describ.Replace(genome, "").Trim
            genome = Mid(genome, 2, Len(genome) - 2)

            Return New ProtFasta With {
                .SequenceData = fasta.SequenceData,
                .Attributes = fasta.Attributes,
                .GI = fasta.Attributes(1),
                .Ref = fasta.Attributes(3),
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
