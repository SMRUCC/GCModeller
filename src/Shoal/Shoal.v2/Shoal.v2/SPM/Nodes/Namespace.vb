Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.ShoalShell.HTML
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Linker.APIHandler

Namespace SPM.Nodes

    <XmlType("PackageNamespace", [Namespace]:="http://SourceForge.net/project/shoal/spm.xlst")>
    Public Class [Namespace] : Implements HTML.IWikiHandle

        ''' <summary>
        ''' 命名空间是可以被分隔为多个模块分别开发于不同的程序模块之中的
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property PartialModules As PartialModule()

        <XmlAttribute> Public Property Url As String
        Public Property Publisher As String
        <XmlAttribute> Public Property Revision As Integer
        Public Property Cites As String()

        ''' <summary>
        ''' A brief description text about the function of this namespace.(关于本模块之中的描述性的摘要文本)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Description As String

        ''' <summary>
        ''' The name value of this namespace module.(本命名空间模块的名称值)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property [Namespace] As String
        <XmlAttribute> Public Property Category As APICategories

        Public Overrides Function ToString() As String
            Return [Namespace]
        End Function

        Public Function GenerateDescription() As String Implements IWikiHandle.GenerateDescription
            Throw New NotImplementedException()
        End Function

        Public Function Match(keyword As String) As String Implements IWikiHandle.Match
            Throw New NotImplementedException()
        End Function

        Private Sub __LoadEntryPoints()
            Dim LQuery = (From [module] As PartialModule In Me.PartialModules
                          Let assm As System.Reflection.Assembly = [module].Assembly.LoadAssembly
                          Let EntryType As Type = __loadTypeInfo(assm, [module].Assembly.TypeId, [module].Assembly.Path)
                          Select CommandLine.Interpreter.GetAllCommands(EntryType)).ToArray
            Dim __loadedAPIList = APIParser(LQuery.ToVector)
            Dim hash = __loadedAPIList.ToDictionary(Function(api) api.Name.ToLower)
            Me.__loadedEntryPoints = New SortedDictionary(Of String, APIEntryPoint)(hash)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="assm"></param>
        ''' <param name="TypeId"></param>
        ''' <param name="path">输出调试信息的时候使用</param>
        ''' <returns></returns>
        Private Function __loadTypeInfo(assm As System.Reflection.Assembly, TypeId As String, path As String) As Type
            If assm Is Nothing Then
                Return Nothing
            End If

            Dim type As Type = assm.GetType(TypeId)

            If type Is Nothing Then  '找不到类型，则可能是开发人员修改了类型的命令空间或者其名称属性，则给出警告信息
                Call $"{path.ToFileURL}!{TypeId} missing!".__DEBUG_ECHO
            End If
            Return type
        End Function

        Dim __loadedEntryPoints As SortedDictionary(Of String, APIEntryPoint)

        <XmlIgnore> Public ReadOnly Property API As APIEntryPoint()
            Get
                If __loadedEntryPoints Is Nothing Then
                    Call __LoadEntryPoints()
                End If

                Return __loadedEntryPoints.Values.ToArray
            End Get
        End Property

        Public Function GetEntryPoint(Name As String) As APIEntryPoint
            Name = Name.ToLower

            If __loadedEntryPoints Is Nothing Then
                Call __LoadEntryPoints()
            End If

            If __loadedEntryPoints.ContainsKey(Name) Then
                Return __loadedEntryPoints(Name)
            Else
                Return Nothing
            End If
        End Function
    End Class
End Namespace