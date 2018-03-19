#Region "Microsoft.VisualBasic::615345bbcd63ce8ebca04e27c10189d5, data\ExternalDBSource\MetaCyc\bio_warehouse\crossreference.vb"

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

    ' Class crossreference
    ' 
    '     Properties: CrossWID, DatabaseName, OtherWID, Relationship, Type
    '                 Version, XID
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
''' DROP TABLE IF EXISTS `crossreference`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `crossreference` (
'''   `OtherWID` bigint(20) NOT NULL,
'''   `CrossWID` bigint(20) DEFAULT NULL,
'''   `XID` varchar(50) DEFAULT NULL,
'''   `Type` varchar(20) DEFAULT NULL,
'''   `Version` varchar(10) DEFAULT NULL,
'''   `Relationship` varchar(50) DEFAULT NULL,
'''   `DatabaseName` varchar(255) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("crossreference", Database:="warehouse")>
Public Class crossreference: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("OtherWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property OtherWID As Long
    <DatabaseField("CrossWID"), DataType(MySqlDbType.Int64, "20")> Public Property CrossWID As Long
    <DatabaseField("XID"), DataType(MySqlDbType.VarChar, "50")> Public Property XID As String
    <DatabaseField("Type"), DataType(MySqlDbType.VarChar, "20")> Public Property Type As String
    <DatabaseField("Version"), DataType(MySqlDbType.VarChar, "10")> Public Property Version As String
    <DatabaseField("Relationship"), DataType(MySqlDbType.VarChar, "50")> Public Property Relationship As String
    <DatabaseField("DatabaseName"), DataType(MySqlDbType.VarChar, "255")> Public Property DatabaseName As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `crossreference` (`OtherWID`, `CrossWID`, `XID`, `Type`, `Version`, `Relationship`, `DatabaseName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `crossreference` (`OtherWID`, `CrossWID`, `XID`, `Type`, `Version`, `Relationship`, `DatabaseName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `crossreference` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `crossreference` SET `OtherWID`='{0}', `CrossWID`='{1}', `XID`='{2}', `Type`='{3}', `Version`='{4}', `Relationship`='{5}', `DatabaseName`='{6}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, OtherWID, CrossWID, XID, Type, Version, Relationship, DatabaseName)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, OtherWID, CrossWID, XID, Type, Version, Relationship, DatabaseName)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
