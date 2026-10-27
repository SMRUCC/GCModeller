#Region "Microsoft.VisualBasic::3bfb964f51a092d99e4cb0963e28f031, annotations\GSEA\test\KOBASTest.vb"

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

    ' Module KOBASTest
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language.Vectorization
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Analysis.HTS.GSEA.KOBAS

Module KOBASTest

    Sub Main()

        Dim genelist = {"A", "B", "C", "D", "E", "F", "G"}
        Dim background As Index(Of String) = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N"}

        Dim name = {"test"}

        ' (labels,phnameA,phnameB,sample_num,sample0,sample1) =gsea.read_cls_file(cls=opt.cls)

        Dim cls As phenotype = phenotype.FromClsFile("D:\GCModeller\src\GCModeller\annotations\GSEA\test_cls.txt")
        Dim matrix As New Vector(Of Vector)

        Dim m = KOBAS_GSEA.get_hit_matrix(genelist, genelist.Length, name, name.ToArray, {background}, 3, 1000)
        Dim rank As (sort_r As Object, sort_gene_index As Object) = KOBAS_GSEA.rank_pro(cls.labels, md:="ttest", expr_data:=matrix, sample0:=cls.sample0, sample1:=cls.sample1)
        Dim wst = 1.0
        Dim nperm = 1000
        Dim gene_num = genelist.Length

        Dim es As (ES As Vector, ESidx As Object, RES As Object) = KOBAS_GSEA.ES_all(sort_r:=rank.sort_r, sort_gene_index:=rank.sort_gene_index, hit_matrix_filtered:=m.hit_matrix_filtered, weighted_score_type:=wst, gene_num:=genelist.Length)
        Dim es_null = KOBAS_GSEA.ES_null(lb:=cls.labels, times:=nperm, method:="ttest", sample0:=cls.sample0, sample1:=cls.sample1, hit_matrix_filtered:=m.hit_matrix_filtered,
                       weighted_score_type:=wst, expr_data:=Nothing, gene_num:=gene_num)
        Dim pval = KOBAS_GSEA.nominal_p(es:=es.ES, es_null:=es_null)
        Dim norm As (NES As Object, nes_null As Object) = KOBAS_GSEA.normalized(es.ES, es_null)
        Dim fdr = KOBAS_GSEA.fdr_cal(nes_obs:=norm.NES, nes_null:=norm.nes_null)

        Dim types As (type0 As Object, type1 As Object) = KOBAS_GSEA.output_set(result_path:="./", phnameA:=cls.phnameA, phnameB:=cls.phnameB, gset_name_filtered:=m.gset_name_filtered,
                                gset_des_filtered:=m.gset_des_filtered, hit_matrix_filtered:=m.hit_matrix_filtered, ES:=es, NES:=norm.NES, nompval:=pval, FDR:=fdr)


        Pause()
    End Sub
End Module
