
Namespace Tamir.SharpSsh.java.util
    ''' <summary>
    ''' Summary description for Arrays.
    ''' </summary>
    Public Class Arrays
        Friend Shared Function equals(ByVal foo As Byte(), ByVal bar As Byte()) As Boolean
            Dim i = foo.Length
            If i <> bar.Length Then Return False

            For j = 0 To i - 1
                If foo(j) <> bar(j) Then Return False
            Next
            'try{while(true){i--; if(foo[i]!=bar[i])return false;}}catch(Exception e){}
            Return True
        End Function
    End Class
End Namespace
