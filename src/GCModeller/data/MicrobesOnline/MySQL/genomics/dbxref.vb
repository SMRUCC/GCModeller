#Region "Microsoft.VisualBasic::9c8f082d84780b65e2eedf4b2d9df620, GCModeller\data\MicrobesOnline\MySQL\genomics\dbxref.vb"

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

    '   Total Lines: 72
    '    Code Lines: 37
    ' Comment Lines: 28
    '   Blank Lines: 7
    '     File Size: 3.40 KB


    ' Class dbxref
    ' 
    '     Properties: id, xref_dbname, xref_desc, xref_key, xref_keytype
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
''' DROP TABLE IF EXISTS `dbxref`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `dbxref` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `xref_dbname` varchar(55) NOT NULL DEFAULT '',
'''   `xref_key` varchar(255) NOT NULL DEFAULT '',
'''   `xref_keytype` varchar(32) DEFAULT NULL,
'''   `xref_desc` varchar(255) DEFAULT NULL,
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `xref_key` (`xref_key`,`xref_dbname`),
'''   UNIQUE KEY `dx0` (`id`),
'''   KEY `dx1` (`xref_dbname`),
'''   KEY `dx2` (`xref_key`),
'''   KEY `dx3` (`id`,`xref_dbname`),
'''   KEY `dx4` (`id`,`xref_key`,`xref_dbname`),
'''   KEY `dx5` (`id`,`xref_key`)
''' ) ENGINE=MyISAM AUTO_INCREMENT=39457 DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("dbxref")>
Public Class dbxref: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property id As Long
    <DatabaseField("xref_dbname"), NotNull, DataType(MySqlDbType.VarChar, "55")> Public Property xref_dbname As String
    <DatabaseField("xref_key"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property xref_key As String
    <DatabaseField("xref_keytype"), DataType(MySqlDbType.VarChar, "32")> Public Property xref_keytype As String
    <DatabaseField("xref_desc"), DataType(MySqlDbType.VarChar, "255")> Public Property xref_desc As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `dbxref` (`xref_dbname`, `xref_key`, `xref_keytype`, `xref_desc`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `dbxref` (`xref_dbname`, `xref_key`, `xref_keytype`, `xref_desc`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `dbxref` WHERE `id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `dbxref` SET `id`='{0}', `xref_dbname`='{1}', `xref_key`='{2}', `xref_keytype`='{3}', `xref_desc`='{4}' WHERE `id` = '{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, xref_dbname, xref_key, xref_keytype, xref_desc)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, xref_dbname, xref_key, xref_keytype, xref_desc)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, xref_dbname, xref_key, xref_keytype, xref_desc, id)
    End Function
#End Region
End Class


End Namespace
