﻿#Region "Microsoft.VisualBasic::ef290ccb65062207b320741898f7574e, Microsoft.VisualBasic.Core\Extensions\Reflection\Reflection.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Module EmitReflection
    ' 
    '     Function: [Get], __getValue, API, AsLambda, Category
    '               Collection2GenericIEnumerable, CreateObject, (+6 Overloads) Description, Enums, ExampleInfo
    '               FullName, GetAllEnumFlags, (+3 Overloads) GetAssemblyDetails, (+2 Overloads) GetAttribute, GetDelegateInvokeEntryPoint
    '               GetDouble, GetFullName, GetInt, (+2 Overloads) GetReadWriteProperties, GetTypeElement
    '               GetTypesHelper, (+2 Overloads) GetValue, (+2 Overloads) GetVersion, IsInheritsFrom, IsModule
    '               IsNumericType, ModuleVersion, NamespaceEntry, ResourcesSatellite, Source
    '               Usage
    ' 
    '     Sub: RunApp
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports DevAssmInfo = Microsoft.VisualBasic.ApplicationServices.Development.AssemblyInfo

''' <summary>
''' Some common used reflection operation extension at here.
''' </summary>
<Package("Emit.Reflection", Category:=APICategories.SoftwareTools, Publisher:="xie.guigang@live.com")>
Public Module EmitReflection

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function ResourcesSatellite(assembly As Assembly) As ResourcesSatellite
        Return New ResourcesSatellite(assembly)
    End Function

    ''' <summary>
    ''' 这个方法的调用是否是不需要任何参数的？
    ''' </summary>
    ''' <param name="method"></param>
    ''' <param name="optionalAsNone"></param>
    ''' <returns></returns>
    <Extension>
    Public Function IsNonParametric(method As MethodInfo, Optional optionalAsNone As Boolean = False) As Boolean
        Dim params = method.GetParameters

        If params.IsNullOrEmpty Then
            Return True
        ElseIf optionalAsNone Then
            Return Not params.Any(Function(p) Not p.IsOptional)
        End If

        Return False
    End Function

    ''' <summary>
    ''' Try to handle for the bugs in VisualBasic language: 
    ''' 
    ''' https://github.com/dotnet/roslyn/issues/23050
    ''' </summary>
    ''' <param name="assm"></param>
    ''' <returns></returns>
    Public Function GetTypesHelper(assm As Assembly) As Type()
        Try
            Return assm.GetTypes

        Catch ex As Exception When TypeOf ex Is ReflectionTypeLoadException
            Dim details = DirectCast(ex, ReflectionTypeLoadException)
            Dim msg$ = details.LoaderExceptions _
                    .Select(Function(e) e.Message) _
                    .ToArray _
                    .GetJson

            Throw New Exception(msg, ex)

        Catch ex As Exception

            Throw

        End Try
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function AsLambda(Of T)(assert As Assert(Of T)) As Func(Of T, Boolean)
        ' System.ArgumentException: '无法绑定到目标方法，因其签名或安全透明度与委托类型的签名或安全透明度不兼容。
        ' assert.Method.CreateDelegate(GetType(Func(Of T, Boolean)))
        Return Function(x) assert(x)
    End Function

    <Extension>
    Public Function Source(m As MemberInfo) As String
        Return m.DeclaringType.FullName & "::" & m.Name
    End Function

    ''' <summary>
    ''' Run external [.NET] Program from RAM Memory
    ''' </summary>
    ''' <param name="app"></param>
    ''' <param name="CLI"></param>
    ''' <param name="cs">Going to running a c# program?</param>
    ''' <remarks>
    ''' http://www.codeproject.com/Tips/1108105/Run-external-NET-Program-from-RAM-Memory
    ''' 
    ''' Run external app directly from RAM. You can load the specific file into a ``Byte[]`` Array 
    ''' with a ``StreamReader()`` or even download it from WEB via a direct link provided. 
    ''' If you loaded the file from disk, you can delete it if you want after it has been loaded 
    ''' by a ``StreamReader()``.
    ''' </remarks>
    Public Sub RunApp(app As String, Optional CLI As String = "", Optional cs As Boolean = False)
        Dim bufs As Byte() = app.GetMapPath.ReadBinary ' Works on both local file or network file. 

        Try
            Dim assm As Assembly = Assembly.Load(bufs) ' or assm = Reflection.Assembly.Load(New WebClient().DownloadData("https://...."))
            Dim method As MethodInfo = assm.EntryPoint

            If (Not method Is Nothing) Then
                Dim o As Object = assm.CreateInstance(method.Name)

                If String.IsNullOrEmpty(CLI) Then
                    Dim null As Object() = If(cs, {Nothing}, Nothing)
                    Call method.Invoke(o, null)
                Else
                    ' if your app receives parameters
                    Call method.Invoke(o, New Object() {CommandLine.GetTokens(CLI)})
                End If
            Else
                Throw New NullReferenceException($"'{app}' No App Entry Point was found!")
            End If
        Catch ex As Exception
            ex = New Exception("CLI:=" & CLI, ex)
            ex = New Exception("app:=" & app, ex)
