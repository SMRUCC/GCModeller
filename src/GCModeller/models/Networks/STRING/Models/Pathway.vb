#Region "Microsoft.VisualBasic::6b9d237fe87a59e934409008420292bc, models\Networks\STRING\Models\Pathway.vb"

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

    '     Class Pathway
    ' 
    '         Properties: Effectors, OCS, TCSSystem, TF, TF_MiST2Type
    '         Enum TFSignalTypes
    ' 
    '             OneComponentType, TwoComponentType
    ' 
    ' 
    ' 
    '  
    ' 
    '     Properties: NotAcceptStrPSignal
    ' 
    '     Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Data.Regprecise

Namespace Models

    ''' <summary>
    ''' 以TF为中心的信号转导网络，即一条信号转导网络可以使用一个输出节点和若干输入节点来表示
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Pathway : Implements INamedValue

        ''' <summary>
        ''' <see cref="Assembler.TF"></see>中的<see cref="RegpreciseMPBBH.QueryName">标识号</see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute("TF_id")> Public Property TF As String Implements INamedValue.Key
        <XmlElement> Public Property Effectors As String()
        ''' <summary>
        ''' 当前的这个<see cref="Pathway.TF">转录调控因子</see>是否为OCS类型
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property OCS As KeyValuePair()
        ''' <summary>
        ''' 当前的这个<see cref="Pathway.TF">转录调控因子</see>是否为TCS类型
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TCSSystem As TCS.TCS()

        <XmlAttribute> Public Property TF_MiST2Type As TFSignalTypes

        Public Enum TFSignalTypes
            TF = 0
            OneComponentType
            TwoComponentType
        End Enum

        ''' <summary>
        ''' 本TF不接受任何信号也可以工作
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property NotAcceptStrPSignal As Boolean
            Get
                Dim f = (OCS.IsNullOrEmpty AndAlso TCSSystem.IsNullOrEmpty)
                Return f
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return TF
        End Function
    End Class
End Namespace
