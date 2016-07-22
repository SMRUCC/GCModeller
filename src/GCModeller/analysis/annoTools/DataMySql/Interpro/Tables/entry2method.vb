#Region "Microsoft.VisualBasic::e0098f462c7371b726356c602ddbe809, ..\GCModeller\analysis\Annotation\Interpro\Tables\entry2method.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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
''' DROP TABLE IF EXISTS `entry2method`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `entry2method` (
'''   `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `evidence` char(3) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `ida` char(1) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   PRIMARY KEY (`entry_ac`,`method_ac`),
'''   KEY `fk_entry2method$evidence` (`evidence`),
'''   KEY `fk_entry2method$method` (`method_ac`),
'''   CONSTRAINT `fk_entry2method$entry` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
'''   CONSTRAINT `fk_entry2method$evidence` FOREIGN KEY (`evidence`) REFERENCES `cv_evidence` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION,
'''   CONSTRAINT `fk_entry2method$method` FOREIGN KEY (`method_ac`) REFERENCES `method` (`method_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("entry2method", Database:="interpro")>
Public Class entry2method: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("entry_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "9")> Public Property entry_ac As String
    <DatabaseField("method_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "25")> Public Property method_ac As String
    <DatabaseField("evidence"), NotNull, DataType(MySqlDbType.VarChar, "3")> Public Property evidence As String
    <DatabaseField("ida"), DataType(MySqlDbType.VarChar, "1")> Public Property ida As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `entry2method` (`entry_ac`, `method_ac`, `evidence`, `ida`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `entry2method` (`entry_ac`, `method_ac`, `evidence`, `ida`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `entry2method` WHERE `entry_ac`='{0}' and `method_ac`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `entry2method` SET `entry_ac`='{0}', `method_ac`='{1}', `evidence`='{2}', `ida`='{3}' WHERE `entry_ac`='{4}' and `method_ac`='{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, entry_ac, method_ac)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, entry_ac, method_ac, evidence, ida)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, entry_ac, method_ac, evidence, ida)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, entry_ac, method_ac, evidence, ida, entry_ac, method_ac)
    End Function
#End Region
End Class


End Namespace

