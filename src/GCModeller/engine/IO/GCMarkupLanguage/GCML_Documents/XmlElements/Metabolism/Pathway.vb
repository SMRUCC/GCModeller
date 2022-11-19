#Region "Microsoft.VisualBasic::908527b375ca90aee6fabbcaed8bcfdb, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\XmlElements\Metabolism\Pathway.vb"

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

    '   Total Lines: 33
    '    Code Lines: 19
    ' Comment Lines: 8
    '   Blank Lines: 6
    '     File Size: 1.16 KB


    '     Class Pathway
    ' 
    '         Properties: MetabolismNetwork
    ' 
    '         Function: CastTo, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels

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
