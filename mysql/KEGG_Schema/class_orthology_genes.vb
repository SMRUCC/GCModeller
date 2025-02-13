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
''' DROP TABLE IF EXISTS `class_orthology_genes`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `class_orthology_genes` (
'''   `uid` int(11) NOT NULL,
'''   `orthology` varchar(45) NOT NULL,
'''   `locus_tag` varchar(45) NOT NULL,
'''   `organism` varchar(45) NOT NULL,
'''   PRIMARY KEY (`uid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("class_orthology_genes", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `class_orthology_genes` (
  `uid` int(11) NOT NULL,
  `orthology` varchar(45) NOT NULL,
  `locus_tag` varchar(45) NOT NULL,
  `organism` varchar(45) NOT NULL,
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class class_orthology_genes: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="uid"), XmlAttribute> Public Property uid As Long
    <DatabaseField("orthology"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="orthology")> Public Property orthology As String
    <DatabaseField("locus_tag"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="locus_tag")> Public Property locus_tag As String
    <DatabaseField("organism"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="organism")> Public Property organism As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `class_orthology_genes` (`uid`, `orthology`, `locus_tag`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `class_orthology_genes` (`uid`, `orthology`, `locus_tag`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `class_orthology_genes` (`uid`, `orthology`, `locus_tag`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `class_orthology_genes` (`uid`, `orthology`, `locus_tag`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `class_orthology_genes` WHERE `uid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `class_orthology_genes` SET `uid`='{0}', `orthology`='{1}', `locus_tag`='{2}', `organism`='{3}' WHERE `uid` = '{4}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `class_orthology_genes` WHERE `uid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, uid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `class_orthology_genes` (`uid`, `orthology`, `locus_tag`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, uid, orthology, locus_tag, organism)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `class_orthology_genes` (`uid`, `orthology`, `locus_tag`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, uid, orthology, locus_tag, organism)
        Else
        Return String.Format(INSERT_SQL, uid, orthology, locus_tag, organism)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{uid}', '{orthology}', '{locus_tag}', '{organism}')"
        Else
            Return $"('{uid}', '{orthology}', '{locus_tag}', '{organism}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `class_orthology_genes` (`uid`, `orthology`, `locus_tag`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, uid, orthology, locus_tag, organism)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `class_orthology_genes` (`uid`, `orthology`, `locus_tag`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, uid, orthology, locus_tag, organism)
        Else
        Return String.Format(REPLACE_SQL, uid, orthology, locus_tag, organism)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `class_orthology_genes` SET `uid`='{0}', `orthology`='{1}', `locus_tag`='{2}', `organism`='{3}' WHERE `uid` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, orthology, locus_tag, organism, uid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As class_orthology_genes
                         Return DirectCast(MyClass.MemberwiseClone, class_orthology_genes)
                     End Function
End Class


End Namespace
