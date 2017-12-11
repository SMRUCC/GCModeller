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
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function OTUsilvaTaxonomy(blastn As IEnumerable(Of Query), OTUs As Dictionary(Of String, NamedValue(Of Integer))) As IEnumerable(Of gastOUT)
        Return blastn.gastTaxonomyInternal(
            getTaxonomy:=Function(hitName)
                             Dim t$ = hitName _
                                 .GetTagValue(vbTab, trim:=True) _
                                 .Value
                             Return New Taxonomy(t)
                         End Function,
            getOTU:=OTUs
        )
    End Function
End Module
