Imports Microsoft.VisualBasic

Partial Class Guid

    Public Class UidF

        Dim UniqueId As String

        ''' <summary>
        ''' Write to the serials number section.
        ''' </summary>
        ''' <remarks></remarks>
        Friend Uid2 As String
        Public Overrides Function ToString() As String
            Return UniqueId
        End Function

        ''' <summary>
        ''' 根据MetaCyc数据库中的对象的UniqueId属性的值生成一个新的Id对象
        ''' </summary>
        ''' <param name="s"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Generate(s As String) As UidF
            s = s.GetMd5Hash
            Dim Id As List(Of Char) = New List(Of Char)

            For i As Integer = 0 To 32 - 1 Step 4
                Id.Add(s(i))
            Next

            Return New UidF With {.UniqueId = Id.Take(4).ToArray, .Uid2 = Id.Skip(4).ToArray}
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="s"></param>
        ''' <returns></returns>
        ''' <remarks>从Guid对象之中进行解析</remarks>
        Public Shared Widening Operator CType(s As String) As UidF
            Return New UidF With {.UniqueId = s}
        End Operator

        Public Shared Narrowing Operator CType(e As UidF) As String
            Return e.UniqueId
        End Operator
    End Class
End Class
