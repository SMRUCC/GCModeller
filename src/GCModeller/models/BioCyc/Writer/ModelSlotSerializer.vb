Imports System.ComponentModel
Imports System.Globalization
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Data.BioCyc.Assembly.MetaCyc.Schema.Metabolism

Namespace Writer

    ''' <summary>
    ''' BioCyc 数据模型之中一个被 <see cref="AttributeField"/> 标注的属性与其数据槽位名称的映射关系
    ''' </summary>
    Public Class SlotField

        ''' <summary>
        ''' BioCyc 槽位名称，例如 ``COMMON-NAME``
        ''' </summary>
        ''' <returns></returns>
        Public Property name As String

        ''' <summary>
        ''' 对应的 .NET 属性
        ''' </summary>
        ''' <returns></returns>
        Public Property [property] As PropertyInfo

        Public Overrides Function ToString() As String
            Return $"{name} -> {[property].Name}"
        End Function
    End Class

    ''' <summary>
    ''' 将 <see cref="Model"/> 对象序列化为 BioCyc 的属性-值记录(<see cref="AttrRecord"/>)。
    ''' 
    ''' 本类型与 <see cref="ObjectWriter"/> 是严格对称的一对：<see cref="ObjectWriter"/> 依据
    ''' ``AttributeField`` 把属性-值反序列化为对象，本类型则依据同样的映射把对象还原为属性-值。
    ''' 因此在默认情况下，``Serialize`` 写出的文本可以被 ``AttrDataCollection(Of T).LoadFile`` 读回。
    ''' </summary>
    Public Class ModelSlotSerializer

        ''' <summary>
        ''' 按照基类在前、声明顺序在后的顺序排列的槽位映射
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property schema As SlotField()

        Public ReadOnly Property model As Type

        Shared ReadOnly cache As New Dictionary(Of String, ModelSlotSerializer)

        Private Sub New(type As Type)
            model = type
            schema = GetSlotFields(type)
        End Sub

        ''' <summary>
        ''' 获取指定数据模型上面的全部槽位映射(已按照稳定的顺序排序)
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        Public Shared Function GetSlotFields(type As Type) As SlotField()
            Return type _
                .GetProperties(BindingFlags.Public Or BindingFlags.Instance) _
                .Where(Function(p) p.CanRead AndAlso p.GetIndexParameters.Length = 0) _
                .Select(Function(p)
                            Return New SlotField With {
                                .name = AttributeField.getMappingKey(p),
                                .[property] = p
                            }
                        End Function) _
                .Where(Function(f) Not String.IsNullOrEmpty(f.name)) _
                .OrderBy(Function(f) inheritanceDepth(f.[property].DeclaringType)) _
                .ThenBy(Function(f) f.[property].MetadataToken) _
                .ToArray
        End Function

        ''' <summary>
        ''' 获取指定数据模型上面的全部槽位名称(用于写出文件头的 ``Attributes:`` 列表)
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        Public Shared Function GetSlotNames(type As Type) As String()
            Return GetSlotFields(type) _
                .Select(Function(f) f.name) _
                .Distinct() _
                .ToArray
        End Function

        Private Shared Function inheritanceDepth(type As Type) As Integer
            Dim depth As Integer = 0
            Dim current As Type = type

            While current IsNot Nothing
                depth += 1
                current = current.BaseType
            End While

            Return depth
        End Function

        ''' <summary>
        ''' 获取指定类型的序列化器(带缓存)
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function GetSerializer(Of T As Model)() As ModelSlotSerializer
            Return GetSerializer(GetType(T))
        End Function

        Public Shared Function GetSerializer(type As Type) As ModelSlotSerializer
            SyncLock cache
                Dim serializer As ModelSlotSerializer = Nothing

                If cache.TryGetValue(type.FullName, serializer) Then
                    Return serializer
                End If

                serializer = New ModelSlotSerializer(type)
                Call cache.Add(type.FullName, serializer)

                Return serializer
            End SyncLock
        End Function

        ''' <summary>
        ''' 将一个数据模型对象序列化为属性-值记录
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        Public Function Serialize(data As Model) As AttrRecord
            Dim record As New AttrRecord(data.uniqueId)

            For Each field As SlotField In schema
                If field.name = "UNIQUE-ID" Then
                    Continue For
                End If

                Dim value As Object = field.[property].GetValue(data)

                If value Is Nothing Then
                    Continue For
                End If

                Call writeSlot(record, field.name, value, field.[property].PropertyType)
            Next

            Return record
        End Function

        Private Shared Sub writeSlot(record As AttrRecord, name As String, value As Object, type As Type)
            If type.IsArray Then
                Dim elementType As Type = type.GetElementType

                For Each item As Object In DirectCast(value, Array)
                    If item Is Nothing Then
                        Continue For
                    End If

                    Dim text As String = FormatValue(item, elementType)

                    If String.IsNullOrEmpty(text) Then
                        Continue For
                    End If

                    Dim slot As AttrSlot = record.Add(name, text)

                    Call writeAnnotations(slot, item, elementType)
                Next
            Else
                Dim text As String = FormatValue(value, type)

                If String.IsNullOrEmpty(text) Then
                    Return
                End If

                Dim slot As AttrSlot = record.Add(name, text)

                Call writeAnnotations(slot, value, type)
            End If
        End Sub

        ''' <summary>
        ''' 复合数据类型所携带的注解，例如反应方程式的系数与区室
        ''' </summary>
        Private Shared Sub writeAnnotations(slot As AttrSlot, value As Object, type As Type)
            If type Is GetType(CompoundSpecieReference) Then
                Dim reference As CompoundSpecieReference = DirectCast(value, CompoundSpecieReference)

                Call slot.Annotate("COEFFICIENT", coefficientText(reference.Stoichiometry))
                Call slot.Annotate("COMPARTMENT", reference.Compartment)
            ElseIf type Is GetType(KineticsFactor) Then
                Dim factor As KineticsFactor = DirectCast(value, KineticsFactor)

                Call slot.Annotate("SUBSTRATE", factor.substrate)

                For Each citation As String In factor.citations.SafeQuery
                    Call slot.Annotate("CITATIONS", citation)
                Next
            End If
        End Sub

        ''' <summary>
        ''' 化学计量数的文本形式：1 视为默认值而不输出，无穷大表示未知数量 ``n``
        ''' </summary>
        ''' <param name="stoichiometry"></param>
        ''' <returns></returns>
        Public Shared Function coefficientText(stoichiometry As Double) As String
            If Double.IsInfinity(stoichiometry) Then
                Return "n"
            ElseIf stoichiometry = 1 OrElse stoichiometry = 0 Then
                Return Nothing
            Else
                Return stoichiometry.ToString("G15", CultureInfo.InvariantCulture)
            End If
        End Function

        ''' <summary>
        ''' 将一个属性值格式化为 BioCyc 平面文件之中的文本
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="type"></param>
        ''' <returns>空字符串表示该值应当被忽略</returns>
        Public Shared Function FormatValue(value As Object, type As Type) As String
            If value Is Nothing Then
                Return Nothing
            End If

            If type.IsEnum Then
                Return enumText(value)
            End If

            If type Is GetType(String) Then
                Dim str As String = DirectCast(value, String)

                If str.Trim.Length = 0 Then
                    Return Nothing
                End If

                Return str
            End If

            If type Is GetType(Boolean) Then
                ' BioCyc 使用 T 表示真值，假值直接省略该槽位
                Return If(DirectCast(value, Boolean), "T", Nothing)
            End If

            If IsNumericType(type) Then
                Return numericText(value)
            End If

            Dim ec As ECNumber = TryCast(value, ECNumber)

            If ec IsNot Nothing Then
                Return "EC-" & ec.ECNumberString
            End If

            Dim reference As CompoundSpecieReference = TryCast(value, CompoundSpecieReference)

            If reference IsNot Nothing Then
                Return reference.ID
            End If

            Dim factor As KineticsFactor = TryCast(value, KineticsFactor)

            If factor IsNot Nothing Then
                Return $"({factor.substrate} {factor.Km.ToString("G15", CultureInfo.InvariantCulture)})"
            End If

            Return value.ToString
        End Function

        Private Shared Function IsNumericType(type As Type) As Boolean
            Select Case Type.GetTypeCode(type)
                Case TypeCode.Byte, TypeCode.SByte, TypeCode.Int16, TypeCode.UInt16,
                     TypeCode.Int32, TypeCode.UInt32, TypeCode.Int64, TypeCode.UInt64,
                     TypeCode.Single, TypeCode.Double, TypeCode.Decimal
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Private Shared Function numericText(value As Object) As String
            Select Case Type.GetTypeCode(value.GetType)
                Case TypeCode.Single, TypeCode.Double, TypeCode.Decimal
                    Dim d As Double = Convert.ToDouble(value, CultureInfo.InvariantCulture)

                    If d = 0 OrElse Double.IsNaN(d) Then
                        Return Nothing
                    End If

                    Return d.ToString("G15", CultureInfo.InvariantCulture)
                Case Else
                    Dim l As Long = Convert.ToInt64(value, CultureInfo.InvariantCulture)

                    If l = 0 Then
                        Return Nothing
                    End If

                    Return l.ToString(CultureInfo.InvariantCulture)
            End Select
        End Function

        ''' <summary>
        ''' 枚举值使用 <see cref="DescriptionAttribute"/> 上面的 BioCyc 符号文本，
        ''' 例如 ``REVERSIBLE``(反应方向)或者 ``+``(转录方向)
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Private Shared Function enumText(value As Object) As String
            Dim type As Type = value.GetType
            Dim field As FieldInfo = type.GetField(value.ToString)

            If field Is Nothing Then
                Return value.ToString
            End If

            Dim description As DescriptionAttribute = field.GetCustomAttribute(Of DescriptionAttribute)()

            If description Is Nothing Then
                Return value.ToString
            End If

            Return description.Description
        End Function
    End Class

End Namespace
