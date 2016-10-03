#Region "Microsoft.VisualBasic::ecb283c5411e3933b629e891db00580a, ..\GCModeller\analysis\annoTools\DataMySql\Interpro\interpro\entry_xref.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:52:47 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Interpro.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `entry_xref`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `entry_xref` (
'''   `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `ac` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `name` varchar(70) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   PRIMARY KEY (`entry_ac`,`dbcode`,`ac`),
'''   KEY `fk_entry_xref$dbcode` (`dbcode`),
'''   CONSTRAINT `fk_entry_xref$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION,
'''   CONSTRAINT `fk_entry_xref$entry_ac` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("entry_xref", Database:="interpro")>
Public Class entry_xref: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("entry_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "9")> Public Property entry_ac As String
    <DatabaseField("dbcode"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "1")> Public Property dbcode As String
    <DatabaseField("ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "30")> Public Property ac As String
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "70")> Public Property name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `entry_xref` (`entry_ac`, `dbcode`, `ac`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `entry_xref` (`entry_ac`, `dbcode`, `ac`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `entry_xref` WHERE `entry_ac`='{0}' and `dbcode`='{1}' and `ac`='{2}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `entry_xref` SET `entry_ac`='{0}', `dbcode`='{1}', `ac`='{2}', `name`='{3}' WHERE `entry_ac`='{4}' and `dbcode`='{5}' and `ac`='{6}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, entry_ac, dbcode, ac)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, entry_ac, dbcode, ac, name)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, entry_ac, dbcode, ac, name)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, entry_ac, dbcode, ac, name, entry_ac, dbcode, ac)
    End Function
#End Region
End Class


End Namespace
