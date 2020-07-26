#Region "Microsoft.VisualBasic::ec32c7cff75a4a510df0345f7438317c, visualize\Circos\Circos\ConfFiles\ComponentModel\CircosConfig.vb"

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

'     Class CircosConfig
' 
'         Properties: FilePath, includes, main, RefPath
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: GenerateIncludes, (+2 Overloads) Save
' 
'         Sub: appendLine
' 
' 
' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text

Namespace Configurations.ComponentModel

    ''' <summary>
    ''' Abstract of the circos config files.
    ''' </summary>
    Public MustInherit Class CircosConfig
        Implements ICircosDocument
        Implements IFileReference

        ''' <summary>
        ''' 文档的对其他的配置文件的引用列表
        ''' </summary>
        ''' <returns></returns>
        Public Property includes As List(Of CircosConfig)

        ''' <summary>
        ''' This config file was included in ``circos.conf``.(主配置文件Circos.conf)
        ''' </summary>
        ''' <returns></returns>
        Public Property main As Circos

        ''' <summary>
        ''' 配置文件的引用的相对路径
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property refPath As String
            Get
                Return Tools.TrimPath(Me)
            End Get
        End Property

        Public Property filePath As String Implements IFileReference.FilePath

        Sub New(fileName As String, circos As Circos)
            Me.filePath = fileName
            Me.main = circos
        End Sub

        Protected Function GenerateIncludes(directory As String) As String
            If includes.IsNullOrEmpty Then
                Return ""
            End If

            Dim sb As New StringBuilder(1024)

            For Each includeFile As CircosConfig In includes
                Call appendLine(sb, includeFile)
                Call includeFile.Save(directory, Encodings.ASCII)
            Next

            Return sb.ToString
        End Function

        Private Shared Sub appendLine(ByRef sb As StringBuilder, include As CircosConfig)
            Dim refPath As String = Tools.TrimPath(include)

            If TypeOf include Is CircosDistributed Then
                Dim name As String = DirectCast(include, CircosDistributed).section

                If Not String.IsNullOrEmpty(name) Then
                    Call sb.AppendLine($"<{name}>")
                    Call sb.AppendLine($"   <<include {refPath}>>")
                    Call sb.AppendLine($"</{name}>")
                Else
                    Call sb.AppendLine($"<<include {refPath}>>")
                End If
            Else
                Call sb.AppendLine($"<<include {refPath}>>")
            End If
        End Sub

        ''' <summary>
        ''' ``ticks.conf``
        ''' </summary>
        Public Const TicksConf As String = "ticks.conf"
        ''' <summary>
        ''' ``ideogram.conf``
        ''' </summary>
        Public Const IdeogramConf As String = "ideogram.conf"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="indents"></param>
        ''' <param name="directory">The config base directory path</param>
        ''' <returns></returns>
        Protected MustOverride Function build(indents%, directory$) As String Implements ICircosDocument.Build

        ''' <summary>
        ''' Auto detected that current is circos distribution or not, if true, then this file will not be saved.
        ''' </summary>
        ''' <param name="directory"></param>
        ''' <param name="Encoding"></param>
        ''' <returns></returns>
        Public Overridable Function Save(directory$, Encoding As Encoding) As Boolean Implements ICircosDocument.Save
            If TypeOf Me Is CircosDistributed Then
                ' 系统自带的不需要进行保存了
                Return True
            End If

            Dim doc As String = build(indents:=Scan0, directory:=directory)
            Dim path$ = $"{directory}/{filePath}"

            Return doc.SaveTo(path, Encoding Or Encoding.ASCII.AsDefault)
        End Function

        Public Function Save(directory$, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(directory, encoding.CodePage)
        End Function
    End Class
End Namespace
