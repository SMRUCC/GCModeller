#Region "Microsoft.VisualBasic::0a5da63eec5e78f0a531926c21bafaab, core\Bio.Assembly\Assembly\NCBI\Taxonomy\AccessionTaxID.vb"

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

    '   Total Lines: 72
    '    Code Lines: 45 (62.50%)
    ' Comment Lines: 14 (19.44%)
    '    - Xml Docs: 21.43%
    ' 
    '   Blank Lines: 13 (18.06%)
    '     File Size: 2.77 KB


    '     Class AccessionTaxID
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetNcbiTaxID
    ' 
    '         Sub: (+2 Overloads) Dispose, ImportsToCacheDb
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace Assembly.NCBI.Taxonomy

    ''' <summary>
    ''' database for <see cref="Accession2Taxid"/>
    ''' </summary>
    Public Class AccessionTaxID : Implements IDisposable

        ReadOnly taxdb As InMemoryDb
        Private disposedValue As Boolean

        Sub New(taxdb As InMemoryDb)
            Me.taxdb = taxdb
        End Sub

        Public Function GetNcbiTaxID(accession_id As String) As String
            If taxdb.HasKey(accession_id) Then
                Return Encoding.ASCII.GetString(taxdb.Get(accession_id))
            End If

            Dim trim As NamedValue(Of String) = accession_id.GetTagValue("."c, failureNoName:=False)

            ' accesss not found
            If trim.Value = "" Then
                Return Nothing
            ElseIf taxdb.HasKey(trim.Name) Then
                Return Encoding.ASCII.GetString(taxdb.Get(trim.Name))
            Else
                Return Nothing
            End If
        End Function

        Public Shared Sub ImportsToCacheDb(accession2Taxid As IEnumerable(Of Accession2Taxid), taxdb As InMemoryDb)
            For Each map As Accession2Taxid In accession2Taxid
                Dim ncbi_taxid As String = CStr(map.taxid)
                Dim taxid_data As Byte() = Encoding.ASCII.GetBytes(ncbi_taxid)

                Call taxdb.Put(map.accession, taxid_data)
                Call taxdb.Put(map.accession_version, taxid_data)
            Next
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects)
                    Call taxdb.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
                ' TODO: set large fields to null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
        ' Protected Overrides Sub Finalize()
        '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace
