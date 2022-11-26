#Region "Microsoft.VisualBasic::7a9324dbe81bd4e07a83f0664694c434, GCModeller\core\Bio.Assembly\Assembly\iGEM\PartSeq.vb"

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

    '   Total Lines: 48
    '    Code Lines: 38
    ' Comment Lines: 5
    '   Blank Lines: 5
    '     File Size: 1.91 KB


    '     Class PartSeq
    ' 
    '         Properties: Description, Id, PartName, SequenceData, Status
    '                     Type
    ' 
    '         Function: Parse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Assembly.iGEM

    Public Class PartSeq : Implements IPolymerSequenceModel, INamedValue

        Public Property PartName As String Implements IKeyedEntity(Of String).Key
        Public Property Status As String
        Public Property Id As String
        Public Property Type As String
        Public Property Description As String
        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        Public Shared Iterator Function Parse(file As String) As IEnumerable(Of PartSeq)
            For Each seq As FastaSeq In StreamIterator.SeqSource(file)
                Dim headers = CommandLine.GetTokens(seq.Title)

                If headers.Length = 5 Then
                    Yield New PartSeq With {
                        .PartName = headers(0),
                        .Status = headers(1),
                        .Id = headers(2),
                        .Type = headers(3),
                        .Description = headers(4),
                        .SequenceData = seq.SequenceData
                    }
                Else
                    Yield New PartSeq With {
                       .PartName = headers(0),
                       .Status = headers(1),
                       .Id = headers(2),
                       .Type = Nothing,
                       .Description = headers(3),
                       .SequenceData = seq.SequenceData
                   }
                End If
            Next
        End Function
    End Class
End Namespace
