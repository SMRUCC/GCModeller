Namespace Framework.Provider

    Public Module DefaultEntity

        <LinqEntity("integer", GetType(Integer))>
        Public Function GetInt32(uri As String) As IEnumerable
            Dim lines As String() = IO.File.ReadAllLines(uri)
            Dim LQuery = (From line As String In lines Select Scripting.CastInteger(line))
            Return LQuery
        End Function

        <LinqEntity("long", GetType(Long))>
        Public Function GetInt64(uri As String) As IEnumerable
            Dim lines As String() = IO.File.ReadAllLines(uri)
            Dim LQuery = (From line As String In lines Select Scripting.CastLong(line))
            Return LQuery
        End Function

        <LinqEntity("double", GetType(Double))>
        Public Function GetDouble(uri As String) As IEnumerable
            Dim lines As String() = IO.File.ReadAllLines(uri)
            Dim LQuery = lines.Select(AddressOf Val)
            Return LQuery
        End Function
    End Module
End Namespace