#If DEBUG Then
            Call ex.PrintException
#End If
            Throw ex
        End Try
    End Sub

#Region "IsNumericType"
    ''' <summary>
    ''' Determines whether the specified value is of numeric type.
    ''' </summary>
    ''' <param name="o">The object to check.</param>
    ''' <returns>
    ''' true if o is a numeric type; otherwise, false.
    ''' </returns>
    Public Function IsNumericType(o As Object) As Boolean
        Return (TypeOf o Is Byte OrElse
            TypeOf o Is SByte OrElse
            TypeOf o Is Short OrElse
            TypeOf o Is UShort OrElse
            TypeOf o Is Integer OrElse
            TypeOf o Is UInteger OrElse
            TypeOf o Is Long OrElse
            TypeOf o Is ULong OrElse
            TypeOf o Is Single OrElse
            TypeOf o Is Double OrElse
            TypeOf o Is Decimal)
    End Function
#End Region

    <Extension>
    Public Function GetDouble(field As FieldInfo, Optional obj As Object = Nothing) As Double
        Return CType(field.GetValue(obj), Double)
    End Function

    <Extension>
    Public Function GetInt(field As FieldInfo, Optional obj As Object = Nothing) As Integer
        Return CType(field.GetValue(obj), Integer)
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="[nameOf]"></param>
    ''' <returns></returns>
    <Extension> Public Function API(type As Type, [nameOf] As String, Optional strict As Boolean = False) As String
#If NET_40 = 0 Then
        Dim methods = type.GetMethods(BindingFlags.Public Or BindingFlags.Static)
        Dim mBase As MethodInfo = (From m As MethodInfo In methods
                                   Where String.Equals([nameOf], m.Name)
                                   Select m).FirstOrDefault
        If mBase Is Nothing Then
NULL:       If Not strict Then
                Return [nameOf]
            Else
                Return ""
            End If
        Else
            Dim APIExport As ExportAPIAttribute = mBase.GetCustomAttribute(Of ExportAPIAttribute)
            If APIExport Is Nothing Then
                GoTo NULL
            Else
                Return APIExport.Name
            End If
        End If
#Else
        Throw New NotSupportedException
