Imports System.Text.RegularExpressions

Namespace Interpolate

    Module ForeachInterpolate

        Public Sub ForeachTemplate(vbhtml As VBHtml)
            Dim templates = VBHtml.foreach.Matches(vbhtml.ToString).ToArray
            Dim str As String

            For Each template As String In templates
                str = FillTemplate(vbhtml, template)

                ' nothing and "" empty string has different meaning at here:
                '
                ' nothing means not found: the foreach variable is existed in the variables
                ' no action at here
                ' "" empty string means empty foreach collection at here, due to 
                ' the reason of no elements inside the collection, so foreach template
                ' interplote generates empty string

                If Not str Is Nothing Then
                    Call vbhtml.Replace(template, str)
                End If
            Next
        End Sub

        ReadOnly array_name As New Regex("<foreach [^>]+>", RegexOptions.IgnoreCase Or RegexOptions.Singleline Or RegexOptions.Compiled)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="vbhtml"></param>
        ''' <param name="template">
        ''' the for each template
        ''' </param>
        ''' <returns></returns>
        Private Function FillTemplate(vbhtml As VBHtml, template As String) As String
            Dim foreach As String = array_name.Match(template).Value
            Dim var As String = VBHtml.variable.Match(foreach).Value.Trim("@"c)

            If Not vbhtml.HasSymbol(var) Then
                Return Nothing
            End If


        End Function

    End Module
End Namespace