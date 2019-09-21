#Region "Microsoft.VisualBasic::eaa6127ea37c255b1965d1d565538466, engine\GCModeller.Framework.Kernel_Driver\DataServices\ModelBase\Model.vb"

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

    '     Class ModelBaseType
    ' 
    '         Properties: IteractionLoops, ModelProperty
    ' 
    '         Function: Save, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization

Namespace LDM

    ''' <summary>
    ''' All of the model file basetype definition in the GCModeller program group, all of the model file must inherits from this class object.
    ''' (GCModeller程序包内的所有模型文件的基类型，所有的模型文件对象必须从本对象类型继承)
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <XmlRoot("LANS-SystemsBiology-GCML", Namespace:="http://code.google.com/p/genome-in-code/GCMarkupLanguage/")>
    Public MustInherit Class ModelBaseType

        <XmlElement("GCModeller.DB.Properties", Namespace:="http://code.google.com/p/genome-in-code/GCMarkupLanguage/GCModeller/Components")>
        Public Property ModelProperty As [Property]
        <XmlAttribute> Public Property IteractionLoops As Integer

        Public Function Save(FilePath As String, Optional Encoding As Encoding = Nothing) As Boolean
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
