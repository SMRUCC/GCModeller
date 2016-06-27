Imports System.Text
Imports System.Xml.Serialization

Namespace Assembly.NCBI.GenBank.GBFF.Keywords

    Public Class COMMENT : Inherits KeyWord

        <XmlIgnore> Public Property Comment As String

        Public Overrides Function ToString() As String
            Return Comment
        End Function

        Public Shared Narrowing Operator CType(data As COMMENT) As String
            Return data.Comment
        End Operator

        Public Shared Widening Operator CType(s_Data As String()) As COMMENT
            If s_Data Is Nothing OrElse s_Data.Length = 0 Then _
                Return New COMMENT With {.Comment = String.Empty}

            Dim sBuilder As StringBuilder =
                New StringBuilder(Mid$(s_Data.First, BLANKS_INDEX).Trim)

            For i As Integer = 1 To s_Data.Length - 1
                sBuilder.AppendFormat(" {0}", s_Data(i).Trim)
            Next

            Return New COMMENT With {.Comment = sBuilder.ToString}
        End Operator

        ''' <summary>
        ''' This constant using for NCBI.Genbank keywords parsing.
        ''' </summary>
        ''' <remarks></remarks>
        Public Const BLANKS_INDEX As UInteger = 12
    End Class
End Namespace