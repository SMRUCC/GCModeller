#Region "Microsoft.VisualBasic::17e9ff9a4a1c891c9352dc77474dda09, data\ExternalDBSource\MetaCyc\MySQL\biosourcewidgenewid.vb"

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

    ' Class biosourcewidgenewid
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
''' DROP TABLE IF EXISTS `biosourcewidgenewid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `biosourcewidgenewid` (
'''   `BioSourceWID` bigint(20) NOT NULL,
'''   `GeneWID` bigint(20) NOT NULL,
'''   KEY `FK_BioSourceWIDGeneWID1` (`BioSourceWID`),
'''   KEY `FK_BioSourceWIDGeneWID2` (`GeneWID`),
'''   CONSTRAINT `FK_BioSourceWIDGeneWID1` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioSourceWIDGeneWID2` FOREIGN KEY (`GeneWID`) REFERENCES `gene` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("biosourcewidgenewid", Database:="warehouse", SchemaSQL:="
CREATE TABLE `biosourcewidgenewid` (
  `BioSourceWID` bigint(20) NOT NULL,
  `GeneWID` bigint(20) NOT NULL,
  KEY `FK_BioSourceWIDGeneWID1` (`BioSourceWID`),
  KEY `FK_BioSourceWIDGeneWID2` (`GeneWID`),
  CONSTRAINT `FK_BioSourceWIDGeneWID1` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioSourceWIDGeneWID2` FOREIGN KEY (`GeneWID`) REFERENCES `gene` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class biosourcewidgenewid: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("BioSourceWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property BioSourceWID As Long
    <DatabaseField("GeneWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property GeneWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `biosourcewidgenewid` (`BioSourceWID`, `GeneWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `biosourcewidgenewid` (`BioSourceWID`, `GeneWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `biosourcewidgenewid` WHERE `BioSourceWID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `biosourcewidgenewid` SET `BioSourceWID`='{0}', `GeneWID`='{1}' WHERE `BioSourceWID` = '{2}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `biosourcewidgenewid` WHERE `BioSourceWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, BioSourceWID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `biosourcewidgenewid` (`BioSourceWID`, `GeneWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, BioSourceWID, GeneWID)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{BioSourceWID}', '{GeneWID}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `biosourcewidgenewid` (`BioSourceWID`, `GeneWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, BioSourceWID, GeneWID)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `biosourcewidgenewid` SET `BioSourceWID`='{0}', `GeneWID`='{1}' WHERE `BioSourceWID` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, BioSourceWID, GeneWID, BioSourceWID)
    End Function
#End Region
End Class


End Namespace
