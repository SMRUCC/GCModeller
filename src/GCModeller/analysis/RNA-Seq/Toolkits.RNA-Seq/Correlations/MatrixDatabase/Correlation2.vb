Imports SMRUCC.genomics.Toolkits.RNA_Seq
Imports SMRUCC.genomics.Toolkits.RNA_Seq.WGCNA

''' <summary>
''' 基因表达相关性的本地文件数据库服务
''' </summary>
Public Class Correlation2 : Implements ICorrelations

    Public ReadOnly Property Pcc As PccMatrix
    Public ReadOnly Property WGCNA As WGCNAWeight
    Public ReadOnly Property SPcc As PccMatrix

    Sub New(Pcc As String, SPcc As String, WGCNA As String)
        Call $"Load WGCNA data from {WGCNA.ToFileURL}".__DEBUG_ECHO
        Dim WGCNAar = Function() FastImports(WGCNA)
        Dim loadWGCNA = WGCNAar.BeginInvoke(Nothing, Nothing)

        Call $"Load pcc data from {Pcc.ToFileURL}".__DEBUG_ECHO
        Me.Pcc = MatrixSerialization.Load(from:=Pcc)
        Call $"Load spcc data from {SPcc.ToFileURL}".__DEBUG_ECHO
        Me.SPcc = MatrixSerialization.Load(from:=SPcc)
        Me.WGCNA = WGCNAar.EndInvoke(loadWGCNA)

        Call "OK!".__DEBUG_ECHO
    End Sub

    Sub New(dumpDir As String)
        Call Me.New(Pcc:=dumpDir & "/Pcc.db", SPcc:=dumpDir & "/sPcc.db", WGCNA:=dumpDir & "/CytoscapeEdges.txt")
    End Sub

    Public Function GetPcc(id1 As String, id2 As String) As Double Implements ICorrelations.GetPcc
        Return Pcc.GetValue(id1, id2)
    End Function

    Public Function GetSPcc(id1 As String, id2 As String) As Double Implements ICorrelations.GetSPcc
        Return SPcc.GetValue(id1, id2)
    End Function

    Public Function GetWGCNAWeight(id1 As String, id2 As String) As Double Implements ICorrelations.GetWGCNAWeight
        Return WGCNA.GetValue(id1, id2)
    End Function

    Public Function GetPccGreaterThan(id As String, cutoff As Double) As Dictionary(Of String, Double) Implements ICorrelations.GetPccGreaterThan
        Dim sample = Pcc(id)
        Dim array = Pcc.lstGenes
        Dim dict As New Dictionary(Of String, Double)

        For i As Integer = 0 To array.Length - 1
            If sample.Values(i) >= cutoff Then
                Call dict.Add(array(i), sample.Values(i))
            End If
        Next

        Return dict
    End Function

    Public Function GetPccSignificantThan(id As String, cutoff As Double) As Dictionary(Of String, Double) Implements ICorrelations.GetPccSignificantThan
        Dim sample = Pcc(id)
        Dim array = Pcc.lstGenes
        Dim dict As New Dictionary(Of String, Double)

        For i As Integer = 0 To array.Length - 1
            If Math.Abs(sample.Values(i)) >= cutoff Then
                Call dict.Add(array(i), sample.Values(i))
            End If
        Next

        Return dict
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="codeName">这个一般是KEGG之中的物种编号</param>
    ''' <returns></returns>
    Public Shared Function CreateFromName(codeName As String) As Correlation2
        Call Settings.Session.Initialize()

        Dim DIR As String = GCModeller.FileSystem.Correlations & $"/{codeName}/"
        If Not DIR.DirectoryExists OrElse String.IsNullOrEmpty(codeName) Then
            Call $"{codeName} is not exists in the repository...".__DEBUG_ECHO
            Return Nothing
        End If

        Dim corr As New Correlation2(DIR)
        Return corr
    End Function

    ''' <summary>
    ''' 自动识别加载，加载失败的话会返回空值
    ''' </summary>
    ''' <param name="SpNameOrDIR"></param>
    ''' <returns></returns>
    Public Shared Function LoadAuto(SpNameOrDIR As String) As Correlation2
        If SpNameOrDIR.DirectoryExists Then
            Return New Correlation2(SpNameOrDIR)
        Else
            Return CreateFromName(SpNameOrDIR)
        End If
    End Function
End Class
