Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.GCModeller.Workbench.DatabaseServices.Model_Repository.MySQL
Imports SMRUCC.genomics.Toolkits.RNA_Seq
Imports SMRUCC.genomics.Toolkits
Imports SMRUCC.genomics.Toolkits.RNA_Seq.RTools
Imports SMRUCC.genomics.Toolkits.RNA_Seq.RTools.WGCNA

''' <summary>
''' 基因表达相关性的数据库服务
''' </summary>
''' 
Public Class Correlations

    Public ReadOnly Property MySQL As Oracle.LinuxCompatibility.MySQL.MySQL

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="uri">一般情况下这个参数为空，程序会自动根据配置文件来查找数据源</param>
    Sub New(Optional uri As Oracle.LinuxCompatibility.MySQL.ConnectionUri = Nothing)
        MySQL = MySQLExtensions.GetMySQLClient(uri, "correlations")
        MySQL.UriMySQL.TimeOut = 60 * 600
    End Sub

    Public Overrides Function ToString() As String
        Return MySQL.ToString
    End Function

    ''' <summary>
    ''' 无方向性的
    ''' </summary>
    ''' <param name="id1"></param>
    ''' <param name="id2"></param>
    ''' <returns></returns>
    Public Function GetPcc(id1 As String, id2 As String) As Double
        Dim correlation As Tables.xcb = GetCorrelation(id1, id2)
        If correlation Is Nothing Then
            Return 0
        Else
            Return correlation.pcc
        End If
    End Function

    Public Function GetCorrelation(id1 As String, id2 As String) As Tables.xcb
        Return MySQL.ExecuteScalarAuto(Of Tables.xcb)($"(`g1_entity`='{id1}' and `g2_entity`='{id2}') or (`g1_entity`='{id2}' and `g2_entity`='{id1}')")
    End Function

    ''' <summary>
    ''' 无方向性的
    ''' </summary>
    ''' <param name="id1"></param>
    ''' <param name="id2"></param>
    ''' <returns></returns>
    Public Function GetSPcc(id1 As String, id2 As String) As Double
        Dim correlation As Tables.xcb = GetCorrelation(id1, id2)
        If correlation Is Nothing Then
            Return 0
        Else
            Return correlation.spcc
        End If
    End Function

    ''' <summary>
    ''' 无方向性的
    ''' </summary>
    ''' <param name="id1"></param>
    ''' <param name="id2"></param>
    ''' <returns></returns>
    Public Function GetWGCNAWeight(id1 As String, id2 As String) As Double
        Dim correlation As Tables.xcb = GetCorrelation(id1, id2)
        If correlation Is Nothing Then
            Return 0
        Else
            Return correlation.wgcna_weight
        End If
    End Function

    Public Function GetPccGreaterThan(id As String, cutoff As Double) As Dictionary(Of String, Double)
        Dim SQL As String = String.Format(PCC_GREATER_THAN, id, cutoff)
        Dim correlations = MySQL.Query(Of Tables.xcb)(SQL)
        Dim dict = correlations.ToDictionary(
            Function(paired) paired.GetConnectedNode(id),
            elementSelector:=Function(paired) paired.pcc)
        Return dict
    End Function

    ''' <summary>
    ''' <see cref="GetPccGreaterThan"/>不取绝对值，这个函数是取绝对值的
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="cutoff"></param>
    ''' <returns></returns>
    Public Function GetPccSignificantThan(id As String, cutoff As Double) As Dictionary(Of String, Double)
        Dim SQL As String = String.Format(PCC_SIGNIFICANT_THAN, id, Math.Abs(cutoff))
        Dim correlations = MySQL.Query(Of Tables.xcb)(SQL)
        Dim dict = correlations.ToDictionary(
            Function(paired) paired.GetConnectedNode(id),
            elementSelector:=Function(paired) paired.pcc)
        Return dict
    End Function

    Public Function BuildHash(idList As String()) As Dictionary(Of String, Dictionary(Of String, Tables.xcb))
        Dim SQL As String = "SELECT * FROM correlations.xcb where (g1_entity = '{0}' or g2_entity = '{0}');"
        Dim LQuery = (From id As String
                      In idList.AsParallel
                      Select id,
                          dataSet = MySQL.Query(Of Tables.xcb)(String.Format(SQL, id))).ToArray

    End Function
End Class
