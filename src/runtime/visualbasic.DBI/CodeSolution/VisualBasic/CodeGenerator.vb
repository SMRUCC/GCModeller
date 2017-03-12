#Region "Microsoft.VisualBasic::4875db9ec3242c834080ab84acbcc57b, ..\LibMySQL\CodeBridge\CodeGenerator.vb"

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

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace VisualBasic

    ''' <summary>
    ''' Automatically generates visualbasic source code from the SQL schema dump document.(根据SQL文档生成Visual Basic源代码)
    ''' </summary>
    ''' <remarks></remarks>
    Public Module CodeGenerator

        Const VBKeywords As String = "|AddHandler|AddressOf|Alias|And|AndAlso|As|Boolean|ByRef|Byte||Call|Case|Catch|CBool|CByte|CChar|CDate|CDec|CDbl|Char|CInt|Class|CLng|CObj|Const|Continue|CSByte|CShort|CSng|CStr|" &
                                 "|CType|CUInt|CULng|CUShort|Date|Decimal|Declare|Default|Delegate|Dim|DirectCast|Do|Double|Each|Else|ElseIf|End|EndIf|Enum|Erase|Error|Event|Exit|False|Finally|For|Friend|Function|Get|" &
                                 "|GetType|GetXMLNamespace|Global|GoSub|GoTo|Handles|If|Implements|Imports|In|Inherits|Integer|Interface|Is|IsNot|Let|Lib|Like|Long|Loop|Me|Mod|Module|MustInherit|MustOverride|MyBase|MyClass|" &
                                 "|Namespace|Narrowing|New|Next|Not|Nothing|NotInheritable|NotOverridable|Object|Of|On|Operator|Option|Optional|Or|OrElse|Overloads|Overridable|Overrides|ParamArray|Partial|Private|Property|" &
                                 "|Protected|Public|RaiseEvent|ReadOnly|ReDim|REM|RemoveHandler|Resume|Return|SByte|Select|Set|Shadows|Shared|Short|Single|Static|Step|Stop|String|Structure|Sub|SyncLock|Then|Throw|To|True|" &
                                 "|Try|TryCast|TypeOf|Variant|Wend|UInteger|ULong|UShort|Using|When|While|Widening|With|WithEvents|WriteOnly|Xor|"

        ''' <summary>
        ''' Works with the conflicts of the VisualBasic keyword.(处理VB里面的关键词的冲突)
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Function TrimKeyword(name As String) As String
            name = name.Replace("-", "_")  ' mysql之中允许在名称中使用-，但是vb并不允许，在这里替换掉

            If InStr(VBKeywords, $"|{name.ToLower}|", CompareMethod.Text) > 0 Then
                Return $"[{name}]"
            Else
                Return name
            End If
        End Function

        ''' <summary>
        ''' 生成Class代码
        ''' </summary>
        ''' <param name="SQL"></param>
        ''' <param name="[Namesapce]"></param>
        ''' <returns></returns>
        Public Function GenerateClass(SQL As String, [Namesapce] As String) As KeyValuePair
            Dim Table As Reflection.Schema.Table = SQLParser.ParseTable(SQL)
            Dim vb As String = CodeGenerator.GenerateCode({Table}, Namesapce)

            Return New KeyValuePair With {
            .Key = Table.TableName,
            .Value = vb
        }
        End Function

