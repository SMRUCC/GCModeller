#Region "Microsoft.VisualBasic::65ecdb39c85826ead4487aae7c79387c, ..\GCModeller\analysis\SequenceToolkit\Pfam-HMM\PfamHMMScan\Pfam.hmm\DBI.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection

''' <summary>
''' Retrieve hidden Markov model (HMM) profile from PFAM database
''' </summary>
Public Class DBI

    Public ReadOnly Property Stockholm As Dictionary(Of Stockholm)
    Public ReadOnly Property ActiveSites As Dictionary(Of ActiveSite)

    Sub New(PfamA As String, activeSite As String)
        ActiveSites = PfamHMMScan.ActiveSite.LoadStream(activeSite).ToDictionary
        Stockholm = PfamHMMScan.Stockholm.DocParser(PfamA).ToDictionary
    End Sub

    Sub New(model As String)

    End Sub

    ''' <summary>
    ''' Retrieve hidden Markov model (HMM) profile from PFAM database
    ''' </summary>
    ''' <param name="PFAMName">String specifying a protein family name (unique identifier) of an HMM profile record in the PFAM database. For example, '7tm_2'.</param>
    ''' <returns></returns>
    Public Function GetHMMprof(PFAMName As String) As HMMStruct

    End Function

    ''' <summary>
    ''' HMMStruct = gethmmprof(PFAMNumber) determines a protein family accession number from PFAMNumber (an integer), 
    ''' searches the PFAM database for the associated record, retrieves the HMM profile information, 
    ''' and stores it in HMMStruct, a MATLAB structure.
    ''' </summary>
    ''' <param name="PFAMNumber">
    ''' Integer specifying a protein family number of an HMM profile record in the PFAM database. For example, 2 is the protein family number for the protein family 'PF00002'.
    ''' </param>
    ''' <returns></returns>
    Public Function GetHMMprof(PFAMNumber As Integer) As HMMStruct

    End Function

    Public Shared Function Compile(PfamA As String, activeSite As String, save As String) As Boolean
        Dim db As New DBI(PfamA, activeSite)
    End Function
End Class
