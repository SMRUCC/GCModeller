#Region "Microsoft.VisualBasic::835378c1289fac3b930f393c976336dc, DataMySql\Interpro\Tables\method.vb"

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

    ' Class method
    ' 
    '     Properties: candidate, dbcode, method_ac, method_date, name
    '                 skip_flag
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
''' DROP TABLE IF EXISTS `method`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `method` (
'''   `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `name` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `method_date` datetime NOT NULL,
'''   `skip_flag` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL DEFAULT 'N',
'''   `candidate` char(1) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   PRIMARY KEY (`method_ac`),
'''   KEY `fk_method$dbcode` (`dbcode`),
'''   CONSTRAINT `fk_method$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("method", Database:="interpro", SchemaSQL:="
CREATE TABLE `method` (
  `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `name` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `method_date` datetime NOT NULL,
  `skip_flag` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL DEFAULT 'N',
  `candidate` char(1) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`method_ac`),
  KEY `fk_method$dbcode` (`dbcode`),
  CONSTRAINT `fk_method$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class method: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("method_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "25"), Column(Name:="method_ac"), XmlAttribute> Public Property method_ac As String
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "30"), Column(Name:="name")> Public Property name As String
    <DatabaseField("dbcode"), NotNull, DataType(MySqlDbType.VarChar, "1"), Column(Name:="dbcode")> Public Property dbcode As String
    <DatabaseField("method_date"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="method_date")> Public Property method_date As Date
    <DatabaseField("skip_flag"), NotNull, DataType(MySqlDbType.VarChar, "1"), Column(Name:="skip_flag")> Public Property skip_flag As String
    <DatabaseField("candidate"), DataType(MySqlDbType.VarChar, "1"), Column(Name:="candidate")> Public Property candidate As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `method` (`method_ac`, `name`, `dbcode`, `method_date`, `skip_flag`, `candidate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `method` (`method_ac`, `name`, `dbcode`, `method_date`, `skip_flag`, `candidate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `method` (`method_ac`, `name`, `dbcode`, `method_date`, `skip_flag`, `candidate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `method` (`method_ac`, `name`, `dbcode`, `method_date`, `skip_flag`, `candidate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `method` WHERE `method_ac` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `method` SET `method_ac`='{0}', `name`='{1}', `dbcode`='{2}', `method_date`='{3}', `skip_flag`='{4}', `candidate`='{5}' WHERE `method_ac` = '{6}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `method` WHERE `method_ac` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, method_ac)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `method` (`method_ac`, `name`, `dbcode`, `method_date`, `skip_flag`, `candidate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, method_ac, name, dbcode, MySqlScript.ToMySqlDateTimeString(method_date), skip_flag, candidate)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `method` (`method_ac`, `name`, `dbcode`, `method_date`, `skip_flag`, `candidate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, method_ac, name, dbcode, MySqlScript.ToMySqlDateTimeString(method_date), skip_flag, candidate)
        Else
        Return String.Format(INSERT_SQL, method_ac, name, dbcode, MySqlScript.ToMySqlDateTimeString(method_date), skip_flag, candidate)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{method_ac}', '{name}', '{dbcode}', '{method_date}', '{skip_flag}', '{candidate}')"
        Else
            Return $"('{method_ac}', '{name}', '{dbcode}', '{method_date}', '{skip_flag}', '{candidate}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `method` (`method_ac`, `name`, `dbcode`, `method_date`, `skip_flag`, `candidate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, method_ac, name, dbcode, MySqlScript.ToMySqlDateTimeString(method_date), skip_flag, candidate)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `method` (`method_ac`, `name`, `dbcode`, `method_date`, `skip_flag`, `candidate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, method_ac, name, dbcode, MySqlScript.ToMySqlDateTimeString(method_date), skip_flag, candidate)
        Else
        Return String.Format(REPLACE_SQL, method_ac, name, dbcode, MySqlScript.ToMySqlDateTimeString(method_date), skip_flag, candidate)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `method` SET `method_ac`='{0}', `name`='{1}', `dbcode`='{2}', `method_date`='{3}', `skip_flag`='{4}', `candidate`='{5}' WHERE `method_ac` = '{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, method_ac, name, dbcode, MySqlScript.ToMySqlDateTimeString(method_date), skip_flag, candidate, method_ac)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As method
                         Return DirectCast(MyClass.MemberwiseClone, method)
                     End Function
End Class


End Namespace
