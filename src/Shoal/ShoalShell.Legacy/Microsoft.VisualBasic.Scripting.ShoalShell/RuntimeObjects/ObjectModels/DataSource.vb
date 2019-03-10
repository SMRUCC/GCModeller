Namespace Runtime.Objects.ObjectModels.DataSourceMapping

    ''' <summary>
    ''' 由于数据源的作用主要是设置共享变量或者环境参数，故而在获取参数变量的时候，系统的自由参数会被优先读取
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DataSourceMappingHandler : Inherits ShoalShell.Runtime.Objects.ObjectModels.IScriptEngineComponent

        Dim _InternalImportsDataSource As Microsoft.VisualBasic.ComponentModel.Collection.Generic.HashDictionary(Of DataSourceModel)

        Public ReadOnly Property IsEmpty As Boolean
            Get
                Return _InternalImportsDataSource.IsNullOrEmpty
            End Get
        End Property

        Sub New(ScriptEngine As ShoalShell.Runtime.Objects.ShellScript)
            Call MyBase.New(ScriptEngine)
            _InternalImportsDataSource = New ComponentModel.Collection.Generic.HashDictionary(Of DataSourceModel)(Nothing)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Name">请去除$前缀符号</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExistsVariable(Name As String) As Boolean
            Return _InternalImportsDataSource.ContainsKey(Name)
        End Function

        ''' <summary>
        ''' 目标对象不存在则会返回空值
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetValue(Name As String) As Object
            Dim Entry = _InternalImportsDataSource(Name)
            If Entry Is Nothing Then
                Return Nothing
            Else
                Return Entry.GetValue
            End If
        End Function

        Public Function GetDataEntry(Name As String) As DataSourceModel
            Dim Entry = _InternalImportsDataSource(Name)
            Return Entry
        End Function

        Public Function SetValue(Variable As String, value As Object) As Boolean
            Dim Entry = _InternalImportsDataSource(Variable)
            Call Entry.SetValue(value)
            Return True
        End Function

        Public Function [Imports]([Module] As System.Reflection.TypeInfo) As Integer
            Dim DataSource = DataSourceModel.Imports([Module], False)
            For Each Entry As DataSourceModel In DataSource
                _InternalImportsDataSource(Entry.Name) = Entry
            Next

            Return DataSource.Count
        End Function

        Public Function ToMemorySource() As KeyValuePair(Of String, Object)()
            Dim LQuery = (From Handle As KeyValuePair(Of String, DataSourceModel)
                          In Me._InternalImportsDataSource
                          Select New KeyValuePair(Of String, Object)(Handle.Value.Name, Handle.Value.GetValue)).ToArray
            Return LQuery
        End Function
    End Class

    Public MustInherit Class DataSourceModel

        Protected _Name As String

        Public ReadOnly Property Name As String
            Get
                Return _Name
            End Get
        End Property

        Public MustOverride ReadOnly Property ReflectionType As Type

#Region "Public Property"

        Public MustOverride Sub SetValue(value As Object)
        Public MustOverride Function GetValue() As Object
