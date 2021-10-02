#Region "Microsoft.VisualBasic::21ece383bb312605e290cf65c1194fc9, markdown2pdf\PdfConvert\Arguments\Options\Global.vb"

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

    '     Class GlobalOptions
    ' 
    '         Properties: cookiejar, copies, dpi, enablelocalfileaccess, grayscale
    '                     imagedpi, imagequality, lowquality, marginbottom, marginleft
    '                     marginright, margintop, nocollate, nopdfcompression, orientation
    '                     title
    ' 
    '     Enum Orientations
    ' 
    '         Landscape, Portrait
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection

Namespace Arguments

    Public Class GlobalOptions

        ''' <summary>
        ''' Do not collate when printing multiple copies
        ''' </summary>
        ''' <returns></returns>
        <Argv("--no-collate", CLITypes.Boolean)>
        Public Property nocollate As Boolean

        ''' <summary>
        ''' Read and write cookies from and to the supplied cookie jar file
        ''' </summary>
        ''' <returns></returns>
        <Argv("--cookie-jar", CLITypes.File)>
        Public Property cookiejar As String

        ''' <summary>
        ''' Number of copies to print into the pdf file (default 1)
        ''' </summary>
        ''' <returns></returns>
        <Argv("--copies", CLITypes.Integer)>
        Public Property copies As Integer?

        ''' <summary>
        ''' Change the dpi explicitly (this has no effect on X11 based systems) 
        ''' (default 96)
        ''' </summary>
        ''' <returns></returns>
        <Argv("--dpi", CLITypes.Integer)>
        Public Property dpi As Integer?

        ''' <summary>
        ''' PDF will be generated in grayscale
        ''' </summary>
        ''' <returns></returns>
        <Argv("--grayscale", CLITypes.Boolean)>
        Public Property grayscale As Boolean = False

        ''' <summary>
        ''' When embedding images scale them down to this dpi(default 600)
        ''' </summary>
        ''' <returns></returns>
        <Argv("--image-dpi", CLITypes.Integer)>
        Public Property imagedpi As Integer?

        ''' <summary>
        ''' When jpeg compressing images use this quality (default 94)
        ''' </summary>
        ''' <returns></returns>
        <Argv("--image-quality", CLITypes.Integer)>
        Public Property imagequality As Integer?

        ''' <summary>
        ''' Generates lower quality pdf/ps. Useful to shrink the result document space
        ''' </summary>
        ''' <returns></returns>
        <Argv("--lowquality", CLITypes.Boolean)>
        Public Property lowquality As Boolean = False

        <Argv("--enable-local-file-access", CLITypes.Boolean)>
        Public Property enablelocalfileaccess As Boolean = True

        ''' <summary>
        ''' Set the page bottom margin
        ''' </summary>
        ''' <returns></returns>
        <Argv("--margin-bottom", CLITypes.String)>
        Public Property marginbottom As String

        ''' <summary>
        ''' Set the page left margin (default 10mm)
        ''' </summary>
        ''' <returns></returns>
        <Argv("--margin-left", CLITypes.String)>
        Public Property marginleft As String

        ''' <summary>
        ''' Set the page right margin (default 10mm)
        ''' </summary>
        ''' <returns></returns>
        <Argv("--margin-right", CLITypes.String)>
        Public Property marginright As String

        ''' <summary>
        ''' Set the page top margin
        ''' </summary>
        ''' <returns></returns>
        <Argv("--margin-top", CLITypes.String)>
        Public Property margintop As String

        ''' <summary>
        ''' Set orientation to Landscape or Portrait (default Portrait)
        ''' </summary>
        ''' <returns></returns>
        <Argv("--orientation", CLITypes.String)>
        Public Property orientation As Orientations?

        ''' <summary>
        ''' Do not use lossless compression on pdf objects
        ''' </summary>
        ''' <returns></returns>
        <Argv("--no-pdf-compression", CLITypes.Boolean)>
        Public Property nopdfcompression As Boolean

        ''' <summary>
        ''' The title of the generated pdf file (The title of the first document 
        ''' Is used if Not specified)
        ''' </summary>
        ''' <returns></returns>
        <Argv("--title", CLITypes.String)>
        Public Property title As String

    End Class

    Public Enum Orientations
        Portrait
        Landscape
    End Enum
End Namespace
