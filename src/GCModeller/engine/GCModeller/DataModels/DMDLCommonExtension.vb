Imports System.Runtime.CompilerServices

''' <summary>
''' Something commonly operation in the datamodel building procedure.
''' </summary>
''' <remarks></remarks>
Module DMDLCommonExtension

    ''' <summary>
    ''' Convert the object types definition from the stirng value into the enumerate value.
    ''' (将一个对象的字符串形式的类型定义值转换为枚举形式的定义值)
    ''' </summary>
    ''' <param name="e">待转换的目标对象</param>
    ''' <returns>
    ''' Return a collection of this type definition value.
    ''' (返回类型枚举值的集合)
    ''' </returns>
    ''' <remarks></remarks>
    <Extension> Public Function GetTypes(e As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Object) As LANS.SystemsBiology.Assembly.MetaCyc.Schema.Slots.Types()
        Dim Query As Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.MetaCyc.Schema.Slots.Types) =
            From s As String In e.Types
            Select CType(s, LANS.SystemsBiology.Assembly.MetaCyc.Schema.Slots.Types) 'Types Converting LINQ Query.
        Return Query.ToArray
    End Function
End Module
