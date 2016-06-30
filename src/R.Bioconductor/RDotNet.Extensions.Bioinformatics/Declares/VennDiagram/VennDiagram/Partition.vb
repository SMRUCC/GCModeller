Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language

Namespace VennDiagram.ModelAPI

    ''' <summary>
    ''' A partition in the venn diagram.
    ''' </summary>
    Public Class Partition : Inherits ClassObject
        Implements sIdEnumerable

        ''' <summary>
        ''' The name of this partition
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Name As String Implements sIdEnumerable.Identifier
        ''' <summary>
        ''' The color string of the partition
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Color As String
        Public Property Title As String

        ''' <summary>
        ''' 使用数字来表示成员的一个向量
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlText> Public Property Vector As String

        Public ReadOnly Property DisplName As String
            Get
                If String.IsNullOrEmpty(Title) Then
                    Return Name
                Else
                    Return Title
                End If
            End Get
        End Property

        Sub New()
        End Sub

        Sub New(Name As String)
            Me.Name = Name
        End Sub

        Public Function ApplyOptions([Option] As String()) As Partition
            Name = [Option].First
            Color = [Option].Get(1)
            Title = [Option].Get(2)
            Console.WriteLine("{0}(color: {1}) {2} counts.", Me.Name, Me.Color, Me.Vector.Split(CChar(",")).Length)
            Return Me
        End Function
    End Class
End Namespace