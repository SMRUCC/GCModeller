#Region "Microsoft.VisualBasic::888feba94aff5ad9acc763708966225c, data\ExternalDBSource\MetaCyc\MySQL\biosourcewidcontactwid.vb"

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

    ' Class biosourcewidcontactwid
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
''' DROP TABLE IF EXISTS `biosourcewidcontactwid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `biosourcewidcontactwid` (
'''   `BioSourceWID` bigint(20) NOT NULL,
'''   `ContactWID` bigint(20) NOT NULL,
'''   KEY `FK_BioSourceWIDContactWID1` (`BioSourceWID`),
'''   KEY `FK_BioSourceWIDContactWID2` (`ContactWID`),
'''   CONSTRAINT `FK_BioSourceWIDContactWID1` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioSourceWIDContactWID2` FOREIGN KEY (`ContactWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("biosourcewidcontactwid", Database:="warehouse", SchemaSQL:="
CREATE TABLE `biosourcewidcontactwid` (
  `BioSourceWID` bigint(20) NOT NULL,
  `ContactWID` bigint(20) NOT NULL,
  KEY `FK_BioSourceWIDContactWID1` (`BioSourceWID`),
  KEY `FK_BioSourceWIDContactWID2` (`ContactWID`),
  CONSTRAINT `FK_BioSourceWIDContactWID1` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioSourceWIDContactWID2` FOREIGN KEY (`ContactWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class biosourcewidcontactwid: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("BioSourceWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property BioSourceWID As Long
    <DatabaseField("ContactWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property ContactWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `biosourcewidcontactwid` (`BioSourceWID`, `ContactWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `biosourcewidcontactwid` (`BioSourceWID`, `ContactWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `biosourcewidcontactwid` WHERE `BioSourceWID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `biosourcewidcontactwid` SET `BioSourceWID`='{0}', `ContactWID`='{1}' WHERE `BioSourceWID` = '{2}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `biosourcewidcontactwid` WHERE `BioSourceWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, BioSourceWID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `biosourcewidcontactwid` (`BioSourceWID`, `ContactWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, BioSourceWID, ContactWID)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{BioSourceWID}', '{ContactWID}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `biosourcewidcontactwid` (`BioSourceWID`, `ContactWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, BioSourceWID, ContactWID)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `biosourcewidcontactwid` SET `BioSourceWID`='{0}', `ContactWID`='{1}' WHERE `BioSourceWID` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, BioSourceWID, ContactWID, BioSourceWID)
    End Function
#End Region
End Class


End Namespace
