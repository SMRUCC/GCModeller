Imports System.Text

Namespace DelegateHandlers.TypeLibraryRegistry.RegistryNodes

    ''' <summary>
    ''' 一个方法的元数据
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MethodMeta : Implements I_Wiki_Handler

        <Xml.Serialization.XmlAttribute> Public Property Name As String
        Public Property Description As String
        <Xml.Serialization.XmlElement> Public Property Parameters As Microsoft.VisualBasic.ComponentModel.KeyValuePair()
        Public Property ReturnedType As String

        Public Overrides Function ToString() As String
            If String.IsNullOrEmpty(Description) Then
                Return Name
            Else
                Return Name & ":  " & Description
            End If
        End Function

        Public Function Match(keyword As String) As String Implements I_Wiki_Handler.Match
            Dim Head As String = String.Format("[{0}]", keyword)

            If InStr(Name, keyword, CompareMethod.Text) > 0 Then
                Head = "Function Entry:  " & Name.ToLower.Replace(keyword.ToLower, Head)
            ElseIf InStr(Description, keyword, CompareMethod.Text) > 0 Then
                Head = Description.ToLower.Replace(keyword.ToLower, Head)
            ElseIf InStr(ReturnedType, keyword, CompareMethod.Text) > 0 Then
                Head = "Function Return Type:  " & ReturnedType.ToLower.Replace(keyword.ToLower, Head)
            Else
                Dim n = MatchParameters(keyword)

                If n.IsNullOrEmpty Then
                    Return ""   '没有匹配上任何数据
                Else
                    Head = String.Join(vbCrLf, (From item In n Select String.Format("{0}  {1}", item.Key, item.Value).ToLower.Replace(keyword.ToLower, Head)))
                End If
            End If

            Return ">>>>  " & Head & vbCrLf & vbCrLf & GenerateDescription()
        End Function

        Public Function MatchParameters(keyword As String) As Microsoft.VisualBasic.ComponentModel.KeyValuePair()
            If Parameters.IsNullOrEmpty Then
                Return Nothing
            End If

            Dim LQuery = (From item In Parameters
                          Where InStr(item.Key, keyword, CompareMethod.Text) > 0 OrElse InStr(item.Value, keyword, CompareMethod.Text) > 0
                          Select item).ToArray
            Return LQuery
        End Function

        Public Function GenerateDescription() As String Implements I_Wiki_Handler.GenerateDescription
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Call sBuilder.AppendLine("Name:        " & Name)
            Call sBuilder.AppendLine("Description: " & If(String.IsNullOrEmpty(Description), "This function have no description data defined.", Description))
            Call sBuilder.AppendLine("Return:      " & ReturnedType)

            If Not Parameters.IsNullOrEmpty Then
                Dim Max As Integer = (From item In Parameters Select Len(item.Key)).ToArray.Max

                Call sBuilder.AppendLine(vbCrLf & String.Format("Function have {0} parameters:", Parameters.Count))
                Call sBuilder.AppendLine(String.Format("-Name-{0}------Type--------------", New String("-"c, Max)))

                For Each p In Parameters
                    Call sBuilder.AppendLine(String.Format(" {0}  {1} {2}", p.Key, New String(" "c, 6 + Max - Len(p.Key)), p.Value))
                Next
            Else
                Call sBuilder.AppendLine("This function doesn't required of the parameters.")
            End If

            Return sBuilder.ToString
        End Function
    End Class

    ''' <summary>
    ''' 一个<see cref="Microsoft.VisualBasic.CommandLine.Reflection.[Namespace]">命名空间对象</see>
    ''' </summary>
    ''' <remarks></remarks>
    <Xml.Serialization.XmlType("Plugin_Module.LoadEntry")>
    Public Class ModuleLoadEntry : Implements I_Wiki_Handler

        <Xml.Serialization.XmlAttribute> Public Property [Namespace] As String
        <Xml.Serialization.XmlAttribute> Public Property Version As String
        <Xml.Serialization.XmlAttribute> Public Property UpdateTime As Long

        <Xml.Serialization.XmlElement> Public Property AssemblyPath As String
        <Xml.Serialization.XmlElement> Public Property Company As String
        <Xml.Serialization.XmlElement> Public Property Guid As String
        <Xml.Serialization.XmlElement> Public Property FrameworkVersion As String
        <Xml.Serialization.XmlElement> Public Property TypeId As String
        <Xml.Serialization.XmlElement> Public Property Description As String

        ''' <summary>
        ''' 本命名空间之下的所有的可以使用的函数命令的简单介绍
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Xml.Serialization.XmlArray("Delegate.Handles")> Public Property CommandHandles As MethodMeta()

        Public Function GenerateDescription() As String Implements I_Wiki_Handler.GenerateDescription
            If CommandHandles.IsNullOrEmpty Then
                Return String.Format("Namespace ""{0}"" have no command entry...", [Namespace])
            End If

            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Dim MaxLength As Integer = (From strKey In CommandHandles Select Len(strKey.Name)).ToArray.Max
            Call sBuilder.AppendLine()

            Call sBuilder.AppendLine("Assembly path:      " & AssemblyPath)
            Call sBuilder.AppendLine("Assembly Lib:       " & TypeId)
            Call sBuilder.AppendLine("Module Description: " & Description)
            Call sBuilder.AppendLine()
            Call sBuilder.AppendLine(vbCrLf & String.Format("     {0} Command(s)", CommandHandles.Count))
            Call sBuilder.AppendLine()
            Call sBuilder.AppendLine(String.Format("  MethodEntry{0}Return Type", New String(" "c, MaxLength)))
            Call sBuilder.AppendLine(String.Format("+---{0}+----------------------------------------------------", New String("-"c, 1.5 * MaxLength)))
            For Each MethodEntry In CommandHandles
                Call sBuilder.AppendLine(String.Format("  {0}{1}  {2}", MethodEntry.Name, New String(" "c, 2 * MaxLength - Len(MethodEntry.Name)), MethodEntry.ReturnedType))
            Next

            Return sBuilder.ToString
        End Function

        ''' <summary>
        ''' 根据<see cref="AssemblyPath"></see>的值来查找目标，然后比较修改时间来判断时候已被更新
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsUpdateToDate() As Boolean
            Dim File = FileIO.FileSystem.GetFileInfo(AssemblyPath)
            Dim ModifyTime As Long = File.LastWriteTime.ToBinary
            Return ModifyTime > UpdateTime
        End Function

        Public Function IsAvaliable() As Boolean
            Return FileIO.FileSystem.FileExists(AssemblyPath)
        End Function

        Public Overloads Function Equals(Of T As ModuleLoadEntry)(item As T) As Boolean
            If Me.IsEmpty OrElse item.IsEmpty Then
                Return False '无法进行比较
            End If
            Return String.Equals(item.Namespace, [Namespace]) AndAlso String.Equals(item.TypeId, TypeId)
        End Function

        Private ReadOnly Property IsEmpty As Boolean
            Get
                Return String.IsNullOrEmpty([Namespace]) OrElse String.IsNullOrEmpty(TypeId)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("<(""{0}"")> <== {1}", Guid, AssemblyPath)
        End Function

        Public Sub CopyFrom(Of T As ModuleLoadEntry)(obj As T)
            Me.Namespace = obj.[Namespace]
            Me.AssemblyPath = obj.AssemblyPath
            Me.Company = obj.Company
            Me.FrameworkVersion = obj.FrameworkVersion
            Me.Guid = obj.Guid
            Me.TypeId = obj.TypeId
            Me.UpdateTime = obj.UpdateTime
            Me.Version = obj.Version
            Me.CommandHandles = obj.CommandHandles
        End Sub

        Public Function Match(keyword As String) As String Implements I_Wiki_Handler.Match
            Dim Head As String = String.Format("[{0}]", keyword)

            If InStr(Company, keyword, CompareMethod.Text) > 0 Then
                Head = "Company:  " & Company.ToLower.Replace(keyword.ToLower, Head)
            ElseIf InStr(TypeId, keyword, CompareMethod.Text) > 0 Then
                Head = "TypeId:  " & TypeId.ToLower.Replace(keyword.ToLower, Head)
            ElseIf InStr(Description, keyword, CompareMethod.Text) > 0 Then
                Head = Description.ToLower.Replace(keyword.ToLower, Head)
            Else
                Dim Commands As String() = MatchCommandsHandles(keyword)

                If Commands.IsNullOrEmpty Then
                    Return ""
                Else
                    Head = String.Join(vbCrLf & vbCrLf, Commands) & vbCrLf & vbCrLf
                    Head = String.Format("Namespace ""{0}"" have {1} matche(s)...{2}{3}", [Namespace], Commands.Count, vbCrLf & vbCrLf, Head)
                    Return Head
                End If
            End If

            Return ">>>>  " & Head & vbCrLf & vbCrLf & GenerateDescription()
        End Function

        Public Function MatchCommandsHandles(keyword As String) As String()
            Dim LQuery = (From MetaData As MethodMeta
                          In CommandHandles
                          Let result As String = MetaData.Match(keyword)
                          Where Not String.IsNullOrEmpty(result)
                          Select result).ToArray
            Return LQuery
        End Function
    End Class

    Public Class HybridScriptingModuleLoadEntry : Inherits ModuleLoadEntry
        <Xml.Serialization.XmlAttribute> Public Property EntryId As String

        Public Overrides Function ToString() As String
            Return EntryId
        End Function
    End Class

    ''' <summary>
    ''' The namespace is the module which contains some functional related methods. 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class [Namespace] : Implements I_Wiki_Handler

        <Xml.Serialization.XmlAttribute("Entry.Namespace")> Public Property ModuleName As String
        ''' <summary>
        ''' 这些模块都具有相同的命名空间，则在加载进入内存的时候就会被合并在一个模块之中
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Xml.Serialization.XmlElement> Public Property Entries As List(Of ModuleLoadEntry)

        Public Overrides Function ToString() As String
            Return ModuleName
        End Function

        Public Function Match(keyword As String) As String Implements I_Wiki_Handler.Match
            Dim Head As String = String.Format("[{0}]", keyword)

            If InStr(ModuleName, keyword, CompareMethod.Text) > 0 Then
                Head = "Namespace:  " & ModuleName.ToLower.Replace(keyword.ToLower, Head)
            Else
                Dim Entries As String() = MatchEntries(keyword)

                If Entries.IsNullOrEmpty Then
                    Return ""
                Else
                    Return String.Join(vbCrLf & vbCrLf, Entries)
                End If
            End If

            Return ">>>>  " & Head & vbCrLf & vbCrLf & GenerateDescriptions()
        End Function

        Public Function MatchEntries(keyword As String) As String()
            Dim LQuery = (From item In Entries Let result As String = item.Match(keyword) Where Not String.IsNullOrEmpty(result) Select result).ToArray
            Return LQuery
        End Function

        Public Function GenerateDescriptions() As String Implements I_Wiki_Handler.GenerateDescription
            Dim sBuilder As StringBuilder = New StringBuilder(String.Format("Entry.Namespace:    {0}", ModuleName))
            For Each Entry As RegistryNodes.ModuleLoadEntry In Entries
                Call sBuilder.AppendLine(Entry.GenerateDescription)
            Next

            Return sBuilder.ToString
        End Function
    End Class

    ''' <summary>
    ''' Internal wiki system queriable object.(这个对象是可以接受wiki查询操作的)
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface I_Wiki_Handler

        Function GenerateDescription() As String
        ''' <summary>
        ''' 模糊匹配并返回匹配结果，当返回空字符串的时候，则说明没有被匹配上
        ''' </summary>
        ''' <param name="keyword"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Match(keyword As String) As String
    End Interface
End Namespace