#Region "Microsoft.VisualBasic::81fcf43b78a8735bb3c000814577cc62, GCModeller\data\MicrobesOnline\MySQL\genomics\iprinfo.vb"

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


    ' Code Statistics:

    '   Total Lines: 66
    '    Code Lines: 37
    ' Comment Lines: 22
    '   Blank Lines: 7
    '     File Size: 3.08 KB


    ' Class iprinfo
    ' 
    '     Properties: iprId, iprName, proteinCount, shortName, type
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:30:29 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.genomics

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `iprinfo`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `iprinfo` (
'''   `iprId` varchar(9) NOT NULL DEFAULT '',
'''   `type` varchar(16) DEFAULT NULL,
'''   `shortName` varchar(50) DEFAULT NULL,
'''   `proteinCount` int(5) DEFAULT NULL,
'''   `iprName` varchar(255) DEFAULT NULL,
'''   PRIMARY KEY (`iprId`),
'''   KEY `type` (`type`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("iprinfo")>
Public Class iprinfo: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("iprId"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "9")> Public Property iprId As String
    <DatabaseField("type"), DataType(MySqlDbType.VarChar, "16")> Public Property type As String
    <DatabaseField("shortName"), DataType(MySqlDbType.VarChar, "50")> Public Property shortName As String
    <DatabaseField("proteinCount"), DataType(MySqlDbType.Int64, "5")> Public Property proteinCount As Long
    <DatabaseField("iprName"), DataType(MySqlDbType.VarChar, "255")> Public Property iprName As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `iprinfo` (`iprId`, `type`, `shortName`, `proteinCount`, `iprName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `iprinfo` (`iprId`, `type`, `shortName`, `proteinCount`, `iprName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `iprinfo` WHERE `iprId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `iprinfo` SET `iprId`='{0}', `type`='{1}', `shortName`='{2}', `proteinCount`='{3}', `iprName`='{4}' WHERE `iprId` = '{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, iprId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, iprId, type, shortName, proteinCount, iprName)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, iprId, type, shortName, proteinCount, iprName)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, iprId, type, shortName, proteinCount, iprName, iprId)
    End Function
#End Region
End Class


End Namespace
