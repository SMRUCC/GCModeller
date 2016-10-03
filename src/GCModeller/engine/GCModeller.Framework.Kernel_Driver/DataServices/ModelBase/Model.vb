#Region "Microsoft.VisualBasic::a4a114f6800762e7986c04371cd52f85, ..\GCModeller\engine\GCModeller.Framework.Kernel_Driver\DataServices\ModelBase\Model.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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
Imports System.Web.Script.Serialization
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData

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

        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
            If String.IsNullOrEmpty(FilePath) Then
                FilePath = Me.FilePath
            End If
            Return XmlExtensions.GetXml(Me, MyClass.GetType).SaveTo(FilePath, Encoding)
        End Function

        Public Overrides Function ToString() As String
            Try
                Return String.Format("{0}::[{1}]", ModelProperty.GUID, ModelProperty.SpecieId)
            Catch ex As Exception
                Return MyBase.ToString
            End Try
        End Function
    End Class
End Namespace
