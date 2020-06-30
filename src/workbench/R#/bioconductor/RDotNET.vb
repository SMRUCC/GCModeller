
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports RDotNET.Extensions.VisualBasic.API
Imports RDotNET.Extensions.VisualBasic.Serialization
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object

<Package("R")>
Module RDotNET

    ''' <summary>
    ''' push any .NET object into R runtime environment
    ''' </summary>
    ''' <param name="any"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("push")>
    Public Function push(any As Object, Optional env As Environment = Nothing) As String
        Dim codepage As Encoding = Encodings.UTF8WithoutBOM.CodePage

        If any Is Nothing Then
            Return base.c(x:=Nothing)
        ElseIf TypeOf any Is list Then
            Return SaveRda.Push(DirectCast(any, list).slots, codepage)
        ElseIf TypeOf any Is vector Then
            With DirectCast(any, vector)
                Dim vector As String = SaveRda.Push(.data, codepage)
                base.names(vector) = .getNames
                Return vector
            End With
        ElseIf TypeOf any Is vbObject Then
            Return SaveRda.Push(DirectCast(any, vbObject).target, codepage)
        ElseIf TypeOf any Is pipeline Then
            Return SaveRda.Push(DirectCast(any, pipeline).createVector(env).data, codepage)
        Else
            Return SaveRda.Push(any, codepage)
        End If
    End Function
End Module
