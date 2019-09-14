#Region "Microsoft.VisualBasic::171e7ea38501cfb9f2fb3278dbfe11b0, ExternalDBSource\MetaCyc\bio_warehouse\sequencematch.vb"

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

    ' Class sequencematch
    ' 
    '     Properties: ComputationWID, EValue, Length, MatchEnd, MatchStart
    '                 MatchWID, PercentIdentical, PercentSimilar, PValue, QueryEnd
    '                 QueryStart, QueryWID, Rank
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:02:47 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MetaCyc.MySQL

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `sequencematch`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `sequencematch` (
'''   `QueryWID` bigint(20) NOT NULL,
'''   `MatchWID` bigint(20) NOT NULL,
'''   `ComputationWID` bigint(20) NOT NULL,
'''   `EValue` double DEFAULT NULL,
'''   `PValue` double DEFAULT NULL,
'''   `PercentIdentical` float DEFAULT NULL,
'''   `PercentSimilar` float DEFAULT NULL,
'''   `Rank` smallint(6) DEFAULT NULL,
'''   `Length` int(11) DEFAULT NULL,
'''   `QueryStart` int(11) DEFAULT NULL,
'''   `QueryEnd` int(11) DEFAULT NULL,
'''   `MatchStart` int(11) DEFAULT NULL,
'''   `MatchEnd` int(11) DEFAULT NULL,
'''   KEY `FK_SequenceMatch` (`ComputationWID`),
'''   CONSTRAINT `FK_SequenceMatch` FOREIGN KEY (`ComputationWID`) REFERENCES `computation` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("sequencematch", Database:="warehouse")>
Public Class sequencematch: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("QueryWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property QueryWID As Long
    <DatabaseField("MatchWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property MatchWID As Long
    <DatabaseField("ComputationWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property ComputationWID As Long
    <DatabaseField("EValue"), DataType(MySqlDbType.Double)> Public Property EValue As Double
    <DatabaseField("PValue"), DataType(MySqlDbType.Double)> Public Property PValue As Double
    <DatabaseField("PercentIdentical"), DataType(MySqlDbType.Double)> Public Property PercentIdentical As Double
    <DatabaseField("PercentSimilar"), DataType(MySqlDbType.Double)> Public Property PercentSimilar As Double
    <DatabaseField("Rank"), DataType(MySqlDbType.Int64, "6")> Public Property Rank As Long
    <DatabaseField("Length"), DataType(MySqlDbType.Int64, "11")> Public Property Length As Long
    <DatabaseField("QueryStart"), DataType(MySqlDbType.Int64, "11")> Public Property QueryStart As Long
    <DatabaseField("QueryEnd"), DataType(MySqlDbType.Int64, "11")> Public Property QueryEnd As Long
    <DatabaseField("MatchStart"), DataType(MySqlDbType.Int64, "11")> Public Property MatchStart As Long
    <DatabaseField("MatchEnd"), DataType(MySqlDbType.Int64, "11")> Public Property MatchEnd As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `sequencematch` (`QueryWID`, `MatchWID`, `ComputationWID`, `EValue`, `PValue`, `PercentIdentical`, `PercentSimilar`, `Rank`, `Length`, `QueryStart`, `QueryEnd`, `MatchStart`, `MatchEnd`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `sequencematch` (`QueryWID`, `MatchWID`, `ComputationWID`, `EValue`, `PValue`, `PercentIdentical`, `PercentSimilar`, `Rank`, `Length`, `QueryStart`, `QueryEnd`, `MatchStart`, `MatchEnd`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `sequencematch` WHERE `ComputationWID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `sequencematch` SET `QueryWID`='{0}', `MatchWID`='{1}', `ComputationWID`='{2}', `EValue`='{3}', `PValue`='{4}', `PercentIdentical`='{5}', `PercentSimilar`='{6}', `Rank`='{7}', `Length`='{8}', `QueryStart`='{9}', `QueryEnd`='{10}', `MatchStart`='{11}', `MatchEnd`='{12}' WHERE `ComputationWID` = '{13}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ComputationWID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, QueryWID, MatchWID, ComputationWID, EValue, PValue, PercentIdentical, PercentSimilar, Rank, Length, QueryStart, QueryEnd, MatchStart, MatchEnd)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, QueryWID, MatchWID, ComputationWID, EValue, PValue, PercentIdentical, PercentSimilar, Rank, Length, QueryStart, QueryEnd, MatchStart, MatchEnd)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, QueryWID, MatchWID, ComputationWID, EValue, PValue, PercentIdentical, PercentSimilar, Rank, Length, QueryStart, QueryEnd, MatchStart, MatchEnd, ComputationWID)
    End Function
#End Region
End Class


End Namespace
