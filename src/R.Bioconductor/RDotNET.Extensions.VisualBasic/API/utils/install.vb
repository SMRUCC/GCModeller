#Region "Microsoft.VisualBasic::9a54a477cfd8b16660938580dda76445, RDotNET.Extensions.VisualBasic\API\utils\install.vb"

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

    '     Module installed
    ' 
    '         Function: packages
    ' 
    '     Module install
    ' 
    '         Function: packages
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

Namespace API.utils

    Public Module installed

        ''' <summary>
        ''' Find Installed Packages. Find (or retrieve) details of all packages installed in the specified libraries.
        ''' (假若指定了包名称的话，没有安装会返回空值)
        ''' </summary>
        ''' <param name="libloc"></param>
        ''' <param name="priority"></param>
        ''' <param name="noCache"></param>
        ''' <param name="fields"></param>
        ''' <param name="subarch"></param>
        ''' <returns></returns>
        Public Function packages(Optional libloc As String = NULL,
                                 Optional priority As String = NULL,
                                 Optional noCache As Boolean = False,
                                 Optional fields As String = NULL,
                                 Optional subarch As String = ".Platform$r_arch") As IO.DataFrame
            Dim out As SymbolicExpression =
                $"installed.packages(lib.loc = {libloc}, priority = {priority},
                                     noCache = {noCache.λ}, fields = {fields},
                                     subarch = {subarch})".__call
            Throw New NotImplementedException
        End Function
    End Module

    Public Module install

        ''' <summary>
        ''' Install Packages from Repositories or Local Files. 
        ''' Download and install packages from CRAN-like repositories or from local files. 
        ''' (查看目标程序包是否已经安装在R系统里面)
        ''' </summary>
        ''' <param name="pkgs">character vector of the names of packages whose current versions should be downloaded from the repositories.
        ''' If repos = NULL, a character vector Of file paths Of '.zip’ files containing binary builds of packages. (http:// and file:// URLs are also accepted and the files will be downloaded and installed from local copies.) Source directories or file paths or URLs of archives may be specified with type = "source", but some packages need suitable tools installed (see the ‘Details’ section).
        ''' If this Is missing Or a zero-length character vector, a listbox Of available packages Is presented where possible In an interactive R session.</param>
        ''' <param name="[lib]">character vector giving the library directories where to install the packages. Recycled as needed. If missing, defaults to the first element of .libPaths().</param>
        ''' <param name="repos">character vector, the base URL(s) of the repositories to use, e.g., the URL of a CRAN mirror such as "http://cran.us.r-project.org". For more details on supported URL schemes see url.
        ''' Can be NULL To install from local files, directories Or URLs: this will be inferred by extension from pkgs If Of length one.</param>
        ''' <param name="contriburl">URL(s) of the contrib sections of the repositories. Use this argument if your repository mirror is incomplete, e.g., because you burned only the ‘contrib’ section on a CD, or only have binary packages. Overrides argument repos. Incompatible with type = "both".</param>
        ''' <param name="method">download method, see download.file. Unused if a non-NULL available is supplied.</param>
        ''' <param name="available">a matrix as returned by available.packages listing packages available at the repositories, or NULL when the function makes an internal call to available.packages. Incompatible with type = "both".</param>
        ''' <param name="destdir">directory where downloaded packages are stored. If it is NULL (the default) a subdirectory downloaded_packages of the session temporary directory will be used (and the files will be deleted at the end of the session).</param>
        ''' <param name="dependencies">logical indicating whether to also install uninstalled packages which these packages depend on/link to/import/suggest (and so on recursively). Not used if repos = NULL. Can also be a character vector, a subset of c("Depends", "Imports", "LinkingTo", "Suggests", "Enhances").
        ''' Only supported If Lib Is Of length one (Or missing), so it Is unambiguous where To install the dependent packages. If this Is Not the Case it Is ignored, With a warning.
        ''' The Default, NA, means c("Depends", "Imports", "LinkingTo").
        ''' TRUE means to use c("Depends", "Imports", "LinkingTo", "Suggests") for pkgs And c("Depends", "Imports", "LinkingTo") for added dependencies: this installs all the packages needed To run pkgs, their examples, tests And vignettes (If the package author specified them correctly).
        ''' In all of these, "LinkingTo" Is omitted for binary packages.</param>
        ''' <param name="type">character, indicating the type of package to download and install. Will be "source" except on Windows and some OS X builds: see the section on ‘Binary packages’ for those.</param>
        ''' <param name="configure_args">(Used only for source installs.) A character vector or a named list. If a character vector with no names is supplied, the elements are concatenated into a single string (separated by a space) and used as the value for the --configure-args flag in the call to R CMD INSTALL. If the character vector has names these are assumed to identify values for --configure-args for individual packages. This allows one to specify settings for an entire collection of packages which will be used if any of those packages are to be installed. (These settings can therefore be re-used and act as default settings.)
        ''' A named list can be used also To the same effect, And that allows multi-element character strings For Each package which are concatenated To a Single String To be used As the value For --configure-args.</param>
        ''' <param name="configure_vars">(Used only for source installs.) Analogous to configure.args for flag --configure-vars, which is used to set environment variables for the configure run.</param>
        ''' <param name="clean">a logical value indicating whether to add the --clean flag to the call to R CMD INSTALL. This is sometimes used to perform additional operations at the end of the package installation in addition to removing intermediate files.</param>
        ''' <param name="Ncpus">the number of parallel processes to use for a parallel install of more than one source package. Values greater than one are supported if the make command specified by Sys.getenv("MAKE", "make") accepts argument -k -j Ncpus.</param>
        ''' <param name="verbose">a logical indicating if some “progress report” should be given.</param>
        ''' <param name="libs_only">a logical value: should the --libs-only option be used to install only additional sub-architectures for source installs? (See also INSTALL_opts.) This can also be used on Windows to install just the DLL(s) from a binary package, e.g. to add 64-bit DLLs to a 32-bit install.</param>
        ''' <param name="INSTALL_opts">an optional character vector of additional option(s) to be passed to R CMD INSTALL for a source package install. E.g., c("--html", "--no-multiarch").
        ''' Can also be a named list Of character vectors To be used As additional options, With names the respective package names.</param>
        ''' <param name="quiet">logical: if true, reduce the amount of output.</param>
        ''' <param name="keep_outputs">a logical: if true, keep the outputs from installing source packages in the current working directory, with the names of the output files the package names with ‘.out’ appended. Alternatively, a character string giving the directory in which to save the outputs. Ignored when installing from local files.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' This is the main function to install packages. It takes a vector of names and a destination library, downloads the packages from the repositories and installs them. (If the library is omitted it defaults to the first directory in .libPaths(), with a message if there is more than one.) If lib is omitted or is of length one and is not a (group) writable directory, in interactive use the code offers to create a personal library tree (the first element of Sys.getenv("R_LIBS_USER")) and install there. Detection of a writable directory is problematic on Windows: see the ‘Note’ section.
        ''' For installs from a repository an attempt Is made To install the packages In an order that respects their dependencies. This does assume that all the entries In Lib are On the Default library path For installs (Set by environment variable R_LIBS).
        ''' You are advised To run update.packages before install.packages To ensure that any already installed dependencies have their latest versions.
        ''' </remarks>
        Public Function packages(pkgs As String,
                                 Optional [lib] As String = NULL,
                                 Optional repos As String = "getOption(""repos"")",
                                 Optional contriburl As String = "contrib.url(repos, type)",
                                 Optional method As String = NULL,
                                 Optional available As String = NULL,
                                 Optional destdir As String = NULL,
                                 Optional dependencies As String = "NA",
                                 Optional type As String = "getOption(""pkgType"")",
                                 Optional configure_args As String = "getOption(""configure.args"")",
                                 Optional configure_vars As String = "getOption(""configure.vars"")",
                                 Optional clean As Boolean = False,
                                 Optional Ncpus As String = "getOption(""Ncpus"", 1L)",
                                 Optional verbose As String = "getOption(""verbose"")",
                                 Optional libs_only As Boolean = False,
                                 Optional INSTALL_opts As String = NULL,
                                 Optional quiet As Boolean = False,
                                 Optional keep_outputs As Boolean = False) As Boolean
            SyncLock R
                With R
                    Try
                        .call = $"install.packages('{pkgs}', {[lib]}, repos = {repos},
                                        contriburl = {contriburl},
                                        {method}, available = {available}, destdir = {destdir},
                                        dependencies = {dependencies}, type = {type},
                                        configure.args = {configure_args},
                                        configure.vars = {configure_vars},
                                        clean = {clean.λ}, Ncpus = {Ncpus},
                                        verbose = {verbose},
                                        libs_only = {libs_only.λ}, {INSTALL_opts}, quiet = {quiet},
                                        keep_outputs = {keep_outputs})"
                        Return True
                    Catch ex As Exception
                        Call App.LogException(ex)
                        Return False
                    End Try
                End With
            End SyncLock
        End Function
    End Module
End Namespace
