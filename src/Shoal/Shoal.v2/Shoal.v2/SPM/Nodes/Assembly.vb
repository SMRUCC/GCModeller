Imports System.Runtime.Versioning
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Scripting.ShoalShell.HTML

Namespace SPM.Nodes

    ''' <summary>
    ''' 用于引用的程序的定位的，用来处理相对路径以及网络路径
    ''' </summary>
    Public Class Assembly : Implements HTML.IWikiHandle

        <XmlElement("FileUrl")> Public Property Path As String
        <XmlAttribute> Public Property Version As String
        <XmlAttribute> Public Property UpdateTime As Long
        <XmlElement> Public Property Company As String
        <XmlElement> Public Property FrameworkVersion As String
        ''' <summary>
        ''' 这个属性使用于记录重复的命名空间模块之间的相互比较的
        ''' 路径可以不一样，但是这个必须要一样
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property TypeId As String
        <XmlElement> Public Property Description As String

        Public Overrides Function ToString() As String
            Return $"{Path}!{TypeId}"
        End Function

        ''' <summary>
        ''' Gets this partial module defined type information.(出错会返回空值)
        ''' </summary>
        ''' <returns></returns>
        Public Overloads Function [GetType]() As Type
            Return [GetType](TypeId)
        End Function

        Public Overloads Function [GetType](Type As String) As Type
            Dim Assembly As System.Reflection.Assembly = LoadAssembly()
            If Assembly Is Nothing Then
                Return Nothing
            Else
                Return Assembly.GetType(Type)
            End If
        End Function

        Public Shared Function CreateObject(Of T As Assembly)(Assembly As System.Reflection.Assembly) As T
            Dim attrs = Assembly.CustomAttributes
            Dim Company As String = (From attr In attrs
                                     Where attr.AttributeType = GetType(System.Reflection.AssemblyCompanyAttribute)
                                     Let value = attr.ConstructorArguments.First.Value.ToString
                                     Select value).FirstOrDefault
            Dim Version As String = (From attr In attrs
                                     Where attr.AttributeType = GetType(System.Reflection.AssemblyFileVersionAttribute)
                                     Let value = attr.ConstructorArguments.First.Value.ToString
                                     Select value).FirstOrDefault
            Dim FrameworkVersion As String = (From attr In attrs
                                              Where attr.AttributeType = GetType(TargetFrameworkAttribute)
                                              Let value = attr.ToString
                                              Select value).FirstOrDefault
            Dim FileInfo = VisualBasic.FileIO.FileSystem.GetFileInfo(Assembly.Location)
            Dim Updates As Long = FileInfo.LastWriteTime.ToBinary
            Dim assmDef = Activator.CreateInstance(Of T)

            assmDef.Path = __createPath(FileInfo.FullName)
            assmDef.Company = Company
            assmDef.FrameworkVersion = FrameworkVersion
            assmDef.UpdateTime = Updates
            assmDef.Version = Version

            Return assmDef
        End Function

        Private Shared Function __createPath(path As String) As String
            path = ProgramPathSearchTool.RelativePath(App.HOME, path)
            Call FileIO.FileSystem.WriteAllText(App.HOME & "/Imports.txt", path & vbCrLf, append:=True)
            Return path
        End Function

        Public Function GenerateDescription() As String Implements IWikiHandle.GenerateDescription
            Throw New NotImplementedException()
        End Function

        Public Function Match(keyword As String) As String Implements IWikiHandle.Match
            Throw New NotImplementedException()
        End Function

        ''' <summary>
        ''' 出错的时候会返回空值
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>请注意，在执行的时候可能当前的工作目录会发生改变，所以计算相对路径的时候总是需要以当前的程序的位置来计算</remarks>
        Public Function LoadAssembly() As System.Reflection.Assembly
            Try
                Dim currentWork As String = FileIO.FileSystem.CurrentDirectory  ' 切换当前的工作目录
                FileIO.FileSystem.CurrentDirectory = App.HOME
                Dim path As String = VisualBasic.FileIO.FileSystem.GetFileInfo(Me.Path).FullName
                FileIO.FileSystem.CurrentDirectory = currentWork
                Return System.Reflection.Assembly.LoadFile(path)
            Catch ex As Exception
                Dim Trace As String = $"{GetType(Assembly).FullName}::{NameOf(LoadAssembly)}"
                Call App.LogException(Path.ToFileURL, Trace)
                Return App.LogException(ex, Trace)
            End Try
        End Function
    End Class
End Namespace