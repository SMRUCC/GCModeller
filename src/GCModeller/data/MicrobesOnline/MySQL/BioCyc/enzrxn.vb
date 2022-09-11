#Region "Microsoft.VisualBasic::127d2bb66d44a07fbc482d49d45f4f10, GCModeller\data\MicrobesOnline\MySQL\BioCyc\enzrxn.vb"

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

    '   Total Lines: 65
    '    Code Lines: 37
    ' Comment Lines: 21
    '   Blank Lines: 7
    '     File Size: 3.12 KB


    ' Class enzrxn
    ' 
    '     Properties: alterSubstrate, direction, enzrxnId, enzymeId, name
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:32:12 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.BioCyc

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `enzrxn`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `enzrxn` (
'''   `enzrxnId` varchar(255) NOT NULL,
'''   `alterSubstrate` text,
'''   `name` varchar(255) DEFAULT NULL,
'''   `enzymeId` varchar(255) NOT NULL,
'''   `direction` varchar(255) DEFAULT NULL,
'''   PRIMARY KEY (`enzrxnId`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("enzrxn", Database:="biocyc")>
Public Class enzrxn: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("enzrxnId"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property enzrxnId As String
    <DatabaseField("alterSubstrate"), DataType(MySqlDbType.Text)> Public Property alterSubstrate As String
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "255")> Public Property name As String
    <DatabaseField("enzymeId"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property enzymeId As String
    <DatabaseField("direction"), DataType(MySqlDbType.VarChar, "255")> Public Property direction As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `enzrxn` (`enzrxnId`, `alterSubstrate`, `name`, `enzymeId`, `direction`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `enzrxn` (`enzrxnId`, `alterSubstrate`, `name`, `enzymeId`, `direction`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `enzrxn` WHERE `enzrxnId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `enzrxn` SET `enzrxnId`='{0}', `alterSubstrate`='{1}', `name`='{2}', `enzymeId`='{3}', `direction`='{4}' WHERE `enzrxnId` = '{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, enzrxnId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, enzrxnId, alterSubstrate, name, enzymeId, direction)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, enzrxnId, alterSubstrate, name, enzymeId, direction)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, enzrxnId, alterSubstrate, name, enzymeId, direction, enzrxnId)
    End Function
#End Region
End Class


End Namespace