#End Region

        Public Overrides Function ToString() As String
            Dim o As Object = GetValue()
            Dim value As String = If(o Is Nothing, "&NULL", o.ToString)
            Return String.Format("{0} = {1}", Name, value)
        End Function

        ''' <summary>
        ''' 只会绑定非实例的属性或者域
        ''' </summary>
        ''' <param name="Assembly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function [Imports](Assembly As System.Reflection.TypeInfo, Explicit As Boolean) As DataSourceModel()
            Dim Fields As Field()
            Dim MappingEntry = GetType(Microsoft.VisualBasic.ComponentModel.DataSourceModel.DataFrameColumnAttribute)

            If Explicit Then '严格的
                Fields = (From p In Assembly.GetFields(System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Static)
                          Let attrs As Object() = p.GetCustomAttributes(MappingEntry, inherit:=True)
                          Where Not attrs.IsNullOrEmpty
                          Let datEntry = DirectCast(attrs.First, Microsoft.VisualBasic.ComponentModel.DataSourceModel.DataFrameColumnAttribute)
                          Let Name = If(String.IsNullOrEmpty(datEntry.Name), p.Name, datEntry.Name)
                          Select New Field(p, Name)).ToArray
            Else
                Fields = (From p In Assembly.GetFields(System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Static)
                          Let attrs As Object() = p.GetCustomAttributes(MappingEntry, inherit:=True)
                          Let Name As String = Function() As String
                                                   If attrs.IsNullOrEmpty Then
                                                       Return p.Name
                                                   Else
                                                       Dim datEntry = DirectCast(attrs.First, Microsoft.VisualBasic.ComponentModel.DataSourceModel.DataFrameColumnAttribute)
                                                       Dim Name = If(String.IsNullOrEmpty(datEntry.Name), p.Name, datEntry.Name)
                                                       Return Name
                                                   End If
                                               End Function() Select New Field(p, Name)).ToArray
            End If

            Dim [Property] As [Property]()

            If Explicit Then '严格的
                [Property] = (From p In Assembly.GetProperties(System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Static)
                              Let attrs As Object() = p.GetCustomAttributes(MappingEntry, inherit:=True)
                              Where Not attrs.IsNullOrEmpty
                              Let datEntry = DirectCast(attrs.First, Microsoft.VisualBasic.ComponentModel.DataSourceModel.DataFrameColumnAttribute)
                              Let Name = If(String.IsNullOrEmpty(datEntry.Name), p.Name, datEntry.Name)
                              Select New [Property](p, Name))
            Else
                [Property] = (From p In Assembly.GetProperties(System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Static)
                              Let attrs As Object() = p.GetCustomAttributes(MappingEntry, inherit:=True)
                              Let Name As String = Function() As String
                                                       If attrs.IsNullOrEmpty Then
                                                           Return p.Name
                                                       Else
                                                           Dim datEntry = DirectCast(attrs.First, Microsoft.VisualBasic.ComponentModel.DataSourceModel.DataFrameColumnAttribute)
                                                           Dim Name = If(String.IsNullOrEmpty(datEntry.Name), p.Name, datEntry.Name)
                                                           Return Name
                                                       End If
                                                   End Function() Select New [Property](p, Name)).ToArray
            End If

            Dim DataSource As New List(Of DataSourceModel)
            Call DataSource.AddRange(Fields)
            Call DataSource.AddRange([Property])

            Return DataSource.ToArray
        End Function

        Public Function Convertable(SourceType As Type) As Boolean
            Dim ConvertType = Me.ReflectionType
            Dim YON As Boolean = Microsoft.VisualBasic.ComponentModel.DataSourceModel.DataFramework.BasicTypesFlushs.ContainsKey(ConvertType) AndAlso
                Microsoft.VisualBasic.ComponentModel.DataSourceModel.DataFramework.BasicTypesFlushs.ContainsKey(SourceType)
            Return YON
        End Function
    End Class

    Public Class Field : Inherits DataSourceModel

        Dim BindingField As System.Reflection.FieldInfo

        Sub New(Target As System.Reflection.FieldInfo, Name As String)
            BindingField = Target
            _Name = Name
        End Sub

        Public Overrides Function GetValue() As Object
            Return BindingField.GetValue(Nothing)
        End Function

        Public Overrides Sub SetValue(value As Object)
            Call BindingField.SetValue(Nothing, value)
        End Sub

        Public Overrides ReadOnly Property ReflectionType As Type
            Get
                Return BindingField.FieldType
            End Get
        End Property
    End Class

    Public Class [Property] : Inherits DataSourceModel

        Dim BindingProperty As System.Reflection.PropertyInfo

        Sub New(Target As System.Reflection.PropertyInfo, Name As String)
            BindingProperty = Target
            _Name = Name
        End Sub

        Public Overrides Function GetValue() As Object
            Return BindingProperty.GetValue(Nothing)
        End Function

        Public Overrides Sub SetValue(value As Object)
            Call BindingProperty.SetValue(Nothing, value)
        End Sub

        Public Overrides ReadOnly Property ReflectionType As Type
            Get
                Return BindingProperty.PropertyType
            End Get
        End Property
    End Class
End Namespace