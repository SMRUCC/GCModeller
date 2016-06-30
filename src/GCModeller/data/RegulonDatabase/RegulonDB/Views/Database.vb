Imports System.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace RegulonDB.Views

    Public Module Database

        Public Function SchemaParser(Of T As Class)() As PropertyInfo()
            Dim typeInfo As Type = GetType(T)
            Dim props As PropertyInfo() = typeInfo.GetProperties(BindingFlags.Public Or BindingFlags.Instance)
            Dim indexing = (From prop As PropertyInfo In props
                            Let index As Index = prop.GetCustomAttribute(Of Index)
                            Where Not index Is Nothing
                            Select index.value,
                                prop
                            Order By value Ascending).ToArray
            Return indexing.ToArray(Function(x) x.prop)
        End Function

        Public Function Load(Of T As Class)(path As String) As T()
            Dim lines As String() = IO.File.ReadAllLines(path)
            lines = (From line As String In lines
                     Where Not String.IsNullOrWhiteSpace(line) AndAlso
                         line.First <> "#"c
                     Select line).ToArray
            Dim schema As PropertyInfo() = SchemaParser(Of T)()
            Dim loadQuery = (From line As String In lines.AsParallel Select __load(Of T)(line, schema)).ToArray
            Return loadQuery
        End Function

        Private Function __load(Of T As Class)(line As String, schema As PropertyInfo()) As T
            Dim Tokens As String() = Strings.Split(line, vbTab)
            Dim obj As Object = Activator.CreateInstance(GetType(T))

            For i As Integer = 0 To schema.Length - 1
                Dim value As String = Tokens(i)
                Dim entry As PropertyInfo = schema(i)
                Call entry.SetValue(obj, value)
            Next

            Return DirectCast(obj, T)
        End Function
    End Module
End Namespace