#End If
    End Function

    <ExportAPI("GET.Assembly.Details")>
    <Extension>
    Public Function GetAssemblyDetails(path As String) As DevAssmInfo
        Return Assembly.LoadFile(path).FromAssembly
    End Function

    <ExportAPI("GET.Assembly.Details")>
    <Extension>
    Public Function GetAssemblyDetails(def As Type) As DevAssmInfo
        Return def.Assembly.FromAssembly
    End Function

    <ExportAPI("GET.Assembly.Details")>
    <Extension>
    Public Function GetAssemblyDetails(assm As Assembly) As DevAssmInfo
        Return assm.FromAssembly
    End Function

    ''' <summary>
    ''' 得到集合类型的对象之中的元素类型
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="strict"></param>
    ''' <returns></returns>
    <Extension> Public Function GetTypeElement(type As Type, strict As Boolean) As Type
        If type.IsInheritsFrom(GetType(Array)) Then
            Return type.GetElementType
        End If
        If type.IsInheritsFrom(GetType(List(Of ))) Then
            Return type.GetGenericArguments.First
        End If
        If type.IsInheritsFrom(GetType(Dictionary(Of ,))) Then
            Dim keyValue As Type() = type.GetGenericArguments
            Return GetType(KeyValuePair(Of ,)).MakeGenericType(keyValue)
        End If
        If type.ImplementInterface(GetType(IEnumerable)) Then
            type = type.GetInterfaces.Where(Function(i) InStr(i.Name, "IEnumerable") = 1).First
            Return type.GenericTypeArguments.First
        End If

        If strict Then
            Return Nothing
        Else
            Throw New NotImplementedException
        End If
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="Product">.NET EXE/DLL assembly path</param>
    ''' <returns></returns>
    '''
    <ExportAPI("Get.Version")>
    Public Function GetVersion(Product As String) As Version
        Dim assm As Assembly = Assembly.LoadFile(Product)
        Return assm.GetVersion
    End Function

    ''' <summary>
    ''' 如果不存在<see cref="DescriptionAttribute"/>定义则会返回空白字符串
    ''' </summary>
    ''' <param name="prop"></param>
    ''' <returns></returns>
    <ExportAPI("Get.Description")>
    <Extension> Public Function Description(prop As PropertyInfo) As String
        Dim attrs As Object() = prop.GetCustomAttributes(GetType(DescriptionAttribute), inherit:=True)

        If attrs.IsNullOrEmpty Then
            Return ""
        Else
            Return DirectCast(attrs(Scan0), DescriptionAttribute).Description
        End If
    End Function

    ''' <summary>
    ''' Gets the <see cref="AssemblyFileVersionAttribute"/> value from the type defined assembly.
    ''' </summary>
    ''' <param name="type"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("Get.Version")>
    <Extension>
    Public Function ModuleVersion(type As Type) As String
        Return type.Assembly.GetVersion.ToString
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="assm">.NET EXE/DLL assembly</param>
    ''' <returns></returns>
    '''
    <ExportAPI("Get.Version")>
    <Extension> Public Function GetVersion(assm As Assembly) As Version
#If NET_40 = 0 Then
        Dim attrs As IEnumerable(Of CustomAttributeData) = assm.CustomAttributes
        Dim vLQuery As CustomAttributeTypedArgument =
            LinqAPI.DefaultFirst(Of CustomAttributeTypedArgument) <=
                    From attr As CustomAttributeData
                    In attrs
                    Where attr.AttributeType.Equals(GetType(AssemblyFileVersionAttribute))
                    Select value = attr.ConstructorArguments(Scan0)

        If vLQuery.Value Is Nothing Then
            Return New Version("1.0.0.0")
        Else
            Return New Version(Scripting.ToString(vLQuery.Value))
        End If
#Else
        Throw New NotSupportedException
