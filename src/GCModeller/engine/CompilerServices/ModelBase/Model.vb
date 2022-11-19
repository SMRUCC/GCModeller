#Region "Microsoft.VisualBasic::39d48bcdfa2e01b63d59d1e723f25f9e, GCModeller\engine\CompilerServices\ModelBase\Model.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 45
    '    Code Lines: 30
    ' Comment Lines: 6
    '   Blank Lines: 9
    '     File Size: 1.64 KB


    ' Class ModelBaseType
    ' 
    '     Properties: properties
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ISaveHandle_Save, Save, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Text

''' <summary>
''' All of the model file basetype definition in the GCModeller program group, all of the model file must inherits from this class object.
''' (GCModeller程序包内的所有模型文件的基类型，所有的模型文件对象必须从本对象类型继承)
''' </summary>
''' <remarks></remarks>
''' 
Public MustInherit Class ModelBaseType : Inherits XmlDataModel
    Implements ISaveHandle

    <XmlElement([Namespace]:=GCModellerVCellKit)>
    Public Property properties As [Property]

    <XmlNamespaceDeclarations()>
    Public xmlns As New XmlSerializerNamespaces

    Public Const GCModellerVCellKit As String = "https://bioCAD.gcmodeller.org/vcellkit/"

    Sub New()
        Call xmlns.Add("vcellkit", GCModellerVCellKit)
    End Sub

    Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
        Return ISaveHandle_Save(path, encoding.CodePage)
    End Function

    Private Function ISaveHandle_Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
        Dim implType As Type = MyClass.GetType
        Dim xml As String = XmlExtensions.GetXml(Me, implType)

        Return xml.SaveTo(path, encoding)
    End Function

    Public Overrides Function ToString() As String
        Try
            Return String.Format("{0}::[{1}]", properties.guid, properties.specieId)
        Catch ex As Exception
            Return MyBase.ToString
        End Try
    End Function
End Class
