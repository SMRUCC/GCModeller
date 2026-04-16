Imports Microsoft.VisualBasic.Language.C

Namespace System

    Public Module Etc

        Public Function CurrentTimeMillis() As Double
            Return Now.Millisecond
        End Function

        Public Sub GC()
            Call FlushMemory()
        End Sub

        Public Sub arraycopy(ByVal a1 As Array, ByVal sourceIndex As Long, ByVal a2 As Array, ByVal destIndex As Long, ByVal len As Long)
            Array.Copy(a1, sourceIndex, a2, destIndex, len)
        End Sub
    End Module

    Public NotInheritable Class out
        Public Shared Sub format(strData As String, ParamArray args As Object())
            Dim str As String = sprintf(strData, (From obj In args Let strItem As String = obj.ToString Select strItem).ToArray)
            Call Console.WriteLine(str)
        End Sub

        Public Shared Sub print(ByVal v As String)
            Console.Write(v)
        End Sub

        Public Shared Sub println(ByVal v As String)
            Console.WriteLine(v)
        End Sub
    End Class

    Public Module Err
        Public Sub println(v As String)
            Call Console.Error.WriteLine(v)
            Call Console.Error.Flush()
        End Sub

        Public Sub print(s As String)
            Call Console.Error.Write(s)
            Call Console.Error.Flush()
        End Sub
    End Module

    Namespace Runtime

        Public Module Runtime

            Public Function availableProcessors() As Integer
                Return Environment.ProcessorCount
            End Function

            ''' <summary>
            ''' Returns the maximum amount Of memory that the Java virtual machine will attempt To use. If there Is no inherent limit Then the value Long.MAX_VALUE will be returned.
            ''' </summary>
            ''' <returns>the maximum amount Of memory that the virtual machine will attempt To use, measured In bytes</returns>
            Public Function maxMemory() As Long

            End Function
        End Module
    End Namespace
End Namespace