#End If
    End Function

    ''' <summary>
    ''' 目标类型是不是VisualBasic之中的``Module``模块类型？
    ''' </summary>
    ''' <param name="typeDef"></param>
    ''' <returns></returns>
    <ExportAPI("Is.Module")>
    <Extension> Public Function IsModule(typeDef As Type) As Boolean
        If typeDef.Name.IndexOf("$") > -1 OrElse typeDef.Name.IndexOf("`") > -1 Then
            Return False ' 匿名类型
        End If

        Return typeDef.IsClass
    End Function

    ''' <summary>
    ''' 出错会返回空集合
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <typeparam name="TProperty"></typeparam>
    ''' <param name="collection"></param>
    ''' <param name="Name">使用System.NameOf()操作符来获取</param>
    ''' <returns></returns>
    <Extension> Public Function [Get](Of T, TProperty)(collection As ICollection(Of T), Name As String, Optional TrimNull As Boolean = True) As TProperty()
        Dim Type As Type = GetType(T)
        Dim Properties = (From p In Type.GetProperties(BindingFlags.Public Or BindingFlags.Instance)
                          Where String.Equals(p.Name, Name)
                          Select p).ToArray
        If Properties.IsNullOrEmpty Then
            Return New TProperty() {}
        End If

        Dim [Property] As PropertyInfo = Properties.First
        Dim resultBuffer As TProperty()

        If TrimNull Then
            resultBuffer = (From obj As T In collection.AsParallel
                            Let value As Object = [Property].GetValue(obj, Nothing)
                            Where Not value Is Nothing
                            Select DirectCast(value, TProperty)).ToArray
        Else
            resultBuffer = (From obj As T In collection.AsParallel
                            Let value As Object = [Property].GetValue(obj, Nothing)
                            Select If(value Is Nothing, Nothing, DirectCast(value, TProperty))).ToArray
        End If

        Return resultBuffer
    End Function

    ''' <summary>
    ''' Is type <paramref name="a"/> inherits from <paramref name="base"/> type?
    ''' </summary>
    ''' <param name="a">继承类型继承自基本类型，具备有基本类型的所有特性</param>
    ''' <param name="base">基本类型</param>
    ''' <param name="strict">
    ''' + 这个参数是为了解决比较来自不同的assembly文件之中的相同类型的比较，但是这个可能会在类型转换出现一些BUG
    ''' + 假若不严格要求的话，那么则两种类型相等的时候也会被算作为继承关系
    ''' + 假若是非严格判断，那么对于泛型而言，只要基本类型也相等也会被判断为成立的继承关系，这个是为了<see cref="Activity"/>操作设计的
    ''' 
    ''' </param>
    ''' <param name="depth">类型继承的距离值，当这个值越大的时候，说明二者的继承越远，当进行函数重载判断的时候，选择这个距离值越小的越好</param>
    ''' <returns></returns>
    ''' <remarks>假若两个类型是来自于不同的assembly文件的话，即使这两个类型是相同的对象，也会无法判断出来</remarks>
    <ExportAPI("Is.InheritsFrom")>
    <Extension> Public Function IsInheritsFrom(a As Type, base As Type, Optional strict As Boolean = True, Optional ByRef depth% = -1) As Boolean
        Dim baseType As Type = a.BaseType

        If Not strict Then
            ' 在这里返回结果的话，depth为-1

            If a Is base Then
                Return True
            End If

            If a.IsGenericType AndAlso base.IsGenericType Then
                ' 2017-3-12
                ' GetType(Dictionary(Of String, Double)).IsInheritsFrom(GetType(Dictionary(Of ,)))

                If a.GetGenericTypeDefinition.Equals(base) Then
                    Return True
                End If
            End If
        End If

        Do While Not baseType Is Nothing
            depth += 1

            If baseType.Equals(base) Then
                Return True
            ElseIf Not strict AndAlso (baseType.FullName = base.FullName) Then
                Return True
            Else
                baseType = baseType.BaseType
            End If
        Loop

        Return False
    End Function

    ''' <summary>
    ''' 如果有<see cref="system.ComponentModel.DescriptionAttribute"/>标记，则会返回该标记的字符串数据，假若没有则只会返回类型的名称
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    Public Function Description(Of T)() As String
        Dim typeRef As Type = GetType(T)
        Return typeRef.Description
    End Function

    ''' <summary>
    ''' 如果有<see cref="system.ComponentModel.DescriptionAttribute"/>标记，则会返回该标记的字符串数据，假若没有则只会返回类型的名称
    ''' </summary>
    ''' <returns></returns>
    '''
    <ExportAPI("Get.Description")>
    <Extension> Public Function Description(typeRef As Type) As String
        Dim CustomAttrs As Object() = typeRef.GetCustomAttributes(GetType(DescriptionAttribute), inherit:=False)

        If Not CustomAttrs.IsNullOrEmpty Then
            Return CType(CustomAttrs(Scan0), DescriptionAttribute).Description
        Else
            Return typeRef.Name
        End If
    End Function

#If FRAMEWORD_CORE Then
    ''' <summary>
    ''' Get the description data from a enum type value, if the target have no <see cref="DescriptionAttribute"></see> attribute data
    ''' then function will return the string value from the ToString() function.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("Get.Description",
               Info:="Get the description data from a enum type value, if the target have no <see cref=""DescriptionAttribute""></see> attribute data then function will return the string value from the ToString() function.")>
    <Extension> Public Function Description(value As [Enum]) As String
