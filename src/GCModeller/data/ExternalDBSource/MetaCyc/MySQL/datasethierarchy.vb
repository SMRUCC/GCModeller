#Region "Microsoft.VisualBasic::f84e1a7e2f80b9be946baee13312436e, ..\GCModeller\data\ExternalDBSource\MetaCyc\MySQL\datasethierarchy.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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
''' DROP TABLE IF EXISTS `datasethierarchy`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `datasethierarchy` (
'''   `SuperWID` bigint(20) NOT NULL,
'''   `SubWID` bigint(20) NOT NULL,
'''   KEY `FK_DataSetHierarchy1` (`SuperWID`),
'''   KEY `FK_DataSetHierarchy2` (`SubWID`),
'''   CONSTRAINT `FK_DataSetHierarchy1` FOREIGN KEY (`SuperWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_DataSetHierarchy2` FOREIGN KEY (`SubWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("datasethierarchy", Database:="warehouse", SchemaSQL:="
CREATE TABLE `datasethierarchy` (
  `SuperWID` bigint(20) NOT NULL,
  `SubWID` bigint(20) NOT NULL,
  KEY `FK_DataSetHierarchy1` (`SuperWID`),
  KEY `FK_DataSetHierarchy2` (`SubWID`),
  CONSTRAINT `FK_DataSetHierarchy1` FOREIGN KEY (`SuperWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DataSetHierarchy2` FOREIGN KEY (`SubWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class datasethierarchy: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("SuperWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property SuperWID As Long
    <DatabaseField("SubWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property SubWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `datasethierarchy` (`SuperWID`, `SubWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `datasethierarchy` (`SuperWID`, `SubWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `datasethierarchy` WHERE `SuperWID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `datasethierarchy` SET `SuperWID`='{0}', `SubWID`='{1}' WHERE `SuperWID` = '{2}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `datasethierarchy` WHERE `SuperWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, SuperWID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `datasethierarchy` (`SuperWID`, `SubWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, SuperWID, SubWID)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{SuperWID}', '{SubWID}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `datasethierarchy` (`SuperWID`, `SubWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, SuperWID, SubWID)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `datasethierarchy` SET `SuperWID`='{0}', `SubWID`='{1}' WHERE `SuperWID` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, SuperWID, SubWID, SuperWID)
    End Function
#End Region
End Class


End Namespace
