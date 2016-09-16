#Region "Microsoft.VisualBasic::8f02ce2bc0328325a9e91d6c6ea2f37c, ..\R.Bioconductor\RDotNET.Extensions.VisualBasic\R.Extension\TableExtensions.vb"

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

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

Public Module TableExtensions

    ''' <summary>
    ''' Push the table data in the VisualBasic into R system.
    ''' </summary>
    ''' <param name="table"></param>
    ''' <param name="tableName"></param>
    ''' <param name="skipFirst">
    ''' If the first column is the rows name, and you don't want these names, then you should set this as TRUE to skips this data.
    ''' </param>
    <Extension>
    Public Sub PushAsTable(table As DocumentStream.File, tableName As String, Optional skipFirst As Boolean = True)
        Dim MAT As New List(Of String)
        Dim ncol As Integer

        For Each row In table.Skip(1)
            If skipFirst Then
                MAT += row.Skip(1)
            Else
                MAT += row.ToArray
            End If

            If ncol = 0 Then
                ncol = MAT.Count
            End If
        Next

        Dim sb As New StringBuilder()
        Dim colNames As String = c(table.First.Skip(If(skipFirst, 1, 0)).ToArray)

        sb.AppendLine($"{tableName} <- matrix(c({MAT.JoinBy(",")}),ncol={ncol},byrow=TRUE);")
        sb.AppendLine($"colnames({tableName}) <- {colNames}")

        SyncLock R
            With R
                .call = sb.ToString
            End With
        End SyncLock
    End Sub

    ''' <summary>
    ''' A data frame is used for storing data tables. It is a list of vectors of equal length. 
    ''' For example, the following variable df is a data frame containing three vectors n, s, b.
    '''
    ''' ```R
    ''' n = c(2, 3, 5) 
    ''' s = c("aa", "bb", "cc") 
    ''' b = c(TRUE, FALSE, TRUE) 
    ''' df = data.frame(n, s, b)       # df Is a data frame
    ''' 
    ''' # df
    ''' #   n  s     b
    ''' # 1 2 aa  TRUE
    ''' # 2 3 bb FALSE
    ''' # 3 5 cc  TRUE
    ''' ```
    ''' </summary>
    ''' <param name="df"></param>
    ''' <param name="var"></param>
    <Extension>
    Public Sub PushAsDataFrame(df As DocumentStream.File,
                               var As String,
                               Optional types As Dictionary(Of String, Type) = Nothing,
                               Optional typeParsing As Boolean = True,
                               Optional rowNames As IEnumerable(Of String) = Nothing)

        Dim names As String() = df.First.ToArray

        df = New DocumentStream.File(df.Skip(1))
        If types Is Nothing Then
            types = New Dictionary(Of String, Type)
        End If

        SyncLock R
            With R
                For Each col As SeqValue(Of String()) In df.Columns.SeqIterator
                    Dim name As String = names(col.i)
                    Dim type As Type = If(
                        types.ContainsKey(name),
                        types(name),
                        If(typeParsing,
                           col.obj.SampleForType,
                           GetType(String)))
                    Dim cc As String

                    Select Case type
                        Case GetType(String)
                            cc = c(col.obj)
                        Case GetType(Boolean)
                            cc = c(col.obj.ToArray(AddressOf getBoolean))
                        Case Else
                            cc = c(col.obj.ToArray(Function(x) DirectCast(x, Object)))
                    End Select

                    .call = $"{name} <- {cc}"   ' x <- c(....)
                Next

                .call = $"{var} <- data.frame({names.JoinBy(", ")})"

                If rowNames IsNot Nothing Then
                    Dim rows As String() = rowNames.ToArray

                    If rows.Length > 0 Then
                        .call = $"rownames({var}) <- {c(rows)}"
                    End If
                End If
            End With
        End SyncLock
    End Sub

    ''' <summary>
    ''' Push this object collection into the R memory as dataframe object.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <param name="var"></param>
    Public Sub PushAsDataFrame(Of T)(source As IEnumerable(Of T), var As String)
        Dim schema As Dictionary(Of String, Type) = Nothing

        ' 初始化schema对象会在save函数之中完成，然后被pushasdataframe调用
        Call Reflector _
            .Save(source, schemaOut:=schema) _
            .PushAsDataFrame(var, types:=schema, typeParsing:=False)
    End Sub

    ''' <summary>
    ''' Push this object collection into the R memory as dataframe object.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <returns>Returns the temp variable name that reference to the dataframe object in R memory.</returns>
    <Extension>
    Public Function dataframe(Of T)(source As IEnumerable(Of T)) As String
        Dim tmp As String = App.NextTempName
        Call PushAsDataFrame(source, var:=tmp)
        Return tmp
    End Function
End Module

