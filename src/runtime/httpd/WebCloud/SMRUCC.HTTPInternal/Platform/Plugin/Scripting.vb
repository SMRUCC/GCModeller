Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.DataFramework
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.WebCloud.HTTPInternal.Platform.Plugins.ScriptingAttribute

Namespace Platform.Plugins

    <AttributeUsage(AttributeTargets.Method, AllowMultiple:=False, Inherited:=True)>
    Public Class ScriptingAttribute : Inherits Attribute

        Public ReadOnly Property FileTypes As String()

        Public Delegate Function ScriptHandler(wwwroot$, path$, encoding As Encodings) As String

        ''' <summary>
        ''' ``*.vbhtml`` etc.
        ''' </summary>
        ''' <param name="extensions$"></param>
        Sub New(ParamArray extensions$())
            If extensions.IsNullOrEmpty Then
                Throw New ArgumentNullException("No file type supports!")
            Else
                FileTypes = extensions
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return FileTypes.GetJson
        End Function
    End Class

    Public Module ScriptingExtensions

        ''' <summary>
        ''' path.<see cref="ExtensionSuffix"/>.ToLower(不带小数点的后缀名，并且全部为小写形式)
        ''' </summary>
        ReadOnly scripting As IReadOnlyDictionary(Of String, ScriptHandler)
        ReadOnly delgTemplate As Type = GetType(ScriptHandler)

        Sub New()
            scripting = App.HOME.LoadHandlers
        End Sub

        <Extension>
        Public Function LoadHandlers(dir$) As Dictionary(Of String, ScriptHandler)
            Dim out As New Dictionary(Of String, ScriptHandler)

            For Each dll As String In ls - l - r - "*.dll" <= dir
                Dim assm As Assembly = Assembly.LoadFile(dll)
                Dim types As Type() = EmitReflection.GetTypesHelper(assm)
                Dim handlers = types _
                    .Select(Function(type)
                                Return type.GetMethods(bindingAttr:=PublicShared)
                            End Function) _
                    .IteratesALL _
                    .Select(Function(m)
                                Dim flag = m.GetCustomAttribute(Of ScriptingAttribute)
                                Return (flag:=flag, container:=m)
                            End Function) _
                    .Where(Function(m) Not m.flag Is Nothing) _
                    .ToArray

                For Each handler In handlers
                    Dim del As ScriptHandler = handler _
                        .container _
                        .CreateDelegate(delgTemplate)

                    For Each type$ In handler.flag.FileTypes
                        With type.Split("."c).Last.ToLower
                            out(type) = del
                        End With
                    Next
                Next
            Next

            Return out
        End Function

        <Extension>
        Public Function ReadHTML(wwwroot$, path$, Optional encoding As Encodings = Encodings.UTF8) As String
            Dim ext$ = path.ExtensionSuffix.ToLower

            If Not scripting.ContainsKey(ext) Then
                Return $"{wwwroot}/{path}".ReadAllText(encoding.CodePage)
            Else
                Return scripting(ext)(wwwroot, path, encoding)
            End If
        End Function
    End Module
End Namespace