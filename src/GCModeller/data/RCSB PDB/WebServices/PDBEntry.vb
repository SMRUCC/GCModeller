#Region "Microsoft.VisualBasic::33cab704696526a3b9f2cb1bc00a29dc, data\RCSB PDB\WebServices\PDBEntry.vb"

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

    '   Total Lines: 26
    '    Code Lines: 20 (76.92%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (23.08%)
    '     File Size: 1.12 KB


    '     Module PDBEntry
    ' 
    '         Function: DownloadEntryList, DownloadPDBFile
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace WebServices

    Public Module PDBEntry

        Public Const PDBENTRY_LIST_FROM_LIGAND_COMPOUND As String = "http://www.ebi.ac.uk/pdbe-srv/pdbechem/PDBEntry/download/{0}"

        Public Function DownloadEntryList(CompoundCode As String) As String()
            Dim PageContent As String = String.Format(PDBENTRY_LIST_FROM_LIGAND_COMPOUND, CompoundCode).GET
            If String.IsNullOrEmpty(PageContent) Then
                Return New String() {}
            End If

            Dim ChunkTemp As String() = Strings.Split(PageContent, vbLf)
            ChunkTemp = ChunkTemp.Skip(2).ToArray
            Return ChunkTemp
        End Function

        Public Const PDBML_NOATOM As String = "http://www.ebi.ac.uk/pdbe/entry-files/download/{0}-noatom.xml"

        Public Function DownloadPDBFile(Entry As String, savedDir As String) As String
            Dim SavedFile As String = String.Format("{0}/{1}.xml", savedDir, Entry)
            Call String.Format(PDBML_NOATOM, Entry).DownloadFile(SavedFile)
            Return SavedFile
        End Function
    End Module
End Namespace
