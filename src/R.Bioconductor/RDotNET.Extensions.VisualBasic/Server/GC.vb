Imports System.Runtime.CompilerServices
Imports RDotNET.Extensions.VisualBasic.API
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

Public Module GC

    Dim objects As New List(Of String)

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub Add(name As String)
        Call objects.Add(name)
    End Sub

    ''' <summary>
    ''' 一次性的将R环境之中的通过<see cref="Add"/>方法所添加的对象进行删除
    ''' </summary>
    Public Sub [Do]()
        Dim names = base.c(objects.ToArray, stringVector:=True)

        Call base.rm(list:=names)
        Call base.rm(list:=names.Rstring)
        Call base.gc()
        Call objects.Clear()
    End Sub
End Module