#Else
    ''' <summary>
    ''' Get the description data from a enum type value, if the target have no <see cref="DescriptionAttribute"></see> attribute data
    ''' then function will return the string value from the ToString() function.
    ''' </summary>
    ''' <param name="e"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function Description(value As [Enum]) As String
#End If
        Dim type As Type = value.GetType()
        Dim s As String = value.ToString
        Dim memInfos As MemberInfo() = type.GetMember(name:=s)

        If memInfos.IsNullOrEmpty Then
            Return s
        End If

        Return memInfos _
            .First _
            .Description([default]:=s)
    End Function

    ''' <summary>
    ''' 获取得到定义该类型成员之上的<see cref="DescriptionAttribute"/>值或者默认定义
    ''' </summary>
    ''' <param name="m"></param>
    ''' <param name="default$"></param>
    ''' <returns></returns>
    <Extension> Public Function Description(m As MemberInfo, Optional default$ = Nothing) As String
        Dim customAttrs() = m.GetCustomAttributes(
            GetType(DescriptionAttribute),
            inherit:=False)

        If Not customAttrs.IsNullOrEmpty Then
            Return DirectCast(customAttrs(Scan0), DescriptionAttribute).Description
        Else
            Return [default]
        End If
    End Function

    <Extension> Public Function Category(m As MemberInfo, Optional default$ = Nothing) As String
        Dim customAttrs() = m.GetCustomAttributes(
           GetType(CategoryAttribute),
           inherit:=False)

        If Not customAttrs.IsNullOrEmpty Then
            Return DirectCast(customAttrs(Scan0), CategoryAttribute).Category
        Else
            Return [default]
        End If
    End Function

    ''' <summary>
    ''' Get array value from the input flaged enum <paramref name="value"/>.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="value"></param>
    ''' <returns></returns>
    Public Function GetAllEnumFlags(Of T As Structure)(value As T) As T()
        Dim type As Type = GetType(T)
        Dim array As New List(Of T)
        Dim enumValue As [Enum] = CType(CObj(value), [Enum])

        For Each flag As [Enum] In Enums(Of T)().Select(Function(o) CType(CObj(o), [Enum]))
            If enumValue.HasFlag(flag) Then
                array += DirectCast(CObj(flag), T)
            End If
        Next

        Return array
    End Function

    ''' <summary>
    ''' Enumerate all of the enum values in the specific <see cref="System.Enum"/> type data.(只允许枚举类型，其他的都返回空集合)
    ''' </summary>
    ''' <typeparam name="T">泛型类型约束只允许枚举类型，其他的都返回空集合</typeparam>
    ''' <returns></returns>
    Public Function Enums(Of T As Structure)() As T()
        Dim EnumType As Type = GetType(T)
        If Not EnumType.IsInheritsFrom(GetType(System.Enum)) Then
            Return Nothing
        End If

        Dim EnumValues As Object() =
            Scripting _
            .CastArray(Of System.Enum)(EnumType.GetEnumValues) _
            .Select(Of Object)(Function(ar)
                                   Return DirectCast(ar, Object)
                               End Function) _
            .ToArray
        Dim values As T() = EnumValues _
            .Select(Of T)(Function([enum]) DirectCast([enum], T)) _
            .ToArray
        Return values
    End Function

    ''' <summary>
    ''' Gets all of the can read and write access property from a type define.
    ''' </summary>
    ''' <param name="type"></param>
    ''' <returns></returns>
#If FRAMEWORD_CORE Then
    <ExportAPI("Get.Properties")>
    <Extension> Public Function GetReadWriteProperties(type As Type) As PropertyInfo()
#Else
    <Extension> Public Function GetReadWriteProperties(type As System.Type) As System.Reflection.PropertyInfo()
