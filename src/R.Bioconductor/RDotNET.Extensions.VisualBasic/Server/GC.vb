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
        Dim name As String = App.NextTempName
        Call objects.Add(name)
        Return name
    End Function

    ''' <summary>
    ''' 从GC队列之中删除所给定的变量, 因为脚本化编程的api总是自动将新的临时变量添加进入gc队列的
    ''' 所以会需要使用这个函数来移除一些不想要被删除的对象
    ''' </summary>
    ''' <param name="names"></param>
    Public Sub Exclude(ParamArray names As String())
        For Each name As String In names
            Call objects.Remove(name)
        Next
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
