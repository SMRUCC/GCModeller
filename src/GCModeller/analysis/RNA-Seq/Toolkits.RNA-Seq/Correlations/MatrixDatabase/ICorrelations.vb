Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Toolkits.RNA_Seq
Imports SMRUCC.genomics.Toolkits

Public Interface ICorrelations

    ''' <summary>
    ''' 无方向性的
    ''' </summary>
    ''' <param name="id1"></param>
    ''' <param name="id2"></param>
    ''' <returns></returns>
    Function GetPcc(id1 As String, id2 As String) As Double

    ''' <summary>
    ''' 无方向性的
    ''' </summary>
    ''' <param name="id1"></param>
    ''' <param name="id2"></param>
    ''' <returns></returns>
    Function GetSPcc(id1 As String, id2 As String) As Double

    ''' <summary>
    ''' 无方向性的
    ''' </summary>
    ''' <param name="id1"></param>
    ''' <param name="id2"></param>
    ''' <returns></returns>
    Function GetWGCNAWeight(id1 As String, id2 As String) As Double
    Function GetPccGreaterThan(id As String, cutoff As Double) As Dictionary(Of String, Double)

    ''' <summary>
    ''' <see cref="GetPccGreaterThan"/>不取绝对值，这个函数是取绝对值的
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="cutoff"></param>
    ''' <returns></returns>
    Function GetPccSignificantThan(id As String, cutoff As Double) As Dictionary(Of String, Double)
End Interface