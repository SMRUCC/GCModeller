#Region "Microsoft.VisualBasic::acbeea79f541bc598dcb2faaa6aa5fa1, ..\Circos\Circos\ConfFiles\ComponentModel\DocAbstract.vb"

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

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports Microsoft.VisualBasic

Namespace Configurations

    ''' <summary>
    ''' Abstract of the circos config files.
    ''' </summary>
    Public MustInherit Class CircosConfig : Inherits ITextFile
        Implements ICircosDocument

        ''' <summary>
        ''' 文档的对其他的配置文件的引用列表
        ''' </summary>
        ''' <returns></returns>
        Public Property Includes As List(Of CircosConfig)

        ''' <summary>
        ''' This config file was included in ``circos.conf``.(主配置文件Circos.conf)
        ''' </summary>
        ''' <returns></returns>
        Public Property main As Circos

        Sub New(FileName As String, Circos As Circos)
            MyBase.FilePath = FileName
            Me.main = Circos
        End Sub

        Protected Function GenerateIncludes() As String
            If Includes.IsNullOrEmpty Then
                Return ""
            End If

            Dim sb As New StringBuilder(1024)

            For Each includeFile As CircosConfig In Includes
                Call __appendLine(sb, includeFile)
                Call includeFile.Save(Encoding:=Encoding.ASCII)
            Next

            Return sb.ToString
        End Function

        Private Shared Sub __appendLine(ByRef sb As StringBuilder, include As CircosConfig)
            Dim refPath As String = Tools.TrimPath(include)

            If TypeOf include Is CircosDistributed Then
                Dim name As String = DirectCast(include, CircosDistributed).Section
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
        ''' 配置文件的引用的相对路径
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property RefPath As String
            Get
                Return Tools.TrimPath(Me)
            End Get
        End Property

        ''' <summary>
        ''' ``ticks.conf``
        ''' </summary>
        Public Const TicksConf As String = "ticks.conf"
        ''' <summary>
        ''' ``ideogram.conf``
        ''' </summary>
        Public Const IdeogramConf As String = "ideogram.conf"

        Protected MustOverride Function GenerateDocument(IndentLevel As Integer) As String Implements ICircosDocument.GenerateDocument

        ''' <summary>
        ''' Auto detected that current is circos distribution or not, if true, then this file will not be saved.
        ''' </summary>
        ''' <param name="FilePath"></param>
        ''' <param name="Encoding"></param>
        ''' <returns></returns>
        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean Implements ICircosDocument.Save
            If TypeOf Me Is CircosDistributed Then
                Return True ' 系统自带的不需要进行保存了
            End If

            Dim doc As String = GenerateDocument(IndentLevel:=Scan0)
            Return doc.SaveTo(getPath(FilePath), If(Encoding Is Nothing, Encoding.ASCII, Encoding))
        End Function
    End Class
End Namespace
