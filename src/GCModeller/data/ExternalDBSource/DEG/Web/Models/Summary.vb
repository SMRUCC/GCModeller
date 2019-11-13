#Region "Microsoft.VisualBasic::f51fc3a39e53b90959eba6a05daba301, data\ExternalDBSource\DEG\Web\Models\Summary.vb"

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

    '     Class Summary
    ' 
    '         Properties: Backup, EssentialGenes, Medium, Method, Organism
    '                     Pubmed, Reference, RefSeq
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace DEG.Web

    Public Class Summary
        Public Property Organism As String
        Public Property RefSeq As String
        Public Property EssentialGenes As Integer
        Public Property Method As String
        Public Property Medium As String
        Public Property Pubmed As String
        Public Property Reference As String
        Public Property Backup As String
    End Class
End Namespace
