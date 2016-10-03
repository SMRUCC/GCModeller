#Region "Microsoft.VisualBasic::da19422a243f712eed739a4e78d5a6a1, ..\GCModeller\data\ExternalDBSource\MetaCyc\bio_warehouse\geneticcode.vb"

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
''' DROP TABLE IF EXISTS `geneticcode`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `geneticcode` (
'''   `WID` bigint(20) NOT NULL,
'''   `NCBIID` varchar(2) DEFAULT NULL,
'''   `Name` varchar(100) DEFAULT NULL,
'''   `TranslationTable` varchar(64) DEFAULT NULL,
'''   `StartCodon` varchar(64) DEFAULT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_GeneticCode` (`DataSetWID`),
'''   CONSTRAINT `FK_GeneticCode` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("geneticcode", Database:="warehouse")>
Public Class geneticcode: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("NCBIID"), DataType(MySqlDbType.VarChar, "2")> Public Property NCBIID As String
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "100")> Public Property Name As String
    <DatabaseField("TranslationTable"), DataType(MySqlDbType.VarChar, "64")> Public Property TranslationTable As String
    <DatabaseField("StartCodon"), DataType(MySqlDbType.VarChar, "64")> Public Property StartCodon As String
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `geneticcode` (`WID`, `NCBIID`, `Name`, `TranslationTable`, `StartCodon`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `geneticcode` (`WID`, `NCBIID`, `Name`, `TranslationTable`, `StartCodon`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `geneticcode` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `geneticcode` SET `WID`='{0}', `NCBIID`='{1}', `Name`='{2}', `TranslationTable`='{3}', `StartCodon`='{4}', `DataSetWID`='{5}' WHERE `WID` = '{6}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, NCBIID, Name, TranslationTable, StartCodon, DataSetWID)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, NCBIID, Name, TranslationTable, StartCodon, DataSetWID)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, NCBIID, Name, TranslationTable, StartCodon, DataSetWID, WID)
    End Function
#End Region
End Class


End Namespace
