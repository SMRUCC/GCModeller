#Region "Microsoft.VisualBasic::de0f38ea7535ba487b83c778a04bc292, data\RCSB PDB\PDB\Keywords\Keywords.vb"

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

    '   Total Lines: 39
    '    Code Lines: 33 (84.62%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (15.38%)
    '     File Size: 1.63 KB


    '     Class Keyword
    ' 
    '         Function: ToString
    ' 
    '         Sub: Flush
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Keywords

    ''' <summary>
    ''' A general base class of the keyword section inside the pdb file.
    ''' </summary>
    Public MustInherit Class Keyword

        Public Const KEYWORD_HEADER As String = "HEADER"
        Public Const KEYWORD_TITLE As String = "TITLE"
        Public Const KEYWORD_COMPND As String = "COMPND"
        Public Const KEYWORD_SOURCE As String = "SOURCE"
        Public Const KEYWORD_KEYWDS As String = "KEYWDS"
        Public Const KEYWORD_AUTHOR As String = "AUTHOR"
        Public Const KEYWORD_REVDAT As String = "REVDAT"
        Public Const KEYWORD_JRNL As String = "JRNL"
        Public Const KEYWORD_REMARK As String = "REMARK"
        Public Const KEYWORD_EXPDTA As String = "EXPDTA"
        Public Const KEYWORD_SEQRES As String = "SEQRES"
        Public Const KEYWORD_HET As String = "HET"
        Public Const KEYWORD_HETNAM As String = "HETNAM"
        Public Const KEYWORD_FORMUL As String = "FORMUL"
        Public Const KEYWORD_HELIX As String = "HELIX"
        Public Const KEYWORD_SHEET As String = "SHEET"
        Public Const KEYWORD_SITE As String = "SITE"
        Public Const KEYWORD_CRYST1 As String = "CRYST1"
        Public Const KEYWORD_ATOM As String = "ATOM"
        Public Const KEYWORD_HETATM As String = "HETATM"
        Public Const KEYWORD_CONECT As String = "CONECT"
        Public Const KEYWORD_MASTER As String = "MASTER"
        Public Const KEYWORD_DBREF As String = "DBREF"

        Public MustOverride ReadOnly Property Keyword As String

        Public Overrides Function ToString() As String
            Return Keyword
        End Function

        Friend Overridable Sub Flush()

        End Sub
    End Class
End Namespace
