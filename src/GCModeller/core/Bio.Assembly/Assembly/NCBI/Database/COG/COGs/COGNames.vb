#Region "Microsoft.VisualBasic::60498aa3e23ac4df6d4e8fdaa6a66ece, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\COG\COGs\COGNames.vb"

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

'   Total Lines: 206
'    Code Lines: 100
' Comment Lines: 79
'   Blank Lines: 27
'     File Size: 6.63 KB


'     Class COGName
' 
'         Properties: COG, Func, Name
' 
'         Function: LoadTable
' 
'     Class COGFunc
' 
'         Properties: Code, Name
' 
'         Constructor: (+2 Overloads) Sub New
'         Function: LoadDoc, ToString
' 
'     Class Genomes
' 
'         Properties: Code, FTP_name, Taxid
' 
'         Constructor: (+2 Overloads) Sub New
'         Function: LoadDoc, ToString
' 
'     Class COGProt
' 
'         Properties: ProtID, RefSeq
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: LoadDoc, ToString
' 
' 
' /********************************************************************************/

#End Region

#If NET48 Then
Imports System.Data.Linq.Mapping
#Else
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
#End If

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Text

Namespace Assembly.NCBI.COG.COGs

    ''' <summary>
    ''' ###### cognames2003-2014.tab
    ''' 
    ''' Contains list of COG annotations. Tab-delimited Functional classes 
    ''' (categories) are described in the file ``fun2003-2014.tab`` (see 2.3). 
    ''' Some COGs belong to more than one functional Class; in these cases 
    ''' the class listed first Is considered to be primary.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class COGName : Implements INamedValue

        ''' <summary>
        ''' COG-id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("COG-id")>
        <Column(Name:="COG")> Public Property COG As String Implements INamedValue.Key

        ''' <summary>
        ''' functional-class
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("functional-class")>
        <Column(Name:="func")> Public Property Func As String

        ''' <summary>
        ''' COG-annotation
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("COG-annotation")>
        <Column(Name:="name")> Public Property Name As String

        ''' <summary>
        ''' Load table data from file.
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Shared Function LoadTable(path As String) As COGName()
            Dim buf As IEnumerable(Of String()) =
 _
                From s As String
                In IO.File.ReadAllLines(path).Skip(1)
                Select s.Split(ASCII.TAB)

            Dim LQuery As COGName() = buf.Select(
                Function(t) New COGName With {
                    .COG = t(0),
                    .Func = t(1),
                    .Name = t(2)
                }).ToArray

            Return LQuery
        End Function
    End Class

    ''' <summary>
    ''' fun2003-2014.tab
    ''' Contains list of functional classes. Tab-delimited
    ''' </summary>
    Public Class COGFunc

        ''' <summary>
        ''' class-id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("class-id")> Public Property Code As String
        ''' <summary>
        ''' description
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("description")> Public Property Name As String

        Sub New()
        End Sub

        Protected Sub New(Line As String)
            Dim Tokens As String() = Strings.Split(Line, vbTab)
            Code = Tokens(Scan0)
            Name = Tokens(1)
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{Code}]  {Name}"
        End Function

        Public Shared Function LoadDoc(path As String) As COGFunc()
            Dim tokens$() = IO.File.ReadAllLines(path) _
                .Skip(1) _
                .ToArray
            Dim codes As COGFunc() = tokens _
                .Select(Function(s) New COGFunc(s)) _
                .ToArray
            Return codes
        End Function
    End Class

    ''' <summary>
    ''' genomes2003-2014.tab
    ''' Contains list of genomes (711). Tab-delimited
    ''' 
    ''' The field &lt;genome-name> serves as a subdirectory name on NCBI Genomes
    ''' FTP site
    '''
    ''' ftp://ftp.ncbi.nih.gov/genomes/Bacteria/[ncbi-ftp-name]
    '''
    ''' where the genome sequence was availavle As Of February 2014. The
    ''' field &lt;ncbi-taxid> Is a semicolon-delimited lineage according to the
    ''' NCBI Taxonomy database (As Of February 2014).
    ''' </summary>
    Public Class Genomes

        ''' <summary>
        ''' genome-code
        ''' </summary>
        ''' <returns></returns>
        Public Property Code As String
        ''' <summary>
        ''' ncbi-taxid
        ''' </summary>
        ''' <returns></returns>
        Public Property Taxid As String
        ''' <summary>
        ''' ncbi-ftp-name
        ''' </summary>
        ''' <returns></returns>
        Public Property FTP_name As String

        Sub New()
        End Sub

        Protected Sub New(Line As String)
            Dim Tokens As String() = Strings.Split(Line, vbTab)
            Code = Tokens(Scan0)
            Taxid = Tokens(1)
            FTP_name = Tokens(2)
        End Sub

        Public Overrides Function ToString() As String
            Return FTP_name
        End Function

        Public Shared Function LoadDoc(path As String) As Genomes()
            Dim tokens As String() = IO.File.ReadAllLines(path).Skip(1).ToArray
            Dim genomes As Genomes() = tokens _
                .Select(Function(line) New Genomes(line)) _
                .ToArray

            Return genomes
        End Function
    End Class

    ''' <summary>
    ''' prot2003-2014.tab
    ''' Contains RefSeq accession codes for all proteins with assigned COG domains.
    ''' 
    ''' At the moment (April 02, 2015) RefSeq database is in a state of
    ''' transition; some of the &lt;refseq-acc> entries are Not accessible. This
    ''' table will be updated As soon As RefSeq Is stable.
    ''' </summary>
    Public Class COGProt

        ''' <summary>
        ''' protein-id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("protein-id")> Public Property ProtID As String
        ''' <summary>
        ''' refseq-acc
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("refseq-acc")> Public Property RefSeq As String

        Protected Sub New(Line As String)
            Dim Tokens As String() = Strings.Split(Line, vbTab)
            ProtID = Tokens(Scan0)
            RefSeq = Tokens(1)
        End Sub

        Public Overrides Function ToString() As String
            Return $"{ProtID}  --> {RefSeq}"
        End Function

        Public Shared Function LoadDoc(path As String) As COGProt()
            Dim tokens$() = IO.File.ReadAllLines(path) _
                .Skip(1) _
                .ToArray
            Dim prots As COGProt() = tokens _
                .Select(Function(line) New COGProt(line)) _
                .ToArray
            Return prots
        End Function
    End Class
End Namespace
