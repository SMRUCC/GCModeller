#Region "Microsoft.VisualBasic::f1402dd878d6ecfa8aa77cdef1376d48, DataMySql\Interpro\Tables\entry2method.vb"

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

    ' Class entry2method
    ' 
    '     Properties: entry_ac, evidence, ida, method_ac
    ' 
    '     Function: Clone, GetDeleteSQL, GetDumpInsertValue, (+2 Overloads) GetInsertSQL, (+2 Overloads) GetReplaceSQL
    '               GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2018/5/23 13:13:37


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace Interpro.Tables

''' <summary>
''' ```SQL
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
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("entry2method", Database:="interpro", SchemaSQL:="
CREATE TABLE `entry2method` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `evidence` char(3) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `ida` char(1) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`entry_ac`,`method_ac`),
  KEY `fk_entry2method$evidence` (`evidence`),
  KEY `fk_entry2method$method` (`method_ac`),
  CONSTRAINT `fk_entry2method$entry` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_entry2method$evidence` FOREIGN KEY (`evidence`) REFERENCES `cv_evidence` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_entry2method$method` FOREIGN KEY (`method_ac`) REFERENCES `method` (`method_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class entry2method: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("entry_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "9"), Column(Name:="entry_ac"), XmlAttribute> Public Property entry_ac As String
    <DatabaseField("method_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "25"), Column(Name:="method_ac"), XmlAttribute> Public Property method_ac As String
    <DatabaseField("evidence"), NotNull, DataType(MySqlDbType.VarChar, "3"), Column(Name:="evidence")> Public Property evidence As String
    <DatabaseField("ida"), DataType(MySqlDbType.VarChar, "1"), Column(Name:="ida")> Public Property ida As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `entry2method` (`entry_ac`, `method_ac`, `evidence`, `ida`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `entry2method` (`entry_ac`, `method_ac`, `evidence`, `ida`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `entry2method` (`entry_ac`, `method_ac`, `evidence`, `ida`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `entry2method` (`entry_ac`, `method_ac`, `evidence`, `ida`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `entry2method` WHERE `entry_ac`='{0}' and `method_ac`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `entry2method` SET `entry_ac`='{0}', `method_ac`='{1}', `evidence`='{2}', `ida`='{3}' WHERE `entry_ac`='{4}' and `method_ac`='{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `entry2method` WHERE `entry_ac`='{0}' and `method_ac`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, entry_ac, method_ac)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `entry2method` (`entry_ac`, `method_ac`, `evidence`, `ida`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, entry_ac, method_ac, evidence, ida)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `entry2method` (`entry_ac`, `method_ac`, `evidence`, `ida`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, entry_ac, method_ac, evidence, ida)
        Else
        Return String.Format(INSERT_SQL, entry_ac, method_ac, evidence, ida)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{entry_ac}', '{method_ac}', '{evidence}', '{ida}')"
        Else
            Return $"('{entry_ac}', '{method_ac}', '{evidence}', '{ida}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `entry2method` (`entry_ac`, `method_ac`, `evidence`, `ida`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, entry_ac, method_ac, evidence, ida)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `entry2method` (`entry_ac`, `method_ac`, `evidence`, `ida`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, entry_ac, method_ac, evidence, ida)
        Else
        Return String.Format(REPLACE_SQL, entry_ac, method_ac, evidence, ida)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `entry2method` SET `entry_ac`='{0}', `method_ac`='{1}', `evidence`='{2}', `ida`='{3}' WHERE `entry_ac`='{4}' and `method_ac`='{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, entry_ac, method_ac, evidence, ida, entry_ac, method_ac)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As entry2method
                         Return DirectCast(MyClass.MemberwiseClone, entry2method)
                     End Function
End Class


End Namespace
