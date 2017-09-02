#Region "Microsoft.VisualBasic::3069e56b30fb8b9d910a3b07c593acdc, ..\repository\DataMySql\Correlations\Correlations.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

'#Region "Microsoft.VisualBasic::c20ab38b01b9a42b04d59915eab7344f, ..\GCModeller\analysis\annoTools\DataMySql\Correlations\Correlations.vb"

'    ' Author:
'    ' 
'    '       asuka (amethyst.asuka@gcmodeller.org)
'    '       xieguigang (xie.guigang@live.com)
'    '       xie (genetics@smrucc.org)
'    ' 
'    ' Copyright (c) 2016 GPL3 Licensed
'    ' 
'    ' 
'    ' GNU GENERAL PUBLIC LICENSE (GPL3)
'    ' 
'    ' This program is free software: you can redistribute it and/or modify
'    ' it under the terms of the GNU General Public License as published by
'    ' the Free Software Foundation, either version 3 of the License, or
'    ' (at your option) any later version.
'    ' 
'    ' This program is distributed in the hope that it will be useful,
'    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
'    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    ' GNU General Public License for more details.
'    ' 
'    ' You should have received a copy of the GNU General Public License
'    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

'#End Region

'Imports Microsoft.VisualBasic.CommandLine.Reflection
'Imports Microsoft.VisualBasic.Linq.Extensions
'Imports Microsoft.VisualBasic.Scripting.MetaData
'Imports SMRUCC.genomics.Analysis
'Imports SMRUCC.genomics.Analysis.RNA_Seq
'Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools
'Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.WGCNA
'Imports SMRUCC.genomics.Data.Model_Repository.MySQL
'Imports SMRUCC.genomics.Data.Model_Repository.MySQL.Tables

'''' <summary>
'''' 基因表达相关性的数据库服务
'''' </summary>
'''' 
'Public Class Correlations

'    Public ReadOnly Property MySQL As Oracle.LinuxCompatibility.MySQL.MySQL

'    ''' <summary>
'    ''' 
'    ''' </summary>
'    ''' <param name="uri">一般情况下这个参数为空，程序会自动根据配置文件来查找数据源</param>
'    Sub New(Optional uri As Oracle.LinuxCompatibility.MySQL.ConnectionUri = Nothing)
'        MySQL = MySQLExtensions.GetMySQLClient(uri, "correlations")
'        MySQL.UriMySQL.TimeOut = 60 * 600
'    End Sub

'    Public Overrides Function ToString() As String
'        Return MySQL.ToString
'    End Function

'    ''' <summary>
'    ''' 无方向性的
'    ''' </summary>
'    ''' <param name="id1"></param>
'    ''' <param name="id2"></param>
'    ''' <returns></returns>
'    Public Function GetPcc(id1 As String, id2 As String) As Double
'        Dim correlation As xcb = GetCorrelation(id1, id2)
'        If correlation Is Nothing Then
'            Return 0
'        Else
'            Return correlation.pcc
'        End If
'    End Function

'    Public Function GetCorrelation(id1 As String, id2 As String) As xcb
'        Return MySQL.ExecuteScalarAuto(Of xcb)($"(`g1_entity`='{id1}' and `g2_entity`='{id2}') or (`g1_entity`='{id2}' and `g2_entity`='{id1}')")
'    End Function

'    ''' <summary>
'    ''' 无方向性的
'    ''' </summary>
'    ''' <param name="id1"></param>
'    ''' <param name="id2"></param>
'    ''' <returns></returns>
'    Public Function GetSPcc(id1 As String, id2 As String) As Double
'        Dim correlation As xcb = GetCorrelation(id1, id2)
'        If correlation Is Nothing Then
'            Return 0
'        Else
'            Return correlation.spcc
'        End If
'    End Function

'    ''' <summary>
'    ''' 无方向性的
'    ''' </summary>
'    ''' <param name="id1"></param>
'    ''' <param name="id2"></param>
'    ''' <returns></returns>
'    Public Function GetWGCNAWeight(id1 As String, id2 As String) As Double
'        Dim correlation As xcb = GetCorrelation(id1, id2)
'        If correlation Is Nothing Then
'            Return 0
'        Else
'            Return correlation.wgcna_weight
'        End If
'    End Function

'    Public Function GetPccGreaterThan(id As String, cutoff As Double) As Dictionary(Of String, Double)
'        Dim SQL As String = String.Format(PCC_GREATER_THAN, id, cutoff)
'        Dim correlations = MySQL.Query(Of xcb)(SQL)
'        Dim dict = correlations.ToDictionary(
'            Function(paired) paired.GetConnectedNode(id),
'            elementSelector:=Function(paired) paired.pcc)
'        Return dict
'    End Function

'    ''' <summary>
'    ''' <see cref="GetPccGreaterThan"/>不取绝对值，这个函数是取绝对值的
'    ''' </summary>
'    ''' <param name="id"></param>
'    ''' <param name="cutoff"></param>
'    ''' <returns></returns>
'    Public Function GetPccSignificantThan(id As String, cutoff As Double) As Dictionary(Of String, Double)
'        Dim SQL As String = String.Format(PCC_SIGNIFICANT_THAN, id, Math.Abs(cutoff))
'        Dim correlations = MySQL.Query(Of xcb)(SQL)
'        Dim dict = correlations.ToDictionary(
'            Function(paired) paired.GetConnectedNode(id),
'            elementSelector:=Function(paired) paired.pcc)
'        Return dict
'    End Function

'    Public Function BuildHash(idList As String()) As Dictionary(Of String, Dictionary(Of String, xcb))
'        Dim SQL As String = "SELECT * FROM correlations.xcb where (g1_entity = '{0}' or g2_entity = '{0}');"
'        Dim LQuery = (From id As String
'                      In idList.AsParallel
'                      Select id,
'                          dataSet = MySQL.Query(Of xcb)(String.Format(SQL, id))).ToArray

'    End Function
'End Class

