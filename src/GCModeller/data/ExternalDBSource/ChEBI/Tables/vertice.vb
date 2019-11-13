#Region "Microsoft.VisualBasic::93a9567391823eb1f87709b7de57b727, data\ExternalDBSource\ChEBI\Tables\vertice.vb"

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

    ' Class vertice
    ' 
    '     Properties: compound_id, id, ontology_id, vertice_ref
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

REM  Dump @2018/5/23 13:13:39


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace ChEBI.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `vertice`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `vertice` (
'''   `id` int(11) NOT NULL,
'''   `vertice_ref` varchar(60) NOT NULL,
'''   `compound_id` int(11) DEFAULT NULL,
'''   `ontology_id` int(11) NOT NULL,
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `UNIQUE_ONTOLOGY_REF` (`vertice_ref`,`ontology_id`),
'''   KEY `ontology_id` (`ontology_id`),
'''   CONSTRAINT `FK_VERTICE_TO_ONTOLOGY` FOREIGN KEY (`ontology_id`) REFERENCES `ontology` (`id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' /*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
''' 
''' /*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
''' /*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
''' /*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
''' /*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
''' /*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
''' /*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
''' /*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
''' 
''' -- Dump completed on 2015-10-22 16:20:17
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("vertice", Database:="chebi", SchemaSQL:="
CREATE TABLE `vertice` (
  `id` int(11) NOT NULL,
  `vertice_ref` varchar(60) NOT NULL,
  `compound_id` int(11) DEFAULT NULL,
  `ontology_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `UNIQUE_ONTOLOGY_REF` (`vertice_ref`,`ontology_id`),
  KEY `ontology_id` (`ontology_id`),
  CONSTRAINT `FK_VERTICE_TO_ONTOLOGY` FOREIGN KEY (`ontology_id`) REFERENCES `ontology` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class vertice: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("vertice_ref"), NotNull, DataType(MySqlDbType.VarChar, "60"), Column(Name:="vertice_ref")> Public Property vertice_ref As String
    <DatabaseField("compound_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="compound_id")> Public Property compound_id As Long
    <DatabaseField("ontology_id"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="ontology_id")> Public Property ontology_id As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `vertice` (`id`, `vertice_ref`, `compound_id`, `ontology_id`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `vertice` (`id`, `vertice_ref`, `compound_id`, `ontology_id`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `vertice` (`id`, `vertice_ref`, `compound_id`, `ontology_id`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `vertice` (`id`, `vertice_ref`, `compound_id`, `ontology_id`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `vertice` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `vertice` SET `id`='{0}', `vertice_ref`='{1}', `compound_id`='{2}', `ontology_id`='{3}' WHERE `id` = '{4}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `vertice` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `vertice` (`id`, `vertice_ref`, `compound_id`, `ontology_id`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, vertice_ref, compound_id, ontology_id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `vertice` (`id`, `vertice_ref`, `compound_id`, `ontology_id`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, vertice_ref, compound_id, ontology_id)
        Else
        Return String.Format(INSERT_SQL, id, vertice_ref, compound_id, ontology_id)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{vertice_ref}', '{compound_id}', '{ontology_id}')"
        Else
            Return $"('{id}', '{vertice_ref}', '{compound_id}', '{ontology_id}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `vertice` (`id`, `vertice_ref`, `compound_id`, `ontology_id`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, id, vertice_ref, compound_id, ontology_id)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `vertice` (`id`, `vertice_ref`, `compound_id`, `ontology_id`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, vertice_ref, compound_id, ontology_id)
        Else
        Return String.Format(REPLACE_SQL, id, vertice_ref, compound_id, ontology_id)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `vertice` SET `id`='{0}', `vertice_ref`='{1}', `compound_id`='{2}', `ontology_id`='{3}' WHERE `id` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, vertice_ref, compound_id, ontology_id, id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As vertice
                         Return DirectCast(MyClass.MemberwiseClone, vertice)
                     End Function
End Class


End Namespace
