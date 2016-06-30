Imports System.Runtime.CompilerServices

Namespace Interpreter.Linker.APIHandler.Alignment

    ''' <summary>
    ''' 这个模块只是用来判断函数如何重载的，数据类型的转换有系统自动完成
    ''' </summary>
    Public Module TypeEquals

        ''' <summary>
        ''' 判断两种类型是否相等
        ''' </summary>
        ''' <param name="FuncDef">当前的这个重载方法的参数定义的类型</param>
        ''' <param name="InputParam">从脚本传递进来的函数参数的类型</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function TypeEquals(FuncDef As Type, InputParam As Type) As Integer
            If FuncDef.Equals(InputParam) OrElse
                String.Equals(FuncDef.FullName, InputParam.FullName) Then  ' 为什么这个会不相等，明明都已经是同一个类型了
                Return 10000
            End If

            If FuncDef.Equals(_ObjectType) Then ' Object类型说明那个函数的参数可以接受任何类型
                Return 1000
            End If

            If InputParam.IsInheritsFrom(FuncDef) Then '继承类型具有基本类型的所有特性，函数的参数是基本类型的话，则可以使用
                Return 2000
            End If

            If InputParam.IsInterfaceInheritsFrom(FuncDef) Then
                Return 2000
            End If

            If CollectionEquals(FuncDef, InputParam) Then
                Return 500
            End If

            Return -10000 '最后实在没辙了，则只能够认为二者不相等
        End Function

        Public Function CollectionEquals(FuncDef As Type, InputParam As Type) As Boolean
            Dim BaseCollection = FuncDef.GetInterfaces
            Dim InputCollection = InputParam.GetInterfaces

            If Array.IndexOf(BaseCollection, GetType(IEnumerable)) = -1 Then Return False

            Dim baseElements = FuncDef.GenericTypeArguments

            If baseElements.Length = 1 Then

                Dim inputElement = InputParam.GetElementType '是否为数组类型
                If inputElement Is Nothing Then
                    If InputParam.GenericTypeArguments.IsNullOrEmpty Then
                        Return False
                    End If
                    inputElement = InputParam.GenericTypeArguments.First
                End If

                Dim b = TypeEquals(baseElements(Scan0), inputElement)
                Return b

            ElseIf baseElements.IsNullOrEmpty Then '假若函数参数是一个枚举的泛型，则可以被使用

                Dim baseElement = FuncDef.GetElementType
                Dim inputElement = InputParam.GetElementType

                If baseElement Is Nothing Then
                    Return True
                ElseIf inputElement Is Nothing Then  '基本类型是一个数组但是输入的参数数据不是数组，则肯定不相同
                    Return False
                Else

                    Dim b = TypeEquals(FuncDef:=baseElement, InputParam:=inputElement)
                    Return b

                End If

            End If

            Return False
        End Function

        ''' <summary>
        ''' Is a inherits from b
        ''' </summary>
        ''' <param name="a">继承类型继承自基本类型，具备有基本类型的所有特性</param>
        ''' <param name="b">基本类型</param>
        ''' <returns></returns>
        <Extension> Private Function IsInterfaceInheritsFrom(a As Type, b As Type) As Boolean
            Dim [Implements] = a.GetInterfaces

            If [Implements].IsNullOrEmpty Then
                Return False '不是接口类型，则不可以使用本函数判断
            End If

            Dim IsInherits As Boolean = Array.IndexOf([Implements], b) > -1

            Return IsInherits
        End Function

        Private ReadOnly _ObjectType As Type = GetType(System.Object)
    End Module
End Namespace