#Region "Microsoft.VisualBasic::8b24305501b610cf8746c581647cb741, CLI_tools\c2\CommandLines\MEME_Workflows.vb"

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

    ' Module CommandLines
    ' 
    '     Function: GenomeWidePromoterParser, MatchMEME_Result, MetaCycPathwayPromoterParsing
    ' 
    ' /********************************************************************************/

#End Region

Partial Module CommandLines

    <CommandLine.Reflection.ExportAPI("-genome_wide_promoter_parse", Info:="",
        Usage:="-genome_wide_promoter_parse -door <door_opr_file> -genome_sequence <genome_sequence_fasta_file> -segment_lengths <length1,length2,length3,...> -wgcna_weights <wgcna_weight_file> -cutoff <cutoff_value> -export <export>",
        Example:="")>
    Public Function GenomeWidePromoterParser(ARGV As CommandLine.CommandLine) As Integer
        Dim Door As String = ARGV("-door"), GenomeSequence As String = ARGV("-genome_sequence"), SegmentLength As Integer() = (From Token As String In ARGV("-segment_lengths").Split(CChar(",")) Select CInt(Val(Token)) Distinct).ToArray
        Dim WGCNAWeights As String = ARGV("-wgcna_weights")
        Dim WeightCutoff As Double = Val(ARGV("-cutoff")), ExportedDir As String = ARGV("-export")
        Dim SplitUnit As Integer = Var(ARGV("-split_units"))

        Using Parser As GenomeWildRandomParser = New GenomeWildRandomParser(LANS.SystemsBiology.Assembly.Door.Load(Door).DoorOperonView, GenomeSequence, SegmentLength)
            Call Parser.TryParse(c2.WGCNAWeight.CreateObject(WGCNAWeights), WeightCutoff, SplitUnit, ExportedDir)
        End Using

        Return 0
    End Function

    ''' <summary>
    ''' 代谢途径的启动子序列的解析
    ''' </summary>
    ''' <param name="ARGV"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <CommandLine.Reflection.ExportAPI("-metacyc_pathway_promoter_parse",
        Usage:="-metacyc_pathway_promoter_parse -metacyc <metacyc_data_dir> -door <door_file> -export <export_dir> -kegg_modules <kegg_module_csv_file>")>
    Public Function MetaCycPathwayPromoterParsing(ARGV As CommandLine.CommandLine) As Integer
        Dim MetaCyc As String = ARGV("-metacyc")
        Dim Door As String = ARGV("-door")
        Dim ExportDir As String = ARGV("-export")
        Dim KEGGModules As String = ARGV("-kegg_modules")

        Using Parser As New Workflows.RegulationNetwork.RegulationNetwork(MetaCyc, Door)
            If Not String.IsNullOrEmpty(KEGGModules) Then
                Call Parser.ExtractPromoterRegion_KEGGModules(ExportLocation:=ExportDir & "/kegg_modules/", KEGGModules:=KEGGModules)
            End If
            Call Parser.ExtractPromoterRegion_Pathway(ExportLocation:=ExportDir & "/metacyc_pathways")
        End Using

        Return 0
    End Function

    <CommandLine.Reflection.ExportAPI("-matches", Usage:="-matches -meme_out <meme_out_dir> -mast_out <mast_out_dir> -fasta_dir <fasta_dir> -regprecise_tfbs <regprecise_tfbs_fasta> -regprecise_bh <regprecise_bh_csv> -wgcna <wgcna> -chipdata <chipdata_csv> -door <door_file>")>
    Public Function MatchMEME_Result(args As CommandLine.CommandLine) As Integer
        Return LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.HtmlMatching.Invoke(
            args("-meme_out"),
            args("-mast_out"),
            args("-fasta_dir"),
            args("-regprecise_tfbs"),
            args("-regprecise_bh"),
            args("-wgcna"),
            args("-chipdata"),
            args("-door"))
    End Function
End Module
