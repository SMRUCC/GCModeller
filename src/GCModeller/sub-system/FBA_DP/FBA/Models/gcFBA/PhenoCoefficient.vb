Imports SMRUCC.genomics.GCModeller.AnalysisTools.ModelSolvers.FBA.DESeq2
Imports SMRUCC.genomics.GCModeller.AnalysisTools.ModelSolvers.FBA.Models.rFBA
Imports SMRUCC.genomics.Toolkits.RNA_Seq
Imports SMRUCC.genomics.Toolkits.RNA_Seq.dataExprMAT
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic
Imports System.Runtime.CompilerServices

<PackageNamespace("Phenotype.Coefficient")>
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
                If Not hash.ContainsKey(flux.Identifier) Then
                    Call hash.Add(flux.Identifier, New List(Of Double))
                End If
                Call hash(flux.Identifier).Add(flux.Properties(name))
            Next
        Next

        Dim exprSamples = hash.ToArray(Function(id, values) New ExprSamples(id, values))
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
        Dim exprSamples = hash.ToArray(Function(id, values) New ExprSamples(id, values))
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
            Call gSample.Properties.Add(flux.Identifier, MAT.GetValue(locus, flux.Identifier))
        Next

        Return gSample
    End Function
End Module
