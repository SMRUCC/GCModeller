Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.Metagenome.gast
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module SILVA_OTU

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="fasta">通过mothur的<see cref="Mothur.GetOTUrep(String, String, String, Double)"/>
    ''' 命令获取得到的OTU代表序列的fasta文件数据</param>
    ''' <returns></returns>
    <Extension>
    Public Function ParseOTUrep(fasta As IEnumerable(Of FastaToken)) As Dictionary(Of String, NamedValue(Of Integer))
        Dim table As New Dictionary(Of String, NamedValue(Of Integer))
        Dim OTU$()

        For Each kseq As FastaToken In fasta
            With kseq.Title.Split(ASCII.TAB)
                OTU = .Last.Split("|"c)
                table(.First) = New NamedValue(Of Integer) With {
                    .Name = OTU(0),
                    .Value = Val(OTU(1))
                }
            End With
        Next

        Return table
    End Function

    ''' <summary>
    ''' Removes all of the OTU that counts less than <paramref name="cutoff"/> percentage.
    ''' </summary>
    ''' <param name="OTUrep">Loaded fasta header data from mothur ``get.oturep`` output.</param>
    ''' <param name="cutoff#">The sequence number cutoff percentage.</param>
    ''' <returns></returns>
    <Extension>
    Public Function RemovesOTUlt(OTUrep As Dictionary(Of String, NamedValue(Of Integer)), Optional cutoff# = 0.0001) As Dictionary(Of String, NamedValue(Of Integer))
        Dim sum# = OTUrep _
            .Values _
            .Select(Function(o) o.Value) _
            .Sum

        cutoff = sum * cutoff

        Return OTUrep _
            .Where(Function(OTU) OTU.Value.Value >= cutoff) _
            .ToDictionary
    End Function

    ''' <summary>
    ''' Assign OTU taxonomy from OTU blastn against silva database with votes
    ''' </summary>
    ''' <param name="blastn"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function OTUsilvaTaxonomy(blastn As IEnumerable(Of Query), OTUs As Dictionary(Of String, NamedValue(Of Integer))) As IEnumerable(Of gastOUT)
        For Each query As Query In blastn
            If Not OTUs.ContainsKey(query.QueryName) Then
                Continue For
            End If

            ' Create an array Of taxonomy objects For all the associated refssu_ids.
            Dim hits = query _
                .SubjectHits _
                .Select(Function(h) h.Name.GetTagValue(" ", trim:=True)) _
                .Select(Function(h)
                            Return New NamedValue(Of gast.Taxonomy) With {
                                .Name = h.Name,
                                .Value = New gast.Taxonomy(h.Value)
                            }
                        End Function) _
                .ToArray
            Dim counts = OTUs(query.QueryName)

            ' Lookup the consensus taxonomy For the array
            Dim taxReturn = gast.Taxonomy.consensus(hits.Values, 0.97)
            ' 0=taxObj, 1=winning vote, 2=minrank, 3=rankCounts, 4=maxPcts, 5=naPcts;
            Dim taxon = taxReturn(0).taxstring
            Dim rank = taxReturn(0).depth

            If (taxon Is Nothing) Then
                taxon = "Unknown"
            End If

            ' (taxonomy, distance, rank, refssu_count, vote, minrank, taxa_counts, max_pcts, na_pcts)
            Dim gastOut As New gastOUT With {
                .taxonomy = taxon,
                .rank = rank,
                .refssu_count = hits.Length,
                .vote = taxReturn(1).taxstring,
                .minrank = taxReturn(2).taxstring,
                .taxa_counts = taxReturn(3).taxstring,
                .max_pcts = taxReturn(4).taxstring,
                .na_pcts = taxReturn(5).taxstring,
                .read_id = counts.Name,
                .refhvr_ids = query.QueryName,
                .counts = counts.Value
            }

            Yield gastOut
        Next
    End Function
End Module
