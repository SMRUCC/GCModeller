REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2018/5/23 13:13:37


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace LocalMySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `xref_pathway_modules`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `xref_pathway_modules` (
'''   `pathway` int(11) NOT NULL,
'''   `module` int(11) NOT NULL,
'''   `KO` varchar(45) DEFAULT NULL,
'''   `name` varchar(45) DEFAULT NULL,
'''   PRIMARY KEY (`pathway`,`module`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("xref_pathway_modules", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `xref_pathway_modules` (
  `pathway` int(11) NOT NULL,
  `module` int(11) NOT NULL,
  `KO` varchar(45) DEFAULT NULL,
  `name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`pathway`,`module`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class xref_pathway_modules: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pathway"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="pathway"), XmlAttribute> Public Property pathway As Long
    <DatabaseField("module"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="module"), XmlAttribute> Public Property [module] As Long
    <DatabaseField("KO"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="KO")> Public Property KO As String
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="name")> Public Property name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `xref_pathway_modules` (`pathway`, `module`, `KO`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `xref_pathway_modules` (`pathway`, `module`, `KO`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `xref_pathway_modules` (`pathway`, `module`, `KO`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `xref_pathway_modules` (`pathway`, `module`, `KO`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `xref_pathway_modules` WHERE `pathway`='{0}' and `module`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `xref_pathway_modules` SET `pathway`='{0}', `module`='{1}', `KO`='{2}', `name`='{3}' WHERE `pathway`='{4}' and `module`='{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `xref_pathway_modules` WHERE `pathway`='{0}' and `module`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, pathway, [module])
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `xref_pathway_modules` (`pathway`, `module`, `KO`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pathway, [module], KO, name)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `xref_pathway_modules` (`pathway`, `module`, `KO`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, pathway, [module], KO, name)
        Else
        Return String.Format(INSERT_SQL, pathway, [module], KO, name)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{pathway}', '{[module]}', '{KO}', '{name}')"
        Else
            Return $"('{pathway}', '{[module]}', '{KO}', '{name}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `xref_pathway_modules` (`pathway`, `module`, `KO`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pathway, [module], KO, name)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `xref_pathway_modules` (`pathway`, `module`, `KO`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, pathway, [module], KO, name)
        Else
        Return String.Format(REPLACE_SQL, pathway, [module], KO, name)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `xref_pathway_modules` SET `pathway`='{0}', `module`='{1}', `KO`='{2}', `name`='{3}' WHERE `pathway`='{4}' and `module`='{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pathway, [module], KO, name, pathway, [module])
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As xref_pathway_modules
                         Return DirectCast(MyClass.MemberwiseClone, xref_pathway_modules)
                     End Function
End Class


End Namespace
