Imports Oracle.Java.IO.Properties.Reflector
Imports System.Text.RegularExpressions

Imports System.Runtime.CompilerServices
Imports System.Drawing

Namespace ExpressionMatrix

    Public Class Configuration : Inherits ConfigCommon

        Public Overrides Function Save(Optional Path As String = "", Optional encoding As System.Text.Encoding = Nothing) As Boolean
            Dim File As String = Me.ToConfigDoc
            Return File.SaveTo(Path, encoding)
        End Function
    End Class
End Namespace