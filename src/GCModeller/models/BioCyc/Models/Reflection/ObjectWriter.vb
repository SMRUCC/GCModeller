
Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data.BioCyc.Assembly.MetaCyc.Schema.Metabolism

''' <summary>
''' load attribute-value data from file into a .NET object instance 
''' </summary>
Public Class ObjectWriter

    ReadOnly schema As NamedValue(Of PropertyInfo)()
    ReadOnly model As Type

    Shared Sub New()
        Call ElementFactory.Register(Of ECNumber)(Function(str) ECNumber.ValueParser(str.value))
        Call ElementFactory.Register(Of CompoundSpecieReference)(AddressOf Factory.ParseCompoundReference)
        Call ElementFactory.Register(Of ReactionDirections)(AddressOf Factory.ParseReactionDirection)
    End Sub

    Private Sub New(type As Type)
        model = type
        schema = DataFramework.Schema(type, PropertyAccess.Writeable, nonIndex:=True) _
            .Values _
            .Select(Function(p)
                        Dim key As String = AttributeField.getMappingKey(p)
                        Dim map As New NamedValue(Of PropertyInfo)(key, p)

                        Return map
                    End Function) _
            .ToArray
    End Sub

    Public Overrides Function ToString() As String
        Return model.FullName
    End Function

    Public Function Deserize(data As FeatureElement) As Model
        Dim obj As Object = Activator.CreateInstance(model)
        Dim val As Object
        Dim castTo As PropertyInfo

        For Each field As NamedValue(Of PropertyInfo) In schema
            castTo = field.Value
            val = castVal(data(field.Name), castTo.PropertyType)
            castTo.SetValue(obj, val)
        Next

        Return obj
    End Function

    Private Shared Function castVal(val As ValueString(), target As Type) As Object
        If val Is Nothing Then
            Return Nothing
        End If

        If target.IsArray Then
            Dim template As Type = target.GetElementType
            Dim vec As Array = Array.CreateInstance(template, val.Length)

            For i As Integer = 0 To vec.Length - 1
                vec(i) = castVal(val(i), template)
            Next

            Return vec
        Else
            Return castVal(val(Scan0), target)
        End If
    End Function

    Private Shared Function castVal(val As ValueString, target As Type) As Object
        If DataFramework.IsPrimitive(target) Then
            Return CTypeDynamic(val.value, target)
        Else
            Return ElementFactory.CastTo(val, target)
        End If
    End Function

    ''' <summary>
    ''' cache of the writer object
    ''' </summary>
    Shared schemaList As New Dictionary(Of String, ObjectWriter)

    Public Shared Function LoadSchema(Of T As Model)() As ObjectWriter
        Return schemaList.ComputeIfAbsent(
            key:=GetType(T).FullName,
            lazyValue:=Function()
                           Return New ObjectWriter(GetType(T))
                       End Function)
    End Function

End Class
