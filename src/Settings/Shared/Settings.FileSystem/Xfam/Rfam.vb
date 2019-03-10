#Region "Microsoft.VisualBasic::227d562ab4ffe2d462a80b023551da1c, Shared\Settings.FileSystem\Xfam\Rfam.vb"

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

    '     Module Rfam
    ' 
    '         Properties: Rfam, RfamFasta
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace GCModeller.FileSystem.Xfam

    Module Rfam

        Public ReadOnly Property Rfam As String
            Get
                Return RepositoryRoot & "/Xfam/Rfam"
            End Get
        End Property

        Public ReadOnly Property RfamFasta As String
            Get
                Return Rfam & "/Fasta/"
            End Get
        End Property
    End Module
End Namespace
