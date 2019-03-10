#Region "Microsoft.VisualBasic::924106832369f484ac5fe729417ace4d, sub-system\FBA\FBA_DP\FBA\Models\gcFBA\PhenoCoefficient.vb"

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

    ' Module PhenoCoefficient
    ' 
    '     Function: __getSample, (+4 Overloads) Coefficient
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.FBA_DP.Models.rFBA
Imports SMRUCC.genomics.Analysis.RNA_Seq
Imports SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2

<Package("Phenotype.Coefficient")>
Public Module PhenoCoefficient

    ''' <summary>
    ''' 这里会预先处理一下编号
    ''' </summary>
    ''' <param name="fluxs"></param>
    ''' <param name="rpkm"></param>
    ''' <param name="samples"></param>
    ''' <param name="PCC"></param>
    ''' <returns></returns>
    <ExportAPI("Flux.Coefficient")>
    Public Function Coefficient(fluxs As IEnumerable(Of PhenoOUT),
                                rpkm As IEnumerable(Of ExprStats),
                                samples As IEnumerable(Of SampleTable),
                                Optional PCC As Boolean = True) As RPKMStat()
        For Each x In fluxs
            x.Rxn = x.Rxn.ToUpper
        Next

        Return fluxs.Coefficient(rpkm, samples, PCC)
    End Function

    ''' <summary>
    ''' 这里会预先处理一下编号
    ''' </summary>
    ''' <param name="fluxs"></param>
    ''' <param name="rpkm"></param>
    ''' <param name="samples"></param>
    ''' <param name="PCC"></param>
    ''' <returns></returns>
    <ExportAPI("Flux.Coefficient")>
    Public Function Coefficient(fluxs As IEnumerable(Of RPKMStat),
                                rpkm As IEnumerable(Of ExprStats),
                                samples As IEnumerable(Of SampleTable),
                                Optional PCC As Boolean = True) As RPKMStat()
        For Each x In fluxs
            x.Locus = x.Locus.ToUpper
        Next

        Return fluxs.Coefficient(rpkm, samples, PCC)
    End Function

    ''' <summary>
    ''' 计算出每一个基因对每一个代谢过程的相关性
    ''' </summary>
    ''' <param name="fluxs"></param>
    ''' <param name="rpkm"></param>
    ''' <param name="samples"></param>
    ''' <returns></returns>
    '''
    <ExportAPI("Flux.Coefficient")>
    <Extension>
    Public Function Coefficient(Of Pheno As IPhenoOUT)(
                                   fluxs As IEnumerable(Of Pheno),
                                   rpkm As IEnumerable(Of ExprStats),
                                   samples As IEnumerable(Of SampleTable),
                                   Optional PCC As Boolean = True) As RPKMStat()

        Dim hash As New Dictionary(Of String, List(Of Double))

        For Each sample As SampleTable In samples
            Dim name As String = sample.sampleName

            For Each gene In rpkm
                If Not hash.ContainsKey(gene.locus) Then
                    Call hash.Add(gene.locus, New List(Of Double))
                End If
                Call hash(gene.locus).Add(gene.GetLevel(name))
            Next
            For Each flux As Pheno In fluxs
                If Not hash.ContainsKey(flux.Key) Then
                    Call hash.Add(flux.Key, New List(Of Double))
                End If
                Call hash(flux.Key).Add(flux.Properties(name))
            Next
        Next

        Dim exprSamples = hash.Select(Function(id, values) New ExprSamples(id, values)).toarray
        Dim MAT As PccMatrix =
            If(PCC, MatrixAPI.CreatePccMAT(exprSamples), MatrixAPI.CreateSPccMAT(exprSamples))
        Dim result As RPKMStat() = (From gene As ExprStats
                                    In rpkm'.AsParallel
                                    Let gSample As RPKMStat = __getSample(gene.locus, fluxs, MAT)
                                    Select gSample
                                    Order By gSample.Locus Ascending).ToArray
        Return result
    End Function

    ''' <summary>
    ''' 计算出每一个基因对每一个代谢过程的相关性
    ''' </summary>
    ''' <param name="fluxs"></param>
    ''' <param name="rpkm"></param>
    ''' <returns></returns>
    '''
    <ExportAPI("Flux.Coefficient")>
    <Extension>
    Public Function Coefficient(fluxs As IEnumerable(Of RPKMStat), rpkm As IEnumerable(Of RPKMStat), Optional PCC As Boolean = True) As RPKMStat()
        Dim hash As New Dictionary(Of String, List(Of Double))

        For Each sample As String In fluxs.First.Properties.Keys
            For Each gene In rpkm
                If Not hash.ContainsKey(gene.Locus) Then
                    Call hash.Add(gene.Locus, New List(Of Double))
                End If
                Call hash(gene.Locus).Add(gene.Properties(sample))
            Next
            For Each flux As RPKMStat In fluxs
                If Not hash.ContainsKey(flux.Locus) Then
                    Call hash.Add(flux.Locus, New List(Of Double))
                End If
                Call hash(flux.Locus).Add(flux.Properties(sample))
            Next
        Next
        Dim exprSamples = hash.Select(Function(id, values) New ExprSamples(id, values))
        Dim MAT As PccMatrix =
            If(PCC, MatrixAPI.CreatePccMAT(exprSamples), MatrixAPI.CreateSPccMAT(exprSamples))
        Dim result As RPKMStat() = (From gene As RPKMStat
                                    In rpkm'.AsParallel
                                    Let gSample As RPKMStat = __getSample(gene.Locus, fluxs, MAT)
                                    Select gSample
                                    Order By gSample.Locus Ascending).ToArray
        Return result
    End Function

    Private Function __getSample(Of PhenoOUT As IPhenoOUT)(locus As String, fluxs As IEnumerable(Of PhenoOUT), MAT As PccMatrix) As RPKMStat
        Dim gSample As RPKMStat = New RPKMStat With {
            .Locus = locus
        }

        For Each flux As PhenoOUT In fluxs
            Call gSample.Properties.Add(flux.Key, MAT.GetValue(locus, flux.Key))
        Next

        Return gSample
    End Function
End Module
