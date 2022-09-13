#Region "Microsoft.VisualBasic::a8f156a28cc6dc3c5126f85ed572764c, GCModeller\data\MicrobesOnline\MySQL\genomics\tree.vb"

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

    '   Total Lines: 67
    '    Code Lines: 37
    ' Comment Lines: 23
    '   Blank Lines: 7
    '     File Size: 3.21 KB


    ' Class tree
    ' 
    '     Properties: modified, name, newick, treeId, type
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
''' DROP TABLE IF EXISTS `tree`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `tree` (
'''   `treeId` int(10) unsigned NOT NULL AUTO_INCREMENT,
'''   `name` varchar(64) NOT NULL DEFAULT '',
'''   `type` varchar(30) NOT NULL DEFAULT '',
'''   `modified` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
'''   `newick` longblob,
'''   PRIMARY KEY (`treeId`),
'''   KEY `name` (`name`),
'''   KEY `type` (`type`,`name`)
''' ) ENGINE=MyISAM AUTO_INCREMENT=5962896 DEFAULT CHARSET=latin1 MAX_ROWS=1000000000 AVG_ROW_LENGTH=10000;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("tree")>
Public Class tree: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("treeId"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property treeId As Long
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "64")> Public Property name As String
    <DatabaseField("type"), NotNull, DataType(MySqlDbType.VarChar, "30")> Public Property type As String
    <DatabaseField("modified"), NotNull, DataType(MySqlDbType.DateTime)> Public Property modified As Date
    <DatabaseField("newick"), DataType(MySqlDbType.Blob)> Public Property newick As Byte()
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `tree` (`name`, `type`, `modified`, `newick`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `tree` (`name`, `type`, `modified`, `newick`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `tree` WHERE `treeId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `tree` SET `treeId`='{0}', `name`='{1}', `type`='{2}', `modified`='{3}', `newick`='{4}' WHERE `treeId` = '{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, treeId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, name, type, DataType.ToMySqlDateTimeString(modified), newick)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, name, type, DataType.ToMySqlDateTimeString(modified), newick)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, treeId, name, type, DataType.ToMySqlDateTimeString(modified), newick, treeId)
    End Function
#End Region
End Class


End Namespace
