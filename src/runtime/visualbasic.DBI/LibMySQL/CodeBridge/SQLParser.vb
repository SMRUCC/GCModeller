#Region "Microsoft.VisualBasic::534fd02a97727e6899ef33e6f496ab16, ..\LibMySQL\CodeBridge\SQLParser.vb"

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
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

Public Module SQLParser

    ''' <summary>
    ''' Parsing the create table statement in the SQL document.
    ''' </summary>
    Const SQL_CREATE_TABLE As String = "CREATE TABLE `.+?` \(.+?(PRIMARY KEY \(`.+?`\))?.+?ENGINE=.+?;"

    Public Function ParseTable(SQL As String) As Reflection.Schema.Table
        Dim CTMatch As Match = Regex.Match(SQL, SQL_CREATE_TABLE, RegexOptions.Singleline)
        Dim Tokens As KeyValuePair(Of String, String()) =
            __sqlParser(CTMatch.Value.Replace(vbLf, vbCr))

        Try
            Return __parseTable(SQL, CTMatch, Tokens)
        Catch ex As Exception
            Dim dump As StringBuilder = New StringBuilder
            Call dump.AppendLine(SQL)
            Call dump.AppendLine(vbCrLf)
            Call dump.AppendLine(NameOf(CTMatch) & "   ====> ")
            Call dump.AppendLine(CTMatch.Value)
            Call dump.AppendLine(vbCrLf)
            Call dump.AppendLine($"TableName:={Tokens.Key}")
            Call dump.AppendLine(New String("-"c, 120))
            Call dump.AppendLine(vbCrLf)
            Call dump.AppendLine(String.Join(vbCrLf & "  >  ", Tokens.Value))

            Throw New Exception(dump.ToString, ex)
        End Try
    End Function

    Private Function __parseTable(SQL As String, CTMatch As Match, Tokens As KeyValuePair(Of String, String())) As Reflection.Schema.Table
        Dim DB As String = __getDBName(SQL)
        Dim TableName As String = Tokens.Value(Scan0)
        Dim PrimaryKey As String = Tokens.Key
        Dim FieldsTokens As String() = Tokens.Value.Skip(1).ToArray
        Dim Table As Reflection.Schema.Table =
            SetValue(Of Reflection.Schema.Table).InvokeSet(
                __createSchema(FieldsTokens,
                               TableName,
                               PrimaryKey,
                               SQL),
                NameOf(Reflection.Schema.Table.Database),
                DB)
        Return Table
    End Function

    ''' <summary>
    ''' Loading the table schema from a specific SQL doucment.
    ''' </summary>
    ''' <param name="Path"></param>
    ''' <returns></returns>
    Public Function LoadSQLDoc(Path As String) As Reflection.Schema.Table()
        Using file As New StreamReader(New FileStream(Path, FileMode.Open))
            Return file.LoadSQLDoc
        End Using
    End Function

    <Extension>
    Public Function LoadSQLDoc(stream As StreamReader, Optional ByRef raw As String = Nothing) As Reflection.Schema.Table()
        Dim doc As String = stream.ReadToEnd
        Dim DB As String = __getDBName(doc)
        Dim Tables = (From m As Match
                      In Regex.Matches(doc, SQL_CREATE_TABLE, RegexOptions.Singleline)
                      Let Tokens As KeyValuePair(Of String, String()) = __sqlParser(SQL:=m.Value)
                      Let TableName As String = Tokens.Value(Scan0)
                      Let PrimaryKey As String = Tokens.Key
                      Let FieldsTokens = Tokens.Value.Skip(1).ToArray
                      Select PrimaryKey,
                          TableName,
                          Fields = FieldsTokens,
                          Original = m.Value).ToArray
        Dim setValue = New SetValue(Of Reflection.Schema.Table)() _
            .GetSet(NameOf(Reflection.Schema.Table.Database))
        Dim SqlSchema =
            LinqAPI.Exec(Of Reflection.Schema.Table) <=
                From Table
                In Tables
                Let tbl As Reflection.Schema.Table =
                    __createSchema(
                    Table.Fields,
                    Table.TableName,
                    Table.PrimaryKey,
                    Table.Original)
                Select setValue(tbl, DB)

        raw = doc

        Return SqlSchema
    End Function

    ''' <summary>
    ''' 获取数据库的名称
    ''' </summary>
    ''' <param name="SQL"></param>
    ''' <returns></returns>
    Private Function __getDBName(SQL As String) As String
        Dim Name As String = Regex.Match(SQL, DB_NAME, RegexOptions.IgnoreCase).Value
        If String.IsNullOrEmpty(Name) Then
            Return ""
        Else
            Name = Regex.Match(Name, "`.+?`").Value
            Name = Mid(Name, 2, Len(Name) - 2)
            Return Name
        End If
    End Function

    Const DB_NAME As String = "CREATE\s+DATABASE\s+IF\s+NOT\s+EXISTS\s+`.+?`"

    Private Function __sqlParser(SQL As String) As KeyValuePair(Of String, String())
        Dim Tokens As String() = Strings.Split(SQL.Replace(vbLf, ""), vbCr)
        Dim p As Integer = Tokens.Lookup("PRIMARY KEY")
        Dim PrimaryKey As String

        If p = -1 Then ' 没有设置主键
            p = Tokens.Lookup("UNIQUE KEY")
        End If

        If p = -1 Then
            p = Tokens.Lookup("KEY")
        End If

        If p = -1 Then
            PrimaryKey = ""
        Else
_SET_PRIMARYKEY:
            PrimaryKey = Tokens(p)
            Tokens = Tokens.Take(p).ToArray
        End If

        p = Tokens.Lookup(") ENGINE=")
        If Not p = -1 Then
            Tokens = Tokens.Take(p).ToArray
        End If

        Return New KeyValuePair(Of String, String())(PrimaryKey, Tokens)
    End Function

    ''' <summary>
    ''' Create a MySQL table schema object.
    ''' </summary>
    ''' <param name="Fields"></param>
    ''' <param name="TableName"></param>
    ''' <param name="PrimaryKey"></param>
    ''' <param name="CreateTableSQL"></param>
    ''' <returns></returns>
    Private Function __createSchema(Fields As String(), TableName As String, PrimaryKey As String, CreateTableSQL As String) As Reflection.Schema.Table
        Try
            Return __createSchemaInner(Fields, TableName, PrimaryKey, CreateTableSQL)
        Catch ex As Exception
            Dim dump As StringBuilder = New StringBuilder
            Call dump.AppendLine(NameOf(CreateTableSQL))
            Call dump.AppendLine(New String("="c, 120))
            Call dump.AppendLine(CreateTableSQL)
            Call dump.AppendLine(vbCrLf)
            Call dump.AppendLine($"{NameOf(TableName)}   ===>  {TableName}")
            Call dump.AppendLine($"{NameOf(PrimaryKey)}  ===>  {PrimaryKey}")
            Call dump.AppendLine(vbCrLf)
            Call dump.AppendLine(NameOf(Fields))
            Call dump.AppendLine(New String("="c, 120))
            Call dump.AppendLine(String.Join(vbCrLf, Fields))

            Throw New Exception(dump.ToString, ex)
        End Try
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Fields"></param>
    ''' <param name="TableName"></param>
    ''' <param name="PrimaryKey"></param>
    ''' <param name="CreateTableSQL">Create table SQL raw.</param>
    ''' <returns></returns>
    Private Function __createSchemaInner(Fields As String(), TableName As String, PrimaryKey As String, CreateTableSQL As String) As Reflection.Schema.Table
        TableName = Regex.Match(TableName, "`.+?`").Value
        TableName = Mid(TableName, 2, Len(TableName) - 2)
        PrimaryKey = Regex.Match(PrimaryKey, "\(`.+?`\)").Value

        Dim PrimaryKeys As String()

        If Not String.IsNullOrEmpty(PrimaryKey) Then
            PrimaryKey = Regex.Replace(PrimaryKey, "\(\d+\)", "")
            PrimaryKey = Mid(PrimaryKey, 2, Len(PrimaryKey) - 2)
            PrimaryKey = Mid(PrimaryKey, 2, Len(PrimaryKey) - 2)
            PrimaryKeys = Strings.Split(PrimaryKey, "`,`")
        Else
            PrimaryKeys = New String() {}
        End If

        Dim Comment As String = Regex.Match(CreateTableSQL, "COMMENT='.+';", RegexOptions.Singleline).Value
        Dim FieldLQuery = (From Field As String
                           In Fields
                           Select __createField(Field)).ToDictionary(Function(Field) Field.FieldName)

        If Not String.IsNullOrEmpty(Comment) Then
            Comment = Mid(Comment, 10)
            Comment = Mid(Comment, 1, Len(Comment) - 2)
        End If

        CreateTableSQL = ASCII.ReplaceQuot(CreateTableSQL, "\'")

        Dim TableSchema As New Reflection.Schema.Table With {
            ._databaseFields = FieldLQuery,        ' The database fields reflection result {Name, Attribute}
            .TableName = TableName,
            .PrimaryFields = PrimaryKeys.ToList,   ' Assuming at least only one primary key in a table
            .Index = PrimaryKey,
            .Comment = Comment,
            .SQL = CreateTableSQL
        }
        Return TableSchema
    End Function

    ''' <summary>
    ''' Regex expression for parsing the comments of the field in a table definition.
    ''' </summary>
    Const FIELD_COMMENTS As String = "COMMENT '.+?',"

    Private Function __createField(FieldDef As String, Tokens As String()) As Reflection.Schema.Field
        Dim FieldName As String = Tokens(0)
        Dim DataType As String = Tokens(1)
        Dim Comment As String = Regex.Match(FieldDef, FIELD_COMMENTS).Value
        Dim i As Integer = InStr(FieldDef, FieldName)
        FieldDef = Mid(FieldDef, i + Len(FieldName))
        i = InStr(FieldDef, DataType)
        FieldDef = Mid(FieldDef, i + Len(DataType)).Replace(",", "").Trim
        FieldName = Mid(FieldName, 2, Len(FieldName) - 2)

        If Not String.IsNullOrEmpty(Comment) Then
            Comment = Mid(Comment, 10)
            Comment = Mid(Comment, 1, Len(Comment) - 2)
        End If

        Dim p_CommentKeyWord As Integer = InStr(FieldDef, "COMMENT '", CompareMethod.Text)
        Dim p As New int

        If p_CommentKeyWord = 0 Then  '没有注释，则百分之百就是列属性了
            p_CommentKeyWord = Integer.MaxValue
        End If

        Dim IsAutoIncrement As Boolean = (p = InStr(FieldDef, "AUTO_INCREMENT", CompareMethod.Text)) > 0 AndAlso p < p_CommentKeyWord
        Dim IsNotNull As Boolean = (p = InStr(FieldDef, "NOT NULL", CompareMethod.Text)) > 0 AndAlso p < p_CommentKeyWord

        Dim FieldSchema As New Reflection.Schema.Field With {
            .FieldName = FieldName,
            .DataType = __createDataType(DataType.Replace(",", "").Trim),  'Some data type can be merged into a same type when we mapping a database table
            .Comment = Comment,
            .AutoIncrement = IsAutoIncrement,
            .NotNull = IsNotNull
        }
        Return FieldSchema
    End Function

    Private Function __createField(FieldDef As String) As Reflection.Schema.Field
        Dim Tokens As String() = FieldDef.Trim.Split
        Try
            Return __createField(FieldDef, Tokens)
        Catch ex As Exception
            Throw New Exception($"{NameOf(__createField)} ===>  {FieldDef}{vbCrLf & vbCrLf & vbCrLf}", ex)
        End Try
    End Function

    ''' <summary>
    ''' Mapping the MySQL database type and visual basic data type 
    ''' </summary>
    ''' <param name="TypeDef"></param>
    ''' <returns></returns>
    Private Function __createDataType(TypeDef As String) As Reflection.DbAttributes.DataType
        Dim Type As Reflection.DbAttributes.MySqlDbType
        Dim Parameter As String = ""

        If Regex.Match(TypeDef, "int\(\d+\)", RegexOptions.IgnoreCase).Success Then
            Type = Reflection.DbAttributes.MySqlDbType.Int64
            Parameter = __getNumberValue(TypeDef)

        ElseIf Regex.Match(TypeDef, "varchar\(\d+\)", RegexOptions.IgnoreCase).Success OrElse Regex.Match(TypeDef, "char\(\d+\)", RegexOptions.IgnoreCase).Success Then
            Type = Reflection.DbAttributes.MySqlDbType.VarChar
            Parameter = __getNumberValue(TypeDef)

        ElseIf Regex.Match(TypeDef, "double", RegexOptions.IgnoreCase).Success OrElse InStr(TypeDef, "float") > 0 Then
            Type = Reflection.DbAttributes.MySqlDbType.Double

        ElseIf Regex.Match(TypeDef, "datetime", RegexOptions.IgnoreCase).Success OrElse
            Regex.Match(TypeDef, "date", RegexOptions.IgnoreCase).Success OrElse
            Regex.Match(TypeDef, "timestamp", RegexOptions.IgnoreCase).Success Then

            Type = Reflection.DbAttributes.MySqlDbType.DateTime

        ElseIf Regex.Match(TypeDef, "text", RegexOptions.IgnoreCase).Success Then
            Type = Reflection.DbAttributes.MySqlDbType.Text

        ElseIf InStr(TypeDef, "enum(", CompareMethod.Text) > 0 Then   ' enum类型转换为String类型？？？？
            Type = Reflection.DbAttributes.MySqlDbType.String

        ElseIf InStr(TypeDef, "Blob", CompareMethod.Text) > 0 OrElse
            Regex.Match(TypeDef, "varbinary\(\d+\)", RegexOptions.IgnoreCase).Success OrElse
            Regex.Match(TypeDef, "binary\(\d+\)", RegexOptions.IgnoreCase).Success Then
            Type = Reflection.DbAttributes.MySqlDbType.Blob

        ElseIf Regex.Match(TypeDef, "decimal\(", RegexOptions.IgnoreCase).Success Then
            Type = Reflection.DbAttributes.MySqlDbType.Decimal

        Else

            'More complex type is not support yet, but you can easily extending the mapping code at here
            Throw New NotImplementedException($"Type define is not support yet for    {NameOf(TypeDef)}   >>> ""{TypeDef}""")

        End If

        Return New Reflection.DbAttributes.DataType(Type, Parameter)
    End Function

    Private Function __getNumberValue(TypeDef As String) As String
        Dim Parameter As String = Regex.Match(TypeDef, "\(.+?\)").Value
        Parameter = Mid(Parameter, 2, Len(Parameter) - 2)
        Return Parameter
    End Function
End Module

