#Region "Microsoft.VisualBasic::73d1b49805b29c189af31ece4d5eda4a, LocalBLAST\LocalBLAST\BlastOutput\Reader\Blast+\2.6.0+\BlastX\BlastX.vb"

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

    '     Class v228_BlastX
    ' 
    '         Properties: Queries
    ' 
    '         Function: BlastXHits, CheckIntegrity, ExportAllBestHist, ExportBestHit, ExportOverview
    '                   Grep, Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.Views

Namespace LocalBLAST.BLASTOutput.BlastPlus.BlastX

    ''' <summary>
    ''' The output result data model of the blastx output text file.
    ''' </summary>
    Public Class v228_BlastX : Inherits IBlastOutput

        ''' <summary>
        ''' The blastx query inputs
        ''' </summary>
        ''' <returns></returns>
        Public Property Queries As BlastX.Components.Query()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function BlastXHits() As BlastXHit()
            Return ExportAllBestHist(0, 0) _
                .Select(Function(x)
                            Return DirectCast(x, BlastXHit)
                        End Function) _
                .ToArray
        End Function

        Public Overrides Function ExportAllBestHist(Optional coverage As Double = 0.5, Optional identities_cutoff As Double = 0.15) As BBH.BestHit()
            Dim list As New List(Of BBH.BestHit)

            For Each query As Components.Query In Queries
                For Each x In BlastXHit.CreateObjects(query)
                    Call list.Add(x)
                Next
            Next

            Return list.ToArray
        End Function

        Public Overrides Function ExportBestHit(Optional coverage As Double = 0.5, Optional identities_cutoff As Double = 0.15) As BBH.BestHit()
            Throw New NotImplementedException
        End Function

        Public Overrides Function ExportOverview() As Overview
            Throw New NotImplementedException
        End Function

        Public Overrides Function Grep(Query As TextGrepMethod, Hits As TextGrepMethod) As IBlastOutput
            If Not Query Is Nothing Then
                For Each QueryObject As Components.Query In Queries
                    QueryObject.QueryName = Query(QueryObject.QueryName)
                Next
            End If
            If Not Hits Is Nothing Then
                For Each QueryObject In Queries
                    For Each subject In QueryObject.Subjects.SafeQuery
                        subject.SubjectName = Hits(subject.SubjectName)
                    Next
                Next
            End If

            Return Me
        End Function

        Public Function Save(FilePath As String, Encoding As Encoding) As Boolean
            Return Me.GetXml.SaveTo(FilePath, Encoding)
        End Function

        Public Overrides Function CheckIntegrity(QuerySource As SequenceModel.FASTA.FastaFile) As Boolean
            Throw New NotImplementedException
        End Function
    End Class
End Namespace
