#Region "Microsoft.VisualBasic::153b62405d5c8f0a59bb2821317b80a7, data\ExternalDBSource\MetaCyc\MySQL\datasethierarchy.vb"

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

    ' Class datasethierarchy
    ' 
    '     Properties: SubWID, SuperWID
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

REM  Dump @2018/5/23 13:13:40


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace MetaCyc.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `datasethierarchy`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `datasethierarchy` (
'''   `SuperWID` bigint(20) NOT NULL,
'''   `SubWID` bigint(20) NOT NULL,
'''   KEY `FK_DataSetHierarchy1` (`SuperWID`),
'''   KEY `FK_DataSetHierarchy2` (`SubWID`),
'''   CONSTRAINT `FK_DataSetHierarchy1` FOREIGN KEY (`SuperWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_DataSetHierarchy2` FOREIGN KEY (`SubWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("datasethierarchy", Database:="warehouse", SchemaSQL:="
CREATE TABLE `datasethierarchy` (
  `SuperWID` bigint(20) NOT NULL,
  `SubWID` bigint(20) NOT NULL,
  KEY `FK_DataSetHierarchy1` (`SuperWID`),
  KEY `FK_DataSetHierarchy2` (`SubWID`),
  CONSTRAINT `FK_DataSetHierarchy1` FOREIGN KEY (`SuperWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DataSetHierarchy2` FOREIGN KEY (`SubWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class datasethierarchy: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("SuperWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="SuperWID"), XmlAttribute> Public Property SuperWID As Long
    <DatabaseField("SubWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="SubWID")> Public Property SubWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `datasethierarchy` (`SuperWID`, `SubWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `datasethierarchy` (`SuperWID`, `SubWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `datasethierarchy` (`SuperWID`, `SubWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `datasethierarchy` (`SuperWID`, `SubWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `datasethierarchy` WHERE `SuperWID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `datasethierarchy` SET `SuperWID`='{0}', `SubWID`='{1}' WHERE `SuperWID` = '{2}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `datasethierarchy` WHERE `SuperWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, SuperWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `datasethierarchy` (`SuperWID`, `SubWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, SuperWID, SubWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `datasethierarchy` (`SuperWID`, `SubWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, SuperWID, SubWID)
        Else
        Return String.Format(INSERT_SQL, SuperWID, SubWID)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{SuperWID}', '{SubWID}')"
        Else
            Return $"('{SuperWID}', '{SubWID}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `datasethierarchy` (`SuperWID`, `SubWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, SuperWID, SubWID)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `datasethierarchy` (`SuperWID`, `SubWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, SuperWID, SubWID)
        Else
        Return String.Format(REPLACE_SQL, SuperWID, SubWID)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `datasethierarchy` SET `SuperWID`='{0}', `SubWID`='{1}' WHERE `SuperWID` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, SuperWID, SubWID, SuperWID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As datasethierarchy
                         Return DirectCast(MyClass.MemberwiseClone, datasethierarchy)
                     End Function
End Class


End Namespace
