Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace ContextModel

    Public Module Gtf

        Public Function ParseFile(file As String) As PTT
            Dim geneLines As String() = file.SolveStream.LineTokens
            Dim genes As GeneBrief() = geneLines _
                .Select(Function(l) l.Split(ASCII.TAB).doParseOfGeneInfo) _
                .ToArray

            Return New PTT(genes)
        End Function

        <Extension>
        Private Function doParseOfGeneInfo(tokens As String()) As GeneBrief
            Dim chr As String = tokens(Scan0)
            Dim seqType As String = tokens(1)
            Dim type As String = tokens(2)
            Dim left As Integer = Integer.Parse(tokens(3))
            Dim right As Integer = Integer.Parse(tokens(4))
            Dim strand As Strands = tokens(6).GetStrand
            Dim info = tokens(8).StringSplit(";\s+") _
                .Select(AddressOf GetTagValue) _
                .ToDictionary(Function(t) t.Name,
                              Function(t)
                                  Return t.Value.Trim(""""c, " "c)
                              End Function)
            Dim geneId As String = info!gene_id

            Return New GeneBrief With {
                .Code = "",
                .COG = "",
                .Gene = geneId,
                .IsORF = True,
                .Length = right - left,
                .Location = New NucleotideLocation(left, right, strand),
                .PID = geneId,
                .Product = geneId,
                .Synonym = geneId
            }
        End Function
    End Module
End Namespace