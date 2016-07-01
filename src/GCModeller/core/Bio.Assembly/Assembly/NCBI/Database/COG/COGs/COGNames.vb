#Region "Microsoft.VisualBasic::8e9c28e64e764a812cfcfdeb2fb5c603, ..\GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\COG\COGs\COGNames.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace Assembly.NCBI.COG.COGs

    ''' <summary>
    ''' cognames2003-2014.tab
    ''' Contains list of COG annotations. Tab-delimited
    ''' Functional classes (categories) are described in the file
    ''' fun2003-2014.tab (see 2.3). Some COGs belong to more than one
    ''' functional Class; in these cases the class listed first Is considered
    ''' to be primary.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class COGName

        ''' <summary>
        ''' COG-id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("COG-id")>
        <Column(Name:="COG")> Public Property COG As String
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

        Public Shared Function LoadDocument(path As String) As COGName()
            Dim ChunkBuffer As String()() = (From s As String
                                             In IO.File.ReadAllLines(path).Skip(1)
                                             Select Strings.Split(s, vbTab)).ToArray
            Dim LQuery = (From Line As String() In ChunkBuffer.AsParallel
                          Let COG As COGName = New COGName With {
                              .COG = Line(0),
                              .Func = Line(1),
                              .Name = Line(2)
                              }
                          Select COG
                          Order By COG.COG Ascending).ToArray
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
            Dim Tokens = IO.File.ReadAllLines(path).Skip(1).ToArray
            Dim Codes = Tokens.ToArray(Function(Line) New COGFunc(Line))
            Return Codes
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
            Dim Tokens As String() = IO.File.ReadAllLines(path).Skip(1).ToArray
            Dim genomes = Tokens.ToArray(Function(Line) New Genomes(Line))
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
            Dim Tokens As String() = IO.File.ReadAllLines(path).Skip(1).ToArray
            Dim Prots = Tokens.ToArray(Function(Line) New COGProt(Line), Parallel:=True)
            Return Prots
        End Function
    End Class
End Namespace
