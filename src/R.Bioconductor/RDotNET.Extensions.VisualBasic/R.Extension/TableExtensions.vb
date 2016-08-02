Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic
Imports System.Text
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq

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

        Dim R As New StringBuilder()
        Dim colNames As String = c(table.First.Skip(If(skipFirst, 1, 0)).ToArray)

        R.AppendLine($"{tableName} <- matrix(c({MAT.JoinBy(",")}),ncol={ncol},byrow=TRUE);")
        R.AppendLine($"colnames({tableName}) <- {colNames}")

        Call RServer.Evaluate(R.ToString)
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

            Call $"{name} <- {cc}".ζ   ' x <- c(....)
        Next

        Call $"{var} <- data.frame({names.JoinBy(", ")})".ζ

        If rowNames IsNot Nothing Then
            Dim rows As String() = rowNames.ToArray

            If rows.Length > 0 Then
                Call $"rownames({var}) <- {c(rows)}".ζ
            End If
        End If
    End Sub

    Public Sub PushAsDataFrame(Of T)(source As IEnumerable(Of T), var As String)
        Dim schema As Dictionary(Of String, Type) = Nothing
        Reflector _
            .Save(source, schemaOut:=schema) _
            .PushAsDataFrame(var, types:=schema, typeParsing:=False)
    End Sub
End Module
