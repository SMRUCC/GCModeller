Imports System.Runtime.CompilerServices
Imports RDotNET.Extensions.VisualBasic.API
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

''' <summary>
''' Garbage Collection
''' </summary>
Public Module RDotNetGC

    Dim objects As New List(Of String)

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub Add(name As String)
        Call objects.Add(name)
    End Sub

    ''' <summary>
    ''' 主要是用于脚本化编程, 这个函数在分配了一个新的变量之后, 会将这个变量加入到GC列表之中
    ''' </summary>
    ''' <returns></returns>
    Friend Function Allocate() As String
        Dim name As String = RDotNetGC.Allocate
        Call objects.Add(name)
        Return name
    End Function

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