#End If
        Dim LQuery = LinqAPI.Exec(Of PropertyInfo) <=
 _
            From p As PropertyInfo
            In type.GetProperties
            Where p.CanRead AndAlso p.CanWrite
            Select p

        Return LQuery
    End Function

    ''' <summary>
    ''' Get object usage information
    ''' </summary>
    ''' <param name="m"></param>
    ''' <returns></returns>
    <Extension> Public Function Usage(m As MemberInfo) As String
        Try
            Dim attr As UsageAttribute = m.GetCustomAttribute(Of UsageAttribute)
            Return attr.UsageInfo
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Get example code of the <see cref="Usage"/>
    ''' </summary>
    ''' <param name="m"></param>
    ''' <returns></returns>
    <Extension> Public Function ExampleInfo(m As MemberInfo) As String
        Try
            Dim attr As ExampleAttribute = m.GetCustomAttribute(Of ExampleAttribute)
            Return attr.ExampleInfo
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' 只对属性有效，出错会返回空值
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <param name="Name"></param>
    ''' <returns></returns>
    '''
    <ExportAPI("GetValue")>
    <Extension> Public Function GetValue(Type As Type, obj As Object, Name As String) As Object
        Try
            Return __getValue(Type, obj, Name)
        Catch ex As Exception
            Return App.LogException(ex, $"{GetType(Extensions).FullName}::{NameOf(GetValue)}")
        End Try
    End Function

    Private Function __getValue(Type As Type, obj As Object, Name As String) As Object
        Dim [property] = Type.GetProperty(Name, BindingFlags.Public Or BindingFlags.Instance)
        If [property] Is Nothing Then
            Return Nothing
        End If
        Dim value = [property].GetValue(obj, Nothing)
        Return value
    End Function

    ''' <summary>
    ''' 只对属性有效，出错会返回空值
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <param name="Name"></param>
    ''' <returns></returns>
    <Extension> Public Function GetValue(Of T)(Type As Type, obj As Object, Name As String) As T
        Dim value = Type.GetValue(obj, Name)
        If value Is Nothing Then
            Return Nothing
        End If
        Dim cast As T = DirectCast(value, T)
        Return cast
    End Function

#If NET_40 = 0 Then

    ''' <summary>
    ''' Try convert the type specific collection data type into a generic enumerable collection data type.(尝试将目标集合类型转换为通用的枚举集合类型)
    ''' </summary>
    ''' <param name="Type">The type specific collection data type.(特定类型的集合对象类型，当然也可以是泛型类型)</param>
    ''' <returns>If the target data type is not a collection data type then the original data type will be returns and the function displays a warning message.</returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("Collection2GenericIEnumerable", Info:="Try convert the type specific collection data type into a generic enumerable collection data type.")>
    <Extension> Public Function Collection2GenericIEnumerable(
                                                        Type As Type,
                                                        Optional DebuggerMessage As Boolean = True) As Type

        If Array.IndexOf(Type.GetInterfaces, GetType(IEnumerable)) = -1 Then
EXIT_:      If DebuggerMessage Then Call $"[WARN] Target type ""{Type.FullName}"" is not a collection type!".__DEBUG_ECHO
            Return Type
        End If

        Dim GenericType As Type = GetType(Generic.IEnumerable(Of )) 'Type.GetType("System.Collections.Generic.IEnumerable")
        Dim ElementType As Type = Type.GetElementType

        If ElementType Is Nothing Then
            Dim Generics = Type.GenericTypeArguments

            If Generics.IsNullOrEmpty Then
                GoTo EXIT_
            Else
                ElementType = Generics(Scan0)
            End If
        End If

        GenericType = GenericType.MakeGenericType({ElementType})

        Return GenericType
    End Function
