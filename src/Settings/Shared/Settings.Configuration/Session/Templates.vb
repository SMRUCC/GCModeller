#Region "Microsoft.VisualBasic::4965ba47858e254c43fd7cd81d372879, Shared\Settings.Configuration\Session\Templates.vb"

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

    '     Module Templates
    ' 
    '         Properties: TemplateFolder
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: WriteExcelTemplate
    ' 
    ' 
    ' /********************************************************************************/

#End Region

#If Not DisableCsvTemplate Then
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Text
#End If
Namespace Settings

    ''' <summary>
    ''' ``<see cref="App.HOME"/> &amp; "/Templates/"``
    ''' </summary>
    Module Templates

        Public ReadOnly Property TemplateFolder As String

        Sub New()
            TemplateFolder = App.HOME & $"/Templates"
        End Sub

#If Not DisableCsvTemplate Then
        Public Sub WriteExcelTemplate(Of T)()
            Dim path$ = $"{TemplateFolder}/{App.AssemblyName}/{GetType(T).Name}.csv"

            If Not path.FileLength > 0 Then
                Call (New T() {}).SaveTo(path,, Encodings.ASCII.CodePage)
            End If
        End Sub
#End If
    End Module
End Namespace