#Region "Mapping the MySQL database type and visual basic data type"

        ''' <summary>
        ''' 将MySQL类型枚举转换为VisualBasic之中的数据类型定义
        ''' </summary>
        ''' <param name="TypeDef"></param>
        ''' <returns></returns>
        Private Function __toDataType(TypeDef As Reflection.DbAttributes.DataType) As String
            Select Case TypeDef.MySQLType

                Case Reflection.DbAttributes.MySqlDbType.BigInt,
                 Reflection.DbAttributes.MySqlDbType.Int16,
                 Reflection.DbAttributes.MySqlDbType.Int24,
                 Reflection.DbAttributes.MySqlDbType.Int32,
                 Reflection.DbAttributes.MySqlDbType.MediumInt
                    Return " As Integer"
                Case Reflection.DbAttributes.MySqlDbType.Bit,
                 Reflection.DbAttributes.MySqlDbType.Byte
                    Return " As Byte"
                Case Reflection.DbAttributes.MySqlDbType.Date,
                 Reflection.DbAttributes.MySqlDbType.DateTime
                    Return " As Date"
                Case Reflection.DbAttributes.MySqlDbType.Decimal
                    Return " As Decimal"
                Case Reflection.DbAttributes.MySqlDbType.Double,
                 Reflection.DbAttributes.MySqlDbType.Float
                    Return " As Double"
                Case Reflection.DbAttributes.MySqlDbType.Int64
                    Return " As Long"
                Case Reflection.DbAttributes.MySqlDbType.UByte
                    Return " As UByte"
                Case Reflection.DbAttributes.MySqlDbType.UInt16,
                 Reflection.DbAttributes.MySqlDbType.UInt24,
                 Reflection.DbAttributes.MySqlDbType.UInt32
                    Return " As UInteger"
                Case Reflection.DbAttributes.MySqlDbType.UInt64
                    Return " As ULong"
                Case Reflection.DbAttributes.MySqlDbType.LongText,
                 Reflection.DbAttributes.MySqlDbType.MediumText,
                 Reflection.DbAttributes.MySqlDbType.String,
                 Reflection.DbAttributes.MySqlDbType.Text,
                 Reflection.DbAttributes.MySqlDbType.TinyText,
                 Reflection.DbAttributes.MySqlDbType.VarChar,
                 Reflection.DbAttributes.MySqlDbType.VarString
                    Return " As String"

                Case Reflection.DbAttributes.MySqlDbType.Blob,
                 Reflection.DbAttributes.MySqlDbType.LongBlob,
                 Reflection.DbAttributes.MySqlDbType.MediumBlob,
                 Reflection.DbAttributes.MySqlDbType.TinyBlob
                    Return " As Byte()"

                Case Else
                    Throw New NotImplementedException($"{NameOf(TypeDef)}={TypeDef.ToString}")
            End Select
        End Function
#End Region

        ''' <summary>
        ''' Convert each table schema into a visualbasic class object definition.
        ''' </summary>
        ''' <param name="SqlDoc"></param>
        ''' <returns></returns>
        Public Function GenerateCode(SqlDoc As IEnumerable(Of Reflection.Schema.Table), Optional [Namespace] As String = "") As String
            Return __generateCode(SqlDoc, "", "", Nothing, [Namespace])
        End Function

        Const SCHEMA_SECTIONS As String = "-- Table structure for table `.+?`"

        ''' <summary>
        ''' Generate the source code file from the table schema dumping
        ''' </summary>
        ''' <param name="SqlDoc"></param>
        ''' <param name="Head"></param>
        ''' <param name="FileName"></param>
        ''' <param name="TableSql"></param>
        ''' <returns></returns>
        Private Function __generateCode(SqlDoc As IEnumerable(Of Reflection.Schema.Table),
                                    Head As String,
                                    FileName As String,
                                    TableSql As Dictionary(Of String, String),
                                    [Namespace] As String) As String

            Dim VbCodeGenerator As New StringBuilder(1024)
            Dim haveNamespace As Boolean = Not String.IsNullOrEmpty([Namespace])

            Call VbCodeGenerator.AppendLine($"REM  {GetType(CodeGenerator).FullName}")
            Call VbCodeGenerator.AppendLine($"REM  Microsoft VisualBasic MYSQL")
            Call VbCodeGenerator.AppendLine()
            Call VbCodeGenerator.AppendLine($"' SqlDump= {FileName}")
            Call VbCodeGenerator.AppendLine()
            Call VbCodeGenerator.AppendLine()

            If TableSql Is Nothing Then
                TableSql = New Dictionary(Of String, String)
            End If

            Dim Tokens As String() = Strings.Split(Head.Replace(vbLf, ""), vbCr)
            For Each Line As String In Tokens
                Call VbCodeGenerator.AppendLine("' " & Line)
            Next

            Call VbCodeGenerator.AppendLine()
            Call VbCodeGenerator.AppendLine("Imports " & AttributesNs)
            Call VbCodeGenerator.AppendLine()

            If haveNamespace Then
                Call VbCodeGenerator.AppendLine($"Namespace {[Namespace]}")
            End If

            For Each Line As String In From Table As Reflection.Schema.Table
                                   In SqlDoc
                                       Let SqlDef As String =
                                       If(TableSql.ContainsKey(Table.TableName),
                                       TableSql(Table.TableName),
                                       "")
                                       Select GenerateTableClass(Table, SqlDef)

                Call VbCodeGenerator.AppendLine()
                Call VbCodeGenerator.AppendLine(Line)
                Call VbCodeGenerator.AppendLine()
            Next

            If haveNamespace Then
                Call VbCodeGenerator.AppendLine("End Namespace")
            End If

            Return VbCodeGenerator.ToString
        End Function

        Private ReadOnly Property AttributesNs As String = GetType(Reflection.DbAttributes.MySqlDbType).FullName.Replace(".MySqlDbType", "")
        Private ReadOnly Property InheritsAbstract As String = GetType(SQLTable).FullName

        ''' <summary>
        ''' Generate the class object definition to mapping a table in the mysql database.
        ''' </summary>
        ''' <param name="Table"></param>
        ''' <param name="DefSql"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GenerateTableClass(Table As Reflection.Schema.Table, DefSql As String, Optional TrimAutoIncrement As Boolean = True) As String
            Dim Tokens As String() = Strings.Split(DefSql.Replace(vbLf, ""), vbCr)
            Dim CodeGenerator As New StringBuilder("''' <summary>" & vbCrLf)
            Dim DBName As String = Table.Database
            Dim refConflict As Boolean = Not String.IsNullOrEmpty((From field As String
                                                               In Table.FieldNames
                                                                   Where String.Equals(field, "datatype", StringComparison.OrdinalIgnoreCase)
                                                                   Select field).FirstOrDefault)
            If Not String.IsNullOrEmpty(DBName) Then
                DBName = $", Database:=""{DBName}"""
            End If
            If Not String.IsNullOrEmpty(Table.SQL) Then
                DBName &= $", {NameOf(TableName.SchemaSQL)}:=""{vbCrLf}{Table.SQL}"""
            End If

            Call CodeGenerator.AppendLine("''' ```SQL")
            If Not String.IsNullOrEmpty(Table.Comment) Then
                Call CodeGenerator.AppendLine("''' " & Table.Comment)
            End If

            For Each Line As String In Tokens
                Call CodeGenerator.AppendLine("''' " & Line)
            Next
            Call CodeGenerator.AppendLine("''' ```")
            Call CodeGenerator.AppendLine("''' </summary>")
            Call CodeGenerator.AppendLine("''' <remarks></remarks>")

            Call CodeGenerator.AppendLine($"<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName(""{Table.TableName}""{DBName})>")
            Call CodeGenerator.AppendLine($"Public Class {TrimKeyword(Table.TableName)}: Inherits {InheritsAbstract}")
            Call CodeGenerator.AppendLine("#Region ""Public Property Mapping To Database Fields""")
            For Each Field As Reflection.Schema.Field In Table.Fields

                If Not String.IsNullOrEmpty(Field.Comment) Then
                    Call CodeGenerator.AppendLine("''' <summary>")
                    Call CodeGenerator.AppendLine("''' " & Field.Comment)
                    Call CodeGenerator.AppendLine("''' </summary>")
                    Call CodeGenerator.AppendLine("''' <value></value>")
                    Call CodeGenerator.AppendLine("''' <returns></returns>")
                    Call CodeGenerator.AppendLine("''' <remarks></remarks>")
                End If

                Call CodeGenerator.Append(__createAttribute(Field, IsPrimaryKey:=Table.PrimaryFields.Contains(Field.FieldName))) 'Apply the custom attribute on the property 
                Call CodeGenerator.Append("Public Property " & TrimKeyword(Field.FieldName))                                     'Generate the property name 
                Call CodeGenerator.Append(__toDataType(Field.DataType))                                                          'Generate the property data type
                Call CodeGenerator.AppendLine()
            Next
            Call CodeGenerator.AppendLine("#End Region")

            Dim SQLlist As New Dictionary(Of String, Value(Of String)) From {
            {"INSERT", New Value(Of String)},
            {"REPLACE", New Value(Of String)},
            {"DELETE", New Value(Of String)},
            {"UPDATE", New Value(Of String)}
        }

            Call CodeGenerator.AppendLine("#Region ""Public SQL Interface""")
            Call CodeGenerator.AppendLine("#Region ""Interface SQL""")
            Call CodeGenerator.AppendLine(___INSERT_SQL(Table, TrimAutoIncrement, SQLlist("INSERT")))
            Call CodeGenerator.AppendLine(___REPLACE_SQL(Table, TrimAutoIncrement, SQLlist("REPLACE")))
            Call CodeGenerator.AppendLine(___DELETE_SQL(Table, SQLlist("DELETE")))
            Call CodeGenerator.AppendLine(___UPDATE_SQL(Table, SQLlist("UPDATE")))
            Call CodeGenerator.AppendLine("#End Region")
            Call CodeGenerator.Append(SQLlist("DELETE").SQLComments)
            Call CodeGenerator.AppendLine("    Public Overrides Function GetDeleteSQL() As String")
            Call CodeGenerator.AppendLine(___DELETE_SQL_Invoke(Table, refConflict))
            Call CodeGenerator.AppendLine("    End Function")
            Call CodeGenerator.Append(SQLlist("INSERT").SQLComments)
            Call CodeGenerator.AppendLine("    Public Overrides Function GetInsertSQL() As String")
            Call CodeGenerator.AppendLine(___INSERT_SQL_Invoke(Table, TrimAutoIncrement, refConflict))
            Call CodeGenerator.AppendLine("    End Function")
            Call CodeGenerator.Append(SQLlist("REPLACE").SQLComments)
            Call CodeGenerator.AppendLine("    Public Overrides Function GetReplaceSQL() As String")
            Call CodeGenerator.AppendLine(___REPLACE_SQL_Invoke(Table, TrimAutoIncrement, refConflict))
            Call CodeGenerator.AppendLine("    End Function")
            Call CodeGenerator.Append(SQLlist("UPDATE").SQLComments)
            Call CodeGenerator.AppendLine("    Public Overrides Function GetUpdateSQL() As String")
            Call CodeGenerator.AppendLine(___UPDATE_SQL_Invoke(Table, refConflict))
            Call CodeGenerator.AppendLine("    End Function")
            Call CodeGenerator.AppendLine("#End Region")

            Call CodeGenerator.AppendLine("End Class")

            Return CodeGenerator.ToString
        End Function

        ''' <summary>
        ''' ```SQL
        ''' ....
        ''' ```
        ''' </summary>
        ''' <param name="sql"></param>
        ''' <returns></returns>
        <Extension>
        Private Function SQLComments(sql As Value(Of String)) As String
            Dim sb As New StringBuilder
            Call sb.AppendLine("''' <summary>")
            Call sb.AppendLine("''' ```SQL")
            Call sb.AppendLine("''' " & sql.ToString)
            Call sb.AppendLine("''' ```")
            Call sb.AppendLine("''' </summary>")

            Return sb.ToString
        End Function

