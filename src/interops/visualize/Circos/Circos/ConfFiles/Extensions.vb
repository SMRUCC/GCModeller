#Region "Microsoft.VisualBasic::3cc81d437b3ae08356a281e9e0c148fe, visualize\Circos\Circos\ConfFiles\Extensions.vb"

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

    '     Module Extensions
    ' 
    '         Function: CircosOption, GenerateCircosDocumentElement
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Visualize.Circos.Configurations.ComponentModel

Namespace Configurations

    Public Module Extensions

        ''' <summary>
        ''' <see cref="yes"/>, <see cref="no"/>
        ''' </summary>
        ''' <param name="b"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CircosOption(b As Boolean) As String
            Return If(b, yes, no)
        End Function

        ''' <summary>
        ''' Generates the docuemtn text data for write circos file.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="data"></param>
        ''' <param name="Tag"></param>
        ''' <param name="IndentLevel"></param>
        ''' <param name="inserts"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GenerateCircosDocumentElement(Of T As CircosDocument)(data As T, tag$, indentLevel%, inserts As IEnumerable(Of ICircosDocNode), directory$) As String
            Dim IndentBlanks As String = New String(" "c, indentLevel + 2)
            Dim sb As New StringBuilder(1024)

            For Each strLine$ In SimpleConfig.GenerateConfigurations(Of T)(data)
                Call sb.AppendLine($"{IndentBlanks}{strLine}")
            Next

            If Not inserts Is Nothing AndAlso inserts.Any Then
                Call sb.AppendLine()

                For Each item In inserts
                    If item Is Nothing Then
                        Continue For
                    End If

                    Call sb.AppendLine(item.Build(indentLevel + 2, directory))
                Next
            End If

            Return String.Format("<{0}>{1}{2}{1}</{0}>", tag, vbCrLf, sb.ToString)
        End Function
    End Module
End Namespace
