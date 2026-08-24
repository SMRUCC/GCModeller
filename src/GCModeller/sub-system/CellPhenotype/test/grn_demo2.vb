#Region "Microsoft.VisualBasic::47bcbac3a5f27bf5d7f2658a9923b76b, sub-system\CellPhenotype\test\grn_demo2.vb"

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

    '   Total Lines: 22
    '    Code Lines: 17 (77.27%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (22.73%)
    '     File Size: 848 B


    ' Module grn_demo2
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Framework.StorageProvider
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports SMRUCC.genomics.Analysis.BNLearn.IO
Imports SMRUCC.genomics.Analysis.CellPhenotype
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Module grn_demo2

    Sub Main()
        Dim data = "K:\hsa\Homo_sapiens_expr_advanced_all_conditions.csv"
        Dim wgcna = NetworkFileIO.Load("K:\hsa_grn").CreateGraph
        Dim TF = DataFrameResolver.Load("K:\hsa_grn\Homo_sapiens_TF.txt", tsv:=True)
        Dim TFlist As String() = TF("Ensembl")
        Dim grn = GeneRegulatoryNetwork.BuildBNNetwork(wgcna, TFlist)


        Dim exprData = BnIO.ReadGeneExpressionMatrix(Matrix.LoadData(data, tqdm_wrap:=True))

        Pause()
    End Sub
End Module