#Region "INSERT_SQL 假若有列是被标记为自动增长的，则不需要在INSERT_SQL之中在添加他的值了"

        Private Function ___REPLACE_SQL(Schema As Reflection.Schema.Table, TrimAutoIncrement As Boolean, ByRef SQL As Value(Of String)) As String
            Return __replaceInsertCommon(Schema, TrimAutoIncrement, True, SQL)
        End Function

        Private Function ___REPLACE_SQL_Invoke(Schema As Reflection.Schema.Table, TrimAutoIncrement As Boolean, refConflict As Boolean) As String
            Return __replaceInsertInvokeCommon(Schema, TrimAutoIncrement, True, refConflict)
        End Function

        Private Function __replaceInsertCommon(Schema As Reflection.Schema.Table,
                                           TrimAutoIncrement As Boolean,
                                           isReplace As Boolean,
                                           ByRef SQL As Value(Of String)) As String
            Dim Name As String = If(isReplace, "REPLACE", "INSERT")
            Dim SqlBuilder As New StringBuilder($"    Private Shared ReadOnly {Name}_SQL As String = <SQL>%s</SQL>")
            SQL.value = Reflection.SQL.SqlGenerateMethods.GenerateInsertSql(Schema, TrimAutoIncrement)

            If isReplace Then
                SQL = SQL.value.Replace("INSERT INTO", "REPLACE INTO")
            End If

            Call SqlBuilder.Replace("%s", SQL.ToString)

            Return SqlBuilder.ToString
        End Function

        Private Function __replaceInsertInvokeCommon(Schema As Reflection.Schema.Table,
                                                 TrimAutoIncrement As Boolean,
                                                 Replace As Boolean,
                                                 refConflict As Boolean) As String
            Dim SqlBuilder As StringBuilder = New StringBuilder("        ")
            Dim Name As String = If(Replace, "REPLACE", "INSERT")
            Call SqlBuilder.Append($"Return String.Format({Name}_SQL, ")
            If Not TrimAutoIncrement Then
                Call SqlBuilder.Append(String.Join(", ", (From Field In Schema.Fields Select __getExprInvoke(Field, refConflict)).ToArray))
            Else
                Call SqlBuilder.Append(String.Join(", ", (From Field In Schema.Fields Where Not Field.AutoIncrement Select __getExprInvoke(Field, refConflict)).ToArray))
            End If
            Call SqlBuilder.Append(")")

            Return SqlBuilder.ToString
        End Function

        Private Function ___INSERT_SQL(Schema As Reflection.Schema.Table, TrimAutoIncrement As Boolean, ByRef SQL As Value(Of String)) As String
            Return __replaceInsertCommon(Schema, TrimAutoIncrement, False, SQL)
        End Function

        Private Function ___INSERT_SQL_Invoke(Schema As Reflection.Schema.Table, TrimAutoIncrement As Boolean, refConflict As Boolean) As String
            Return __replaceInsertInvokeCommon(Schema, TrimAutoIncrement, False, refConflict)
        End Function
