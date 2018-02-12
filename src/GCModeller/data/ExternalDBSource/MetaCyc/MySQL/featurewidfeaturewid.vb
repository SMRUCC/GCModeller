#Region "Microsoft.VisualBasic::662ff4e3478dec96f3a9c2e45937d90d, data\ExternalDBSource\MetaCyc\MySQL\featurewidfeaturewid.vb"

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

    ' Class featurewidfeaturewid
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 8:48:56 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MetaCyc.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `featurewidfeaturewid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `featurewidfeaturewid` (
'''   `FeatureWID1` bigint(20) NOT NULL,
'''   `FeatureWID2` bigint(20) NOT NULL,
'''   KEY `FK_FeatureWIDFeatureWID1` (`FeatureWID1`),
'''   KEY `FK_FeatureWIDFeatureWID2` (`FeatureWID2`),
'''   CONSTRAINT `FK_FeatureWIDFeatureWID1` FOREIGN KEY (`FeatureWID1`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_FeatureWIDFeatureWID2` FOREIGN KEY (`FeatureWID2`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("featurewidfeaturewid", Database:="warehouse", SchemaSQL:="
CREATE TABLE `featurewidfeaturewid` (
  `FeatureWID1` bigint(20) NOT NULL,
  `FeatureWID2` bigint(20) NOT NULL,
  KEY `FK_FeatureWIDFeatureWID1` (`FeatureWID1`),
  KEY `FK_FeatureWIDFeatureWID2` (`FeatureWID2`),
  CONSTRAINT `FK_FeatureWIDFeatureWID1` FOREIGN KEY (`FeatureWID1`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FeatureWIDFeatureWID2` FOREIGN KEY (`FeatureWID2`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class featurewidfeaturewid: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("FeatureWID1"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property FeatureWID1 As Long
    <DatabaseField("FeatureWID2"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property FeatureWID2 As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `featurewidfeaturewid` (`FeatureWID1`, `FeatureWID2`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `featurewidfeaturewid` (`FeatureWID1`, `FeatureWID2`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `featurewidfeaturewid` WHERE `FeatureWID1` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `featurewidfeaturewid` SET `FeatureWID1`='{0}', `FeatureWID2`='{1}' WHERE `FeatureWID1` = '{2}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `featurewidfeaturewid` WHERE `FeatureWID1` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, FeatureWID1)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `featurewidfeaturewid` (`FeatureWID1`, `FeatureWID2`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, FeatureWID1, FeatureWID2)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{FeatureWID1}', '{FeatureWID2}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `featurewidfeaturewid` (`FeatureWID1`, `FeatureWID2`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, FeatureWID1, FeatureWID2)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `featurewidfeaturewid` SET `FeatureWID1`='{0}', `FeatureWID2`='{1}' WHERE `FeatureWID1` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, FeatureWID1, FeatureWID2, FeatureWID1)
    End Function
#End Region
End Class


End Namespace
