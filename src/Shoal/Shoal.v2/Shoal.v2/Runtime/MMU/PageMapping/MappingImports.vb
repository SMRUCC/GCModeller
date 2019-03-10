Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter

Namespace Runtime.MMU.PageMapping

    Public Class MappingImports

        ReadOnly MMUDevice As MMUDevice

        Sub New(MMUDevice As MMU.MMUDevice)
            Me.MMUDevice = MMUDevice
        End Sub

        Public Function [Imports]([Module] As System.Reflection.TypeInfo) As Integer
            Dim DataSource As DataSourceModel() =
                MappingImports.Imports([Module], False)
            For Each Entry As DataSourceModel In DataSource
                Call __imports(Entry)
            Next

            Return DataSource.Length
        End Function

        Private Sub __imports(Page As MMU.PageMapping.DataSourceModel)
            Dim KeyFind As String = "$" & Page.Name.ToLower
            Dim Handle As Long

            If MMUDevice.PageMapping.ContainsKey(KeyFind) Then ' 已经存在一个同名的变量了，则新的会替换掉旧的
                Handle = MMUDevice.PageMapping(KeyFind).Address
                Call MMUDevice.PageMapping.Remove(KeyFind)
            End If

            Call MMUDevice.PageMapping.Add(KeyFind, Page)

            If Handle > 0 Then '更新内存区块
                Page.Address = Handle
                MMUDevice.MMU_CHUNKS(Handle) = Page
            Else
                Call MMUDevice.Allocate(Page)
            End If
        End Sub

#Region "DataSource Mapping Imports"

        ''' <summary>
        ''' 只会绑定非实例的属性或者域
        ''' </summary>
        ''' <param name="Assembly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function [Imports](Assembly As System.Reflection.TypeInfo, Explicit As Boolean) As DataSourceModel()
            Return If(Explicit, __importsExplicit(Assembly), __imports(Assembly))
        End Function

        ''' <summary>
        ''' 只会绑定非实例的属性或者域
        ''' </summary>
        ''' <param name="Assembly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function __imports(Assembly As System.Reflection.TypeInfo) As DataSourceModel()
            Dim MappingEntry = GetType(DataFrameColumnAttribute)
            Dim Fields As Field() = (From p As System.Reflection.FieldInfo
                                     In Assembly.GetFields(System.Reflection.BindingFlags.NonPublic Or
                                         System.Reflection.BindingFlags.Public Or
                                         System.Reflection.BindingFlags.Static)
                                     Let attrs As Object() = p.GetCustomAttributes(MappingEntry, inherit:=True)
                                     Let Name As String = __getName(p.Name, attrs)
                                     Select New Field(p, Name)).ToArray
            Dim [Property] As [Property]() = (From p As System.Reflection.PropertyInfo
                                              In Assembly.GetProperties(System.Reflection.BindingFlags.NonPublic Or
                                                  System.Reflection.BindingFlags.Public Or
                                                  System.Reflection.BindingFlags.Static)
                                              Let attrs As Object() = p.GetCustomAttributes(MappingEntry, inherit:=True)
                                              Let Name As String = __getName(p.Name, attrs)
                                              Select New [Property](p, Name)).ToArray
            Return Fields.Cast(Of DataSourceModel).Join([Property]).ToArray
        End Function

        Private Shared Function __importsExplicit(Assembly As System.Reflection.TypeInfo) As DataSourceModel()
            Dim MappingEntry = GetType(DataFrameColumnAttribute)
            Dim Fields As Field() = (From p In Assembly.GetFields(System.Reflection.BindingFlags.NonPublic Or
                                         System.Reflection.BindingFlags.Public Or
                                         System.Reflection.BindingFlags.Static)
                                     Let attrs As Object() = p.GetCustomAttributes(MappingEntry, inherit:=True)
                                     Where Not attrs.IsNullOrEmpty
                                     Let Name = __getName(p.Name, attrs)
                                     Select New Field(p, Name)).ToArray
            Dim [Property] As [Property]() = (From p In Assembly.GetProperties(System.Reflection.BindingFlags.NonPublic Or
                                                  System.Reflection.BindingFlags.Public Or
                                                  System.Reflection.BindingFlags.Static)
                                              Let attrs As Object() = p.GetCustomAttributes(MappingEntry, inherit:=True)
                                              Where Not attrs.IsNullOrEmpty
                                              Let Name = __getName(p.Name, attrs)
                                              Select New [Property](p, Name))
            Return Fields.Cast(Of DataSourceModel).Join([Property]).ToArray
        End Function

        Private Shared Function __getName(pName As String, attrs As Object()) As String
            If attrs.IsNullOrEmpty Then Return pName

            Dim datEntry = DirectCast(attrs(Scan0), DataFrameColumnAttribute)
            Dim Name = If(String.IsNullOrEmpty(datEntry.Name), pName, datEntry.Name)
            Return Name
        End Function
#End Region

    End Class
End Namespace