#Region "Microsoft.VisualBasic::c183f9be440b0b7feb5273ed82d601f0, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\XmlElements\ComponentModels\T_MetaCycEntity.vb"

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

    '   Total Lines: 40
    '    Code Lines: 19
    ' Comment Lines: 15
    '   Blank Lines: 6
    '     File Size: 1.60 KB


    '     Class T_MetaCycEntity
    ' 
    '         Properties: Identifier
    ' 
    '         Function: ToString
    ' 
    '     Interface I_BiologicalProcess_EventHandle
    ' 
    '         Function: _add_Regulator, get_Regulators
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace GCML_Documents.ComponentModels

    Public MustInherit Class T_MetaCycEntity(Of T As Slots.Object)
        Implements INamedValue

        <XmlIgnore> Friend BaseType As T

        ''' <summary>
        ''' Current object that define in the MetaCyc database.(当前所定义的MetaCyc数据库对象的唯一标识符)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute("UniqueId")>
        Public Overridable Property Identifier As String Implements INamedValue.Key

        Public Overrides Function ToString() As String
            Return Identifier
        End Function
    End Class

    ''' <summary>
    ''' 目标对象是一种生物过程
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface I_BiologicalProcess_EventHandle
        Function get_Regulators() As SignalTransductions.Regulator()
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="internal_GUID">这个可以是调控的motif位点对象，对于反应对象而言，这个参数似乎是没有用的</param>
        ''' <param name="Regulator"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function _add_Regulator(internal_GUID As String, Regulator As SignalTransductions.Regulator) As Boolean
    End Interface
End Namespace
