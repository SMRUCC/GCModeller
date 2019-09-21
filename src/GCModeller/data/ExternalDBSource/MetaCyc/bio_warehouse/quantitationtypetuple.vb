#Region "Microsoft.VisualBasic::5431abe04a48c7187a4bec1943da78e7, ExternalDBSource\MetaCyc\bio_warehouse\quantitationtypetuple.vb"

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

    ' Class quantitationtypetuple
    ' 
    '     Properties: DataSetWID, DesignElementTuple, QuantitationType, QuantitationTypeTuple_Datum, WID
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
''' DROP TABLE IF EXISTS `quantitationtypetuple`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `quantitationtypetuple` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `DesignElementTuple` bigint(20) DEFAULT NULL,
'''   `QuantitationType` bigint(20) DEFAULT NULL,
'''   `QuantitationTypeTuple_Datum` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_QuantitationTypeTuple1` (`DataSetWID`),
'''   KEY `FK_QuantitationTypeTuple2` (`DesignElementTuple`),
'''   KEY `FK_QuantitationTypeTuple3` (`QuantitationType`),
'''   KEY `FK_QuantitationTypeTuple4` (`QuantitationTypeTuple_Datum`),
'''   CONSTRAINT `FK_QuantitationTypeTuple1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_QuantitationTypeTuple2` FOREIGN KEY (`DesignElementTuple`) REFERENCES `designelementtuple` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_QuantitationTypeTuple3` FOREIGN KEY (`QuantitationType`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_QuantitationTypeTuple4` FOREIGN KEY (`QuantitationTypeTuple_Datum`) REFERENCES `datum` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("quantitationtypetuple", Database:="warehouse")>
Public Class quantitationtypetuple: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("DesignElementTuple"), DataType(MySqlDbType.Int64, "20")> Public Property DesignElementTuple As Long
    <DatabaseField("QuantitationType"), DataType(MySqlDbType.Int64, "20")> Public Property QuantitationType As Long
    <DatabaseField("QuantitationTypeTuple_Datum"), DataType(MySqlDbType.Int64, "20")> Public Property QuantitationTypeTuple_Datum As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `quantitationtypetuple` (`WID`, `DataSetWID`, `DesignElementTuple`, `QuantitationType`, `QuantitationTypeTuple_Datum`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `quantitationtypetuple` (`WID`, `DataSetWID`, `DesignElementTuple`, `QuantitationType`, `QuantitationTypeTuple_Datum`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `quantitationtypetuple` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `quantitationtypetuple` SET `WID`='{0}', `DataSetWID`='{1}', `DesignElementTuple`='{2}', `QuantitationType`='{3}', `QuantitationTypeTuple_Datum`='{4}' WHERE `WID` = '{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, DesignElementTuple, QuantitationType, QuantitationTypeTuple_Datum)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, DesignElementTuple, QuantitationType, QuantitationTypeTuple_Datum)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, DesignElementTuple, QuantitationType, QuantitationTypeTuple_Datum, WID)
    End Function
#End Region
End Class


End Namespace
