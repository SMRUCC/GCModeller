#Region "Microsoft.VisualBasic::c35f9fb0f9232731994fc36b6e9db80b, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\Tabular DataFiles\Pathway.vb"

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

    '   Total Lines: 76
    '    Code Lines: 42
    ' Comment Lines: 22
    '   Blank Lines: 12
    '     File Size: 2.46 KB


    '     Class Pathway
    ' 
    '         Properties: DisplayingName, GeneCollection, UniqueId
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.DataTabular

Namespace Assembly.MetaCyc.File.TabularDataFiles

    ''' <summary>
    ''' 代谢途径对象
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Pathway

        ''' <summary>
        ''' 这个途径对象在PGDB数据库中的唯一标识符
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property UniqueId As String
        ''' <summary>
        ''' 途径的名称
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property DisplayingName As String

        ''' <summary>
        ''' 与其他的数据库的对象连接关系
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlArray("GENE-Association")>
        Public Property GeneCollection As List(Of GeneLink)

        Public Overrides Function ToString() As String
            Dim sBuilder As StringBuilder = New StringBuilder(256)

            sBuilder.Append("(Pathway) " & UniqueId)
            sBuilder.AppendLine("; [" & DisplayingName & "]")
            sBuilder.AppendLine("GENES associated:")
            For Each link As GeneLink In GeneCollection
                sBuilder.AppendLine(link.ToString)
            Next

            Return sBuilder.ToString
        End Function

        Public Shared Widening Operator CType(e As String()) As Pathway
            Dim newPwy As New Pathway

            With newPwy
                .UniqueId = e(0)
                .DisplayingName = e(1)
                .GeneCollection = New List(Of GeneLink)

                Dim n As Integer = 1 + (e.Length - 2) / 2
                For i As Integer = 2 To n
                    .GeneCollection += New GeneLink With {
                        .GENEId = e(i),
                        .CGSCId = e(i + n - 1)
                    }
                    If String.IsNullOrEmpty(e(i + 1)) Then
                        Exit For
                    End If
                Next
            End With

            Return newPwy
        End Operator
    End Class

End Namespace