#End Region

        Private Function ___UPDATE_SQL(Schema As Reflection.Schema.Table, ByRef SQL As Value(Of String)) As String
            Dim SqlBuilder As New StringBuilder("    Private Shared ReadOnly UPDATE_SQL As String = <SQL>%s</SQL>")
            SQL.value = Reflection.SQL.SqlGenerateMethods.GenerateUpdateSql(Schema)
            Call SqlBuilder.Replace("%s", SQL.value)

            Return SqlBuilder.ToString
        End Function

        Private Function ___UPDATE_SQL_Invoke(Schema As Reflection.Schema.Table, refConflict As Boolean) As String
            Dim PrimaryKeys = Schema.GetPrimaryKeyFields

            If PrimaryKeys.IsNullOrEmpty Then
                Return "        " & $"Throw New NotImplementedException(""Table key was Not found, unable To generate {NameOf(___UPDATE_SQL_Invoke)} automatically, please edit this Function manually!"")"
            End If

            Dim SqlBuilder As StringBuilder = New StringBuilder("        ")
            Call SqlBuilder.Append("Return String.Format(UPDATE_SQL, ")
            Call SqlBuilder.Append(String.Join(", ", (From Field In Schema.Fields Select __getExprInvoke(Field, refConflict)).ToArray))
            Call SqlBuilder.Append(", " & String.Join(", ", PrimaryKeys.ToArray(Function(field) __getExprInvoke(field, refConflict))) & ")")

            Return SqlBuilder.ToString
        End Function

        Private Function ___DELETE_SQL(Schema As Reflection.Schema.Table, ByRef SQL As Value(Of String)) As String
            Dim SqlBuilder As StringBuilder = New StringBuilder("    Private Shared ReadOnly DELETE_SQL As String = <SQL>%s</SQL>")
            SQL.value = Reflection.SQL.SqlGenerateMethods.GenerateDeleteSql(Schema)
            Call SqlBuilder.Replace("%s", SQL.value)

            Return SqlBuilder.ToString
        End Function

        Private Function ___DELETE_SQL_Invoke(Schema As Reflection.Schema.Table, refConflict As Boolean) As String
            Try
                Dim SqlBuilder As String
                Dim PrimaryKeys As Reflection.Schema.Field() = Schema.GetPrimaryKeyFields
                Dim refInvoke As String = String.Join(", ", PrimaryKeys.ToArray(Function(field) __getExprInvoke(field, refConflict)))

                If PrimaryKeys.IsNullOrEmpty Then
                    GoTo NO_KEY
                End If

                SqlBuilder = $"Return String.Format(DELETE_SQL, {refInvoke})"
                SqlBuilder = "        " & SqlBuilder

                Return SqlBuilder.ToString
            Catch ex As Exception
                Call App.LogException(ex)
            End Try
