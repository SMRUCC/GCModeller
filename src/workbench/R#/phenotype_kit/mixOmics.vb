#Region "Microsoft.VisualBasic::efd3c8bf3a16f9fa6caa9a61d43c3cd5, R#\phenotype_kit\multiOmics.vb"

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

'   Total Lines: 63
'    Code Lines: 56 (88.89%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 7 (11.11%)
'     File Size: 2.66 KB


' Module multiOmics
' 
'     Function: getData, map_force, omics2DScatterPlot
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.KEGG
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Model.Network.Regulons
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports REnv = SMRUCC.Rsharp.Runtime
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

<Package("mixOmics")>
Module mixOmics

    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(SignificantPair()), AddressOf spearmanMIC_table)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function spearmanMIC_table(data As SignificantPair(), args As list, env As Environment) As dataframe
        Dim df As New dataframe With {.columns = New Dictionary(Of String, Array)}

        Call df.add("source", From pair As SignificantPair In data Select pair.OtuId)
        Call df.add("target", From pair As SignificantPair In data Select pair.MetaboliteId)
        Call df.add("spearman-rho", From pair As SignificantPair In data Select pair.SpearmanRho)
        Call df.add("spearman-pval", From pair As SignificantPair In data Select pair.SpearmanPValue)
        Call df.add("MIC", From pair As SignificantPair In data Select pair.MIC)
        Call df.add("MIC-pval", From pair As SignificantPair In data Select pair.MICPValue)
        Call df.add("score", From pair As SignificantPair In data Select pair.CombinedScore)
        Call df.add("pvalue", From pair As SignificantPair In data Select pair.CombinedPValue)
        Call df.add("association", From pair As SignificantPair In data Select pair.AssociationType)

        Return df
    End Function

    <ExportAPI("nearZeroVar")>
    Public Function FindNearZeroVarColumns(expr_mat As Matrix,
                                           Optional freqCut As Double = 19.0,
                                           Optional uniqueCut As Double = 10.0) As String()

        Return expr_mat.FindNearZeroVarColumns(freqCut, uniqueCut).ToArray
    End Function

    <ExportAPI("omics.2D_scatter")>
    Public Function omics2DScatterPlot(x As Object, y As Object,
                                       Optional xlab$ = "X",
                                       Optional ylab$ = "Y",
                                       Optional size As Object = "3000,3000",
                                       Optional padding As Object = "padding: 200px 250px 200px 100px;",
                                       Optional ptSize! = 10,
                                       Optional env As Environment = Nothing) As Object

        If x Is Nothing OrElse y Is Nothing Then
            Return REnv.Internal.debug.stop("data can not be null!", env)
        End If

        Return OmicsScatter2D.Plot(
            omicsX:=getData(x, xlab),
            omicsY:=getData(y, ylab),
            xlab:=xlab,
            ylab:=ylab,
            pointSize:=ptSize,
            size:=InteropArgumentHelper.getSize(size, env, "3000,3000"),
            padding:=InteropArgumentHelper.getPadding(padding, "padding: 200px 250px 200px 100px;")
        )
    End Function

    Private Function getData(x As Object, ByRef label$) As NamedValue(Of Double)()
        Dim type As Type = x.GetType

        If type Is GetType(Dictionary(Of String, Double)) Then
            Return DirectCast(x, Dictionary(Of String, Double)) _
                .Select(Function(gene)
                            Return New NamedValue(Of Double)(gene.Key, gene.Value)
                        End Function) _
                .ToArray
        ElseIf type Is GetType(list) Then
            Return DirectCast(x, list).slots _
                .Select(Function(gene)
                            Return New NamedValue(Of Double)(gene.Key, CDbl(REnv.getFirst(gene.Value)))
                        End Function) _
                .ToArray
        Else
            Return {}
        End If
    End Function

    <ExportAPI("map_force")>
    Public Function map_force(x As Matrix, y As Matrix, maps As Background) As Matrix
        Return PathForceBuilder.CreateForce(x, y, maps)
    End Function

    <ExportAPI("sparcc")>
    Public Function sparcc(x As Matrix, y As Matrix, Optional strict As Boolean = True)
        Call TRN.ValidateSamples(x, y, strict:=strict)

        Dim sparccResult As CrossOmicsCorrelation = SparCCComputation.ComputeCrossCorrelation(x, y, New SparCCConfig)
        Return sparccResult
    End Function

    <ExportAPI("cclasso")>
    <RApiReturn(GetType(CrossOmicsCorrelation))>
    Public Function cclasso(x As Matrix, y As Matrix,
                            Optional lam_min_ratio As Double = 0.001,
                            Optional nfold As Integer = 5,
                            Optional n_bootstraps As Integer = 500,
                            Optional strict As Boolean = True,
                            Optional env As Environment = Nothing)

        Call TRN.ValidateSamples(x, y, strict:=strict)

        Dim cclassoResult As CrossOmicsCorrelation = CrossCorrelationCalculator.ComputeCCLasso(
        x, y,
        lambda:=lam_min_ratio,
        maxIterations:=n_bootstraps,
        tolerance:=0.0000001,
        pseudoCount:=0.5)

        Return cclassoResult
    End Function

    <ExportAPI("connections")>
    <RApiReturn(GetType(Connection), GetType(SignificantPair))>
    Public Function connections(cor As Object, Optional pval_cutoff As Double = 0.05, Optional cor_cutoff As Double = 0.6)
        If TypeOf cor Is CrossOmicsCorrelation Then
            Return DirectCast(cor, CrossOmicsCorrelation) _
                .Network(pval_cutoff, cor_cutoff) _
                .OrderBy(Function(c) c.pval) _
                .ToArray
        ElseIf TypeOf cor Is SpearmanMICResult Then
            Return SpearmanMICCombined.FilterSignificantPairs(DirectCast(cor, SpearmanMICResult), pValueThreshold:=pval_cutoff) _
                .Where(Function(a) a.SpearmanRho <> 0) _
                .ToArray
        Else
            Throw New NotImplementedException
        End If
    End Function

    ''' <summary>
    ''' Spearman + MIC
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <returns></returns>
    <ExportAPI("mine")>
    <RApiReturn(GetType(SpearmanMICResult))>
    Public Function mine(x As Matrix, y As Matrix)
        Return CrossCorrelationCalculator.ComputeSpearmanMIC(x, y)
    End Function
End Module
