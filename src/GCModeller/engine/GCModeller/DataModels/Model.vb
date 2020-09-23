#Region "Microsoft.VisualBasic::e5203e82ce7b9f6009ebcc7f430a05ac, engine\GCModeller\DataModels\Model.vb"

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

    '     Class Model
    ' 
    '         Properties: Time
    ' 
    '         Sub: Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace DataModels

    Public Class Model : Inherits LANS.SystemsBiology.Assembly.Xml.Model

        ''' <summary>
        ''' 模拟计算的运行时间
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Xml.Serialization.XmlAttribute> Public Property Time As Long

        Public Overrides Sub Save(Optional File As String = "")
            Dim Xml As String = GNU.Linux.VisualBasic.Compatibility.Stdio.GetXml(Of Model)(Me)
            Call FileIO.FileSystem.WriteAllText(File, Xml, append:=False)
        End Sub
    End Class
End Namespace
