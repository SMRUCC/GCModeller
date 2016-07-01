#Region "Microsoft.VisualBasic::54b34754b2ca501ecfa6e483b0e3d0e9, ..\GCModeller\engine\GCMarkupLanguage\GCML_Documents\XmlElements\Metabolism\Pathway.vb"

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

Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.ComponentModels

Namespace GCML_Documents.XmlElements.Metabolism

    Public Class Pathway : Inherits T_MetaCycEntity(Of Slots.Pathway)

        ''' <summary>
        ''' Unique-Id
        ''' </summary>
        ''' <remarks></remarks>
        <XmlElement> Public Name As String

        ''' <summary>
        ''' Reaction Handles.(指向代谢组网络中的反应对象的句柄)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute("Reaction-List")> Public Property MetabolismNetwork As String()
        Public Comment As String

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}", Identifier, Name)
        End Function

        Public Shared Function CastTo(e As Slots.Pathway) As Pathway
            Dim Pathway As Pathway = New Pathway With {.BaseType = e}
            Pathway.Name = e.CommonName
            Pathway.Identifier = e.Identifier
            Return Pathway
        End Function
    End Class
End Namespace
