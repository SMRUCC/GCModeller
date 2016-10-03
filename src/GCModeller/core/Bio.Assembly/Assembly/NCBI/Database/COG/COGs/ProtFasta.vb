#Region "Microsoft.VisualBasic::118e1757ff713e98528238e2c86df7b6, ..\GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\COG\COGs\ProtFasta.vb"

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

        Public Shared Function Parser(Fasta As SequenceModel.FASTA.FastaToken) As ProtFasta
            Dim Describ As String = Fasta.Attributes(4)
            Dim Genome As String = __genomeNameParser(Describ)
            Describ = Describ.Replace(Genome, "").Trim
            Genome = Mid(Genome, 2, Len(Genome) - 2)
            Return New ProtFasta With {
                .SequenceData = Fasta.SequenceData,
                .Attributes = Fasta.Attributes,
                .GI = Fasta.Attributes(1),
                .Ref = Fasta.Attributes(3),
                .Description = Describ,
                .GenomeName = Genome
            }
        End Function

        Private Shared Function __genomeNameParser(attr As String) As String
            Dim values As String() = (From m As Match
                                      In Regex.Matches(attr, "\[.*?\]")
                                      Select m.Value).ToArray
            Return values.LastOrDefault
        End Function

        Public Overloads Shared Function LoadDocument(File As String) As ProtFasta()
            Dim FastaFile = SequenceModel.FASTA.FastaFile.Read(File)
            Call $"Load fasta stream job done! Start fasta parsing job".__DEBUG_ECHO
            Return FastaFile.ToArray(Function(Fasta) ProtFasta.Parser(Fasta), Parallel:=True)
        End Function
    End Class
End Namespace
