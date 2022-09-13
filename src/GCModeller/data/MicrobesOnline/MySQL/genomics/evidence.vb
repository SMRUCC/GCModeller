#Region "Microsoft.VisualBasic::6adde0451017e4cce872916c39993fd0, GCModeller\data\MicrobesOnline\MySQL\genomics\evidence.vb"

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

    '   Total Lines: 73
    '    Code Lines: 37
    ' Comment Lines: 29
    '   Blank Lines: 7
    '     File Size: 3.41 KB


    ' Class evidence
    ' 
    '     Properties: association_id, code, dbxref_id, id, seq_acc
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
''' DROP TABLE IF EXISTS `evidence`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `evidence` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `code` varchar(8) NOT NULL DEFAULT '',
'''   `association_id` int(11) NOT NULL DEFAULT '0',
'''   `dbxref_id` int(11) NOT NULL DEFAULT '0',
'''   `seq_acc` varchar(255) DEFAULT NULL,
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `association_id` (`association_id`,`dbxref_id`,`code`),
'''   UNIQUE KEY `ev0` (`id`),
'''   UNIQUE KEY `ev5` (`id`,`association_id`),
'''   UNIQUE KEY `ev6` (`id`,`code`,`association_id`),
'''   KEY `ev1` (`association_id`),
'''   KEY `ev2` (`code`),
'''   KEY `ev3` (`dbxref_id`),
'''   KEY `ev4` (`association_id`,`code`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("evidence")>
Public Class evidence: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property id As Long
    <DatabaseField("code"), NotNull, DataType(MySqlDbType.VarChar, "8")> Public Property code As String
    <DatabaseField("association_id"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property association_id As Long
    <DatabaseField("dbxref_id"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property dbxref_id As Long
    <DatabaseField("seq_acc"), DataType(MySqlDbType.VarChar, "255")> Public Property seq_acc As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `evidence` (`code`, `association_id`, `dbxref_id`, `seq_acc`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `evidence` (`code`, `association_id`, `dbxref_id`, `seq_acc`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `evidence` WHERE `id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `evidence` SET `id`='{0}', `code`='{1}', `association_id`='{2}', `dbxref_id`='{3}', `seq_acc`='{4}' WHERE `id` = '{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, code, association_id, dbxref_id, seq_acc)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, code, association_id, dbxref_id, seq_acc)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, code, association_id, dbxref_id, seq_acc, id)
    End Function
#End Region
End Class


End Namespace