NO_KEY:
            Return "        " & $"Throw New NotImplementedException(""Table key was Not found, unable To generate {NameOf(___DELETE_SQL_Invoke)} automatically, please edit this Function manually!"")"
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Field"></param>
        ''' <returns></returns>
        Private Function __getExprInvoke(Field As Reflection.Schema.Field, dtype_conflicts As Boolean) As String
            If Field.DataType.MySQLType = Reflection.DbAttributes.MySqlDbType.Date OrElse
                Field.DataType.MySQLType = Reflection.DbAttributes.MySqlDbType.DateTime Then
                If dtype_conflicts Then
                    Dim ref As String = GetType(Reflection.DbAttributes.DataType).FullName
                    Return $"{ref}.ToMySqlDateTimeString({TrimKeyword(Field.FieldName)})"
                Else
                    Return $"DataType.ToMySqlDateTimeString({TrimKeyword(Field.FieldName)})"
                End If
            Else
                Return TrimKeyword(Field.FieldName)
            End If
        End Function

        ReadOnly DataTypeFullNamesapce As String = GetType(Reflection.DbAttributes.MySqlDbType).Name

        Private Function __createAttribute(Field As Reflection.Schema.Field, IsPrimaryKey As Boolean) As String
            Dim Code As String = $"    <DatabaseField(""{Field.FieldName}"")"

            If IsPrimaryKey Then
                Code &= ", PrimaryKey"
            End If
            If Field.AutoIncrement Then
                Code &= ", AutoIncrement"
            End If
            If Field.NotNull Then
                Code &= ", NotNull"
            End If

            Code &= $", DataType({DataTypeFullNamesapce}.{Field.DataType.MySQLType.ToString}{If(String.IsNullOrEmpty(Field.DataType.ParameterValue), "", ", """ & Field.DataType.ParameterValue & """")})"
            Code &= "> "
            Return Code
        End Function

        ''' <summary>
        ''' Convert the sql definition into the visualbasic source code.
        ''' </summary>
        ''' <param name="SqlDump">The SQL dumping file path.(Dump sql文件的文件路径)</param>
        ''' <returns>VisualBasic source code</returns>
        ''' <remarks></remarks>
        Public Function GenerateCode(SqlDump As String, Optional [Namespace] As String = "") As String
            Return GenerateCode(New StreamReader(New FileStream(SqlDump, FileMode.Open)), [Namespace], SqlDump)
        End Function

        Public Function GenerateCode(file As StreamReader,
                                 Optional [Namespace] As String = "",
                                 Optional path As String = Nothing) As String

            Dim SqlDump As String = ""
            Dim Schema As Reflection.Schema.Table() = SQLParser.LoadSQLDoc(file, SqlDump)
            Dim CreateTables As String() = Regex.Split(SqlDump, SCHEMA_SECTIONS)
            Dim SchemaSQLLQuery = From tbl As String
                              In CreateTables.Skip(1)           ' The first block of the text splits is the SQL comments from the MySQL data exporter. 
                                  Let s_TableName As String = Regex.Match(tbl, "`.+?`").Value
                                  Select tableName = Mid(s_TableName, 2, Len(s_TableName) - 2),
                                  tbl
            Dim SchemaSQL As Dictionary(Of String, String) =
            SchemaSQLLQuery.ToDictionary(Function(x) x.tableName,
                                         Function(x) x.tbl)

            Return __generateCode(Schema,
                              Head:=CreateTables.First,
                              FileName:=FileIO.FileSystem.GetFileInfo(path).Name,
                              TableSql:=SchemaSQL,
                              [Namespace]:=[Namespace])
        End Function

        ''' <summary>
        ''' 返回 {类名, 类定义}
        ''' </summary>
        ''' <returns></returns>
        ''' <param name="SqlDump">The SQL dumping file path.(Dump sql文件的文件路径)</param>
        ''' <param name="ns">The namespace of the source code classes</param>
        Public Function GenerateCodeSplit(SqlDump As String, Optional ns As String = "") As Dictionary(Of String, String)
            Return GenerateCodeSplit(New StreamReader(New FileStream(SqlDump, FileMode.Open)), ns, SqlDump)
        End Function

        ''' <summary>
        ''' 返回 {类名, 类定义}
        ''' </summary>
        ''' <returns></returns>
        ''' <param name="file">The SQL dumping file path.(Dump sql文件的文件路径)</param>
        ''' <param name="ns">The namespace of the source code classes</param>
        Public Function GenerateCodeSplit(file As StreamReader, Optional ns As String = "", Optional path As String = Nothing) As Dictionary(Of String, String)
            Dim sqlDump As String = Nothing
            Dim Schema As Reflection.Schema.Table() = SQLParser.LoadSQLDoc(file, sqlDump)
            Dim CreateTables As String() = Regex.Split(sqlDump, SCHEMA_SECTIONS)
            Dim SchemaSQLLQuery = From tbl As String
                              In CreateTables.Skip(1)          ' The first block of the text splits is the SQL comments from the MySQL data exporter. 
                                  Let s_TableName As String = Regex.Match(tbl, "`.+?`").Value
                                  Select tableName = Mid(s_TableName, 2, Len(s_TableName) - 2),
                                  tbl
            Dim SchemaSQL As Dictionary(Of String, String) = Nothing
            Try
                SchemaSQL = SchemaSQLLQuery.ToDictionary(Function(x) x.tableName,
                                                     Function(x) x.tbl)
            Catch ex As Exception
                Dim g = SchemaSQLLQuery.ToArray.CheckDuplicated(Of String)(Function(x) x.tableName)
                Dim dupliTables As String =
                String.Join(", ", g.ToArray(Function(tb) tb.Tag))
                Throw New Exception("Duplicated tables:  " & dupliTables, ex)
            End Try

            Return __generateCodeSplit(Schema,
                                   Head:=CreateTables.First,
                                   FileName:=If(path.FileExists, FileIO.FileSystem.GetFileInfo(path).Name, ""),
                                   TableSql:=SchemaSQL,
                                   [Namespace]:=ns)
        End Function

        ''' <summary>
        ''' Generate the source code file from the table schema dumping
        ''' </summary>
        ''' <param name="SqlDoc"></param>
        ''' <param name="Head"></param>
        ''' <param name="FileName"></param>
        ''' <param name="TableSql"></param>
        ''' <returns></returns>
        Private Function __generateCodeSplit(SqlDoc As IEnumerable(Of Reflection.Schema.Table),
                                         Head As String,
                                         FileName As String,
                                         TableSql As Dictionary(Of String, String),
                                         [Namespace] As String) As Dictionary(Of String, String)

            Dim haveNamespace As Boolean = Not String.IsNullOrEmpty([Namespace])

            If TableSql Is Nothing Then
                TableSql = New Dictionary(Of String, String)
            End If

            Dim ClassList = (From Table As Reflection.Schema.Table
                         In SqlDoc
                             Let SqlDef As String = If(TableSql.ContainsKey(Table.TableName), TableSql(Table.TableName), "")
                             Select ClassDef = GenerateTableClass(Table, SqlDef), Table).ToArray
            Dim LQuery = (From Table
                      In ClassList.AsParallel
                          Select Table.Table,
                          doc = GenerateSingleDocument(haveNamespace, [Namespace], Table.ClassDef)).ToArray
            Return LQuery.ToDictionary(Function(x) x.Table.TableName,
                                   Function(x) x.doc)
        End Function

        Private Function __schemaDb(DbName As String, ns As String) As String
            Dim classDef As String =
            $"Public MustInherits Class {DbName}: Inherits {GetType(SQLTable).FullName}" & vbCrLf &
            $"" & vbCrLf &
            $"End Class"
            Return GenerateSingleDocument(Not String.IsNullOrEmpty(ns), ns, classDef)
        End Function

        Private Function GenerateSingleDocument(haveNamespace As Boolean, [Namespace] As String, ClassDef As String) As String
            Dim VbCodeGenerator As StringBuilder = New StringBuilder(1024)

            Call VbCodeGenerator.AppendLine($"REM  {GetType(CodeGenerator).FullName}")
            Call VbCodeGenerator.AppendLine($"REM  MYSQL Schema Mapper")
            Call VbCodeGenerator.AppendLine($"REM      for Microsoft VisualBasic.NET {GetType(CodeGenerator).ModuleVersion}")
            Call VbCodeGenerator.AppendLine()
            Call VbCodeGenerator.AppendLine($"REM  Dump @{Now.ToString}")
            Call VbCodeGenerator.AppendLine()

            Call VbCodeGenerator.AppendLine()
            Call VbCodeGenerator.AppendLine("Imports " & GetType(Reflection.DbAttributes.MySqlDbType).FullName.Replace(".MySqlDbType", ""))
            Call VbCodeGenerator.AppendLine()

            If haveNamespace Then
                Call VbCodeGenerator.AppendLine($"Namespace {[Namespace]}")
            End If

            Call VbCodeGenerator.AppendLine()
            Call VbCodeGenerator.AppendLine(ClassDef)
            Call VbCodeGenerator.AppendLine()

            If haveNamespace Then
                Call VbCodeGenerator.AppendLine("End Namespace")
            End If

            Return VbCodeGenerator.ToString
        End Function
    End Module
End Namespace