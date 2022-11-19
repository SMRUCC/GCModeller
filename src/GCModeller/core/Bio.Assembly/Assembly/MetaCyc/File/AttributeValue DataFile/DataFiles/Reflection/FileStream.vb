#Region "Microsoft.VisualBasic::8a0485561e4945c7733ab1d5867a1dd5, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Reflection\FileStream.vb"

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


    ' Code Statistics:

    '   Total Lines: 372
    '    Code Lines: 237
    ' Comment Lines: 81
    '   Blank Lines: 54
    '     File Size: 17.34 KB


    '     Module FileStream
    ' 
    ' 
    '         Delegate Function
    ' 
    '             Function: [CType], (+3 Overloads) Equals, Generate, Read, SplitSlotName
    ' 
    '             Sub: GetMetaCycField, Write
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Reflection
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.MetaCyc.File.DataFiles.Reflection

    Public Module FileStream

        ''' <summary>
        ''' 数据库并发读取线程委托
        ''' </summary>
        ''' <typeparam name="TObject"></typeparam>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="File"></param>
        ''' <param name="Stream"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Delegate Function ReadingThread(Of TObject As Slots.Object, T As DataFile(Of TObject))(File As String, ByRef Stream As T) As T

        Public Sub Write(Of TObject As Slots.Object, T As DataFile(Of TObject))(File As String, ByRef Stream As T)
            Dim ItemProperties As PropertyInfo() = New PropertyInfo() {}, FieldAttributes As MetaCycField() = New MetaCycField() {}
            Dim sBuilder As StringBuilder = New StringBuilder(1024 * 1024)

            If Not Stream.DbProperty Is Nothing Then
                sBuilder.AppendLine(Stream.DbProperty.Generate)
            End If
            Call GetMetaCycField(Of TObject)(ItemProperties, FieldAttributes)
            Dim LQuery As IEnumerable(Of String) = From e As TObject In Stream.Values Select Generate(e, ItemProperties, FieldAttributes) 'AsParallel
            For Each [Object] As String In LQuery
                sBuilder.Append([Object])
            Next
            Call FileIO.FileSystem.WriteAllText(File, sBuilder.ToString, append:=False)
        End Sub

        ''' <summary>
        ''' 将一个MetaCyc数据库中的对象转换为字符串
        ''' </summary>
        ''' <typeparam name="TObject"></typeparam>
        ''' <param name="e"></param>
        ''' <param name="props"></param>
        ''' <param name="Fieldattrs"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Generate(Of TObject As Slots.Object)(e As TObject, props As PropertyInfo(), Fieldattrs As MetaCycField()) As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)

            For idx As Integer = 0 To props.Length - 1
                Dim Field As MetaCycField = Fieldattrs(idx)
                If Field.Type = MetaCycField.Types.String Then
                    Dim s As String = DirectCast(props(idx).GetValue(e), String)
                    If Not String.IsNullOrEmpty(s) Then
                        sBuilder.AppendLine(String.Format("{0} - {1}", Field.Name, s))
                    End If
                Else
                    Dim List = CType(props(idx).GetValue(e), List(Of String))
                    If List Is Nothing Then Continue For
                    For i As Integer = 0 To List.Count - 1
                        sBuilder.AppendLine(String.Format("{0} - {1}", Field.Name, List(i)))
                    Next
                End If
            Next
            sBuilder.AppendLine("//")

            Return sBuilder.ToString
        End Function

        ''' <summary>
        ''' 获取一个MetaCyc记录类型对象的所有域以及相对应的属性信息
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="PropertyInfo"></param>
        ''' <param name="FieldAttributes"></param>
        ''' <remarks></remarks>
        Public Sub GetMetaCycField(Of T As Slots.Object)(ByRef PropertyInfo As PropertyInfo(), ByRef FieldAttributes As MetaCycField())
            Dim ItemProperties As PropertyInfo() = GetType(T).GetProperties
            Dim List1 As List(Of PropertyInfo) = New List(Of PropertyInfo)
            Dim List2 As List(Of MetaCycField) = New List(Of MetaCycField)

            For Each [Property] In ItemProperties
                Dim FieldAttribute As MetaCycField = [Property].GetAttribute(Of MetaCycField)()
                If Not FieldAttribute Is Nothing Then
                    If String.IsNullOrEmpty(FieldAttribute.Name) Then
                        FieldAttribute.Name = SplitSlotName([Property].Name)
                    End If

                    List1.Add([Property])
                    List2.Add(FieldAttribute)
                End If
            Next

            PropertyInfo = List1.ToArray
            FieldAttributes = List2.ToArray
        End Sub

        Const A As Integer = Asc("A"c)
        Const Z As Integer = Asc("Z"c)

        ''' <summary>
        ''' 将字符串按照大写字母进行分割，生成符合MetaCyc字段名称格式的字符串
        ''' </summary>
        ''' <param name="SlotName"></param>
        ''' <returns></returns>
        ''' <remarks>SlotName:  SLOT-NAME</remarks>
        Private Function SplitSlotName(SlotName As String) As String
            Dim sBuilder As StringBuilder = New StringBuilder

            For Each c In SlotName
                If Asc(c) >= A AndAlso Asc(c) <= Z Then
                    sBuilder.Append("-")
                    sBuilder.Append(c)
                Else
                    sBuilder.Append(c.ToString.ToUpper)
                End If
            Next
            If sBuilder.Chars(0) = "-"c Then sBuilder.Remove(0, 1)

            Return sBuilder.ToString
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <typeparam name="TObject"></typeparam>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="file"></param>
        ''' <param name="Stream">
        ''' The stream object for output the read data, it must be construct first before call this method.
        ''' (用于输出所读取的数据的流对象，其在调用本函数之前必须被构造出来)
        ''' </param>
        ''' <remarks></remarks>
        Public Function Read(Of TObject As Slots.Object, T As DataFile(Of TObject))(file As String, ByRef Stream As T) As T
            Dim ItemProperties As PropertyInfo() = New PropertyInfo() {}, FieldAttributes As MetaCycField() = New MetaCycField() {}
            Dim FileStream As AttributeValue = AttributeValue.LoadDoc(file)
            Dim PerformenceClock = Stopwatch.StartNew

            Call Console.WriteLine("DataSize:={0} Bytes", FileIO.FileSystem.GetFileInfo(file).Length)
            Call GetMetaCycField(Of TObject)(ItemProperties, FieldAttributes)

            Dim TSchema As PropertyInfo() = (From [propertyInfo] As PropertyInfo
                                             In GetType(TObject).GetProperties(BindingFlags.Public Or BindingFlags.Instance)
                                             Where propertyInfo.PropertyType = stringTypeInfo AndAlso
                                                 propertyInfo.CanWrite
                                             Select propertyInfo).ToArray

#If DEBUG Then
            Dim LQuery = (From c As ObjectModel
                          In FileStream.Objects
                          Select [CType](Of TObject)(c, TSchema, ItemProperties, FieldAttributes)).AsList '.AsParallel
#Else
            Dim LQuery = (From c As ObjectModel
                          In FileStream.Objects.AsParallel
                          Select [CType](Of TObject)(c, TSchema, ItemProperties, FieldAttributes)).AsList  '.AsParallel
#End If
            Stream.Values = LQuery
            Stream.DbProperty = FileStream.DbProperty

            Call Console.WriteLine("DATABASE_LOAD_DATA() -> Performance {0} ms", PerformenceClock.ElapsedMilliseconds)

            Return Stream
        End Function

        ReadOnly stringTypeInfo As Type = GetType(String)

        ''' <summary>
        ''' 使用反射，将字典之中的数据赋值到相对应的属性之上
        ''' </summary>
        ''' <typeparam name="TObject"></typeparam>
        ''' <param name="om"></param>
        ''' <param name="ItemProperties"></param>
        ''' <param name="FieldAttributes"></param>
        ''' <param name="TSchema"><typeparamref name="TObject"/>的Schema的缓存</param>
        ''' <returns></returns>
        Private Function [CType](Of TObject As Slots.Object)(
                                    om As ObjectModel,
                                    TSchema As PropertyInfo(),
                                    ItemProperties As PropertyInfo(),
                                    FieldAttributes As MetaCycField()) As TObject

            Dim x As TObject = Activator.CreateInstance(Of TObject)()

            Call Slots.[Object].TypeCast(Of TObject)(ObjectModel.CreateDictionary(om), x)

            For i As Integer = 0 To TSchema.Length - 1
                Dim [Property] = TSchema(i)
                Call [Property].SetValue(x, "")
            Next

            For Index As Integer = 0 To ItemProperties.Length - 1
                Dim [Property] As PropertyInfo = ItemProperties(Index)
                Dim Field As MetaCycField = FieldAttributes(Index)

                If Field.Type = MetaCycField.Types.TStr Then
                    If [Property].PropertyType.IsArray Then
                        Call [Property].SetValue(x, x.StringQuery(Field.Name, True).ToArray)
                    Else
                        Call [Property].SetValue(x, x.StringQuery(Field.Name).AsList)
                    End If
                Else
                    If x.Exists(Field.Name) Then
                        [Property].SetValue(x, x.StringQuery(Field.Name).DefaultFirst(""))
                    End If
                End If
            Next

            Return x
        End Function

        ''' <summary>
        ''' 使用反射的方法判断两个对象的值是否相同
        ''' </summary>
        ''' <typeparam name="TObject">将要进行判断的对象的具体类型</typeparam>
        ''' <param name="objA"></param>
        ''' <param name="objB"></param>
        ''' <param name="ComparedFields">将要进行比较的域，假若该列表为空的话，则默认比较所有域，对于列表中不存在的域，则忽略</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Equals(Of TObject As Slots.Object)(objA As TObject, objB As TObject, ParamArray ComparedFields As String()) As Boolean
            Dim ItemProperties As PropertyInfo() = New PropertyInfo() {}, FieldAttributes As MetaCycField() = New MetaCycField() {}
            ComparedFields = If(ComparedFields Is Nothing, New String() {}, ComparedFields)

            Call FileStream.GetMetaCycField(Of TObject)(ItemProperties, FieldAttributes) 'Get all of the type information of the target object

            For i As Integer = 0 To ItemProperties.Count - 1
                If ComparedFields.Count > 0 AndAlso
                    Array.IndexOf(ComparedFields, FieldAttributes(i).Name) = -1 Then
                    Continue For
                End If

                Dim type As MetaCycField.Types = FieldAttributes(i).Type
                Dim [Property] As PropertyInfo = ItemProperties(i)

                If type = MetaCycField.Types.String Then
                    Dim s1 As String = DirectCast([Property].GetValue(objA), String)
                    Dim s2 As String = DirectCast([Property].GetValue(objB), String)

                    If Not String.Equals(s1, s2) Then
                        Return False
                    End If
                ElseIf type = MetaCycField.Types.TStr Then
                    Dim scl1 As List(Of String) = DirectCast([Property].GetValue(objA), List(Of String))
                    Dim scl2 As List(Of String) = DirectCast([Property].GetValue(objB), List(Of String))

                    If scl1.Count <> scl2.Count Then
                        Return False
                    Else
                        Call scl1.Sort()
                        Call scl2.Sort()

                        For j As Integer = 0 To scl1.Count - 1
                            If Not String.Equals(scl1(j), scl2(j)) Then
                                Return False
                            End If
                        Next
                    End If
                Else
                    Dim c1 As Char = DirectCast([Property].GetValue(objA), Char)
                    Dim c2 As Char = DirectCast([Property].GetValue(objB), Char)

                    If c1 <> c2 Then
                        Return False
                    End If
                End If
            Next

            Return True
        End Function

        ''' <summary>
        ''' 采用类型信息缓存机制的等价判断函数，请保证ItemProperties和FieldAttributes着两个参数的列表的数目的一致性
        ''' </summary>
        ''' <typeparam name="TObject"></typeparam>
        ''' <param name="objA"></param>
        ''' <param name="objB"></param>
        ''' <param name="ItemProperties"></param>
        ''' <param name="FieldAttributes"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Equals(Of TObject As Slots.Object)(objA As TObject, objB As TObject, ItemProperties As PropertyInfo(), FieldAttributes As MetaCycField()) As Boolean
            For i As Integer = 0 To ItemProperties.Count - 1
                Dim Type As MetaCycField.Types = FieldAttributes(i).Type
                Dim [Property] As PropertyInfo = ItemProperties(i)

                If Type = MetaCycField.Types.String Then
                    Dim s1 As String = DirectCast([Property].GetValue(objA), String)
                    Dim s2 As String = DirectCast([Property].GetValue(objB), String)

                    If Not String.Equals(s1, s2) Then
                        Return False
                    End If
                ElseIf Type = MetaCycField.Types.TStr Then
                    Dim scl1 As List(Of String) = DirectCast([Property].GetValue(objA), List(Of String))
                    Dim scl2 As List(Of String) = DirectCast([Property].GetValue(objB), List(Of String))

                    If scl1.Count <> scl2.Count Then
                        Return False
                    Else
                        Call scl1.Sort()
                        Call scl2.Sort()

                        For j As Integer = 0 To scl1.Count - 1
                            If Not String.Equals(scl1(j), scl2(j)) Then
                                Return False
                            End If
                        Next
                    End If
                Else
                    Dim c1 As Char = DirectCast([Property].GetValue(objA), Char)
                    Dim c2 As Char = DirectCast([Property].GetValue(objB), Char)

                    If c1 <> c2 Then
                        Return False
                    End If
                End If
            Next

            Return True
        End Function

        ''' <summary>
        ''' 采用类型信息缓存机制的等价判断函数，请保证ItemProperties和FieldAttributes着两个参数的列表的数目的一致性，（注意：在本函数中不允许出现空值）
        ''' </summary>
        ''' <typeparam name="TObject"></typeparam>
        ''' <param name="objA"></param>
        ''' <param name="objB"></param>
        ''' <param name="ItemProperties"></param>
        ''' <param name="FieldAttributes"></param>
        ''' <param name="OverloadsNullable">无意义的参数，任意值</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Equals(Of TObject As Slots.Object)(objA As TObject, objB As TObject, ItemProperties As PropertyInfo(), FieldAttributes As MetaCycField(), OverloadsNullable As Boolean?) As Boolean
            For i As Integer = 0 To ItemProperties.Count - 1
                Dim Type As MetaCycField.Types = FieldAttributes(i).Type
                Dim [Property] As PropertyInfo = ItemProperties(i)

                If Type = MetaCycField.Types.String Then
                    Dim s1 As String = DirectCast([Property].GetValue(objA), String)
                    Dim s2 As String = DirectCast([Property].GetValue(objB), String)

                    If (String.IsNullOrEmpty(s1) OrElse String.IsNullOrEmpty(s2)) OrElse Not String.Equals(s1, s2) Then
                        Return False
                    End If
                ElseIf Type = MetaCycField.Types.TStr Then
                    Dim scl1 As List(Of String) = DirectCast([Property].GetValue(objA), List(Of String))
                    Dim scl2 As List(Of String) = DirectCast([Property].GetValue(objB), List(Of String))

                    If (scl1.Count = 0 OrElse scl2.Count = 0) OrElse scl1.Count <> scl2.Count Then
                        Return False
                    Else
                        Call scl1.Sort()
                        Call scl2.Sort()

                        For j As Integer = 0 To scl1.Count - 1
                            If Not String.Equals(scl1(j), scl2(j)) Then
                                Return False
                            End If
                        Next
                    End If
                Else
                    Dim c1 As Char = DirectCast([Property].GetValue(objA), Char)
                    Dim c2 As Char = DirectCast([Property].GetValue(objB), Char)

                    If (c2 = "" OrElse c2 = "") OrElse c1 <> c2 Then
                        Return False
                    End If
                End If
            Next

            Return True
        End Function
    End Module
End Namespace
