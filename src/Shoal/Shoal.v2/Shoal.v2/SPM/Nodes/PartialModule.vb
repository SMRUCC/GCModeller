Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.ShoalShell.HTML

Namespace SPM.Nodes

    Public Class PartialModule : Inherits PackageNamespace
        Implements HTML.IWikiHandle

        <XmlElement> Public Property Assembly As Assembly
        <XmlElement> Public Property EntryPoints As EntryPointMeta()

        Public Sub New()
        End Sub

        Public Sub New([Namespace] As PackageNamespace)
            Me.Description = [Namespace].Description
            Me.Namespace = [Namespace].Namespace
            Me.Publisher = [Namespace].Publisher
            Me.Revision = [Namespace].Revision
            Me.AutoExtract = [Namespace].AutoExtract
            Me.Url = [Namespace].Url
            Me.Cites = [Namespace].Cites
            Me.Category = [Namespace].Category
        End Sub

        ''' <summary>
        ''' 从文件之中解析出来的新的数据更新当前的模块
        ''' </summary>
        ''' <param name="ns"></param>
        Public Sub Copy(ns As PartialModule)
            Me.Description = ns.Description
            Me.Assembly = ns.Assembly
            Me.EntryPoints = ns.EntryPoints
            Me.Namespace = ns.Namespace
            Me.Publisher = ns.Publisher
            Me.Revision = ns.Revision
            Me.Url = ns.Url
            Me.Cites = ns.Cites
            Me.Category = ns.Category
        End Sub

        Public Overloads Shared Function Equals(ns1 As PartialModule, ns2 As PartialModule) As Boolean
            Dim nsAssm1 = ns1.Assembly?.LoadAssembly
            Dim nsAssm2 = ns2.Assembly?.LoadAssembly

            If nsAssm1 Is Nothing OrElse nsAssm2 Is Nothing Then
                Return False '无法判断则肯定不相等
            End If

            If Not String.Equals(ns1.Assembly.TypeId, ns2.Assembly.TypeId) Then
                Return False '来源不一样，则肯定不一样
            End If

            Return True
        End Function

        Public Function GetCites() As String
            Dim typeDef As Type = Assembly.GetType
            If typeDef Is Nothing Then
                Return ""
            Else
                Return HTML.Cites.GetCites(typeDef)
            End If
        End Function

        Public Function GenerateDescription() As String Implements IWikiHandle.GenerateDescription
            Throw New NotImplementedException()
        End Function

        Public Overloads Function Match(keyword As String) As String Implements IWikiHandle.Match
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace