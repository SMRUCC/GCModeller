#Region "Microsoft.VisualBasic::eb539e2d207d889b710b5c5641921107, data\ExternalDBSource\MetaCyc\bio_warehouse\array_.vb"

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

    ' Class array_
    ' 
    '     Properties: ArrayDesign, ArrayGroup, ArrayIdentifier, ArrayXOrigin, ArrayYOrigin
    '                 DataSetWID, Identifier, Information, Name, OriginRelativeTo
    '                 WID
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
''' DROP TABLE IF EXISTS `array_`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `array_` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `Identifier` varchar(255) DEFAULT NULL,
'''   `Name` varchar(255) DEFAULT NULL,
'''   `ArrayIdentifier` varchar(255) DEFAULT NULL,
'''   `ArrayXOrigin` float DEFAULT NULL,
'''   `ArrayYOrigin` float DEFAULT NULL,
'''   `OriginRelativeTo` varchar(255) DEFAULT NULL,
'''   `ArrayDesign` bigint(20) DEFAULT NULL,
'''   `Information` bigint(20) DEFAULT NULL,
'''   `ArrayGroup` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_Array_1` (`DataSetWID`),
'''   KEY `FK_Array_3` (`ArrayDesign`),
'''   KEY `FK_Array_4` (`Information`),
'''   KEY `FK_Array_5` (`ArrayGroup`),
'''   CONSTRAINT `FK_Array_1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Array_3` FOREIGN KEY (`ArrayDesign`) REFERENCES `arraydesign` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Array_4` FOREIGN KEY (`Information`) REFERENCES `arraymanufacture` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Array_5` FOREIGN KEY (`ArrayGroup`) REFERENCES `arraygroup` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("array_", Database:="warehouse")>
Public Class array_: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("Identifier"), DataType(MySqlDbType.VarChar, "255")> Public Property Identifier As String
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "255")> Public Property Name As String
    <DatabaseField("ArrayIdentifier"), DataType(MySqlDbType.VarChar, "255")> Public Property ArrayIdentifier As String
    <DatabaseField("ArrayXOrigin"), DataType(MySqlDbType.Double)> Public Property ArrayXOrigin As Double
    <DatabaseField("ArrayYOrigin"), DataType(MySqlDbType.Double)> Public Property ArrayYOrigin As Double
    <DatabaseField("OriginRelativeTo"), DataType(MySqlDbType.VarChar, "255")> Public Property OriginRelativeTo As String
    <DatabaseField("ArrayDesign"), DataType(MySqlDbType.Int64, "20")> Public Property ArrayDesign As Long
    <DatabaseField("Information"), DataType(MySqlDbType.Int64, "20")> Public Property Information As Long
    <DatabaseField("ArrayGroup"), DataType(MySqlDbType.Int64, "20")> Public Property ArrayGroup As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `array_` (`WID`, `DataSetWID`, `Identifier`, `Name`, `ArrayIdentifier`, `ArrayXOrigin`, `ArrayYOrigin`, `OriginRelativeTo`, `ArrayDesign`, `Information`, `ArrayGroup`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `array_` (`WID`, `DataSetWID`, `Identifier`, `Name`, `ArrayIdentifier`, `ArrayXOrigin`, `ArrayYOrigin`, `OriginRelativeTo`, `ArrayDesign`, `Information`, `ArrayGroup`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `array_` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `array_` SET `WID`='{0}', `DataSetWID`='{1}', `Identifier`='{2}', `Name`='{3}', `ArrayIdentifier`='{4}', `ArrayXOrigin`='{5}', `ArrayYOrigin`='{6}', `OriginRelativeTo`='{7}', `ArrayDesign`='{8}', `Information`='{9}', `ArrayGroup`='{10}' WHERE `WID` = '{11}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, Identifier, Name, ArrayIdentifier, ArrayXOrigin, ArrayYOrigin, OriginRelativeTo, ArrayDesign, Information, ArrayGroup)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Identifier, Name, ArrayIdentifier, ArrayXOrigin, ArrayYOrigin, OriginRelativeTo, ArrayDesign, Information, ArrayGroup)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, Identifier, Name, ArrayIdentifier, ArrayXOrigin, ArrayYOrigin, OriginRelativeTo, ArrayDesign, Information, ArrayGroup, WID)
    End Function
#End Region
End Class


End Namespace