#End If

    ''' <summary>
    ''' Get the method reflection entry point for a anonymous lambda expression.
    ''' (当函数返回Nothing的时候说明目标对象不是一个函数指针)
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("Delegate.GET_Invoke", Info:="Get the method reflection entry point for a anonymous lambda expression.")>
    Public Function GetDelegateInvokeEntryPoint(obj As Object) As MethodInfo
        Dim type As Type = obj.GetType
        Dim entryPoint = LinqAPI.DefaultFirst(Of MethodInfo) _
 _
            () <= From methodInfo As MethodInfo
                  In type.GetMethods
                  Where String.Equals(methodInfo.Name, "Invoke")
                  Select methodInfo

        Return entryPoint
    End Function

    ''' <summary>
    ''' Get the scripting namespace value from <see cref="[Namespace]"/>
    ''' </summary>
    ''' <param name="app"></param>
    ''' <returns></returns>
    '''
    <ExportAPI("Get.API.Namespace")>
    <Extension> Public Function NamespaceEntry(app As Type, Optional nullWrapper As Boolean = False) As [Namespace]
        Dim attr As Object() = Nothing
        Try
            attr = app.GetCustomAttributes(GetType([Namespace]), True)
        Catch ex As Exception
            Call LogException(New Exception(app.FullName, ex))
        End Try
        If attr.IsNullOrEmpty Then
            Dim descr$ = app.FullName
            If nullWrapper Then
                descr = $"< {descr} >"
            End If
            Return New [Namespace](app.Name, descr, True)
        Else
            Return DirectCast(attr(Scan0), [Namespace])
        End If
    End Function

    ''' <summary>
    ''' Gets the full name of a method reflection meta data.
    ''' </summary>
    ''' <param name="method"></param>
    ''' <param name="IncludeAssembly"></param>
    ''' <returns></returns>
    <ExportAPI("Get.FullName")>
    <Extension> Public Function GetFullName(method As MethodBase, Optional IncludeAssembly As Boolean = False) As String
        Dim Name As String = $"{method.DeclaringType.FullName}::{method.ToString}"
        If Not IncludeAssembly Then
            Return Name
        Else
            Return $"{method.DeclaringType.Module.Assembly.Location.ToFileURL}!{Name}"
        End If
    End Function

    <ExportAPI("Get.FullName")>
    <Extension> Public Function FullName(Method As MethodInfo, Optional IncludeAssembly As Boolean = False) As String
        Return GetFullName(Method, IncludeAssembly)
    End Function

    ''' <summary>
    ''' Get the specific type of custom attribute from a property.
    ''' If the target custom attribute is not declared on the target, then this function returns nothing.
    ''' (从一个属性对象中获取特定的自定义属性对象)
    ''' </summary>
    ''' <typeparam name="T">The type of the custom attribute.(自定义属性的类型)</typeparam>
    ''' <param name="Property">Target property object.(目标属性对象)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function GetAttribute(Of T As Attribute)([Property] As MemberInfo) As T
        Dim attrType As Type = GetType(T)
        Dim attrs As Object() = [Property].GetCustomAttributes(attrType, True)

        If Not attrs Is Nothing AndAlso attrs.Length = 1 Then
            Dim CustomAttr As T = CType(attrs(Scan0), T)

            If Not CustomAttr Is Nothing Then
                Return CustomAttr
            End If
        Else
            attrs = [Property].GetCustomAttributes(attrType, False)
            If Not attrs.IsNullOrEmpty Then
                Return DirectCast(attrs(Scan0), T)
            End If
        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' Get the specific type of custom attribute from a property.
    ''' (从一个属性对象中获取特定的自定义属性对象，找不到的话，就会返回空值)
    ''' </summary>
    ''' <typeparam name="T">The type of the custom attribute.(自定义属性的类型)</typeparam>
    ''' <param name="Property">Target property object.(目标属性对象)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function GetAttribute(Of T As Attribute)([Property] As PropertyInfo) As T
        Dim Attributes As Object() = [Property].GetCustomAttributes(GetType(T), True)

        If Not Attributes Is Nothing AndAlso Attributes.Length = 1 Then
            Dim CustomAttr As T = CType(Attributes(0), T)

            If Not CustomAttr Is Nothing Then
                Return CustomAttr
            End If
        End If
        Return Nothing
    End Function

#If NET_40 = 0 Then

    ''' <summary>
    '''
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="args">构造函数里面的参数信息</param>
    ''' <returns></returns>
    Public Function CreateObject(Of T)(args As Object(),
                                       Optional throwEx As Boolean = True,
                                       <CallerMemberName> Optional caller As String = "") As T
        Try
            Dim obj As Object =
                Activator.CreateInstance(GetType(T), args)
            Return DirectCast(obj, T)
        Catch ex As Exception
            Dim params As String() = args _
                .Select(Function(x) x.GetType.FullName & " ==> " & GetObjectJson(x, x.GetType)) _
                .ToArray
            ex = New Exception(String.Join(vbCrLf, params), ex)
            ex = New Exception("@" & caller, ex)

            Call App.LogException(ex)

            If throwEx Then
                Throw ex
            Else
                Return Nothing
            End If
        End Try
    End Function
#End If
End Module
