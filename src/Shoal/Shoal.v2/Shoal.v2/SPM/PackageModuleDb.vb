Imports System.Text
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.HybridsScripting
Imports Microsoft.VisualBasic.Scripting.ShoalShell.SPM.Nodes

Namespace SPM

    ''' <summary>
    ''' 包管理器的数据库文件
    ''' </summary>
    Public Class PackageModuleDb : Inherits ComponentModel.ITextFile

        Public Property NamespaceCollection As [Namespace]()
        Public Property HybridEnvironments As HybridEnvir()
            Get
                If __innerListEnvir Is Nothing Then
                    __innerListEnvir = New List(Of Nodes.HybridEnvir)
                End If
                Return __innerListEnvir.ToArray
            End Get
            Set(value As SPM.Nodes.HybridEnvir())
                If value Is Nothing Then
                    value = New Nodes.HybridEnvir() {}
                End If

                __innerListEnvir = value.ToList
            End Set
        End Property

        Dim __innerListEnvir As List(Of SPM.Nodes.HybridEnvir)

        ''' <summary>
        ''' 默认的注册表配置文件，该文件是在与本程序同一个文件夹之下的以程序名开始的XML文件.在该文件之中包含有所有的类型注册信息
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property DefaultFile As String = $"{App.ProductSharedDir}/.Settings/shoal-spm.xml"

        Public Shared Function Load(path As String) As PackageModuleDb
            Dim Db = path.LoadXml(Of PackageModuleDb)(ThrowEx:=False)

            If Db Is Nothing Then
                Db = New PackageModuleDb
                Call Db.GetXml(False).SaveTo(path)
            End If

            Db.FilePath = path

            Return Db
        End Function

        Public Shared Function LoadDefault() As PackageModuleDb
            Call $"Load SPM Database from ==> {DefaultFile.ToFileURL}...".__DEBUG_ECHO
            Return Load(DefaultFile)
        End Function

        Public Overrides Function Save(Optional FilePath As String = "",
                                       Optional Encoding As Encoding = Nothing) As Boolean

            FilePath = getPath(FilePath)
            Return Me.GetXml.SaveTo(FilePath, Encoding)
        End Function

        Public Sub Update(Environment As EntryPoint)
            Dim LQuery As HybridEnvir() = (From obj As HybridEnvir
                                           In HybridEnvironments
                                           Where String.Equals(obj.Language, Environment.Language.Language, StringComparison.OrdinalIgnoreCase)
                                           Select obj).ToArray

            Dim Node As HybridEnvir =
                Assembly.CreateObject(Of HybridEnvir)(Environment.DeclaredAssemblyType.Assembly)

            Node.Language = Environment.Language.Language
            Node.TypeId = Environment.DeclaredAssemblyType.FullName
            Node.Description = Environment.Language.Description

            If Not LQuery.IsNullOrEmpty Then
                Call __innerListEnvir.Remove(LQuery(Scan0))
            End If

            Call __innerListEnvir.Add(Node)
        End Sub

        Protected Overrides Function __getDefaultPath() As String
            Return DefaultFile
        End Function
    End Class
End Namespace