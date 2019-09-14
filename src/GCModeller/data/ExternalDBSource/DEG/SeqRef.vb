#Region "Microsoft.VisualBasic::265c75bc57a0c3defe5b7a3925ee573c, ExternalDBSource\DEG\SeqRef.vb"

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

    '     Class SeqRef
    ' 
    '         Properties: fullName, geneName, ID, isVirulence, Organism
    '                     Reference, SequenceData, Xref
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.SequenceModel

Namespace DEG

    ''' <summary>
    ''' The sequence summary and reference data
    ''' </summary>
    Public Class SeqRef : Implements IPolymerSequenceModel

        Public Property ID As String
        Public Property Xref As String
        Public Property geneName As String
        Public Property isVirulence As Boolean
        Public Property fullName As String
        Public Property Organism As String
        Public Property Reference As String
        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData

    End Class
End Namespace
