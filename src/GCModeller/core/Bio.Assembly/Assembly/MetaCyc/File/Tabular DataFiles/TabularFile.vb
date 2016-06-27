Imports System.Drawing
Imports System.Text

Namespace Assembly.MetaCyc.File

    ''' <summary>
    ''' Each tabular file contains data for one class of objects, such as reactions or pathways.
    ''' This type of file contains a single table of tab-delimited columns and newline-delimited
    ''' rows. The first row contains headers which describe the data beneath them. Each of the
    ''' remaining rows represents an object, and each column is an attribute of the object.
    '''
    ''' Column names that would otherwise be the same contain a number x having values 1, 2, 3,
    ''' etc. to distinguish them. Comment lines can be anywhere in the file and must begin with
    ''' the following symbol:
    '''
    ''' #
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TabularFile

        Public Property DbProperty As [Property]
        Public Property Columns As String()
        Public Property Objects As RecordLine()

        Public ReadOnly Property Size As Size
            Get
                Return New Size With {
                    .Width = Columns.Length,
                    .Height = Objects.Length
                }
            End Get
        End Property

        Public Overrides Function ToString() As String
            Dim sBuilder As StringBuilder = New StringBuilder(512)

            If String.IsNullOrEmpty(DbProperty.FileName) Then
                sBuilder.Append("Table Data")
            Else
                sBuilder.Append(DbProperty.FileName)
            End If

            For Each e As String In Columns
                sBuilder.Append(e)
                sBuilder.Append(", ")
            Next

            sBuilder.Remove(sBuilder.Length - 2, 2)
            sBuilder.AppendLine()
            For i As Integer = 0 To 2
                sBuilder.AppendLine(Objects(i).ToString)
            Next

            Return sBuilder.ToString
        End Function

        Public Shared Widening Operator CType(Path As String) As TabularFile
            Dim prop As [Property] = Nothing
            Dim File As String() = Nothing
            Dim Columns As String(), TabularFile As RecordLine()
            Dim first As String = ""

            Call FileReader.TabularParser(Path, prop, File, first).Assertion(Logging.MSG_TYPES.WRN)

            '第一行为列名行，以TAB作为分隔符
            Columns = Strings.Split(first, vbTab)
            TabularFile = (From line As String In File.AsParallel Select New RecordLine(line)).ToArray

            Return New TabularFile With {
                .Columns = Columns,
                .DbProperty = prop,
                .Objects = TabularFile
            }
        End Operator
    End Class
End Namespace