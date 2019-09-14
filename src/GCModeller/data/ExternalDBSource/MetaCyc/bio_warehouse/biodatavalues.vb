#Region "Microsoft.VisualBasic::3b17f3bcf92481052ed605495a762ade, ExternalDBSource\MetaCyc\bio_warehouse\biodatavalues.vb"

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

    ' Class biodatavalues
    ' 
    '     Properties: BioDataCube_DataExternal, BioDataCube_DataInternal, DataSetWID, MAGEClass, Order_
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
''' DROP TABLE IF EXISTS `biodatavalues`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `biodatavalues` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `MAGEClass` varchar(100) NOT NULL,
'''   `Order_` varchar(25) DEFAULT NULL,
'''   `BioDataCube_DataInternal` bigint(20) DEFAULT NULL,
'''   `BioDataCube_DataExternal` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_BioDataValues1` (`DataSetWID`),
'''   KEY `FK_BioDataValues2` (`BioDataCube_DataInternal`),
'''   KEY `FK_BioDataValues3` (`BioDataCube_DataExternal`),
'''   CONSTRAINT `FK_BioDataValues1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioDataValues2` FOREIGN KEY (`BioDataCube_DataInternal`) REFERENCES `datainternal` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioDataValues3` FOREIGN KEY (`BioDataCube_DataExternal`) REFERENCES `dataexternal` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("biodatavalues", Database:="warehouse")>
Public Class biodatavalues: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("MAGEClass"), NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property MAGEClass As String
    <DatabaseField("Order_"), DataType(MySqlDbType.VarChar, "25")> Public Property Order_ As String
    <DatabaseField("BioDataCube_DataInternal"), DataType(MySqlDbType.Int64, "20")> Public Property BioDataCube_DataInternal As Long
    <DatabaseField("BioDataCube_DataExternal"), DataType(MySqlDbType.Int64, "20")> Public Property BioDataCube_DataExternal As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `biodatavalues` (`WID`, `DataSetWID`, `MAGEClass`, `Order_`, `BioDataCube_DataInternal`, `BioDataCube_DataExternal`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `biodatavalues` (`WID`, `DataSetWID`, `MAGEClass`, `Order_`, `BioDataCube_DataInternal`, `BioDataCube_DataExternal`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `biodatavalues` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `biodatavalues` SET `WID`='{0}', `DataSetWID`='{1}', `MAGEClass`='{2}', `Order_`='{3}', `BioDataCube_DataInternal`='{4}', `BioDataCube_DataExternal`='{5}' WHERE `WID` = '{6}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, MAGEClass, Order_, BioDataCube_DataInternal, BioDataCube_DataExternal)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, MAGEClass, Order_, BioDataCube_DataInternal, BioDataCube_DataExternal)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, MAGEClass, Order_, BioDataCube_DataInternal, BioDataCube_DataExternal, WID)
    End Function
#End Region
End Class


End Namespace
