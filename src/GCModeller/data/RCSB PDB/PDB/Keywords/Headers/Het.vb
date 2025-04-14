#Region "Microsoft.VisualBasic::a38debb4b680d687ebd17e3da830865d, data\RCSB PDB\PDB\Keywords\Headers\Het.vb"

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

    '   Total Lines: 25
    '    Code Lines: 17 (68.00%)
    ' Comment Lines: 3 (12.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (20.00%)
    '     File Size: 645 B


    '     Class Het
    ' 
    '         Properties: AnnotationText, Keyword
    ' 
    '         Function: Append
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Keywords

    ''' <summary>
    ''' 非标准残基注释
    ''' </summary>
    Public Class Het : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return KEYWORD_HET
            End Get
        End Property

        Public Property AnnotationText As New List(Of String)

        Friend Shared Function Append(ByRef het As Het, line As String) As Het
            If het Is Nothing Then
                het = New Het
            End If
            het.AnnotationText.Add(line)
            Return het
        End Function

    End Class

    Public Class HetName : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return KEYWORD_HETNAM
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef hetname As HetName, line As String) As HetName
            If hetname Is Nothing Then
                hetname = New HetName
            End If
            hetname.str.Add(line)
            Return hetname
        End Function

    End Class

    Public Class Formula : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return KEYWORD_FORMUL
            End Get
        End Property

        Dim str As New List(Of String)

        Public Shared Function Append(ByRef formula As Formula, line As String) As Formula
            If formula Is Nothing Then
                formula = New Formula
            End If
            formula.str.Append(line)
            Return formula
        End Function

    End Class

    Public Class Link : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "LINK"
            End Get
        End Property

        Dim str As New List(Of String)

        Public Shared Function Append(ByRef links As Link, line As String) As Link
            If links Is Nothing Then
                links = New Link
            End If
            links.str.Append(line)
            Return links
        End Function

    End Class

    Public Class CISPEP : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "CISPEP"
            End Get
        End Property

        Dim str As New List(Of String)

        Public Shared Function Append(ByRef CISPEP As CISPEP, line As String) As CISPEP
            If CISPEP Is Nothing Then
                CISPEP = New CISPEP
            End If
            CISPEP.str.Append(line)
            Return CISPEP
        End Function
    End Class

    Public Class HETATM : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return KEYWORD_HETATM
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef hetatom As HETATM, str As String) As HETATM
            If hetatom Is Nothing Then
                hetatom = New HETATM
            End If
            hetatom.str.Add(str)
            Return hetatom
        End Function

    End Class

    Public Class HETSYN : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "HETSYN"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef hetatom As HETSYN, str As String) As HETSYN
            If hetatom Is Nothing Then
                hetatom = New HETSYN
            End If
            hetatom.str.Add(str)
            Return hetatom
        End Function

    End Class

    Public Class CONECT : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return KEYWORD_CONECT
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef conect As CONECT, str As String) As CONECT
            If conect Is Nothing Then
                conect = New CONECT
            End If
            conect.str.Add(str)
            Return conect
        End Function

    End Class

    Public Class MODRES : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "MODRES"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef word As MODRES, str As String) As MODRES
            If word Is Nothing Then
                word = New MODRES
            End If
            word.str.Add(str)
            Return word
        End Function

    End Class

    Public Class SSBOND : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "SSBOND"
            End Get
        End Property

        Dim str As New List(Of String)

        Public Shared Function Append(ByRef bond As SSBOND, str As String) As SSBOND
            If bond Is Nothing Then
                bond = New SSBOND
            End If
            bond.str.Add(str)
            Return bond
        End Function

    End Class

    Public Class SPRSDE : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "SPRSDE"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef sp As SPRSDE, str As String) As SPRSDE
            If sp Is Nothing Then
                sp = New SPRSDE
            End If
            sp.str.Add(str)
            Return sp
        End Function

    End Class

    Public Class CAVEAT : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "CAVEAT"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef cav As CAVEAT, str As String) As CAVEAT
            If cav Is Nothing Then
                cav = New CAVEAT
            End If
            cav.str.Add(str)
            Return cav
        End Function

    End Class

    Public Class MDLTYP : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "MDLTYP"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef type As MDLTYP, str As String) As MDLTYP
            If type Is Nothing Then
                type = New MDLTYP
            End If
            type.str.Add(str)
            Return type
        End Function

    End Class

    Public Class ANISOU : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "ANISOU"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef ani As ANISOU, str As String) As ANISOU
            If ani Is Nothing Then
                ani = New ANISOU
            End If
            ani.str.Add(str)
            Return ani
        End Function

    End Class
End Namespace
