#Region "Microsoft.VisualBasic::68eb8aa56a8e6cbb95e2ac90ca09321c, ..\Settings\Shared\Settings.Configuration\Config\File.vb"

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

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports Microsoft.VisualBasic.Text

Namespace Settings

    <XmlRoot("Settings.File", Namespace:="http://code.google.com/p/genome-in-code/gcmodeller/settings.file/")>
    Public Class File : Inherits ITextFile
        Implements IDisposable
        Implements IProfile

#Region "General Settings Items"

        ''' <summary>
        ''' Blast程序组所在的文件夹
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement,
            ProfileItem(Name:="DIR.BlastBin", Description:="The ncbi blast program group directory.", Type:=ValueTypes.Directory)>
        Public Property BlastBin As String
        <XmlElement,
            ProfileItem(Name:="blastdb", Description:="The directory which contains the fasta sequence database files.", Type:=ValueTypes.Directory)>
        Public Property BlastDb As String
        <XmlElement,
            ProfileItem(Name:="mothur", Description:="The mothur program group directory.", Type:=ValueTypes.Directory)>
        Public Property Mothur As String

        <XmlElement, ProfileItem(Name:="cog2003-2014", Description:="The prot2003-2014.fasta NCBI cog fasta database for the COG annotation.", Type:=ValueTypes.File)>
        Public Property COG2003_2014 As String

        ''' <summary>
        ''' The R program install location on your computer. This property value is the directory path value like: D:\R\bin or C:\Program Files\R\bin
        ''' </summary>
        ''' <returns></returns>
        <XmlElement,
            ProfileItem(Name:=NameOf(R_HOME),
                        Type:=ValueTypes.Directory,
                        Description:="The R program install location on your computer. This property value is the directory path value like: D:\R\bin or C:\Program Files\R\bin")>
        Public Property R_HOME As String
        <XmlElement,
            ProfileItem(Name:="Phylip", Type:=ValueTypes.Directory,
                        Description:="The directory location of the phylip program group.")>
        Public Property Phylip As String

        ''' <summary>
        ''' The root directory for stores the GCModeller database such as fasta sequence for annotation.
        ''' </summary>
        ''' <returns></returns>
        <XmlElement,
            ProfileItem(Name:="RepositoryRoot", Type:=ValueTypes.Directory,
                        Description:="The root directory for stores the GCModeller database such as fasta sequence for annotation.")>
        Public Property RepositoryRoot As String
            Get
                Return _repositoryRoot
            End Get
            Set(value As String)
                _repositoryRoot = value

                Try
                    Call FileIO.FileSystem.CreateDirectory(_repositoryRoot)
                    Call <README>This is a GCModeller repository; use the 'RQL' or 'GCModeller.Workbench.Model_Repository'
tools to examine it.  Do not add, delete, or modify files here
unless you know how to avoid corrupting the repository.

Visit http://GCModeller.org/ for more information.

GCModeller Dev-Team</README>.SaveTo(_repositoryRoot & "/readme.txt")
                Finally   ' 当使用并行化拓展的时候，可能会出现文件被占用的情况而导致程序出错，在这里不影响应用，忽略掉这个错误
                End Try
            End Set
        End Property

        Dim _repositoryRoot As String

        <ProfileItem(NameOf(Rockhopper), "The java assembly path Of the RNA-seq analysis program Rockhopper.jar")>
        Public Property Rockhopper As String
        <ProfileItem(NameOf(Java), "The java program path For running some external java assembly In the GCModeller.")>
        Public Property Java As String
        ''' <summary>
        ''' External hybrid programming with bio-perl.
        ''' </summary>
        ''' <returns></returns>
        <ProfileItem(NameOf(Perl), "External hybrid programming With bio-perl. The full name Of the perl.exe program, example path value Like: C:\Perl\bin\perl.exe")>
        Public Property Perl As String

        <ProfileItem(NameOf(Python), "Example as ""C:\Python27\python.exe""")>
        Public Property Python As String
        ''' <summary>
        ''' 有一些时候需要进行批量计算的时候，可能会在程序的内部释放出脚本重新调用Shoal进行计算
        ''' </summary>
        ''' <returns></returns>
        <ProfileItem("Shoal", "The ShoalShell maybe is required for some batch operation and optimization for the parallel performance.")>
        Public Property ShoalShell As String

        ''' <summary>
        ''' Connection paramenter to the model data server.
        ''' (连接至存放模型数据的服务器的MySQL连接参数)
        ''' </summary>
        ''' <remarks></remarks>
        <ProfileItem("MySQL")> Public Property MySQL As String

        <ProfileItem("RSS")> Public Property RSS As String = "http://gcmodeller.org/blog/rss"
#End Region

#Region "Program Settings Items"
        <ProfileNodeItem> Public Property C2 As Settings.Programs.C2
        <ProfileNodeItem> Public Property Dev2 As Settings.Programs.IDE
        <ProfileNodeItem> Public Property Gcc As Settings.Programs.GCC
        <ProfileNodeItem> Public Property SMART As Settings.Programs.SMART
        <ProfileNodeItem> Public Property STAMP As Settings.Programs.STAMP
        <ProfileNodeItem> Public Property GCHOST As Settings.Programs.GCHOST
        <ProfileNodeItem> Public Property MPAlignment As Settings.Programs.MPAlignment
#End Region

        Public Function GetMplParam() As Settings.Programs.MPAlignment
            Return Settings.Programs.MPAlignment.GetValue(Me)
        End Function

        ''' <summary>
        ''' 配置文件的默认文件位置为AppData文件夹之中
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 2017-1-26
        ''' 
        ''' 配置文件数据原来是放置在应用程序的根目录之下的，但是考虑到将程序拷贝到不同的计算机之上，
        ''' 环境已经变化了，则放置于应用程序的根目录之中的话，会覆盖掉其他计算机的环境配置，所以
        ''' 最终决定放置于AppData之中
        ''' </remarks>
        Public Shared ReadOnly Property DefaultXmlFile As String = App.ProductProgramData & "/.settings/Settings.xml"

        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
            If String.IsNullOrEmpty(FilePath) Then
                FilePath = DefaultXmlFile
            Else
                MyBase.FilePath = FilePath
            End If

            On Error Resume Next

            Call FileIO.FileSystem.DeleteFile(FilePath, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
            Return Me.GetXml.SaveTo(FilePath, Encoding Or UTF8)
        End Function

        Protected Overrides Sub Dispose(disposing As Boolean)
            Call Save()
            MyBase.Dispose(disposing)
        End Sub
    End Class
End Namespace
