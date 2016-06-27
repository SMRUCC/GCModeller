Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Serialization.BinaryDumping
Imports Microsoft.VisualBasic

Namespace Topologically

    <Serializable> Public Structure SeedData

        Public Seeds As String()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Function Save(path As String) As Boolean
            Return Serialize(path)
        End Function

        Public Shared Function Load(path As String) As SeedData
            Return path.Load(Of SeedData)
        End Function

        Public Shared Function Initialize(chars As Char(), max As Integer) As SeedData
            Dim seedsBuf As List(Of String) = InitializeSeeds(chars, 1).Distinct.ToList
            Dim tmp As String() = seedsBuf

            For i As Integer = 2 To max
                tmp = New List(Of String)(ExtendSequence(tmp, chars).Distinct)
                seedsBuf += tmp
                Call $"{New String("-", 20)}>  {i}bp".__DEBUG_ECHO
            Next

            Return New SeedData With {
                .Seeds = seedsBuf
            }
        End Function
    End Structure
End Namespace