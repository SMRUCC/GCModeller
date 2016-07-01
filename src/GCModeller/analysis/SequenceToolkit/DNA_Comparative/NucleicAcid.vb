Imports LANS.SystemsBiology.SequenceModel
Imports LANS.SystemsBiology.SequenceModel.FASTA
Imports LANS.SystemsBiology.SequenceModel.NucleotideModels

''' <summary>
''' 为了加快计算速度而生成的窗口计算缓存，请注意，在生成缓存的时候已经进行了并行化，所以在内部生成缓存的时候，不需要再进行并行化了
''' </summary>
''' <remarks></remarks>
Public Class NucleicAcid : Inherits NucleotideModels.NucleicAcid

    Dim __biasHash As Dictionary(Of KeyValuePair(Of DNA, DNA), Double)

    ''' <summary>
    ''' Get value by using a paired of base.
    ''' </summary>
    ''' <param name="X"></param>
    ''' <param name="Y"></param>
    ''' <returns></returns>
    Public Function GetValue(X As DNA, Y As DNA) As Double
        Return __biasHash(New KeyValuePair(Of DNA, DNA)(X, Y))
    End Function

    ''' <summary>
    ''' 窗口数据或者缓存数据
    ''' </summary>
    ''' <param name="SequenceData"></param>
    ''' <remarks></remarks>
    Sub New(SequenceData As DNA())
        Call MyBase.New(SequenceData)
        __biasHash = New Dictionary(Of KeyValuePair(Of DNA, DNA), Double)
        Dim dat As KeyValuePair(Of KeyValuePair(Of DNA, DNA), Double)
        dat = __createSigma(Me, DNA.dAMP, DNA.dAMP) : Call __biasHash.Add(dat.Key, dat.Value)
        dat = __createSigma(Me, DNA.dAMP, DNA.dCMP) : Call __biasHash.Add(dat.Key, dat.Value)
        dat = __createSigma(Me, DNA.dAMP, DNA.dGMP) : Call __biasHash.Add(dat.Key, dat.Value)
        dat = __createSigma(Me, DNA.dAMP, DNA.dTMP) : Call __biasHash.Add(dat.Key, dat.Value)
        dat = __createSigma(Me, DNA.dCMP, DNA.dAMP) : Call __biasHash.Add(dat.Key, dat.Value)
        dat = __createSigma(Me, DNA.dCMP, DNA.dCMP) : Call __biasHash.Add(dat.Key, dat.Value)
        dat = __createSigma(Me, DNA.dCMP, DNA.dGMP) : Call __biasHash.Add(dat.Key, dat.Value)
        dat = __createSigma(Me, DNA.dCMP, DNA.dTMP) : Call __biasHash.Add(dat.Key, dat.Value)
        dat = __createSigma(Me, DNA.dGMP, DNA.dAMP) : Call __biasHash.Add(dat.Key, dat.Value)
        dat = __createSigma(Me, DNA.dGMP, DNA.dCMP) : Call __biasHash.Add(dat.Key, dat.Value)
        dat = __createSigma(Me, DNA.dGMP, DNA.dGMP) : Call __biasHash.Add(dat.Key, dat.Value)
        dat = __createSigma(Me, DNA.dGMP, DNA.dTMP) : Call __biasHash.Add(dat.Key, dat.Value)
        dat = __createSigma(Me, DNA.dTMP, DNA.dAMP) : Call __biasHash.Add(dat.Key, dat.Value)
        dat = __createSigma(Me, DNA.dTMP, DNA.dCMP) : Call __biasHash.Add(dat.Key, dat.Value)
        dat = __createSigma(Me, DNA.dTMP, DNA.dGMP) : Call __biasHash.Add(dat.Key, dat.Value)
        dat = __createSigma(Me, DNA.dTMP, DNA.dTMP) : Call __biasHash.Add(dat.Key, dat.Value)
    End Sub

    Sub New(SequenceData As FastaToken)
        Call Me.New(New NucleotideModels.NucleicAcid(SequenceData).ToArray)
    End Sub

    Sub New(SequenceData As String)
        Call Me.New(New NucleotideModels.NucleicAcid(SequenceData).ToArray)
    End Sub

    Private Shared Function __createSigma(SequenceData As NucleotideModels.NucleicAcid,
                                          X As DNA,
                                          Y As DNA) As KeyValuePair(Of KeyValuePair(Of DNA, DNA), Double)
        Dim KEY = New KeyValuePair(Of DNA, DNA)(X, Y)
        Dim n = GenomeSignatures.DinucleotideBIAS_p(SequenceData, X, Y)
        Return New KeyValuePair(Of KeyValuePair(Of DNA, DNA), Double)(KEY, n)
    End Function
End Class
