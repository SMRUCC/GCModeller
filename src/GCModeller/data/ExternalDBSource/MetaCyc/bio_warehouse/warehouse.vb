#Region "Microsoft.VisualBasic::ddacc87adc354601daa16d32482d2c59, data\ExternalDBSource\MetaCyc\bio_warehouse\warehouse.vb"

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

    ' Class warehouse
    ' 
    '     Properties: Description, LoadDate, MaxReservedWID, MaxSpecialWID, Version
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
''' DROP TABLE IF EXISTS `warehouse`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `warehouse` (
'''   `Version` decimal(6,3) NOT NULL,
'''   `LoadDate` datetime NOT NULL,
'''   `MaxSpecialWID` bigint(20) NOT NULL,
'''   `MaxReservedWID` bigint(20) NOT NULL,
'''   `Description` text
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("warehouse", Database:="warehouse")>
Public Class warehouse: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("Version"), NotNull, DataType(MySqlDbType.Decimal)> Public Property Version As Decimal
    <DatabaseField("LoadDate"), NotNull, DataType(MySqlDbType.DateTime)> Public Property LoadDate As Date
    <DatabaseField("MaxSpecialWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property MaxSpecialWID As Long
    <DatabaseField("MaxReservedWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property MaxReservedWID As Long
    <DatabaseField("Description"), DataType(MySqlDbType.Text)> Public Property Description As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `warehouse` (`Version`, `LoadDate`, `MaxSpecialWID`, `MaxReservedWID`, `Description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `warehouse` (`Version`, `LoadDate`, `MaxSpecialWID`, `MaxReservedWID`, `Description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `warehouse` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `warehouse` SET `Version`='{0}', `LoadDate`='{1}', `MaxSpecialWID`='{2}', `MaxReservedWID`='{3}', `Description`='{4}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, Version, DataType.ToMySqlDateTimeString(LoadDate), MaxSpecialWID, MaxReservedWID, Description)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, Version, DataType.ToMySqlDateTimeString(LoadDate), MaxSpecialWID, MaxReservedWID, Description)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
