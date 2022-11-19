#Region "Microsoft.VisualBasic::0bbd60710245af08f226b2de0f805303, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\XmlElements\CultivationMediums.vb"

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

    '   Total Lines: 43
    '    Code Lines: 25
    ' Comment Lines: 10
    '   Blank Lines: 8
    '     File Size: 1.64 KB


    ' Class I_SubstrateRefx
    ' 
    '     Properties: Identifier, InitialQuantity
    ' 
    '     Function: ToString
    ' 
    ' Class CultivationMediums
    ' 
    '     Properties: Uptake_Substrates
    ' 
    '     Function: MetaCycDefault
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization

Public Class I_SubstrateRefx : Implements INamedValue

    <DumpNode> <XmlAttribute>
    Public Property Identifier As String Implements INamedValue.Key

    <DumpNode> <XmlAttribute>
    Public Property InitialQuantity As Double

    Public Overrides Function ToString() As String
        Return String.Format("{0} --> {1}", Identifier, InitialQuantity)
    End Function
End Class

''' <summary>
''' 培养基的功能就是源源不断的对细胞提供物质源
''' </summary>
''' <remarks></remarks>
Public Class CultivationMediums

    ''' <summary>
    ''' 培养基在计算机模型中的组成与代谢组一致，只不过在最开始的时候仅有用户所设定的一些代谢物有浓度值，则本属性提供了一个用于设置培养基初始组成的接口
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DumpNode> <XmlElement> Public Property Uptake_Substrates As I_SubstrateRefx()

    Public Shared Function MetaCycDefault() As CultivationMediums
        Dim Defualts As I_SubstrateRefx() = New I_SubstrateRefx() {
            New I_SubstrateRefx With {.Identifier = "OXYGEN-MOLECULE", .InitialQuantity = 20},
            New I_SubstrateRefx With {.Identifier = "GLUCOSE", .InitialQuantity = 20},
            New I_SubstrateRefx With {.Identifier = "AMMONIA", .InitialQuantity = 20}
        }

        Return New CultivationMediums With {
            .Uptake_Substrates = Defualts
        }
    End Function
End Class
