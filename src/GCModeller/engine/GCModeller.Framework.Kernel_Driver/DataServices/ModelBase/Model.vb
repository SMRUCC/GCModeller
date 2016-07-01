#Region "Microsoft.VisualBasic::d5405172f10efd61e17011621f7202ce, ..\GCModeller\engine\GCModeller.Framework.Kernel_Driver\DataServices\ModelBase\Model.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.ComponentModel
Imports System.Web.Script.Serialization

Namespace LDM

    ''' <summary>
    ''' All of the model file basetype definition in the GCModeller program group, all of the model file must inherits from this class object.
    ''' (GCModeller程序包内的所有模型文件的基类型，所有的模型文件对象必须从本对象类型继承)
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <XmlRoot("LANS-SystemsBiology-GCML", Namespace:="http://code.google.com/p/genome-in-code/GCMarkupLanguage/")>
    Public MustInherit Class ModelBaseType : Inherits ITextFile

        <XmlElement("GCModeller.DB.Properties", Namespace:="http://code.google.com/p/genome-in-code/GCMarkupLanguage/GCModeller/Components")>
        Public Property ModelProperty As [Property]
        <XmlAttribute> Public Property IteractionLoops As Integer

        <XmlIgnore> <ScriptIgnore>
        Public Shadows Property FilePath As String
            Get
                Return MyBase.FilePath
            End Get
            Set(value As String)
                MyBase.FilePath = value
            End Set
        End Property

        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
            If String.IsNullOrEmpty(FilePath) Then
                FilePath = Me.FilePath
            End If
            Return Me.GetXml.SaveTo(FilePath, Encoding)
        End Function

        Public Overloads Shared Sub Save(Of T As ModelBaseType)(File As String, Model As T)
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Using Stream As New System.IO.StringWriter(sb:=sBuilder)
#If DEBUG Then
            Try
                Call (New System.Xml.Serialization.XmlSerializer(GetType(T))).Serialize(Stream, Model)
            Catch ex As Exception
                FileIO.FileSystem.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "/error.log",
                                               ex.ToString & vbCrLf & vbCrLf,
                                               append:=True)
                Throw
            End Try
#Else
                Call (New System.Xml.Serialization.XmlSerializer(GetType(T))).Serialize(Stream, Model)
#End If
            End Using

            Call FileIO.FileSystem.WriteAllText(File, text:=sBuilder.ToString, append:=False)
        End Sub

        Public Overrides Function ToString() As String
            Try
                Return String.Format("{0}::[{1}]", ModelProperty.GUID, ModelProperty.SpecieId)
            Catch ex As Exception
                Return MyBase.ToString
            End Try
        End Function
    End Class

End Namespace
